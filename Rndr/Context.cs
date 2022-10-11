using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Timers;
using Rndr.Game;
using Rndr.Model;
using Silk.NET.Core.Contexts;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Rndr;

public delegate void GlHandler(Context context);

public sealed class Context : IDisposable
{
    public static Context Current { get; private set; } = null!;
    public readonly IWindow window;
    public readonly GL gl;
    public readonly ShaderManager shaders;
    public GameBase game { get; private set; } = null!;
    public long deltaTime { get; private set; }
    private Timer tickTimer = null!;

    public Context(IWindow window)
    {
        Current?.Dispose();
        this.window = window;
        window.Initialize();
        this.gl = window.CreateOpenGL();
        this.shaders = new ShaderManager();
        Current = this;
    }

    public ShaderPack LoadShaderPack(string name, params (ShaderType type, string source)[] sources) => shaders.LoadShader(gl, name, sources);

    public void Run(GameBase game)
    {
        this.game = game;
        
        tickTimer = new Timer(game.TickTime);
        try
        {
            game.Enable();
            long start = 0;
            tickTimer.Elapsed += (_, _) =>
            {
                try
                {
                    game.Tick(this);
                    deltaTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - start;
                    start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception occurred in tick, shutting down!\n" + e);
                    game.Active = false;
                }

                if (!game.Active)
                    tickTimer.Dispose();
            };
            tickTimer.Disposed += (_, _) => game.Dispose();
        }
        catch (Exception e)
        {
            Debug.WriteLine("Could not start Game tick timer\n" + e);
        }
        
        window.Load += () => game.Load(this);
        window.FramebufferResize += Resize;
        window.Render += (frameTime) => game.Draw(this, game.Camera, frameTime);
        window.Closing += Dispose;
        
        tickTimer.Start();
        window.Run();
    }

    private void Resize(Vector2D<int> size) => gl.Viewport(size);

    public void Dispose()
    {
        tickTimer.Dispose();
        gl.Dispose();
    }
}

public sealed class ShaderManager
{
    private readonly Dictionary<string, ShaderPack> _packs = new();
    public ShaderPack Current { get; private set; } = null!;

    internal ShaderManager()
    {
    }

    public ShaderPack this[string name] => _packs[name];

    public ShaderPack LoadShader(GL gl, string name, (ShaderType type, string source)[] sources)
    {
        var pack = _packs[name] = new ShaderPack(gl, name, this);
        foreach (var (type, source) in sources)
            pack.LoadShader(gl, type, source);
        pack.MatrixLocation = gl.GetUniformLocation(pack.Program, "matrix");
        return pack;
    }

    public void EnablePack(GL gl, string name) => gl.UseProgram((Current = _packs[name]).Program);
}

public sealed class ShaderPack
{
    private readonly GL _gl;
    private readonly string _name;
    private readonly ShaderManager _manager;
    private readonly Dictionary<ShaderType, Shader> _shaders = new();
    public readonly uint Program;
    public int MatrixLocation { get; internal set; }

    internal ShaderPack(GL gl, string name, ShaderManager manager)
    {
        _gl = gl;
        _name = name;
        _manager = manager;
        Program = gl.CreateProgram();
    }

    public Shader this[ShaderType type] => _shaders[type];
    
    public Shader LoadShader(GL gl, ShaderType type, string source)
    {
        var shader = gl.CreateShader(type);
        gl.ShaderSource(shader, source);
        gl.CompileShader(shader);
        gl.AttachShader(Program, shader);
        return _shaders[type] = new Shader(shader);;
    }

    public ShaderPack Link()
    {
        if (!_shaders.ContainsKey(ShaderType.VertexShader))
            throw new Exception("Cannot link shader program: Missing vertex shader");
        if (!_shaders.ContainsKey(ShaderType.FragmentShader))
            throw new Exception("Cannot link shader program: Missing fragment shader");
        _gl.LinkProgram(Program);
        return this;
    }

    public void Use() => _manager.EnablePack(_gl, _name);
}

public static class Extensions
{
    public static Vector3 To3Dim(this Vector2 it, float z) => new Vector3(it, z);
    public static Vector4 To4Dim(this Vector2 it, float z, float w = 0) => new Vector4(it, z, w);
    public static Vector4 To4Dim(this Vector3 it, float w) => new Vector4(it, w);

    public static Vector2 Vector2(this Vector3 a)
    {
        return new Vector2(a.X, a.Y);
    }

    public static Vector3 IntCast(this Vector3 a)
    {
        return new Vector3((int)a.X, (int)a.Y, (int)a.Z);
    }

    public static Vector3 ShortCast(this Vector3 a)
    {
        return new Vector3((short)a.X, (short)a.Y, (short)a.Z);
    }

    public static float Aspect(this Vector2D<int> vec) => (float)vec.Y / vec.X;

    public static long CombineIntoLong(this Vector3 a, short metadata)
    {
        if (a.X > short.MaxValue || a.X < short.MinValue
                                 || a.Y > short.MaxValue || a.Y < short.MinValue
                                 || a.Z > short.MaxValue || a.Z < short.MinValue)
            throw new ArgumentException(
                $"Vector values are out of bounds! Vector: {a}; Bounds: {short.MinValue} & {short.MaxValue}");
        a = a.ShortCast();
        long v = 0;
        const byte off = 16;

        v |= (short)a.X << (off * 0);
        v |= (short)a.Y << (off * 1);
        v |= (short)a.Z << (off * 2);
        v |= metadata << (off * 3);

        return v;
    }

    public static Vector4 Decompose(this long value)
    {
        const byte off = 16;

        var x = (short)(value >> (off * 0));
        var y = (short)(value >> (off * 1));
        var z = (short)(value >> (off * 2));
        var meta = (short)(value >> (off * 3));

        return new Vector4(x, y, z, meta);
    }

    public static Vector3 ToVec3(this Vector4 vec)
    {
        return vec.ToVec3(out float nil);
    }

    public static Vector3 ToVec3(this Vector4 vec, out float metadata)
    {
        metadata = vec.W;
        return new Vector3(vec.X, vec.Y, vec.Z);
    }

    public static float Magnitude(this Vector2 a)
    {
        return MathF.Sqrt(a.X * a.X + a.Y * a.Y);
    }

    public static float Magnitude(this Vector3 a)
    {
        return MathF.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z);
    }

    public static float Magnitude(this Vector4 a)
    {
        return MathF.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z + a.W * a.W);
    }

    public static Vector2 Normalize(this Vector2 a)
    {
        return a / a.Magnitude();
    }

    public static Vector3 Normalize(this Vector3 a)
    {
        return a / a.Magnitude();
    }

    public static Vector4 Normalize(this Vector4 a)
    {
        return a / a.Magnitude();
    }

    public static Vector3 Forward(this Quaternion it)
    {
        return new Vector3(
            2 * (it.X * it.Z + it.W * it.Y),
            2 * (it.Y * it.Z - it.W * it.X),
            1 - 2 * (it.X * it.X + it.Y * it.Y)
        );
    }

    public static Vector3 Up(this Quaternion it)
    {
        return new Vector3(
            2 * (it.X * it.Y - it.W * it.Z),
            1 - 2 * (it.X * it.X + it.Z * it.Z),
            2 * (it.Y * it.Z + it.W * it.X)
        );
    }

    public static Vector3 Left(this Quaternion it)
    {
        return new Vector3(
            1 - 2 * (it.Y * it.Y + it.Z * it.Z),
            2 * (it.X * it.Y + it.W * it.Z),
            2 * (it.X * it.Z - it.W * it.Y)
        );
    }

    public static unsafe float* ToPointer(this Matrix4x4 mtx)
    {
        float[] arr = new float[4 * 4];
        for (int i = 0; i < arr.Length; i++)
            arr[i] = mtx.ToGeneric()[i % 4][i / 4];
        fixed (float* ptr = arr)
            return ptr;
    }

    public static Quaternion Rotation(this Vector3 a, float angle)
    {
        return new Quaternion(a.Normalize(), angle);
    }

    public static Vector3 EulerAngles(this Quaternion q)
    {
        // https://stackoverflow.com/questions/11492299/quaternion-to-euler-angles-algorithm-how-to-convert-to-y-up-and-between-ha

        // Store the Euler angles in radians
        var pitchYawRoll = new Vector3();

        double sqw = q.W * q.W;
        double sqx = q.X * q.X;
        double sqy = q.Y * q.Y;
        double sqz = q.Z * q.Z;

        // If quaternion is normalised the unit is one, otherwise it is the correction factor
        double unit = sqx + sqy + sqz + sqw;
        double test = q.X * q.Y + q.Z * q.W;

        if (test > 0.4999f * unit) // 0.4999f OR 0.5f - EPSILON
        {
            // Singularity at north pole
            pitchYawRoll.Y = 2f * (float)Math.Atan2(q.X, q.W); // Yaw
            pitchYawRoll.X = MathF.PI * 0.5f; // Pitch
            pitchYawRoll.Z = 0f; // Roll
            return pitchYawRoll;
        }

        if (test < -0.4999f * unit) // -0.4999f OR -0.5f + EPSILON
        {
            // Singularity at south pole
            pitchYawRoll.Y = -2f * (float)Math.Atan2(q.X, q.W); // Yaw
            pitchYawRoll.X = -MathF.PI * 0.5f; // Pitch
            pitchYawRoll.Z = 0f; // Roll
            return pitchYawRoll;
        }

        pitchYawRoll.Y = (float)Math.Atan2(2f * q.X * q.W + 2f * q.Y * q.Z, 1 - 2f * (sqz + sqw)); // Yaw 
        pitchYawRoll.X = (float)Math.Asin(2f * (q.X * q.Z - q.W * q.Y)); // Pitch 
        pitchYawRoll.Z = (float)Math.Atan2(2f * q.X * q.Y + 2f * q.Z * q.W, 1 - 2f * (sqy + sqz)); // Roll 

        return pitchYawRoll;
    }
}
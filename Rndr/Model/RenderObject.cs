using System.Numerics;
using System.Runtime.InteropServices;
using Rndr.Game;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Rndr.Model;

public interface IRenderObject : ITransform, IDrawable, ILoadable
{
    IGameObject GameObject { get; }
    ITransform Transform { get; }
    Vector3 ITransform.Position => Transform.Position;
    Quaternion ITransform.Rotation => Transform.Rotation;
    Vector3 ITransform.Scale => Transform.Scale;
}

public abstract class AbstractRenderObject : IRenderObject
{
    protected internal GlHandler? PostAttributes = null;
    protected internal GlHandler? PreDraw = null;
    protected internal GlHandler? PostDraw = null;
    protected uint VAO;
    protected uint VBO;

    protected AbstractRenderObject(IGameObject gameObject, ITransform transform)
    {
        GameObject = gameObject;
        Transform = transform;
    }

    public bool Loaded { get; private set; }
    public IGameObject GameObject { get; }
    public ITransform Transform { get; }

    public Vector3 Position => Transform.Position;
    public Quaternion Rotation => Transform.Rotation;
    public Vector3 Scale => Transform.Scale;
    protected abstract unsafe VertexData VertexData { get; }

    public unsafe bool Load(Context ctx)
    {
        if (Loaded)
            return false;
        var gl = ctx.gl;

        VAO = gl.GenVertexArray();
        VBO = gl.GenBuffer();

        gl.BindVertexArray(VAO);
        gl.BindBuffer(GLEnum.ArrayBuffer, VBO);

        var data = VertexData;
        gl.BufferData(BufferTargetARB.ArrayBuffer, data.ptrSize * sizeof(float), data.ptr, BufferUsageARB.StaticDraw);
        gl.VertexAttribPointer(0, data.arraySize, VertexAttribPointerType.Float, data.normalized, data.stride, null);
        gl.EnableVertexAttribArray(0);
        PostAttributes?.Invoke(ctx);

        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        gl.BindVertexArray(0);
        gl.Flush();
        return Loaded = true;
    }

    public unsafe void Draw(Context ctx, Camera camera, double frameTime)
    {
        var gl = ctx.gl;
        
        gl.BindVertexArray(VAO);
        var mtx = camera.ProjectedViewMatrix;
        var euler = Rotation.EulerAngles();
        mtx *= Matrix4x4.CreateRotationX(euler.X);
        mtx *= Matrix4x4.CreateRotationY(euler.Y);
        mtx *= Matrix4x4.CreateRotationZ(euler.Z);
        mtx *= Matrix4x4.CreateTranslation(Position);
        gl.UniformMatrix4(ctx.shaders.Current.MatrixLocation, 1, false, mtx.ToPointer());
        
        PreDraw?.Invoke(ctx);
        gl.DrawArrays(VertexData.type, 0, VertexData.arrayCount);
        PostDraw?.Invoke(ctx);
        gl.Flush();
    }

    public void Unload(Context ctx)
    {
        var gl = ctx.gl;
        
        gl.DeleteBuffer(VBO);
        gl.DeleteVertexArray(VAO);
        
        Loaded = false;
    }

    public virtual void Dispose()
    {
    }
}

public unsafe struct VertexData
{
    public VertexData(float* ptr, uint ptrSize, PrimitiveType type, uint arrayCount)
    {
        this.ptr = ptr;
        this.ptrSize = ptrSize;
        this.type = type;
        this.arrayCount = arrayCount;
    }

    public float* ptr { get; }
    public uint ptrSize { get; }
    public uint stride { get; init; } = 0;
    public int arraySize { get; init; } = 0;
    public bool normalized { get; init; } = false;
    public PrimitiveType type { get; }
    public uint arrayCount { get; }
}
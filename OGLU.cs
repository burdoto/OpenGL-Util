using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using OpenGL_Util.Model;
using OpenGL_Util.Physics;
using SharpGL;
using SharpGL.SceneGraph;

namespace OpenGL_Util
{
    public interface IContainer : IDisposable
    {
        IEnumerable<IDisposable> Children { get; }

        bool AddChild(IDisposable container);

        bool RemoveChild(IDisposable container);

        IEnumerable<T> GetChildren<T>() where T : IDisposable;
    }

    public interface ILoadable : IDisposable
    {
        bool Loaded { get; }
        bool Load();
        void Unload();
    }

    public interface IEnableable : IDisposable
    {
        bool Enabled { get; }

        bool Enable();
        void Disable();
    }

    public interface ITickable : IContainer, ILoadable, IEnableable
    {
        void Tick();
    }

    public interface IGameObject : ITransform
    {
        Singularity Transform { get; }
        Vector3 ITransform.Position => Transform.Position;
        Quaternion ITransform.Rotation => Transform.Rotation;
        Vector3 ITransform.Scale => Transform.Scale;
        List<IRenderObject> RenderObjects { get; }
        IPhysicsObject? PhysicsObject { get; }
        ICollider? Collider { get; }
        short Metadata { get; set; }
    }

    public interface IRenderObject : ITransform, IDrawable
    {
        IGameObject GameObject { get; }
        ITransform Transform { get; }
        Vector3 ITransform.Position => Transform.Position;
        Quaternion ITransform.Rotation => Transform.Rotation;
        Vector3 ITransform.Scale => Transform.Scale;
    }

    public interface IDrawable
    {
        void Draw(OpenGL gl, ITransform camera);
    }

    public interface ITransform
    {
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
        public Vector3 Scale { get; }

        public Vector3 Forward => Rotation.Forward();
        public Vector3 Up => Rotation.Up();
        public Vector3 Left => Rotation.Left();
    }

    public static class Extensions
    {
        public static Vertex Vertex(this Vector3 a) => new Vertex(a.X, a.Y, a.Z);

        public static Vertex Convert(this ObjParser.Types.Vertex a) => new Vertex((float) a.X, (float) a.Y, (float) a.Z);

        public static Vector2 Vector2(this Vector3 a) => new Vector2(a.X, a.Y);

        public static Vector3 Vector(this Vertex a) => new Vector3(a.X, a.Y, a.Z);

        public static Vector3 IntCast(this Vector3 a) => new Vector3((int)a.X, (int)a.Y, (int)a.Z);
        
        public static Vector3 ShortCast(this Vector3 a) => new Vector3((short)a.X, (short)a.Y, (short)a.Z);

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

            v |= ((short)a.X << off * 0);
            v |= ((short)a.Y << off * 1);
            v |= ((short)a.Z << off * 2);
            v |= (metadata << off * 3);

            return v;
        }

        public static Vector4 Decompose(this long value)
        {
            const byte off = 16;

            short x = (short)(value >> off * 0);
            short y = (short)(value >> off * 1);
            short z = (short)(value >> off * 2);
            short meta = (short)(value >> off * 3);

            return new Vector4(x, y, z, meta);
        }

        public static Vector3 ToVec3(this Vector4 vec) => vec.ToVec3(out float nil);
        
        public static Vector3 ToVec3(this Vector4 vec, out float metadata)
        {
            metadata = vec.W;
            return new Vector3(vec.X, vec.Y, vec.Z);
        }

        public static float Magnitude(this Vector2 a) => MathF.Sqrt(a.X * a.X + a.Y * a.Y);
        public static float Magnitude(this Vector3 a) => MathF.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z);

        public static Vector3 Normalize(this Vector3 a) => a / a.Magnitude();

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
            var unit = sqx + sqy + sqz + sqw;
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
}
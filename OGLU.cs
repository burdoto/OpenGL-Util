using System;
using System.Collections.Generic;
using System.Numerics;
using SharpGL;
using SharpGL.SceneGraph;

namespace OpenGL_Util
{
    public interface IContainer : IDisposable
    {
        IEnumerable<IContainer> Children { get; }

        bool AddChild(IContainer container);

        bool RemoveChild(IContainer container);
    }

    public interface ILoadable
    {
        bool Loaded { get; }
        bool Load();
        void Unload();
    }

    public interface IEnableable
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
        IRenderObject RenderObject { get; }
    }

    public interface IRenderObject : ITransform, IDrawable
    {
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
        public static Vertex Vertex(this Vector3 a)
        {
            return new Vertex(a.X, a.Y, a.Z);
        }

        public static Vector3 Vector(this Vertex a)
        {
            return new Vector3(a.X, a.Y, a.Z);
        }

        public static Vector3 IntCast(this Vector3 a)
        {
            return new Vector3((int)a.X, (int)a.Y, (int)a.Z);
        }

        public static float Magnitude(this Vector3 a)
        {
            return MathF.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z);
        }

        public static Vector3 Normalize(this Vector3 a)
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
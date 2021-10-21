using System;
using System.Numerics;
using OpenGL_Util.Model;

namespace OpenGL_Util.Physics
{
    public static class Physics
    {
        public static Vector3 Gravity = Vector3.Zero;
        public static float Friction = 0.93f;
    }

    public interface IPhysicsObject : ITickable, ITransform
    {
        public IGameObject GameObject { get; }

        Vector3 ITransform.Position => GameObject.Position;
        Quaternion ITransform.Rotation => GameObject.Rotation;
        Vector3 ITransform.Scale => GameObject.Scale;

        public ITransform Transform { get; }
        public ICollider Collider { get; }
        public Vector3 Gravity  { get; }
        public Vector3 Velocity { get; set; } // in units per second
        public Quaternion RotationVelocity { get; set; } // in units per second 
        public float Mass { get; set; } // in grams

        public void ApplyAcceleration(Vector3 force);
    }
}
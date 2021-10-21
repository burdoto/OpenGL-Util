using System;
using System.Numerics;
using OpenGL_Util.Model;

namespace OpenGL_Util.Physics
{
    public static class Physics
    {
        public static Vector3 Gravity = Vector3.Zero;
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
        public Vector3 Velocity { get; } // in units per second
        public Quaternion RotationVelocity { get; } // in units per second 

        public void ApplyForce(Vector3 force);
    }
}
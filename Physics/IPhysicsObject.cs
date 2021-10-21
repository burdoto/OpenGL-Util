using System.Numerics;

namespace OpenGL_Util.Physics
{
    public interface IPhysicsObject
    {
        public IGameObject GameObject { get; }
        public ICollider Collider => GameObject.Collider!;
        public Vector3 Velocity { get; } // in units per second
        public Quaternion RotationVelocity { get; } // in units per second 
    }
}
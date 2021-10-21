using System;
using System.Collections.Generic;
using System.Numerics;
using OpenGL_Util.Game;
using OpenGL_Util.Model;

namespace OpenGL_Util.Physics
{
    public abstract class AbstractCollider : ICollider
    {
        public IGameObject GameObject { get; }
        private readonly ColliderType _type;

        public AbstractCollider(IGameObject gameObject, ColliderType type)
        {
            GameObject = gameObject;
            _type = type;
        }

        public Vector3 Position => GameObject.Position;
        public Quaternion Rotation => GameObject.Rotation;
        public Vector3 Scale => GameObject.Scale;
        public ColliderType ColliderType => _type;

        public abstract bool CollidesWith(ICollider other);
        public abstract bool PointInside(Vector2 point);
        public abstract bool PointInside(Vector3 point);
    }
    
    public class PhysicsObject : Container, IPhysicsObject
    {
        public PhysicsObject(IGameObject gameObject)
        {
            GameObject = gameObject;
        }

        public IGameObject GameObject { get; }
        public virtual Vector3 Gravity => Physics.Gravity;
        public ITransform Transform => GameObject.Transform;
        public ICollider Collider => GameObject.Collider!;
        public Vector3 Velocity { get; protected set; } = Vector3.Zero;
        public Quaternion RotationVelocity { get; protected set; } = Quaternion.Identity;
        
        public void ApplyForce(Vector3 force) => Velocity += force * force;

        public override void Tick()
        {
            // apply gravity to velocity
            if (Gravity != Vector3.Zero)
                ApplyForce(Gravity);

            base.Tick();
            
            // check for collisions
            GameBase.Main?.GetChildren<ICollider>()
            
            // apply to gameobject
            long scale = GameBase.TimeDelta / 1000;
            GameObject.Transform.Position += Velocity * scale;
            GameObject.Transform.Rotation *= RotationVelocity * scale;
        }
    }
}
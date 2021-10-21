using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using OpenGL_Util.Game;
using OpenGL_Util.Model;

namespace OpenGL_Util.Physics
{
    public abstract class AbstractCollider : Container, ICollider
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
        public IList<ICollider> Colliding { get; } = new List<ICollider>();

        public abstract bool CollidesWith(ICollider other);
        public abstract bool PointInside(Vector2 point);
        public abstract bool PointInside(Vector3 point);

        protected override void _Tick()
        {
            Colliding.Clear();
            foreach (var collider in GameBase.Main?.Grid.GetGameObjects()
                                         .Where(it => it != GameObject)
                                         .Where(it => it.Collider != null)
                                         .Select(it => it.Collider!)
                                     ?? (IEnumerable<ICollider>)Array.Empty<IGameObject>())
                if (CollidesWith(collider)) // todo Update this
                    Colliding.Add(collider);
        }
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
        public Vector3 Velocity { get; set; } = Vector3.Zero;
        public Quaternion RotationVelocity { get; set; } = Quaternion.Identity;
        
        public void ApplyAcceleration(Vector3 force) => Velocity += force * force;

        public override void Tick()
        {
            // apply gravity to velocity
            if (Gravity != Vector3.Zero)
                ApplyAcceleration(Gravity);

            base.Tick();
            
            // check for collisions
            if (Collider.Colliding.Count > 0)
            { // todo: transport forces to the colliding objects
            }

            // apply to gameobject
            float scale = GameBase.TimeDelta / 1000f;
            GameObject.Transform.Position += Velocity * scale;
            GameObject.Transform.Rotation *= RotationVelocity * scale;
        }
    }
}
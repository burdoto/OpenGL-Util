using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using OpenGL_Util.Game;
using OpenGL_Util.Model;

namespace OpenGL_Util.Physics
{
    public abstract class AbstractCollider : Container, ICollider
    {
        public ITransform Transform { get; }
        private readonly ColliderType _type;

        public AbstractCollider(ITransform transform, ColliderType type)
        {
            Transform = transform;
            _type = type;
        }

        public Vector3 Position => Transform.Position;
        public Quaternion Rotation => Transform.Rotation;
        public Vector3 Scale => Transform.Scale;
        public ColliderType ColliderType => _type;
        public IList<ICollider> Colliding { get; } = new List<ICollider>();

        public abstract bool CollidesWith(ICollider other);
        public abstract bool PointInside(Vector2 point);
        public abstract bool PointInside(Vector3 point);

        protected override void _Tick()
        {
            Colliding.Clear();

            foreach (var go in GameBase.Main?.Grid.GetGameObjects() ?? Array.Empty<IGameObject>())
                if (go.Collider != null)
                    if (CollidesWith(go.Collider))
                        Colliding.Add(go.Collider);
        }
    }
    
    public class InverseCollider : AbstractCollider
    {
        public ICollider Collider { get; } 
        
        public InverseCollider(ICollider collider) : base(collider.Transform, collider.ColliderType)
        {
            Collider = collider;
        }

        public override bool CollidesWith(ICollider other) => !Collider.CollidesWith(other);

        public override bool PointInside(Vector2 point) => !Collider.PointInside(point);

        public override bool PointInside(Vector3 point) => !Collider.PointInside(point);
    }

    public class MultiCollider : AbstractCollider
    {
        public List<AbstractCollider> Colliders { get; } = new List<AbstractCollider>();
        
        public MultiCollider(IGameObject gameObject, ColliderType type) : base(gameObject, type)
        {
        }

        public override bool CollidesWith(ICollider other) => Colliders.Any(it => it.CollidesWith(other));

        public override bool PointInside(Vector2 point) => Colliders.Any(it => it.PointInside(point));

        public override bool PointInside(Vector3 point) => Colliders.Any(it => it.PointInside(point));
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
        public float Inertia { get; set; } = 0;
        
        public void ApplyAcceleration(Vector3 force) => Velocity += force * force;

        public override void Tick()
        {
            // apply gravity to velocity
            if (Gravity != Vector3.Zero)
                ApplyAcceleration(Gravity);
            const float scala = 100;
            float scale = GameBase.TickTime / scala;
            Velocity *= Inertia;

            base.Tick();
            
            // check for collisions
            if (Collider.Colliding.Count > 0)
            { // todo: transport forces to the colliding objects
                Debug.WriteLine("Collided with " + Collider.ColliderType);
            }

            // apply to gameobject
            GameObject.Transform.Position += Velocity * scale;
            GameObject.Transform.Rotation *= RotationVelocity * scale;
        }
    }
}
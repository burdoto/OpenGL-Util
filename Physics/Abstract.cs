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
        public ISet<IGameObject> Colliding { get; } = new HashSet<IGameObject>();
        public bool ActiveCollider { get; set; } = false;

        public abstract bool CollidesWith(ICollider other);
        public abstract bool PointInside(Vector2 point);
        public abstract bool PointInside(Vector3 point);

        protected override void _Tick()
        {
            if (!ActiveCollider)
                return;
            
            Colliding.Clear();

            foreach (var go in GameBase.Main?.Grid.GetGameObjects() ?? Array.Empty<IGameObject>())
                if (go.Collider != null)
                    if (CollidesWith(go.Collider))
                        Colliding.Add(go);
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
        public Vector3 Velocity { get; set; }
        public Quaternion RotationVelocity { get; set; }
        public float Mass { get; set; }
        public ITransform Transform => GameObject.Transform;
        public ICollider Collider => GameObject.Collider!;
        public float Inertia { get; set; }
        
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
            if (Collider.Colliding.Count > 0 && Velocity != Vector3.Zero)
            {
                // todo: transport forces to the colliding objects
                Debug.WriteLine(GameObject.Metadata + " collided with " + string.Join(',', Collider.Colliding.Select(it => it.Metadata)));

                foreach (var other in Collider.Colliding)
                {
                    var energy0 = Velocity * Mass;
                    var energy1 = other.PhysicsObject?.Velocity * other.PhysicsObject?.Mass ?? Vector3.Zero;
                    var totalEnergy = energy0 + energy1;
                    var myFactor = 1 / (other.PhysicsObject?.Mass / Mass) ?? 1;
                    var myOutput = totalEnergy * myFactor;
                    if (myOutput.Magnitude() > totalEnergy.Magnitude())
                        throw new NotSupportedException("no");
                    if (other.PhysicsObject == null)
                    {
                        // other is immovable
                        Velocity = totalEnergy / Mass;
                    }
                    else
                    {
                        // todo: outputs need to be translated
                        Velocity = myOutput / Mass;
                        other.PhysicsObject.Velocity = (totalEnergy - myOutput) / Mass;
                    }
                }
            }

            // apply to gameobject
            GameObject.Transform.Position += Velocity * scale;
            GameObject.Transform.Rotation *= RotationVelocity * scale;
        }
    }
}
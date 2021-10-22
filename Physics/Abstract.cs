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
        public IGameObject GameObject { get; }
        public ITransform Transform { get; }
        private readonly ColliderType _type;

        public AbstractCollider(IGameObject gameObject, ITransform transform, ColliderType type)
        {
            GameObject = gameObject;
            Transform = transform;
            _type = type;
        }

        public Vector3 Position => Transform.Position;
        public Quaternion Rotation => Transform.Rotation;
        public Vector3 Scale => Transform.Scale;
        public ColliderType ColliderType => _type;
        public ISet<Collision> Collisions { get; } = new HashSet<Collision>();
        public bool ActiveCollider { get; set; } = false;

        public abstract bool CollidesWith(ICollider other, ref Vector3 v3, bool recursive = false, float z = 0);
        public abstract bool PointInside(Vector2 point);
        public abstract bool PointInside(Vector3 point);

        protected override void _Tick()
        {
            if (!ActiveCollider)
                return;
            
            Collisions.Clear();
            Vector3 pos = Vector3.Zero;
            foreach (var go in GameBase.Main?.Grid.GetGameObjects() ?? Array.Empty<IGameObject>())
                if (go.Collider != null)
                    if (CollidesWith(go.Collider, ref pos))
                        Collisions.Add(new Collision(GameObject, go, pos));
        }
    }
    
    public class InverseCollider : AbstractCollider
    {
        public ICollider Collider { get; } 
        
        public InverseCollider(ICollider collider) : base(collider.GameObject, collider.Transform, collider.ColliderType)
        {
            Collider = collider;
        }

        public override bool CollidesWith(ICollider other, ref Vector3 v3, bool recursive = false, float z = 0) =>
            !Collider.CollidesWith(other, ref v3, recursive);

        public override bool PointInside(Vector2 point) => !Collider.PointInside(point);

        public override bool PointInside(Vector3 point) => !Collider.PointInside(point);
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
        public ITransform Transform => GameObject.Transform;
        public ICollider Collider => GameObject.Collider!;
        public float Mass { get; set; }
        public float Inertia { get; set; }

        public void ApplyAcceleration(Vector3 force) => Velocity += force * force;

        public override void Tick()
        {
            // apply gravity to velocity
            if (Gravity != Vector3.Zero)
                ApplyAcceleration(Gravity);
            const float scala = 100;
            float scale = GameBase.TickTime / scala;
            // apply inertia to velocity
            if (Velocity.Magnitude() > 0.09f)
                Velocity *= Inertia;
            else Velocity = Vector3.Zero;

            base.Tick();
            
            // check for collisions
            if (Collider.Collisions.Count > 0 && Velocity != Vector3.Zero)
            {
                // todo: transport forces to the colliding objects
                Debug.WriteLine(GameObject.Metadata + " collided with " + string.Join(',', Collider.Collisions.Select(it => it.Other.Metadata)));

                foreach (var collision in Collider.Collisions)
                {
                    var other = collision.Other;
                    var kinetic0 = Velocity * Mass;
                    var kinetic1 = other.PhysicsObject?.Velocity * other.PhysicsObject?.Mass ?? Vector3.Zero;
                    
                    // todo: other cases than circle collider

                    var hitAngle = 10;
                    var rot0 = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, -hitAngle);
                    var rot1 = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, hitAngle);
                    var totalEnergy = kinetic0 + kinetic1;
                    // todo: needs to take angular loss into account
                    kinetic0 -= kinetic1;
                    var myFactor = 1 / (other.PhysicsObject?.Mass / Mass) ?? 1;
                    if (myFactor > 1)
                        throw new NotSupportedException("no");
                    kinetic0 = totalEnergy * myFactor;
                    kinetic0 = Vector3.Transform(kinetic0, rot0);
                    kinetic1 = totalEnergy * MathF.Abs(myFactor-1);
                    kinetic1 = Vector3.Transform(kinetic1, rot1);
                    if (other.PhysicsObject == null)
                    {
                        // other is immovable
                        Velocity = totalEnergy / Mass;
                    }
                    else
                    {
                        other.PhysicsObject.Velocity = kinetic1 / Mass;
                        Velocity = kinetic0 / Mass;
                    }
                }
            }

            // apply to gameobject
            GameObject.Transform.Position += Velocity * scale;
            GameObject.Transform.Rotation *= RotationVelocity * scale;
        }
    }
}
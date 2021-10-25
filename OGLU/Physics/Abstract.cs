using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using OGLU.Game;
using OGLU.Model;
using OGLU.Shape2;
using OGLU.Shape3;
using SharpGL;
using SharpGL.SceneGraph;

namespace OGLU.Physics
{
    public abstract class AbstractCollider : Container, ICollider
    {
        public AbstractCollider(IGameObject gameObject, ITransform transform, ColliderType type)
        {
            GameObject = gameObject;
            Transform = transform;
            ColliderType = type;
        }

        public IGameObject GameObject { get; }
        public ITransform Transform { get; }

        public Vector3 Position => Transform.Position;
        public Quaternion Rotation => Transform.Rotation;
        public Vector3 Scale => Transform.Scale;
        public ColliderType ColliderType { get; }

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
            var pos = Vector3.Zero;
            foreach (var go in GameBase.Main?.Grid.GetGameObjects() ?? Array.Empty<IGameObject>())
                if (go.Collider != null)
                    if (CollidesWith(go.Collider, ref pos))
                        Collisions.Add(new Collision(GameObject, go, pos));
        }
    }

    public class InverseCollider : AbstractCollider
    {
        public InverseCollider(ICollider collider) : base(collider.GameObject, collider.Transform,
            collider.ColliderType)
        {
            Collider = collider;
        }

        public ICollider Collider { get; }

        public override bool CollidesWith(ICollider other, ref Vector3 v3, bool recursive = false, float z = 0)
        {
            return !Collider.CollidesWith(other, ref v3, recursive);
        }

        public override bool PointInside(Vector2 point)
        {
            return !Collider.PointInside(point);
        }

        public override bool PointInside(Vector3 point)
        {
            return !Collider.PointInside(point);
        }
    }

    public class PhysicsObject : Container, IPhysicsObject
    {
        public PhysicsObject(IGameObject gameObject)
        {
            GameObject = gameObject;
        }

        public float Inertia { get; set; }

        public IGameObject GameObject { get; }
        public virtual Vector3 Gravity => Physics.Gravity;
        public Vector3 Velocity { get; set; }
        public Quaternion RotationVelocity { get; set; }
        public ITransform Transform => GameObject.Transform;
        public ICollider Collider => GameObject.Collider!;
        public float Mass { get; set; }

        public void ApplyAcceleration(Vector3 force)
        {
            Velocity += force * force;
        }

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
                Debug.WriteLine(GameObject.Metadata + " collided with " +
                                string.Join(',', Collider.Collisions.Select(it => it.Other.Metadata)));

                foreach (var collision in Collider.Collisions)
                {
                    var other = collision.Other;
                    var kinetic0 = Velocity * Mass;
                    var kinetic1 = other.PhysicsObject?.Velocity * other.PhysicsObject?.Mass ?? Vector3.Zero;

                    // todo: other cases than circle collider

                    var p1 = collision.Position;
                    var p2 = p1 + Vector3.UnitY;
                    var p3 = collision.Active.Position;
                    float hitAngle = MathF.Atan2(p3.Y - p1.Y, p3.X - p2.X) - MathF.Atan2(p2.Y - p1.Y, p2.X - p1.X);
                    var rot0 = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, hitAngle);
                    var rot1 = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, -hitAngle);
                    var totalEnergy = kinetic0 + kinetic1;
                    // todo: needs to take angular loss into account
                    kinetic0 -= kinetic1;
                    float myFactor = 1 / (other.PhysicsObject?.Mass / Mass) ?? 1;
                    if (myFactor > 1)
                        throw new NotSupportedException("no");
                    kinetic0 = Vector3.Transform(totalEnergy * myFactor, rot0) / Mass;
                    kinetic1 = Vector3.Transform(totalEnergy - kinetic0, rot1) / (other.PhysicsObject?.Mass ?? 1);
                    
                    AddDebugLines(collision, Velocity, kinetic0, kinetic1);

                    if (other.PhysicsObject != null) 
                        other.PhysicsObject.Velocity = kinetic1;
                    Velocity = kinetic0;
                }
            }

            // apply to gameobject
            GameObject.Transform.Position += Velocity * scale;
            GameObject.Transform.Rotation *= RotationVelocity * scale;
        }

        #region Debug Code
        private readonly ISet<IRenderObject> DebugLines = new HashSet<IRenderObject>();
        private static readonly GlHandler impactColor = gl => gl.Color(Color.Red);
        private static readonly GlHandler k0Color = gl => gl.Color(Color.Green);
        private static readonly GlHandler k1Color = gl => gl.Color(Color.Blue);
        private delegate IRenderObject LineTool(IGameObject go, Vector3 pos, Vector3 vec, GlHandler color);

        [Conditional("DEBUG")]
        private void AddDebugLines(Collision collision, Vector3 impact, Vector3 kinetic0, Vector3 kinetic1)
        {
            // clear DebugLines from object
            GameObject.RenderObjects.RemoveAll(DebugLines.Remove);
            var lineTool = Collider.IsTwoDimensional()
                ? (LineTool)((go, pos, vec, clr) => new Line2(go, new Singularity(pos, vec)){PostBegin = clr})
                : (go, pos, vec, clr) => new Line3(go, new Singularity(pos, vec)){PostBegin = clr};
            var pos0 = GameObject.Position;
            var pos1 = collision.Other.Position;
            IRenderObject obj;
            obj = new Circle(GameObject, new Singularity(collision.Position, new Vector3(0.5f, 0, 0))){PostBegin = impactColor};
            GameObject.RenderObjects.Add(obj);
            DebugLines.Add(obj);
            obj = lineTool(GameObject, pos0, impact, impactColor);
            GameObject.RenderObjects.Add(obj);
            DebugLines.Add(obj);
            obj = lineTool(GameObject, pos0, kinetic0, k0Color);
            GameObject.RenderObjects.Add(obj);
            DebugLines.Add(obj);
            obj = lineTool(GameObject, pos1, kinetic1, k1Color);
            GameObject.RenderObjects.Add(obj);
            DebugLines.Add(obj);
        }
        #endregion
    }
}
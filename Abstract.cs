using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using OpenGL_Util.Model;
using OpenGL_Util.Physics;
using SharpGL;

namespace OpenGL_Util
{
    public abstract class AbstractGameObject : Container, IGameObject
    {
        public Singularity Transform { get; }

        protected AbstractGameObject(Singularity transform, short metadata = 0)
        {
            Transform = transform;
            Metadata = metadata;
        }

        public virtual Vector3 Position => Transform.Position;
        public virtual Quaternion Rotation => Transform.Rotation;
        public virtual Vector3 Scale => Transform.Scale;
        public List<IRenderObject> RenderObjects { get; } = new List<IRenderObject>();
        public IPhysicsObject? PhysicsObject { get; protected set; }
        public ICollider? Collider  { get; protected set; }
        public override IEnumerable<IDisposable> Children => new IDisposable?[]{Collider, PhysicsObject}
            .Where(x => x!=null)
            .Select(x => x!)
            .Concat(_children);
        public short Metadata { get; set; }
        
        public virtual void Dispose() { }
    }

    public abstract class AbstractRenderObject : IRenderObject
    {
        public IGameObject GameObject { get; }
        public ITransform Transform { get; }

        protected AbstractRenderObject(IGameObject gameObject) : this(gameObject, gameObject)
        {
        }

        protected AbstractRenderObject(IGameObject gameObject, ITransform transform)
        {
            GameObject = gameObject;
            Transform = transform;
        }

        public virtual Vector3 Position => Transform.Position;
        public virtual Quaternion Rotation => Transform.Rotation;
        public virtual Vector3 Scale => Transform.Scale;
        public abstract void Draw(OpenGL gl, ITransform camera);
        public Action<OpenGL>? PostBegin;

        protected void CallPostBegin(OpenGL gl) => PostBegin?.Invoke(gl);
    }
}
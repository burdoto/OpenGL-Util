using System;
using System.Numerics;
using SharpGL;

namespace OpenGL_Util
{
    public abstract class AbstractGameObject : IGameObject, IDisposable
    {
        public readonly ITransform Transform;

        protected AbstractGameObject(ITransform transform, short metadata = 0)
        {
            Transform = transform;
            Metadata = metadata;
        }

        public virtual Vector3 Position => Transform.Position;
        public virtual Quaternion Rotation => Transform.Rotation;
        public virtual Vector3 Scale => Transform.Scale;
        public abstract IRenderObject RenderObject { get; }
        public short Metadata { get; set; }
        
        public virtual void Dispose() { }
    }

    public abstract class AbstractRenderObject : IRenderObject
    {
        public readonly IGameObject GameObject;

        protected AbstractRenderObject(IGameObject gameObject)
        {
            GameObject = gameObject;
        }

        public virtual Vector3 Position => GameObject.Position;
        public virtual Quaternion Rotation => GameObject.Rotation;
        public virtual Vector3 Scale => GameObject.Scale;
        public abstract void Draw(OpenGL gl, ITransform camera);
        public Action<OpenGL>? PostBegin;

        protected void CallPostBegin(OpenGL gl) => PostBegin?.Invoke(gl);
    }
}
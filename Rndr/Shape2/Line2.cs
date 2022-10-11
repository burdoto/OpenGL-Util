using Rndr.Model;
using Silk.NET.OpenGL;

namespace Rndr.Shape2
{
    public class Line2 : AbstractRenderObject
    {
        public Line2(IGameObject gameObject) : this(gameObject, gameObject)
        {
        }

        public unsafe Line2(IGameObject gameObject, ITransform transform) : base(gameObject, transform)
        {
            var a = transform.Position;
            var b = transform.Position + transform.Scale;
            float[] vertices = new float[]
            {
                a.X, a.Y, a.Z,
                b.X, b.Y, b.Z
            };
            fixed (float* ptr = vertices)
                VertexData = new VertexData(ptr, (uint)vertices.Length, PrimitiveType.Lines, 2);
        }

        protected override unsafe VertexData VertexData { get; }
    }
}
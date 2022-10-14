using System.Numerics;
using Rndr.Model;
using Silk.NET.OpenGL;

namespace Rndr.Shape2;

public class Rect : AbstractRenderObject
{
    public Rect(IGameObject gameObject) : this(gameObject, gameObject)
    {
    }

    public unsafe Rect(IGameObject gameObject, ITransform transform) : base(gameObject, transform)
    {
        var a = new Vector3(-1, -1, 0) * Scale;
        var b = new Vector3(-1, 1, 0) * Scale;
        var c = new Vector3(1, -1, 0) * Scale;
        var d = new Vector3(1, 1, 0) * Scale;
        float[] verts = new float[]
        {
            a.X, a.Y, a.Z,
            b.X, b.Y, b.Z,
            c.X, c.Y, c.Z,
            d.X, d.Y, d.Z
        };
        fixed (float* ptr = verts)
            VertexData = new VertexData(ptr, (uint)verts.Length, PrimitiveType.TriangleStrip, 4);
    }

    protected override unsafe VertexData VertexData { get; }
}
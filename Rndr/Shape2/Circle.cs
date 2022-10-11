using System;
using System.Collections.Generic;
using Rndr.Model;
using Silk.NET.OpenGL;

namespace Rndr.Shape2
{
    public class Circle : AbstractRenderObject
    {
        public Circle(IGameObject gameObject) : this(gameObject, gameObject)
        {
        }

        public unsafe Circle(IGameObject gameObject, ITransform transform) : base(gameObject, transform)
        {
            var pos = transform.Position;
            var radius = transform.Scale.X;
            var segments = (int)transform.Scale.Y;
            var vertices = new List<float>();
            for (int i = 0; i < segments; i++)
            {
                var theta = 2f * MathF.PI * i / segments;
                vertices.Add(radius * MathF.Sin(theta));
                vertices.Add(radius * MathF.Cos(theta));
            }
            fixed (float* ptr = vertices.ToArray())
                VertexData = new VertexData(ptr, (uint)vertices.Count, PrimitiveType.Lines, (uint)segments);
        }

        protected override unsafe VertexData VertexData { get; }
    }
}
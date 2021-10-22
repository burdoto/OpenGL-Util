using System;
using SharpGL;
using SharpGL.Enumerations;

namespace OGLU.Shape2
{
    public class Circle : AbstractRenderObject
    {
        public Circle(IGameObject gameObject) : this(gameObject, gameObject)
        {
        }

        public Circle(IGameObject gameObject, ITransform transform) : base(gameObject, transform)
        {
        }

        public int Segments { get; set; } = 100;
        public bool Filled { get; set; } = true;

        public override void Draw(OpenGL gl, ITransform camera)
        {
            var radius = (int)Scale.X;

            gl.Begin(Filled ? BeginMode.TriangleFan : BeginMode.LineLoop);
            CallPostBegin(gl);
            for (var a = 0; a < Segments; a++)
            {
                float theta = 2f * MathF.PI * a / Segments;
                float x = radius * MathF.Sin(theta);
                float y = radius * MathF.Cos(theta);
                gl.Vertex(Position.X + x, Position.Y + y);
            }

            gl.End();
        }
    }
}
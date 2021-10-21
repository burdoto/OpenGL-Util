using System;
using SharpGL;
using SharpGL.Enumerations;

namespace OpenGL_Util.Shape2
{
    public class Circle : AbstractRenderObject
    {
        public int Segments { get; set; } = 100;
        public bool Filled { get; set; } = true;

        public override void Draw(OpenGL gl, ITransform camera)
        {
            int radius = (int)Scale.X;
            
            gl.Begin(Filled ? BeginMode.TriangleFan : BeginMode.LineLoop);
            CallPostBegin(gl);
            for (int a = 0; a < Segments; a++)
            {
                var theta = 2f * MathF.PI * a / Segments;
                float x = radius * MathF.Sin(theta);
                float y = radius * MathF.Cos(theta);
                gl.Vertex(Position.X + x, Position.Y + y);
            }
            gl.End();
        }

        public Circle(IGameObject gameObject) : this(gameObject, gameObject.Transform)
        {
        }

        public Circle(IGameObject gameObject, ITransform transform) : base(gameObject, transform)
        {
        }
    }
}
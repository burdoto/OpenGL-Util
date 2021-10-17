using System;
using System.Drawing;
using System.Numerics;
using OpenGL_Util.Model;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.SceneGraph;

namespace OpenGL_Util.Shape2
{
    public class Rectangle : AbstractRenderObject
    {
        public Color Color { get; }
        public bool Filled { get; set; } = true;

        public Rectangle(IGameObject gameObject) : this(gameObject, Color.White)
        {
        }

        public Rectangle(IGameObject gameObject, Color color) : base(gameObject)
        {
            Color = color;
        }

        public override void Draw(OpenGL gl, ITransform camera)
        {
            int radius = (int)Scale.X;
            //fixme
            gl.Begin(Filled ? BeginMode.TriangleFan : BeginMode.LineLoop);
            gl.Color(Color);
            var pos = Position.Vertex();
            var scale = Scale.Vertex();
            var vec = pos;
            vec += scale;
            gl.Vertex(vec);
            vec -= scale;
            gl.Vertex(vec);
            vec.X += scale.X;
            gl.Vertex(vec);
            gl.End();
        }
    }
}
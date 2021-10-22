using System;
using System.Drawing;
using System.Numerics;
using OpenGL_Util.Game;
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
            //fixme
            gl.Begin(Filled ? BeginMode.TriangleFan : BeginMode.LineLoop);
            gl.Color(Color);
            var mainPos = GameObject.Transform.Position;
            var vec = Position - mainPos;
            var scale = Scale;
            vec.X -= scale.X / 2;
            vec.Y -= scale.Y / 2;
            gl.Vertex((Vector3.Transform(vec, Rotation) + mainPos).Vertex());
            vec.X += scale.X;
            gl.Vertex((Vector3.Transform(vec, Rotation) + mainPos).Vertex());
            vec.Y += scale.Y;
            gl.Vertex((Vector3.Transform(vec, Rotation) + mainPos).Vertex());
            vec.X -= scale.X;
            gl.Vertex((Vector3.Transform(vec, Rotation) + mainPos).Vertex());
            gl.End();
        }
    }
}
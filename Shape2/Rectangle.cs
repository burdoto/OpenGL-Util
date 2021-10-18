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
            int radius = (int)Scale.X;
            //fixme
            gl.Begin(Filled ? BeginMode.TriangleFan : BeginMode.LineLoop);
            gl.Color(Color);
            var vec = Position.Vertex();
            var scale = Scale.Vertex();
            var angle = Rotation.EulerAngles().Z;
            var origin = GameObject.Transform.Position.Vertex();
            // lol breakdown of the (false) quaternion effect
            //scale = new Vertex(scale.X * MathF.Cos(angle) - scale.Y * MathF.Sin(angle), scale.X * MathF.Sin(angle) + scale.Y * MathF.Cos(angle), 0);
            vec.X -= scale.X / 2;
            vec = RotateAround(vec, origin, angle);
            gl.Vertex(vec);
            vec.X += scale.X;
            vec = RotateAround(vec, origin, angle);
            gl.Vertex(vec);
            vec.Y += scale.Y;
            vec = RotateAround(vec, origin, angle);
            gl.Vertex(vec);
            vec.X -= scale.X;
            vec = RotateAround(vec, origin, angle);
            gl.Vertex(vec);
            gl.End();
        }
        
        private static Vertex RotateAround(Vertex point, Vertex origin, float angle) {
            angle = angle * MathF.PI / 180.0f;
            return new Vertex(
                MathF.Cos(angle) * (point.X - origin.X) - MathF.Sin(angle) * (point.Y - origin.Y) + origin.X,
                y: MathF.Sin(angle) * (point.X - origin.X) + MathF.Cos(angle) * (point.Y - origin.Y) + origin.Y, 0);
        }
    }
}
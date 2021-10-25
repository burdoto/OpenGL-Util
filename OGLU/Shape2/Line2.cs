using SharpGL;
using SharpGL.Enumerations;

namespace OGLU.Shape2
{
    public class Line2 : AbstractRenderObject
    {
        public Line2(IGameObject gameObject) : base(gameObject)
        {
        }

        public Line2(IGameObject gameObject, ITransform transform) : base(gameObject, transform)
        {
        }

        public override void Draw(OpenGL gl, ITransform camera)
        {
            var pos = Position;
            gl.Begin(BeginMode.Lines);
            CallPostBegin(gl);
            gl.Vertex(pos.X, pos.Y);
            pos += Scale;
            gl.Vertex(pos.X, pos.Y);
            gl.End();
        }
    }
}
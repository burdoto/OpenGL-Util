using SharpGL;
using SharpGL.Enumerations;

namespace OGLU.Shape3
{
    public class Line3 : AbstractRenderObject
    {
        public Line3(IGameObject gameObject) : base(gameObject)
        {
        }

        public Line3(IGameObject gameObject, ITransform transform) : base(gameObject, transform)
        {
        }

        public override void Draw(OpenGL gl, ITransform camera)
        {
            var pos = Position.Vertex();
            gl.Begin(BeginMode.Lines);
            CallPostBegin(gl);
            gl.Vertex(pos);
            pos += Scale.Vertex();
            gl.Vertex(pos);
            gl.End();
        }
    }
}
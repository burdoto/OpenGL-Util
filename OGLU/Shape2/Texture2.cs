using System.Drawing;
using System.Numerics;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Assets;

namespace OGLU.Shape2
{
    public class Texture2 : AbstractRenderObject
    {
        public Texture Texture { get; set; }

        public Texture2(IGameObject gameObject, Texture texture) : this(gameObject, gameObject, texture)
        {
        }

        public Texture2(IGameObject gameObject, ITransform transform, Texture texture) : base(gameObject, transform)
        {
            Texture = texture;
        }

        public override void Draw(OpenGL gl, ITransform camera)
        {
            //fixme
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, Texture.TextureName);
            gl.Begin(BeginMode.Quads);
            
            var mainPos = GameObject.Transform.Position;
            var vec = Position - mainPos;
            var scale = Scale;
            vec.X -= scale.X / 2;
            vec.Y -= scale.Y / 2;
            gl.TexCoord(0,0);
            gl.Vertex((Vector3.Transform(vec, Rotation) + mainPos).Vertex());
            vec.X += scale.X;
            gl.TexCoord(1,0);
            gl.Vertex((Vector3.Transform(vec, Rotation) + mainPos).Vertex());
            vec.Y += scale.Y;
            gl.TexCoord(1,1);
            gl.Vertex((Vector3.Transform(vec, Rotation) + mainPos).Vertex());
            vec.X -= scale.X;
            gl.TexCoord(0,1);
            gl.Vertex((Vector3.Transform(vec, Rotation) + mainPos).Vertex());
            gl.End();
        }   
    }
}
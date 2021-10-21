using System.Drawing;
using SharpGL;

namespace OpenGL_Util.Shape2
{
    public class Text : AbstractRenderObject
    {
        public string Content { get; set; }
        public Color FontColor { get; set; } = Color.Red;
        public string FontName { get; set; } = "Courier New";
        public float FontSize { get; set; } = 12;
        
        public Text(IGameObject gameObject) : base(gameObject)
        {
        }

        public Text(IGameObject gameObject, ITransform transform) : base(gameObject, transform)
        {
        }

        public override void Draw(OpenGL gl, ITransform camera) => 
            gl.DrawText((int)Position.X, (int)Position.Y, FontColor.R, FontColor.G, FontColor.B, FontName, FontSize, Content);
    }
}
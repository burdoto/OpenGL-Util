using System;
using OpenGL_Util.Matrix;
using OpenGL_Util.Model;
using SharpGL;

namespace OpenGL_Util.Game
{
    public abstract class GameBase : Container, IDrawable
    {
        public bool Active { get; set; }
        public RenderMatrix RenderMatrix { get; } = new RenderMatrix();
        
        public void Draw(OpenGL gl, ITransform camera)
        {
            RenderMatrix.Draw(gl, camera);
        }

        public void Start()
        {
            if (!Load())
                throw new Exception("Could not load game");
            if (!Enable())
                throw new Exception("Could not enable game");
            Active = true;
            while (Active)
                Tick();
            Disable();
            Unload();
        }
    }
}
using System;
using OpenGL_Util.Matrix;
using OpenGL_Util.Model;
using SharpGL;

namespace OpenGL_Util.Game
{
    public abstract class GameBase : Container, IDrawable
    {
        public long TimeDelta { get; private set; }
        public bool Active { get; set; }
        public RenderMatrix RenderMatrix { get; } = new RenderMatrix();

        public void Draw(OpenGL gl, ITransform camera)
        {
            RenderMatrix.Draw(gl, camera);
            gl.DrawText(5, 20, 255, 0, 0, "Courier New", 12, $"Tick: {TimeDelta}ms");
        }

        public void Run()
        {
            if (!Load())
                throw new Exception("Could not load game");
            if (!Enable())
                throw new Exception("Could not enable game");
            Active = true;
            long start;
            while (Active)
            {
                start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                Tick();
                TimeDelta = DateTimeOffset.Now.ToUnixTimeMilliseconds() - start;
            }

            Disable();
            Unload();
        }
    }
}
using System;
using System.Numerics;
using OpenGL_Util.Matrix;
using OpenGL_Util.Model;
using SharpGL;

namespace OpenGL_Util.Game
{
    public abstract class GameBase : Container, IDrawable
    {
        protected GameBase() : this(new RenderMatrix())
        {
        }
        
        protected GameBase(GridMatrix gridMatrix) : this(new RenderMatrix(gridMatrix))
        {
        }

        protected GameBase(RenderMatrix renderMatrix)
        {
            RenderMatrix = renderMatrix;
        }

        public long TimeDelta { get; private set; }
        public bool Active { get; set; }
        public RenderMatrix RenderMatrix { get; }
        public GridMatrix Grid => RenderMatrix.Grid;
        public abstract ITransform Player { get; }

        public void Draw(OpenGL gl, ITransform camera)
        {
            var cam = camera.Position;
            var look = cam + Vector3.Transform(Vector3.UnitZ, camera.Rotation);
            gl.LookAt(cam.X, cam.Y, cam.Z, 
                look.X,look.Y,look.Z, 
                0, 1, 0);
            
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
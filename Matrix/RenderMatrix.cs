using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using SharpGL;

namespace OpenGL_Util.Matrix
{
    public class RenderMatrix : IDrawable
    {
        public readonly GridMatrix Grid;

        public RenderMatrix() : this(new MapMatrix3())
        {
        }

        public RenderMatrix(GridMatrix grid)
        {
            this.Grid = grid;
        }

        public void Draw(OpenGL gl, ITransform camera)
        {
            //Debug.WriteLine("Drawing Matrix");
            foreach (var draw in Grid.GetVisibles(camera))
                draw?.Draw(gl, camera);
        }

        public T AddRenderObject<T>(IRenderObject it) where T : IRenderObject
        {
            return (T) (Grid[it.Position] = it);
        }

        public void AddRenderObjects(params IRenderObject[] objs)
        {
            foreach (var obj in objs)
                AddRenderObject<IRenderObject>(obj);
        }

        public void Clear() => Grid.Clear();
    }
}

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
        private readonly
            ConcurrentDictionary<int, ConcurrentDictionary<int, ConcurrentDictionary<int, IRenderObject?>>> _matrix =
                new ConcurrentDictionary<int, ConcurrentDictionary<int, ConcurrentDictionary<int, IRenderObject?>>>();

        public IRenderObject? this[Vector2 vec]
        {
            get => this[(int) vec.X, (int) vec.Y];
            set => this[(int) vec.X, (int) vec.Y] = value;
        }

        public IRenderObject? this[int x, int y]
        {
            get => this[x, y, 0];
            set => this[x, y, 0] = value;
        }

        public IRenderObject? this[Vector3 vec]
        {
            get => this[(int) vec.X, (int) vec.Y, (int) vec.Z];
            set => this[(int) vec.X, (int) vec.Y, (int) vec.Z] = value;
        }

        public IRenderObject? this[int x, int y, int z]
        {
            get
            {
                _matrix.GetOrAdd(x, key => new ConcurrentDictionary<int, ConcurrentDictionary<int, IRenderObject?>>())
                    .GetOrAdd(y, key => new ConcurrentDictionary<int, IRenderObject?>())
                    .TryGetValue(z, out IRenderObject? draw);
                return draw;
            }
            set => _matrix.GetOrAdd(x, key => new ConcurrentDictionary<int, ConcurrentDictionary<int, IRenderObject?>>())
                .GetOrAdd(y, key => new ConcurrentDictionary<int, IRenderObject?>())
                .TryAdd(z, value);
        }

        public void Draw(OpenGL gl, ITransform camera)
        {
            //Debug.WriteLine("Drawing Matrix");
            foreach (var draw in _matrix.Values
                .SelectMany(it => it.Values)
                .SelectMany(it => it.Values))
                draw?.Draw(gl, camera);
        }

        public T AddRenderObject<T>(IRenderObject it) where T : IRenderObject
        {
            return (T) (this[it.Position] = it);
        }

        public void AddRenderObjects(params IRenderObject[] objs)
        {
            foreach (var obj in objs)
                AddRenderObject<IRenderObject>(obj);
        }
    }
}

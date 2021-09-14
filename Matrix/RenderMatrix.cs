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
            ConcurrentDictionary<float, ConcurrentDictionary<float, ConcurrentDictionary<float, IRenderObject?>>> _matrix =
                new ConcurrentDictionary<float, ConcurrentDictionary<float, ConcurrentDictionary<float, IRenderObject?>>>();

        public IRenderObject? this[Vector2 vec]
        {
            get => this[vec.X, vec.Y];
            set => this[vec.X, vec.Y] = value;
        }

        public IRenderObject? this[float x, float y]
        {
            get => this[x, y, 0];
            set => this[x, y, 0] = value;
        }

        public IRenderObject? this[Vector3 vec]
        {
            get => this[vec.X, vec.Y, vec.Z];
            set => this[vec.X, vec.Y, vec.Z] = value;
        }

        public IRenderObject? this[float x, float y, float z]
        {
            get
            {
                _matrix.GetOrAdd(x, key => new ConcurrentDictionary<float, ConcurrentDictionary<float, IRenderObject?>>())
                    .GetOrAdd(y, key => new ConcurrentDictionary<float, IRenderObject?>())
                    .TryGetValue(z, out IRenderObject? draw);
                return draw;
            }
            set => _matrix.GetOrAdd(x, key => new ConcurrentDictionary<float, ConcurrentDictionary<float, IRenderObject?>>())
                .GetOrAdd(y, key => new ConcurrentDictionary<float, IRenderObject?>())
                .TryAdd(z, value);
        }

        public void Draw(OpenGL gl, Vector3 offset, ITransform camera)
        {
            //Debug.WriteLine("Drawing Matrix");
            foreach (var draw in _matrix.Values
                .SelectMany(it => it.Values)
                .SelectMany(it => it.Values))
                draw?.Draw(gl, offset, camera);
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

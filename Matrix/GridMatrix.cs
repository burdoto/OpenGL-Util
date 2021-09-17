using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace OpenGL_Util.Matrix
{
    public abstract class GridMatrix
    {
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

        public abstract IRenderObject? this[int x, int y, int z] { get; set; }

        public abstract IEnumerable<IRenderObject?> GetVisibles(ITransform? camera);

        public abstract void Clear();
    }

    public class MapMatrix2 : GridMatrix
    {
        private readonly
            ConcurrentDictionary<int, ConcurrentDictionary<int, IRenderObject?>> _matrix =
                new ConcurrentDictionary<int, ConcurrentDictionary<int, IRenderObject?>>();
        public override IRenderObject? this[int x, int y, int z]
        {
            get
            {
                _matrix.GetOrAdd(x, key => new ConcurrentDictionary<int, IRenderObject?>())
                    .TryGetValue(y, out IRenderObject? draw);
                return draw;
            }
            set => _matrix.GetOrAdd(x, key => new ConcurrentDictionary<int, IRenderObject?>())
                .TryAdd(y, value);
        }

        public override IEnumerable<IRenderObject?> GetVisibles(ITransform? camera)
        {
            return _matrix.Values.SelectMany(y => y.Values);
        }

        public override void Clear()
        {
            foreach (var xStage in _matrix.Values) 
                xStage.Clear();
            _matrix.Clear();
        }
    }

    public class MapMatrix3 : GridMatrix
    {
        private readonly
            ConcurrentDictionary<int, ConcurrentDictionary<int, ConcurrentDictionary<int, IRenderObject?>>> _matrix =
                new ConcurrentDictionary<int, ConcurrentDictionary<int, ConcurrentDictionary<int, IRenderObject?>>>();
        public override IRenderObject? this[int x, int y, int z]
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

        public override IEnumerable<IRenderObject?> GetVisibles(ITransform? camera)
        {
            return _matrix.Values
                .SelectMany(x => x.Values)
                .SelectMany(y => y.Values);
        }

        public override void Clear()
        {
            foreach (var xStage in _matrix.Values)
            {
                foreach (var yStage in xStage.Values) 
                    yStage.Clear();
                xStage.Clear();
            }
            _matrix.Clear();
        }
    }
}
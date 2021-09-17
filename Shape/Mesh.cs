using System;
using System.IO;
using System.Numerics;
using ObjParser;
using SharpGL;

namespace OpenGL_Util.Shape
{
    public class Mesh : IRenderObject, ILoadable
    {
        private readonly FileInfo _file;
        private readonly Obj _mesh;
        private readonly ITransform _transform;

        public Mesh(ITransform transform, string file)
        {
            _transform = transform;
            _file = new FileInfo(file);
            _mesh = new Obj();
        }

        public bool Load()
        {
            _mesh.LoadObj(_file.OpenRead());
            return Loaded = true;
        }

        public void Unload()
        {
        }
        
        public bool Loaded { get; private set; }
        
        public Vector3 Position => _transform.Position;
        public Quaternion Rotation => _transform.Rotation;
        public Vector3 Scale => _transform.Scale;

        public void Draw(OpenGL gl, ITransform camera)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Numerics;
using ObjParser;
using SharpGL;
using SharpGL.Enumerations;

namespace OpenGL_Util.Shape
{
    public class Mesh : IRenderObject, ILoadable
    {
        private readonly ITransform _transform;
        private readonly FileInfo _file;
        private readonly Obj _mesh;

        public Mesh(ITransform transform, FileInfo file)
        {
            _transform = transform;
            _file = file;
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
            if (!Loaded)
                return;
            
            var offset = Position.Vertex();
            
            gl.Begin(BeginMode.Triangles);
            foreach (var vertex in _mesh.VertexList.Select(it => it.Convert())) 
                gl.Vertex(vertex + offset);
            gl.End();
        }

        public void Dispose() => Unload();
    }
}
using System;
using System.IO;
using System.Numerics;
using ObjParser;
using SharpGL;
using SharpGL.Enumerations;

namespace OpenGL_Util.Shape3
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
            // todo Needs some work
            if (!Loaded)
                return;
            
            var offset = Position.Vertex();

            for (int h = 0; h < _mesh.FaceList.Count; h++)
            {
                var face = _mesh.FaceList[h];
                
                if (face.VertexIndexList.Length != face.TextureVertexIndexList.Length)
                    continue; // cannot draw face
                
                gl.Begin(BeginMode.Quads);
                for (int i = 0; i < face.VertexIndexList.Length; i++)
                {
                    var vtx = _mesh.VertexList[face.VertexIndexList[i] - 1].Convert() + offset;
                    var tex = _mesh.TextureList[face.TextureVertexIndexList[i] - 1];
                    
                    gl.TexCoord(tex.X, tex.Y);
                    gl.Vertex(vtx.X, vtx.Y, vtx.Z);
                }
                gl.End();
            }
        }

        public void Dispose() => Unload();
        public byte[] ColorArray => throw new NotSupportedException();
    }
}
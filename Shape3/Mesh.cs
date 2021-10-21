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
        public IGameObject GameObject { get; }
        public ITransform Transform { get; }
        private readonly FileInfo _file;
        private readonly Obj _mesh;

        public Mesh(IGameObject gameObject, ITransform transform, FileInfo file)
        {
            GameObject = gameObject;
            Transform = transform;
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
        
        public Vector3 Position => Transform.Position;
        public Quaternion Rotation => Transform.Rotation;
        public Vector3 Scale => Transform.Scale;

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
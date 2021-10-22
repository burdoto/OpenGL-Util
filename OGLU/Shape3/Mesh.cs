using System;
using System.IO;
using System.Numerics;
using SharpGL;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.Serialization;

namespace OpenGL_Util.Shape3
{
    public class Mesh : IRenderObject, ILoadable
    {
        public readonly FileInfo File;

        public Mesh(IGameObject gameObject, ITransform transform, FileInfo file)
        {
            GameObject = gameObject;
            Transform = transform;
            File = file;
        }

        public Polygon Polygon { get; private set; }

        public byte[] ColorArray => throw new NotSupportedException();

        public bool Load()
        {
            Polygon = (SerializationEngine.Instance.LoadScene(File.FullName).SceneContainer.Children[0] as Polygon)!;
            return Loaded = true;
        }

        public void Unload()
        {
        }

        public bool Loaded { get; private set; }

        public void Dispose()
        {
            Unload();
        }

        public IGameObject GameObject { get; }
        public ITransform Transform { get; }

        public Vector3 Position => Transform.Position;
        public Quaternion Rotation => Transform.Rotation;
        public Vector3 Scale => Transform.Scale;

        public void Draw(OpenGL gl, ITransform camera)
        {
            // todo Needs some work
            if (!Loaded)
                return;

            gl.Translate(Position.X, Position.Y, Position.Z);
            Polygon.Render(gl, RenderMode.Render);
            gl.Translate(0, 0, 0); // todo test
        }
    }
}
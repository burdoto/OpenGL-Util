using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace OpenGL_Util.Shape
{
    public class Singularity : ITransform
    {
        public Singularity(Vector3 position) : this(position, Quaternion.Identity)
        {
        }

        public Singularity(Vector3 position, Quaternion rotation) : this(position, rotation, Vector3.One)
        {
        }

        public Singularity(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
    }
}

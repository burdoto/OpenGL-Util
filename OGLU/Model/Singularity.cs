using System.Numerics;

namespace OGLU.Model
{
    public class DeltaTransform : ITransform
    {
        public DeltaTransform(ITransform transform) : this(transform, Vector3.Zero)
        {
        }

        public DeltaTransform(ITransform transform, Vector3 positionDelta) : this(transform, positionDelta,
            Quaternion.Identity)
        {
        }

        public DeltaTransform(ITransform transform, Vector3 positionDelta, Quaternion rotationDelta) : this(transform,
            positionDelta, rotationDelta, Vector3.One)
        {
        }

        public DeltaTransform(ITransform transform, Vector3 positionDelta, Quaternion rotationDelta, Vector3 scaleDelta)
        {
            Transform = transform;
            PositionDelta = positionDelta;
            RotationDelta = rotationDelta;
            ScaleDelta = scaleDelta;
        }

        public ITransform Transform { get; }
        public Vector3 PositionDelta { get; set; }
        public Quaternion RotationDelta { get; set; }
        public Vector3 ScaleDelta { get; set; }
        public Vector3 Position => Transform.Position + PositionDelta;
        public Quaternion Rotation => Transform.Rotation * RotationDelta;
        public Vector3 Scale => Transform.Scale * ScaleDelta;
    }

    public class Singularity : ITransform
    {
        public static readonly Singularity Default = new Singularity();
        
        public Singularity() : this(Vector3.Zero)
        {
        }

        public Singularity(Vector3 position) : this(position, Quaternion.Identity)
        {
        }

        public Singularity(Vector3 position, Quaternion rotation) : this(position, rotation, Vector3.One)
        {
        }

        public Singularity(Vector3 position, Vector3 scale) : this(position, Quaternion.Identity, scale)
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
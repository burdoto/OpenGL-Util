using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace OpenGL_Util.Physics
{
    public enum ColliderType
    {
        // other
        OtherUnknown,
        
        // 2 dimensional
        d2_Rect,
        d2_Circle,
        
        // 3 dimensional
        d3_Sphere,
        d3_Cuboid,
        // todo: d3_Mesh
    }
    
    public interface ICollider : ITransform, ITickable
    {
        ITransform Transform { get; }

        Vector3 ITransform.Position => Transform.Position;
        Quaternion ITransform.Rotation => Transform.Rotation;
        Vector3 ITransform.Scale => Transform.Scale;

        bool CollidesWith(ICollider other);
        bool PointInside(Vector2 point);
        bool PointInside(Vector3 point);
        ColliderType ColliderType { get; }
        IList<ICollider> Colliding { get; }
    }
}
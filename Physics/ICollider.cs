using System.Numerics;

namespace OpenGL_Util.Physics
{
    public enum ColliderType
    {
        // 2 dimensional
        d2_Square,
        d2_Circle,
        
        // 3 dimensional
        d3_Sphere,
        d3_Cuboid,
        // todo: d3_Mesh
    }
    
    public interface ICollider : ITransform
    {
        IGameObject GameObject { get; }

        Vector3 ITransform.Position => GameObject.Position;
        Quaternion ITransform.Rotation => GameObject.Rotation;
        Vector3 ITransform.Scale => GameObject.Scale;

        bool CollidesWith(ICollider other);
        bool PointInside(Vector2 point);
        bool PointInside(Vector3 point);
        ColliderType ColliderType { get; }
    }
}
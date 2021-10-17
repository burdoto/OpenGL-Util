using System;
using System.Numerics;

namespace OpenGL_Util.Physics
{
    public abstract class AbstractCollider : ICollider
    {
        public IGameObject GameObject { get; }
        private readonly ColliderType _type;

        public AbstractCollider(IGameObject gameObject, ColliderType type)
        {
            GameObject = gameObject;
            _type = type;
        }

        public Vector3 Position => GameObject.Position;
        public Quaternion Rotation => GameObject.Rotation;
        public Vector3 Scale => GameObject.Scale;

        public delegate bool AreaCheck(ICollider a, ICollider b);
        public delegate bool Point2Check(ICollider a, Vector2 b);
        public delegate bool Point3Check(ICollider a, Vector3 b);

        public bool CollidesWith(ICollider other) => FindAreaCheck(other.ColliderType)(this, other);
        public bool PointInside(Vector2 point) => FindPoint2Check()(this, point);
        public bool PointInside(Vector3 point) => FindPoint3Check()(this, point);

        public ColliderType ColliderType => _type;

        public AreaCheck FindAreaCheck(ColliderType other) => (ColliderType, other) switch
        {
            (ColliderType.d2_Square, ColliderType.d2_Square) => RectCollider.CheckOverlap,
            (ColliderType.d2_Circle, ColliderType.d2_Circle) => CircleCollider.CheckOverlapping,
            (ColliderType.d2_Square, ColliderType.d2_Circle) => RectCollider.CheckOverlap,
            (ColliderType.d2_Circle, ColliderType.d2_Square) => CircleCollider.CheckOverlapping,
            
            (ColliderType.d3_Cuboid, ColliderType.d3_Cuboid) => CuboidCollider.CheckAABBCC,
            (ColliderType.d3_Sphere, ColliderType.d3_Sphere) => SphereCollider.CheckOverlapping,
            (ColliderType.d3_Cuboid, ColliderType.d3_Sphere) => CuboidCollider.CheckPointsOverlapping,
            (ColliderType.d3_Sphere, ColliderType.d3_Cuboid) => SphereCollider.CheckPointsOverlapping,
            _ => throw new ArgumentOutOfRangeException()
        };
        public Point2Check FindPoint2Check() => ColliderType switch
        {
            ColliderType.d2_Square => RectCollider.CheckPointOnRect,
            ColliderType.d2_Circle => CircleCollider.CheckPointInCircle,
            ColliderType.d3_Sphere => SphereCollider.CheckPointInSphere,
            ColliderType.d3_Cuboid => CuboidCollider.CheckPointInCuboid,
            _ => throw new ArgumentOutOfRangeException()
        };
        public Point3Check FindPoint3Check() => ColliderType switch
        {
            ColliderType.d2_Square => RectCollider.CheckPointOnRect,
            ColliderType.d2_Circle => CircleCollider.CheckPointInCircle,
            ColliderType.d3_Sphere => SphereCollider.CheckPointInSphere,
            ColliderType.d3_Cuboid => CuboidCollider.CheckPointInCuboid,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
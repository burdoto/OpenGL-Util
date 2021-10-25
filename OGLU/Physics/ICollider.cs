using System;
using System.Collections.Generic;
using System.Numerics;

namespace OGLU.Physics
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

        d3_Cuboid
        // todo: d3_Mesh
    }

    public interface ICollider : ITransform, ITickable
    {
        IGameObject GameObject { get; }
        ITransform Transform { get; }
        ColliderType ColliderType { get; }
        ISet<Collision> Collisions { get; }
        bool ActiveCollider { get; set; }

        Vector3 ITransform.Position => Transform.Position;
        Quaternion ITransform.Rotation => Transform.Rotation;
        Vector3 ITransform.Scale => Transform.Scale;

        bool CollidesWith(ICollider other, ref Vector3 v3, bool recursive = false, float z = 0);
        bool PointInside(Vector2 point);
        bool PointInside(Vector3 point);
    }

    public static class ColliderExtensions
    {
        public static bool IsTwoDimensional(this ICollider collider) => collider.ColliderType switch
        {
            ColliderType.d2_Rect => true,
            ColliderType.d2_Circle => true,
            _ => false
        };
    }

    public readonly struct Collision
    {
        public readonly IGameObject Active;
        public readonly IGameObject Other;
        public readonly Vector3 Position;

        public Collision(IGameObject active, IGameObject other, Vector3 point)
        {
            Active = active;
            Other = other;
            Position = point;
        }
    }
}
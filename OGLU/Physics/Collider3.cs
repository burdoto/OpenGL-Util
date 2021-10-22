using System;
using System.Numerics;

namespace OGLU.Physics
{
    public sealed class CuboidCollider : AbstractCollider
    {
        public CuboidCollider(IGameObject gameObject) : this(gameObject, gameObject)
        {
        }

        public CuboidCollider(IGameObject gameObject, ITransform transform) : base(gameObject, transform,
            ColliderType.d3_Sphere)
        {
        }

        public static bool CheckPointInCuboid(ICollider a, Vector2 b)
        {
            return CheckPointInCuboid(a, new Vector3(b, a.Position.Z));
        }

        public static bool CheckPointInCuboid(ICollider rect, Vector3 point)
        {
            throw new NotImplementedException();
        }

        public static bool CheckAABBCC(ICollider rect1, ICollider rect2)
        {
            if (rect1 == rect2)
                return false;
            throw new NotImplementedException();
        }

        public static bool CheckPointsOverlapping(ICollider rect, ICollider check)
        {
            throw new NotImplementedException();
        }

        public override bool CollidesWith(ICollider other, ref Vector3 v3, bool recursive = false, float _ = 0)
        {
            return other.ColliderType == ColliderType.d3_Cuboid
                ? CheckAABBCC(this, other)
                : CheckPointsOverlapping(this, other);
        }

        public override bool PointInside(Vector2 point)
        {
            return CheckPointInCuboid(this, point);
        }

        public override bool PointInside(Vector3 point)
        {
            return CheckPointInCuboid(this, point);
        }
    }

    public sealed class SphereCollider : AbstractCollider
    {
        public SphereCollider(IGameObject gameObject) : this(gameObject, gameObject)
        {
        }

        public SphereCollider(IGameObject gameObject, ITransform transform) : base(gameObject, transform,
            ColliderType.d3_Sphere)
        {
        }

        public static bool CheckPointInSphere(ICollider a, Vector2 b)
        {
            return CheckPointInSphere(a, new Vector3(b, a.Position.Z));
        }

        public static bool CheckPointInSphere(ICollider sphere, Vector3 point)
        {
            throw new NotImplementedException();
        }

        public static bool CheckOverlapping(ICollider sphere1, ICollider sphere2)
        {
            if (sphere1 == sphere2)
                return false;
            throw new NotImplementedException();
        }

        public static bool CheckPointsOverlapping(ICollider sphere, ICollider check)
        {
            return check.ColliderType switch
            {
                ColliderType.d3_Cuboid => CuboidCollider.CheckPointsOverlapping(check, sphere),
                ColliderType.d3_Sphere => CheckOverlapping(sphere, check),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override bool CollidesWith(ICollider other, ref Vector3 v3, bool recursive = false, float _ = 0)
        {
            return other.ColliderType == ColliderType.d3_Sphere
                ? CheckOverlapping(this, other)
                : CheckPointsOverlapping(this, other);
        }

        public override bool PointInside(Vector2 point)
        {
            return CheckPointInSphere(this, point);
        }

        public override bool PointInside(Vector3 point)
        {
            return CheckPointInSphere(this, point);
        }
    }
}
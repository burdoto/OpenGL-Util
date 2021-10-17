using System;
using System.Numerics;

namespace OpenGL_Util.Physics
{
    public sealed class CuboidCollider : AbstractCollider
    {
        public CuboidCollider(IGameObject gameObject) : base(gameObject, ColliderType.d3_Sphere)
        {
        }

        public static bool CheckPointInCuboid(ICollider rect, Vector3 point)
        {
            throw new System.NotImplementedException();
        }

        public static bool CheckAABBCC(ICollider rect1, ICollider rect2)
        {
            throw new System.NotImplementedException();
        }

        public static bool CheckPointsOverlapping(ICollider rect, ICollider check)
        {
            throw new System.NotImplementedException();
        }

        public static bool CheckPointInCuboid(ICollider a, Vector2 b) => CheckPointInCuboid(a, new Vector3(b, a.Position.Z));
    }
    
    public sealed class SphereCollider : AbstractCollider
    {
        public SphereCollider(IGameObject gameObject) : base(gameObject, ColliderType.d3_Sphere)
        {
        }

        public static bool CheckPointInSphere(ICollider sphere, Vector3 point)
        {
            throw new System.NotImplementedException();
        }

        public static bool CheckOverlapping(ICollider sphere1, ICollider sphere2)
        {
            throw new System.NotImplementedException();
        }

        public static bool CheckPointsOverlapping(ICollider sphere, ICollider check) => check.ColliderType switch
            {
                ColliderType.d3_Cuboid => CuboidCollider.CheckPointsOverlapping(check, sphere),
                ColliderType.d3_Sphere => CheckOverlapping(sphere, check),
                _ => throw new ArgumentOutOfRangeException()
            };

        public static bool CheckPointInSphere(ICollider a, Vector2 b) => CheckPointInSphere(a, new Vector3(b, a.Position.Z));
    }
}
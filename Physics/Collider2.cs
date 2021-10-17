using System;
using System.Linq;
using System.Numerics;
using OpenGL_Util.Shape2;

namespace OpenGL_Util.Physics
{   
    public sealed class RectCollider : AbstractCollider
    {
        public RectCollider(IGameObject gameObject) : base(gameObject, ColliderType.d2_Square)
        {
        }

        public static bool CheckOverlap(ICollider rect, ICollider check)
        {
            var v3 = rect.Position + Vector3.Transform(rect.Scale, rect.Rotation);
            var v2 = v3.Vector2();
            if (check.PointInside(v2))
                return true;
            v3 = rect.Position + Vector3.Transform(-rect.Scale, rect.Rotation);
            return check.PointInside(v3.Vector2());
        }
        
        public static bool CheckPointOnRect(ICollider rect, Vector2 point)
        {
            var aa = rect.Position.Vector2() + Vector2.Transform(rect.Scale.Vector2(), rect.Rotation);
            var bb = rect.Position.Vector2() + Vector2.Transform((-rect.Scale).Vector2(), rect.Rotation);
            return aa.X > point.X && aa.Y > point.Y && bb.X < point.X && bb.Y < point.Y;
        }

        public static bool CheckPointOnRect(ICollider a, Vector3 b) => CheckPointOnRect(a, b.Vector2());
    }
    
    public sealed class CircleCollider : AbstractCollider
    {
        public CircleCollider(IGameObject gameObject) : base(gameObject, ColliderType.d2_Circle)
        {
        }

        public static bool CheckOverlapping(ICollider circle, ICollider check)
        {
            switch (check.ColliderType)
            {
                case ColliderType.d2_Circle:
                    var dist = Vector2.Distance(circle.Position.Vector2(), check.Position.Vector2());
                    var r1 = circle.Scale.X;
                    var r2 = check.Scale.X;
                    return r1 - r2 < dist || dist < r1 + r2;
                case ColliderType.d2_Square:
                    return RectCollider.CheckOverlap(check, circle);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static bool CheckPointInCircle(ICollider circle, Vector2 point) => Vector2.Distance(circle.Position.Vector2(), point) < circle.Scale.X;

        public static bool CheckPointInCircle(ICollider a, Vector3 b) => CheckPointInCircle(a, b.Vector2());
    }
}
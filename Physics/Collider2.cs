using System;
using System.Linq;
using System.Numerics;
using OpenGL_Util.Shape2;

namespace OpenGL_Util.Physics
{   
    public sealed class RectCollider : AbstractCollider
    {
        public RectCollider(ITransform transform) : base(transform, ColliderType.d2_Rect)
        {
        }

        public static bool CheckOverlap(ICollider rect, ICollider check)
        {
            if (rect == check)
                return false;
            var v3 = rect.Position + Vector3.Transform(rect.Scale, rect.Rotation);
            var v2 = v3.Vector2();
            if (check.PointInside(v2))
                return true;
            v3 = rect.Position + Vector3.Transform(-rect.Scale, rect.Rotation);
            return check.PointInside(v3.Vector2());
        }
        
        public static bool CheckPointInRect(ICollider rect, Vector2 point)
        {
            var aa = rect.Position.Vector2() + Vector2.Transform(rect.Scale.Vector2(), rect.Rotation);
            var bb = rect.Position.Vector2() + Vector2.Transform((-rect.Scale).Vector2(), rect.Rotation);
            return aa.X > point.X && aa.Y > point.Y && bb.X < point.X && bb.Y < point.Y;
        }

        public static bool CheckPointInRect(ICollider a, Vector3 b) => CheckPointInRect(a, b.Vector2());
        public override bool CollidesWith(ICollider other) => CheckOverlap(this, other);
        public override bool PointInside(Vector2 point) => CheckPointInRect(this, point);

        public override bool PointInside(Vector3 point) => CheckPointInRect(this, point);
    }
    
    public sealed class CircleCollider : AbstractCollider
    {
        public CircleCollider(ITransform transform) : base(transform, ColliderType.d2_Circle)
        {
        }

        public static bool CheckOverlap(ICollider circle, ICollider check)
        {
            if (circle == check)
                return false;
            switch (check.ColliderType)
            {
                case ColliderType.d2_Circle:
                    var dist = Vector2.Distance(circle.Position.Vector2(), check.Position.Vector2());
                    var r1 = circle.Scale.X;
                    var r2 = check.Scale.X;
                    return dist < r1 + r2;
                case ColliderType.d2_Rect:
                    return RectCollider.CheckOverlap(check, circle);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static bool CheckPointInCircle(ICollider circle, Vector2 point) => Vector2.Distance(circle.Position.Vector2(), point) < circle.Scale.X;

        public static bool CheckPointInCircle(ICollider a, Vector3 b) => CheckPointInCircle(a, b.Vector2());
        public override bool CollidesWith(ICollider other) => CheckOverlap(this, other);

        public override bool PointInside(Vector2 point) => CheckPointInCircle(this, point);

        public override bool PointInside(Vector3 point) => CheckPointInCircle(this, point);
    }
}
using System;
using System.Numerics;
using Rndr.Model;

namespace Rndr.Physics
{
    public sealed class RectCollider : AbstractCollider
    {
        public RectCollider(IGameObject gameObject) : this(gameObject, gameObject)
        {
        }

        public RectCollider(IGameObject gameObject, ITransform transform) : base(gameObject, transform,
            ColliderType.d2_Rect)
        {
        }

        public static bool CheckOverlap(ICollider rect, ICollider check, ref Vector3 v3, float z,
            bool recursive = false)
        {
            if (rect == check)
                return false;
            v3 = rect.Position + Vector3.Transform(rect.Scale / 2, rect.Rotation);
            if (check.PointInside(v3.Vector2()))
                return true;
            v3 = rect.Position + Vector3.Transform(-rect.Scale / 2, rect.Rotation);
            if (check.PointInside(v3.Vector2()))
                return true;
            return !recursive && check.CollidesWith(rect, ref v3, true, z);
        }

        public static bool CheckPointInRect(ICollider rect, Vector2 point)
        {
            var aa = rect.Position.Vector2() + Vector2.Transform((rect.Scale / 2).Vector2(), rect.Rotation);
            var bb = rect.Position.Vector2() + Vector2.Transform((-rect.Scale / 2).Vector2(), rect.Rotation);
            return aa.X > point.X && aa.Y > point.Y && bb.X < point.X && bb.Y < point.Y;
        }

        public static bool CheckPointInRect(ICollider a, Vector3 b)
        {
            return CheckPointInRect(a, b.Vector2());
        }

        public override bool CollidesWith(ICollider other, ref Vector3 v3, bool recursive = false, float z = 0)
        {
            return CheckOverlap(this, other, ref v3, z, recursive);
        }

        public override bool PointInside(Vector2 point)
        {
            return CheckPointInRect(this, point);
        }

        public override bool PointInside(Vector3 point)
        {
            return CheckPointInRect(this, point);
        }
    }

    public sealed class CircleCollider : AbstractCollider
    {
        public const int Segments = 50;

        public CircleCollider(IGameObject gameObject) : this(gameObject, gameObject)
        {
        }

        public CircleCollider(IGameObject gameObject, ITransform transform) : base(gameObject, transform,
            ColliderType.d2_Circle)
        {
        }

        public static bool CheckOverlap(ICollider circle, ICollider check, ref Vector3 v3, float z,
            bool recursive = false)
        {
            if (circle == check)
                return false;
            float r1 = circle.Scale.X;
            float r2 = check.Scale.X;
            switch (check.ColliderType, recursive)
            {
                case (ColliderType.d2_Circle, _):
                    float dist = Vector2.Distance(circle.Position.Vector2(), check.Position.Vector2());
                    if (dist < r1 + r2)
                        return true;
                    break;
                case (ColliderType.d2_Rect, false):
                    return RectCollider.CheckOverlap(check, circle, ref v3, z, recursive);
                case (_, true):
                    // in recursion, check all points on circle
                    for (var a = 0; a < Segments; a++)
                    {
                        float theta = 2f * MathF.PI * a / Segments;
                        float x = r1 * MathF.Sin(theta);
                        float y = r1 * MathF.Cos(theta);
                        v3 = new Vector3(circle.Position.X + x, circle.Position.Y + y, z);
                        if (check.PointInside(v3))
                            return true;
                    }

                    return false;
            }

            return !recursive && check.CollidesWith(circle, ref v3, true, z);
        }

        public static bool CheckPointInCircle(ICollider circle, Vector2 point)
        {
            return Vector2.Distance(circle.Position.Vector2(), point) < circle.Scale.X;
        }

        public static bool CheckPointInCircle(ICollider a, Vector3 b)
        {
            return CheckPointInCircle(a, b.Vector2());
        }

        public override bool CollidesWith(ICollider other, ref Vector3 v3, bool recursive = false, float z = 0)
        {
            return CheckOverlap(this, other, ref v3, z, recursive);
        }

        public override bool PointInside(Vector2 point)
        {
            return CheckPointInCircle(this, point);
        }

        public override bool PointInside(Vector3 point)
        {
            return CheckPointInCircle(this, point);
        }
    }
}
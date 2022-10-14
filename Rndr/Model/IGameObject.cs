using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Rndr.Physics;

namespace Rndr.Model;

public interface IGameObject : ITransform, ITickable
{
    Singularity Transform { get; }
    List<IRenderObject> RenderObjects { get; }
    IPhysicsObject? PhysicsObject { get; }
    ICollider? Collider { get; }
    short Metadata { get; set; }
    Vector3 ITransform.Position => Transform.Position;
    Quaternion ITransform.Rotation => Transform.Rotation;
    Vector3 ITransform.Scale => Transform.Scale;
}

public abstract class AbstractGameObject : Container, IGameObject
{
    protected AbstractGameObject(Singularity? transform = null, short metadata = 0)
    {
        Transform = transform ?? new Singularity();
        Metadata = metadata;
    }

    public override IEnumerable<IDisposable> Children => new IDisposable?[] { Collider, PhysicsObject }
        .Where(x => x != null)
        .Select(x => x!)
        .Concat(RenderObjects)
        .Concat(_children);

    public Singularity Transform { get; }

    public List<IRenderObject> RenderObjects { get; } = new();
    public IPhysicsObject? PhysicsObject { get; protected set; }
    public ICollider? Collider { get; protected set; }
    public short Metadata { get; set; }

    public virtual void Dispose()
    {
    }
}
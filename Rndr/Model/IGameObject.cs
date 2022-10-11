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
    protected AbstractGameObject(short metadata = 0) : this(new Singularity(), metadata)
    {
    }

    protected AbstractGameObject(Singularity transform, short metadata = 0)
    {
        Transform = transform;
        Metadata = metadata;
    }

    public override IEnumerable<IDisposable> Children => new IDisposable?[] { Collider, PhysicsObject }
        .Where(x => x != null)
        .Select(x => x!)
        .Concat(_children);

    public Singularity Transform { get; }

    public virtual Vector3 Position => Transform.Position;
    public virtual Quaternion Rotation => Transform.Rotation;
    public virtual Vector3 Scale => Transform.Scale;
    public List<IRenderObject> RenderObjects { get; } = new List<IRenderObject>();
    public IPhysicsObject? PhysicsObject { get; protected set; }
    public ICollider? Collider { get; protected set; }
    public short Metadata { get; set; }

    public virtual void Dispose()
    {
    }
}
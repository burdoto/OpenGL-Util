using System;
using System.Numerics;
using Rndr.Model;

namespace Rndr.Game;

public class Camera : AbstractGameObject
{
    public const short Perspective = 0;
    public const short Orthographic = 1;
    
    public Camera(short cameraType = Perspective) : base(cameraType)
    {
        switch (cameraType)
        {
            case Perspective:
            case Orthographic:
                break;
            default: throw new NotSupportedException($"Unknown camera type: {cameraType}");
        }
    }

    public float Width { get; set; } = 160;
    public float Height { get; set; } = 90;
    public float FOV { get; set; } = 80;
    public float NearPlane { get; set; } = 10;
    public float FarPlane { get; set; } = 1000;
    internal Matrix4x4 ViewMatrix => Matrix4x4.CreateLookAt(Position, Vector3.Transform(Position, Rotation), Vector3.UnitY);
    internal Matrix4x4 ProjectionMatrix => Metadata == Orthographic
        ? Matrix4x4.CreateOrthographic(Width, Height, NearPlane, FarPlane)
        : Matrix4x4.CreatePerspectiveFieldOfView(FOV, Context.Current.window.Size.Aspect(), NearPlane, FarPlane);
    internal Matrix4x4 ProjectedViewMatrix => ViewMatrix * ProjectionMatrix;
}
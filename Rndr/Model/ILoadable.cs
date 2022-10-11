using System;
using Silk.NET.OpenGL;

namespace Rndr.Model;

public interface ILoadable : IDisposable
{
    bool Loaded { get; }
    bool Load(Context ctx);
    void Unload(Context ctx);
}
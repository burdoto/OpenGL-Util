using System;

namespace Rndr.Model;

public interface IEnableable : IDisposable
{
    bool Enabled { get; }

    bool Enable();
    void Disable();
}
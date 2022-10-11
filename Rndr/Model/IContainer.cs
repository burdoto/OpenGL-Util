using System;
using System.Collections.Generic;

namespace Rndr.Model;

public interface IContainer : IDisposable
{
    IEnumerable<IDisposable> Children { get; }

    bool AddChild(IDisposable container);

    bool RemoveChild(IDisposable container);

    IEnumerable<T> GetChildren<T>() where T : IDisposable;
}
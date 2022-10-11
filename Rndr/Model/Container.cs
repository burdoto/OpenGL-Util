using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Silk.NET.OpenGL;

namespace Rndr.Model
{
    public abstract class Container : ITickable
    {
        protected readonly List<IDisposable> _children = new();
        private bool _enabled;
        public bool Loaded { get; private set; }

        public bool Enabled => Loaded && _enabled;
        public virtual IEnumerable<IDisposable> Children => _children;

        public virtual bool Load(Context ctx)
        {
            Debug.WriteLine("Loading " + this);
            foreach (var container in Children)
                if (container is ILoadable loadable)
                    loadable.Load(ctx);
            return Loaded = true;
        }

        public virtual bool Enable()
        {
            Debug.WriteLine("Enabling " + this);
            foreach (var container in Children)
                if (container is IEnableable enableable)
                    enableable.Enable();
            return Loaded && (_enabled = true);
        }

        public virtual void Tick(Context ctx)
        {
            if (!Enabled)
                return;
            foreach (var container in Children)
                if (container is ITickable tickable)
                    tickable.Tick(ctx);
        }

        public virtual void Disable()
        {
            if (!Enabled)
                return;
            Debug.WriteLine("Disabling " + this);
            foreach (var container in Children)
                if (container is IEnableable enableable)
                    enableable.Disable();
            _enabled = false;
        }

        public virtual void Unload(Context ctx)
        {
            if (!Loaded)
                return;
            Debug.WriteLine("Unloading " + this);
            foreach (var container in Children)
                if (container is ILoadable loadable)
                    loadable.Unload(ctx);
            Loaded = false;
        }

        public virtual void Dispose()
        {
            Disable();
            Unload(Context.Current);
        }


        public virtual bool AddChild(IDisposable container)
        {
            if (_children.Contains(container))
                return false;
            _children.Add(container);
            return true;
        }

        public virtual bool RemoveChild(IDisposable container)
        {
            return _children.Remove(container);
        }

        public IEnumerable<T> GetChildren<T>() where T : IDisposable
        {
            return _children.Where(it => it.GetType() == typeof(T)).Cast<T>();
        }
    }
}
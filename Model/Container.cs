using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenGL_Util.Model
{
    public abstract class Container : ITickable
    {
        protected readonly List<IDisposable> _children = new List<IDisposable>();
        private bool _loaded, _enabled;
        public bool Loaded => _loaded;
        public bool Enabled => Loaded && _enabled;
        public virtual IEnumerable<IDisposable> Children => _children;

        public virtual bool Load()
        {
            Debug.WriteLine("Loading " + this);
            foreach (var container in _children)
                if (container is ILoadable loadable)
                    loadable.Load();
            return !Loaded && (_loaded = _Load());
        }

        public virtual bool Enable()
        {
            Debug.WriteLine("Enabling " + this);
            foreach (var container in _children)
                if (container is IEnableable enableable)
                    enableable.Enable();
            return Loaded && (_enabled = _Enable());
        }

        public virtual void Tick()
        {
            if (!Enabled)
                return;
            _Tick();
            foreach (var container in _children)
                if (container is ITickable tickable)
                    tickable.Tick();
        }

        public virtual void Disable()
        {
            if (!Enabled)
                return;
            Debug.WriteLine("Disabling " + this);
            foreach (var container in _children)
                if (container is IEnableable enableable)
                    enableable.Disable();
            _Disable();
            _enabled = false;
        }

        public virtual void Unload()
        {
            if (!Loaded)
                return;
            Debug.WriteLine("Unloading " + this);
            foreach (var container in _children)
                if (container is ILoadable loadable)
                    loadable.Unload();
            _Unload();
            _loaded = false;
        }

        public virtual void Dispose()
        {
            Disable();
            Unload();
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

        public IEnumerable<T> GetChildren<T>() where T : IDisposable => _children.Where(it => it.GetType() == typeof(T)).Cast<T>();

        protected virtual bool _Load() => true;
        protected virtual bool _Enable() => true;

        protected virtual void _Tick() {}

        protected virtual void _Disable() {}

        protected virtual void _Unload() {}
    }
}
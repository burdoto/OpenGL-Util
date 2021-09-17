using System.Collections.Generic;

namespace OpenGL_Util.Model
{
    public abstract class Container : ITickable
    {
        private readonly List<IContainer> _children = new List<IContainer>();
        private bool _loaded, _enabled;
        public bool Loaded => _loaded;
        public bool Enabled => Loaded && _enabled;
        public IEnumerable<IContainer> Children => _children;

        public bool Load()
        {
            foreach (var container in _children)
                if (container is ITickable tickable)
                    tickable.Load();
            return !Loaded && (_loaded = _Load());
        }

        public bool Enable()
        {
            foreach (var container in _children)
                if (container is ITickable tickable)
                    tickable.Enable();
            return Loaded && (_enabled = _Enable());
        }

        public void Tick()
        {
            if (!Enabled)
                return;
            _Tick();
            foreach (var container in _children)
                if (container is ITickable tickable)
                    tickable.Tick();
        }

        public void Disable()
        {
            if (!Enabled)
                return;
            foreach (var container in _children)
                if (container is ITickable tickable)
                    tickable.Disable();
            _Disable();
            _enabled = false;
        }

        public void Unload()
        {
            if (!Loaded)
                return;
            foreach (var container in _children)
                if (container is ITickable tickable)
                    tickable.Unload();
            _Unload();
            _loaded = false;
        }

        public void Dispose()
        {
            Disable();
            Unload();
        }


        public bool AddChild(IContainer container)
        {
            if (_children.Contains(container))
                return false;
            _children.Add(container);
            return true;
        }

        public bool RemoveChild(IContainer container)
        {
            return _children.Remove(container);
        }

        protected virtual bool _Load() => true;
        protected virtual bool _Enable() => true;

        protected virtual void _Tick() {}

        protected virtual void _Disable() {}

        protected virtual void _Unload() {}
    }
}
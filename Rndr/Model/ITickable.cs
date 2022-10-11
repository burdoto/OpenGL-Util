using Rndr.Model;

namespace Rndr;

public interface ITickable : IContainer, ILoadable, IEnableable
{
    void Tick(Context ctx);
}
using Rndr.Game;
using Silk.NET.OpenGL;

namespace Rndr.Model;

public interface IDrawable
{
    void Draw(Context ctx, Camera camera, double frameTime);
}
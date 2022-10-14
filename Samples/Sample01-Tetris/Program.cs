using System.Numerics;
using Rndr;
using Rndr.Game;
using Rndr.Model;
using Rndr.Shape2;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Sample01_Tetris;

public class Program
{
    public static void Main(string[] args)
    {
        var assemblyDir = new FileInfo(typeof(Program).Assembly.Location).DirectoryName!;
        var win = Window.Create(WindowOptions.Default);
        var ctx = new Context(win);

        var vertexSource = File.ReadAllText(Path.Combine(assemblyDir, "Assets", "vertex.glsl"));
        var fragmentSource = File.ReadAllText(Path.Combine(assemblyDir, "Assets", "fragment.glsl"));
        ctx.LoadShaderPack("simple", (ShaderType.VertexShader, vertexSource), (ShaderType.FragmentShader, fragmentSource))
            .Link()
            .Use();
        
        ctx.Run(new TetrisGame());
    }
}

public sealed class TetrisGame : GameBase
{
    public TetrisGame()
    {
        AddChild(new TestObj());
    }

    public override Camera Camera { get; } = new();
}

public sealed class TestObj : AbstractGameObject
{
    public TestObj()
    {
        RenderObjects.Add(new Rect(this));
    }
}

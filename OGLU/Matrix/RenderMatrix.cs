using SharpGL;

namespace OGLU.Matrix
{
    public class RenderMatrix : IDrawable
    {
        public readonly GridMatrix Grid;

        public RenderMatrix() : this(new MapMatrix())
        {
        }

        public RenderMatrix(GridMatrix grid)
        {
            Grid = grid;
        }

        public void Draw(OpenGL gl, ITransform camera)
        {
            //Debug.WriteLine("Drawing Matrix");
            foreach (var draw in Grid.GetVisibles(camera))
                draw?.Draw(gl, camera);
        }

        public IGameObject AddGameObject(IGameObject it)
        {
            return Grid[it.Position] = it;
        }

        public void AddGameObjects(params IGameObject[] objs)
        {
            foreach (var obj in objs)
                AddGameObject(obj);
        }

        public void Clear()
        {
            Grid.Clear();
        }
    }
}
using LyCilph.Controllers;

namespace LyCilph.Elements
{
    public class Scene
    {
        public Board Board { get; private set; }
        public Food Food { get; private set; }
        public Snake Snake { get; private set; }
        public SnakeController Controller { get; private set; }
    }
}

using LyCilph.Elements;

namespace LyCilph.Controllers
{
    public abstract class Controller
    {
        protected Cell food;
        protected Snake snake;

        protected Controller(Cell food, Snake snake)
        {
            this.food = food;
            this.snake = snake;
        }

        public virtual void HandleInput(InputManager input) { }
    }
}

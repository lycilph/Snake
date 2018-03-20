using LyCilph.Elements;
using LyCilph.Utils;
using Microsoft.Xna.Framework.Input;

namespace LyCilph.Controllers
{
    public class PlayerController : Controller
    {
        public PlayerController(Cell food, Snake snake) : base(food, snake) { }

        public override void HandleInput(InputManager input)
        {
            if (input.IsPressed(Keys.Up) && snake.Dir != Direction.Down)
                snake.Dir = Direction.Up;

            if (input.IsPressed(Keys.Right) && snake.Dir != Direction.Left)
                snake.Dir = Direction.Right;

            if (input.IsPressed(Keys.Down) && snake.Dir != Direction.Up)
                snake.Dir = Direction.Down;

            if (input.IsPressed(Keys.Left) && snake.Dir != Direction.Right)
                snake.Dir = Direction.Left;
        }
    }
}

using LyCilph.Elements;
using LyCilph.Utils;

namespace LyCilph.Controllers
{
    public class SimpleAIController : Controller
    {
        public SimpleAIController(Cell food, Snake snake) : base(food, snake) { }

        public override void HandleInput(InputManager input)
        {
            switch (snake.Dir)
            {
                case Direction.Right:
                    if (snake.Head.X == (Settings.board_size - 1) || FindFood(0, 1))
                        snake.Dir++;
                    break;
                case Direction.Down:
                    if (snake.Head.Y == (Settings.board_size - 1) || FindFood(-1, 0))
                        snake.Dir++;
                    break;
                case Direction.Left:
                    if (snake.Head.X == 0 || FindFood(0, -1))
                        snake.Dir = Direction.Up;
                    break;
                case Direction.Up:
                    if (snake.Head.Y == 0 || FindFood(1, 0))
                        snake.Dir++;
                    break;
            }
        }

        private bool FindFood(int x, int y)
        {
            var p = new Cell(snake.Head);
            p.Add(x, y);

            while (p.X >= 0 && p.X < Settings.board_size && p.Y >= 0 && p.Y < Settings.board_size)
            {
                if (food.Hit(p))
                    return true;

                p.Add(x, y);
            }

            return false;
        }
    }
}

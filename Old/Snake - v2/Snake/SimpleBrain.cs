using Microsoft.Xna.Framework.Input;

namespace Snake
{
    public class SimpleBrain : Brain
    {
        public SimpleBrain(Board board, Food food, Snake snake) : base(board, food, snake)
        {
        }

        public override void React(KeyboardState new_state, KeyboardState old_state)
        {
            switch (snake.Dir)
            {
                case Direction.Right:
                    if (snake.Head.X == (board.BoardSizeInCells - 1) || FindFood(0, 1))
                        snake.Dir++;
                    break;
                case Direction.Down:
                    if (snake.Head.Y == (board.BoardSizeInCells - 1) || FindFood(-1, 0))
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

            while (p.X >= 0 && p.X < board.BoardSizeInCells && p.Y >= 0 && p.Y < board.BoardSizeInCells)
            {
                if (food.Hit(p))
                    return true;

                p.Add(x, y);
            }

            return false;
        }
    }
}

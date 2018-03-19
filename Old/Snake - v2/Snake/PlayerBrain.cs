using Microsoft.Xna.Framework.Input;

namespace Snake
{
    public class PlayerBrain : Brain
    {
        public PlayerBrain(Board board, Food food, Snake snake) : base(board, food, snake)
        {
        }

        public override void React(KeyboardState new_state, KeyboardState old_state)
        {
            // These will only be called when the key is first pressed
            if (old_state.IsKeyUp(Keys.Up) && new_state.IsKeyDown(Keys.Up) && snake.Dir != Direction.Down)
                snake.Dir = Direction.Up;
            if (old_state.IsKeyUp(Keys.Right) && new_state.IsKeyDown(Keys.Right) && snake.Dir != Direction.Left)
                snake.Dir = Direction.Right;
            if (old_state.IsKeyUp(Keys.Down) && new_state.IsKeyDown(Keys.Down) && snake.Dir != Direction.Up)
                snake.Dir = Direction.Down;
            if (old_state.IsKeyUp(Keys.Left) && new_state.IsKeyDown(Keys.Left) && snake.Dir != Direction.Right)
                snake.Dir = Direction.Left;
        }
    }
}

using Microsoft.Xna.Framework.Input;

namespace Snake
{
    public class SnakePlayer : SnakeBase
    {
        public void UpdateInput(KeyboardState new_state, KeyboardState old_state)
        {
            // These will only be called when the key is first pressed
            if (old_state.IsKeyUp(Keys.Up) && new_state.IsKeyDown(Keys.Up) && dir != Direction.Down)
            {
                dir = Direction.Up;
            }
            if (old_state.IsKeyUp(Keys.Right) && new_state.IsKeyDown(Keys.Right) && dir != Direction.Left)
            {
                dir = Direction.Right;
            }
            if (old_state.IsKeyUp(Keys.Down) && new_state.IsKeyDown(Keys.Down) && dir != Direction.Up)
            {
                dir = Direction.Down;
            }
            if (old_state.IsKeyUp(Keys.Left) && new_state.IsKeyDown(Keys.Left) && dir != Direction.Right)
            {
                dir = Direction.Left;
            }
        }
    }
}

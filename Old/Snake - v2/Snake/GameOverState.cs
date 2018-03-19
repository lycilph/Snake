using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Snake
{
    public class GameOverState : MenuGameState
    {
        private Snake snake;

        public GameOverState(SnakeGame game, int screen_width, Snake snake) : base(game, screen_width)
        {
            this.snake = snake;
        }

        public override void UpdateInput(KeyboardState new_state, KeyboardState old_state)
        {
            if (old_state.IsKeyUp(Keys.C) && new_state.IsKeyDown(Keys.C))
                game.TransitionToMenuState();
        }

        public override void Draw(SpriteBatch sprite_batch)
        {

            sprite_batch.DrawString(header_font, "Game Over", new Vector2(text_x, 220), Color.Black);
            sprite_batch.DrawString(font, $"Score: {snake.Score}", new Vector2(text_x, 250), Color.Black);
            sprite_batch.DrawString(font, $"Age: {snake.Age}", new Vector2(text_x, 270), Color.Black);
            sprite_batch.DrawString(font, $"Death: {snake.CauseOfDeath}", new Vector2(text_x, 290), Color.Black);

            sprite_batch.DrawString(font, "(C)ontinue", new Vector2(text_x, 320), Color.Black);
        }
    }
}

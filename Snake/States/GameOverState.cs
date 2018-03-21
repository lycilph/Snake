using System;
using LyCilph.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LyCilph.States
{
    public class GameOverState : State
    {
        private SpriteFont header_font;
        private SpriteFont font;

        private Snake snake;

        public GameOverState(StateManager state_manager) : base(state_manager) { }

        public void Set(Snake snake)
        {
            this.snake = snake;
        }

        public override void LoadContent(ContentManager content)
        {
            header_font = content.Load<SpriteFont>("header_font");
            font = content.Load<SpriteFont>("font");
        }

        public override void HandleInput(InputManager input)
        {
            if (input.IsPressed(Keys.C) || input.IsPressed(Keys.Escape) || input.IsPressed(Keys.Back))
                state_manager.TransitionToMainMenuState();
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            var text_size = header_font.MeasureString("Game Over");
            var x = (Settings.screen_width - text_size.X) / 2;

            sprite_batch.DrawString(header_font, "Game Over", new Vector2(x, 200), Color.Black);
            sprite_batch.DrawString(font, $"Score: {snake.Score}", new Vector2(x, 250), Color.Black);
            sprite_batch.DrawString(font, $"Age: {snake.Age}", new Vector2(x, 270), Color.Black);
            sprite_batch.DrawString(font, $"Death: {snake.CauseOfDeath}", new Vector2(x, 290), Color.Black);

            sprite_batch.DrawString(font, "(C)ontinue", new Vector2(x, 320), Color.Black);
        }
    }
}

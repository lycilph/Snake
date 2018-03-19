using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Snake
{
    public class MenuGameState : GameElement
    {
        protected SpriteFont header_font;
        protected SpriteFont font;
        protected int screen_width;
        protected float text_x;

        public MenuGameState(SnakeGame game, int screen_width) : base(game)
        {
            this.screen_width = screen_width;
        }

        public override void LoadContent()
        {
            header_font = game.Content.Load<SpriteFont>("header_font");
            font = game.Content.Load<SpriteFont>("font");

            var header_size = header_font.MeasureString("Snake");
            text_x = (screen_width - header_size.X) / 2;
        }

        public override void UpdateInput(KeyboardState new_state, KeyboardState old_state)
        {
            if (old_state.IsKeyUp(Keys.N) && new_state.IsKeyDown(Keys.N))
                game.TransitionToGameState();
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            sprite_batch.DrawString(header_font, "Snake", new Vector2(text_x, 220), Color.Black);
            sprite_batch.DrawString(font, "(N)ew Game", new Vector2(text_x, 250), Color.Black);
            sprite_batch.DrawString(font, "(Esc) Quit", new Vector2(text_x, 270), Color.Black);
        }
    }
}

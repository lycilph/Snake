using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LyCilph.States
{
    public class MainMenuState : GameElement
    {
        private SpriteFont header_font;
        private SpriteFont font;
        private float text_start;

        public MainMenuState(SnakeGame game) : base(game) { }

        public override void LoadContent()
        {
            header_font = game.Content.Load<SpriteFont>("header_font");
            font = game.Content.Load<SpriteFont>("font");

            var header_size = header_font.MeasureString("Snake");
            text_start = (game.ScreenWidth - header_size.X) / 2;
        }

        public override void UpdateInput(KeyboardState new_state, KeyboardState old_state)
        {
            if (old_state.IsKeyUp(Keys.D1) && new_state.IsKeyDown(Keys.D1))
                game.TransitionToGameState();
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            sprite_batch.DrawString(header_font, "Snake", new Vector2(text_start, 220), Color.Black);
            sprite_batch.DrawString(font, "(1) Player Game", new Vector2(text_start, 270), Color.Black);
            sprite_batch.DrawString(font, "(2) Simple AI Game", new Vector2(text_start, 290), Color.Black);
            sprite_batch.DrawString(font, "(3) Neural Network AI Game", new Vector2(text_start, 310), Color.Black);
            sprite_batch.DrawString(font, "(Esc) Quit", new Vector2(text_start, 330), Color.Black);
        }
    }
}

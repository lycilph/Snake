using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace LyCilph.Elements
{
    public class Debug : GameElement
    {
        private SpriteFont font;
        private int frame_rate = 0;
        private int frame_counter = 0;
        private TimeSpan elapsed_time = TimeSpan.Zero;
        private bool enabled = false;

        public Debug(SnakeGame game) : base(game) { }

        public override void LoadContent()
        {
            font = game.Content.Load<SpriteFont>("font");
        }

        public override void UpdateInput(KeyboardState new_state, KeyboardState old_state)
        {
            if (old_state.IsKeyUp(Keys.F3) && new_state.IsKeyDown(Keys.F3))
                enabled = !enabled;
        }

        public override void UpdateLogic(GameTime game_time)
        {
            elapsed_time += game_time.ElapsedGameTime;

            if (elapsed_time > TimeSpan.FromSeconds(1))
            {
                elapsed_time -= TimeSpan.FromSeconds(1);
                frame_rate = frame_counter;
                frame_counter = 0;
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            if (!enabled)
                return;

            frame_counter++;
            sprite_batch.DrawString(font, $"fps {frame_rate} mem: {GC.GetTotalMemory(false)}", new Vector2(5, 5), Color.Black);
        }
    }
}

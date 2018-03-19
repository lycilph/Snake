using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Snake
{
    public class FPSComponent : DrawableGameComponent
    {
        private SpriteBatch sprite_batch;
        private SpriteFont sprite_font;

        private int frame_rate = 0;
        private int frame_counter = 0;
        private TimeSpan elapsed_time = TimeSpan.Zero;

        public FPSComponent(Game game, SpriteBatch batch, SpriteFont font) : base(game)
        {
            sprite_font = font;
            sprite_batch = batch;
        }

        public override void Update(GameTime game_time)
        {
            elapsed_time += game_time.ElapsedGameTime;

            if (elapsed_time > TimeSpan.FromSeconds(1))
            {
                elapsed_time -= TimeSpan.FromSeconds(1);
                frame_rate = frame_counter;
                frame_counter = 0;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            frame_counter++;

            string fps = string.Format("fps: {0} mem: {1}", frame_rate, GC.GetTotalMemory(false));

            sprite_batch.DrawString(sprite_font, fps, new Vector2(5, 5), Color.Black);
        }
    }
}

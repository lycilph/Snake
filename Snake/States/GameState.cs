using LyCilph.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace LyCilph.States
{
    public class GameState : GameElement
    {
        private SpriteFont font;

        private double fps;
        private double interval;
        private double delta;

        public Board Board { get; private set; }
        public Food Food { get; private set; }
        public Snake Snake { get; private set; }

        public GameState(SnakeGame game) : base(game)
        {
            Board = new Board(game);
            Food = new Food(game);
            Snake = new Snake(game);

            interval = game.Settings.IntervalStart;
            fps = 1000.0 / interval;
        }

        public override void LoadContent()
        {
            font = game.Content.Load<SpriteFont>("font");

            Board.LoadContent();
            Food.LoadContent();
            Snake.LoadContent();
        }

        public override void UpdateInput(KeyboardState new_state, KeyboardState old_state)
        {
            if (old_state.IsKeyUp(Keys.Add) && new_state.IsKeyDown(Keys.Add))
            {
                interval = Math.Min(game.Settings.IntervalMax, interval + game.Settings.IntervalChange);
                fps = 1000.0 / interval;
            }

            if (old_state.IsKeyUp(Keys.Subtract) && new_state.IsKeyDown(Keys.Subtract))
            {
                interval = Math.Max(game.Settings.IntervalMin, interval - game.Settings.IntervalChange);
                fps = 1000.0 / interval;
            }
        }

        public override void UpdateLogic(GameTime game_time)
        {
            delta += game_time.ElapsedGameTime.TotalMilliseconds;
            if (delta < interval)
                return;
            delta = 0.0;
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            Board.Draw(sprite_batch);
            Food.Draw(sprite_batch);
            Snake.Draw(sprite_batch);

            sprite_batch.DrawString(font, $"(+/-) fps: {fps:N1}", new Vector2(game.TextAreaStart, game.ScreenHeight - 20 - game.Margin), Color.Black);
        }
    }
}

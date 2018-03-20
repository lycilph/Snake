using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LyCilph.Elements
{
    public class Food : GameElement
    {
        private Random rnd = new Random((int)DateTime.Now.Ticks);
        private Texture2D part;

        public int X { get; set; }
        public int Y { get; set; }

        public Food(SnakeGame game) : base(game)
        {
            Random();
        }

        public override void LoadContent()
        {
            part = game.Content.Load<Texture2D>("circle");
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            var size = game.Settings.CellSize;
            var margin = game.Margin;
            sprite_batch.Draw(part, new Rectangle(X * size + margin, Y * size + margin, size, size), Color.Red);
        }

        public void Random()
        {
            X = rnd.Next(0, game.Settings.Size);
            Y = rnd.Next(0, game.Settings.Size);
        }
    }
}

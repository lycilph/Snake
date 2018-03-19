using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Snake
{
    [DebuggerDisplay("{X}, {Y}")]
    public class Food : Cell
    {
        private Random rnd = new Random((int)DateTime.Now.Ticks);
        private Texture2D part;

        public Food()
        {
            Random();
        }

        public void LoadContent(ContentManager content)
        {
            part = content.Load<Texture2D>("food");
        }

        public void Random()
        {
            X = rnd.Next(0, 40);
            Y = rnd.Next(0, 30);
        }

        public void Draw(SpriteBatch sprite_batch)
        {
            sprite_batch.Draw(part, new Rectangle(X * 20, Y * 20, 20, 20), Color.Red);
        }
    }
}

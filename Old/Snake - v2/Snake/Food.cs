using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Snake
{
    [DebuggerDisplay("{X}, {Y}")]
    public class Food : GameElement
    {
        private Random rnd = new Random((int)DateTime.Now.Ticks);
        private Board board;
        private Texture2D part;

        public int X { get; set; }
        public int Y { get; set; }

        public Food(SnakeGame game, Board board) : base(game)
        {
            this.board = board;
            Random();
        }

        public override void LoadContent()
        {
            part = game.Content.Load<Texture2D>("circle");
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            var size = board.CellSizeInPixels;
            sprite_batch.Draw(part, new Rectangle(X * size, Y * size, size, size), Color.Red);
        }

        public void Random()
        {
            X = rnd.Next(0, board.BoardSizeInCells);
            Y = rnd.Next(0, board.BoardSizeInCells);
        }

        public bool Hit(Cell c)
        {
            return X == c.X && Y == c.Y;
        }
    }
}

using System;
using System.Diagnostics;

namespace LyCilph
{
    [DebuggerDisplay("{X}, {Y}")]
    public class Cell
    {
        private static Random rnd = new Random((int)DateTime.Now.Ticks);

        public int X { get; set; }
        public int Y { get; set; }

        public Cell() : this(0, 0) {}
        public Cell(Cell c) : this(c.X, c.Y) {}
        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Random()
        {
            X = rnd.Next(0, Settings.board_size);
            Y = rnd.Next(0, Settings.board_size);
        }

        public Cell Add(int x, int y)
        {
            X += x;
            Y += y;
            return this;
        }

        public bool Hit(Cell c)
        {
            return X == c.X && Y == c.Y;
        }
    }
}

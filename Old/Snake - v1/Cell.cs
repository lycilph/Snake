using System.Diagnostics;

namespace Snake
{
    [DebuggerDisplay("{X}, {Y}")]
    public class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Cell() : this(0, 0) {}
        public Cell(Cell c) : this(c.X, c.Y) {}
        public Cell(int x, int y)
        {
            X = x;
            Y = y;
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

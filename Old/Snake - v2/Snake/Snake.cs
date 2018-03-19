using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Snake
{
    public class Snake : GameElement
    {
        private Board board;
        private Texture2D part;
        private LinkedList<Cell> body = new LinkedList<Cell>();

        private int energy_in_food = 250;

        public int Dir { get; set; }
        public bool HasEaten { get; set; }
        public Cell Head { get { return body.First(); } }
        public int Score { get { return body.Count - 2; } } // The snake start with 2 segments, and so should not be counted
        public int Age { get; set; }
        public int Energy { get; set; }
        public string CauseOfDeath { get; set; }

        public Snake(SnakeGame game, Board board) : base(game)
        {
            this.board = board;
            Reset();
        }

        public override void LoadContent()
        {
            part = game.Content.Load<Texture2D>("circle");
        }
        
        public override void Draw(SpriteBatch sprite_batch)
        {
            var size = board.CellSizeInPixels;

            // Draw head
            var head = body.First();
            sprite_batch.Draw(part, new Rectangle(head.X * size, head.Y * size, size, size), Color.Black);

            // Draw body
            foreach (var body in body.Skip(1))
            {
                sprite_batch.Draw(part, new Rectangle(body.X * size, body.Y * size, size, size), Color.Gray);
            }
        }

        public void Reset()
        {
            body.Clear();
            body.AddLast(new Cell(board.Center, board.Center));
            body.AddLast(new Cell(board.Center - 1, board.Center));
            Dir = Direction.Right;
            HasEaten = false;
            Age = 0;
            Energy = energy_in_food;
        }

        public void Move()
        {
            var old_head = body.First();

            Cell new_head;
            if (HasEaten)
            {
                new_head = new Cell();
                HasEaten = false;
                Energy = energy_in_food;
            }
            else
            {
                new_head = body.Last();
                body.RemoveLast();
            }

            switch (Dir)
            {
                case Direction.Up:
                    new_head.X = old_head.X;
                    new_head.Y = old_head.Y - 1;
                    break;
                case Direction.Right:
                    new_head.X = old_head.X + 1;
                    new_head.Y = old_head.Y;
                    break;
                case Direction.Down:
                    new_head.X = old_head.X;
                    new_head.Y = old_head.Y + 1;
                    break;
                case Direction.Left:
                    new_head.X = old_head.X - 1;
                    new_head.Y = old_head.Y;
                    break;
            }
            body.AddFirst(new_head);

            Age++;
            Energy--;
        }

        public bool HitSelf()
        {
            // Skipping 3 here, as the snake cannot hit it's own head or the first 2 segments
            var head = body.First();
            foreach (var segment in body.Skip(3))
            {
                if (head.X == segment.X && head.Y == segment.Y)
                    return true;
            }
            return false;
        }

        public bool Hit(Cell c)
        {
            foreach (var segment in body)
            {
                if (segment.X == c.X && segment.Y == c.Y)
                    return true;
            }
            return false;
        }
    }
}

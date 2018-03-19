using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Snake
{
    public class SnakeBase
    {
        protected Texture2D snake_part;

        protected LinkedList<Cell> body = new LinkedList<Cell>();
        protected int dir;

        public bool HasEaten { get; set; }
        public Cell Head { get { return body.First(); } }
        public int Score { get { return body.Count - 2; } } // The snake start with 2 segments, and so should not be counted

        public SnakeBase()
        {
            Reset();
        }

        public void LoadContent(ContentManager content)
        {
            snake_part = content.Load<Texture2D>("snake_part");
        }

        public void Reset()
        {
            body.Clear();
            body.AddLast(new Cell(20, 15));
            body.AddLast(new Cell(19, 15));
            dir = Direction.Right;
            HasEaten = false;
        }

        public void Move()
        {
            var old_head = body.First();

            Cell new_head;
            if (HasEaten)
            {
                new_head = new Cell();
                HasEaten = false;
            }
            else
            {
                new_head = body.Last();
                body.RemoveLast();
            }

            switch (dir)
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
        }

        public bool HitBody()
        {
            // Skipping 3 here, as the snake cannot hit it's own head
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
                if (c.X == segment.X && c.Y == segment.Y)
                    return true;
            }
            return false;
        }

        public void Draw(SpriteBatch sprite_batch)
        {
            // Draw head
            var head = body.First();
            sprite_batch.Draw(snake_part, new Rectangle(head.X * 20, head.Y * 20, 20, 20), Color.Black);

            // Draw body
            foreach (var body in body.Skip(1))
            {
                sprite_batch.Draw(snake_part, new Rectangle(body.X * 20, body.Y * 20, 20, 20), Color.Gray);
            }
        }
    }
}

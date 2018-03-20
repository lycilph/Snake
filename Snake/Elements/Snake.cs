using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace LyCilph.Elements
{
    public class Snake : GameElement
    {
        private Texture2D part;
        private LinkedList<Cell> body = new LinkedList<Cell>();

        public int Dir { get; set; }
        public bool HasEaten { get; set; }
        public Cell Head { get { return body.First(); } }
        public int Score { get { return body.Count - 2; } } // The snake start with 2 segments, and so should not be counted
        public int Age { get; set; }
        public int Energy { get; set; }
        public string CauseOfDeath { get; set; }

        public Snake(SnakeGame game) : base(game)
        {
            Reset();
        }

        public override void LoadContent()
        {
            part = game.Content.Load<Texture2D>("circle");
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            var size = game.Settings.CellSize;
            var margin = game.Margin;

            // Draw head
            sprite_batch.Draw(part, new Rectangle(Head.X * size + margin, Head.Y * size + margin, size, size), Color.Black);

            // Draw body
            foreach (var body in body.Skip(1))
                sprite_batch.Draw(part, new Rectangle(body.X * size + margin, body.Y * size + margin, size, size), Color.Gray);
        }

        public void Reset()
        {
            var center = game.Settings.Size / 2;

            body.Clear();
            body.AddLast(new Cell(center, center));
            body.AddLast(new Cell(center - 1, center));
            Dir = Direction.Right;
            HasEaten = false;
            Age = 0;
            Energy = game.Settings.EnergyPerFood;
        }

        public void Move()
        {
            var old_head = body.First();

            Cell new_head;
            if (HasEaten)
            {
                new_head = new Cell();
                HasEaten = false;
                Energy = game.Settings.EnergyPerFood;
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
    }
}

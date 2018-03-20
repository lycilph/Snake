﻿using System.Collections.Generic;
using System.Linq;

namespace LyCilph
{
    public class Snake
    {
        public LinkedList<Cell> Body { get; private set; }
        public Cell Head { get { return Body.First(); } }
        public int Score { get { return Body.Count - 2; } } // The snake start with 2 segments, and so should not be counted

        public int Dir { get; set; }
        public bool HasEaten { get; set; }
        public int Age { get; set; }
        public int Energy { get; set; }
        public string CauseOfDeath { get; set; }

        public Snake()
        {
            Body = new LinkedList<Cell>();
        }

        public void Reset()
        {
            var center = Settings.board_size / 2;

            Body.Clear();
            Body.AddLast(new Cell(center, center));
            Body.AddLast(new Cell(center - 1, center));

            Dir = Direction.Right;
            HasEaten = false;
            Age = 0;
            Energy = Settings.energy_per_food;
        }

        public void Move()
        {
            var old_head = Body.First();

            Cell new_head;
            if (HasEaten)
            {
                new_head = new Cell();
                HasEaten = false;
                Energy = Settings.energy_per_food;
            }
            else
            {
                new_head = Body.Last();
                Body.RemoveLast();
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
            Body.AddFirst(new_head);

            Age++;
            Energy--;
        }

    }
}

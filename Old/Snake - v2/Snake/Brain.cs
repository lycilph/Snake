using Microsoft.Xna.Framework.Input;

namespace Snake
{
    public abstract class Brain
    {
        protected Board board;
        protected Food food;
        protected Snake snake;

        public Brain(Board board, Food food, Snake snake)
        {
            this.board = board;
            this.food = food;
            this.snake = snake;
        }

        public virtual void Reset() { }

        public virtual void React(KeyboardState new_state, KeyboardState old_state) { }
    }
}

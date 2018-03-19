using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Snake
{
    public class GameElement
    {
        protected SnakeGame game;

        public GameElement(SnakeGame game)
        {
            this.game = game;
        }

        public virtual void LoadContent() { }
        public virtual void UpdateInput(KeyboardState new_state, KeyboardState old_state) { }
        public virtual void UpdateLogic(GameTime game_time) { }
        public virtual void Draw(SpriteBatch sprite_batch) { }
    }
}

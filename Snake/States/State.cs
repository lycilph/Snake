using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LyCilph.States
{
    public abstract class State
    {
        protected StateManager state_manager;

        protected State(StateManager state_manager)
        {
            this.state_manager = state_manager;
        }

        public virtual void Reset() { }
        public virtual void LoadContent(ContentManager content) { }
        public virtual void HandleInput(InputManager input) { }
        public virtual void Update(GameTime game_time) { }
        public virtual void Draw(SpriteBatch sprite_batch) { }
    }
}
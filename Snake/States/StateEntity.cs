using Microsoft.Xna.Framework.Content;

namespace LyCilph.States
{
    public abstract class StateEntity : Entity
    {
        protected StateManager state_manager;

        protected StateEntity(StateManager state_manager)
        {
            this.state_manager = state_manager;
        }

        public virtual void LoadContent(ContentManager content) { }
        public virtual void HandleInput(InputManager input) { }
        public virtual void Reset() { }
    }
}
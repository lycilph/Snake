using LyCilph.States;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LyCilph
{
    public class StateManager
    {
        private StateEntity main_menu_state;
        private StateEntity game_state;

        public StateEntity CurrentState { get; private set; }

        public StateManager(GraphicsDevice graphics_device)
        {
            main_menu_state = new MainMenuState(this);
            game_state = new GameState(this, graphics_device);
        }

        public void LoadContent(ContentManager content)
        {
            main_menu_state.LoadContent(content);
            game_state.LoadContent(content);
        }

        public void TransitionToGameState()
        {
            CurrentState = game_state;
            CurrentState.Reset();
        }

        public void TransitionToGameOverState()
        {
        }

        public void TransitionToMainMenuState()
        {
            CurrentState = main_menu_state;
            CurrentState.Reset();
        }
    }
}

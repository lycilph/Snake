using Microsoft.Xna.Framework.Input;

namespace LyCilph
{
    public class InputManager
    {
        private KeyboardState new_state;
        private KeyboardState old_state;

        public void Begin()
        {
            new_state = Keyboard.GetState();
        }

        public void End()
        {
            old_state = new_state;
        }

        public bool IsPressed(Keys key)
        {
            return old_state.IsKeyUp(key) && new_state.IsKeyDown(key);
        }
    }
}

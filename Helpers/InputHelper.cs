using Microsoft.Xna.Framework.Input;

namespace Stellamod.Helpers
{
    public static class InputHelper
    {
        public static bool KeyUp(Keys keys)
        {
            KeyboardState keyboard = Keyboard.GetState();
            return keyboard.IsKeyUp(keys);
        }

        public static bool KeyDown(Keys keys)
        {
            KeyboardState keyboard = Keyboard.GetState();
            return keyboard.IsKeyDown(keys);
        }


    }
}

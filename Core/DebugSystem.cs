using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Stellamod.Core.Camera;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core
{
    public class DebugSystem : ModSystem
    {
        private bool _pressed;
        private bool _lock;
        private Vector2 _lockPosition;
        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            if (InputHelper.KeyDown(Keys.OemComma))
            {
                _pressed = true;

            }
            if (InputHelper.KeyUp(Keys.OemComma) && !_pressed)
            {
                _pressed = false;
                _lock = !_lock;
                _lockPosition = Main.Camera.Center;
            }

            if (_lock)
            {
                RetargetCameraModifier.ReTargetPosition = _lockPosition;
            }
        }
    }
}

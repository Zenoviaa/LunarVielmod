using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Stellamod.Core.Camera;
using Stellamod.Helpers;
using System;
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
            if (InputHelper.KeyUp(Keys.J) && _pressed)
            {
                _pressed = false;
                _lock = !_lock;
                _lockPosition = Main.Camera.Center;
            }
            if (InputHelper.KeyDown(Keys.J))
            {
                _pressed = true;

            }

 
            if (_lock)
            {
                CameraTargetSystem.AddTarget(_lockPosition);
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace Stellamod.Core.Camera
{
    public class RetargetCameraModifier : ICameraModifier
    {
        private Vector2 _cameraOffset;
        private float _timer;
        private static bool _shouldRetarget;
        public string UniqueIdentity { get; private set; }

        public bool Finished => false;

        private static Vector2 _newTarget;
        public static Vector2 ReTargetPosition
        {
            get
            {
                return _newTarget;
            }
            set
            {
                _shouldRetarget = true;
                _newTarget = value;
            }
        }

        public void Update(ref CameraInfo cameraPosition)
        {
            if (!_shouldRetarget)
            {
                _timer--;
                if (_timer < 0)
                {
                    _timer = 0;
                }
          
            }
            else
            {
                _timer++;
                if (_timer > 60f)
                {
                    _timer = 60f;
                }
            }



            Vector2 targetPosition = (_newTarget - cameraPosition.CameraPosition);
            Vector2 screenBounds = new Vector2(Main.screenWidth, Main.screenHeight);
            screenBounds *= 0.5f;
            targetPosition -= screenBounds;

            _cameraOffset = Vector2.Lerp(Vector2.Zero, targetPosition, EasingFunction.InOutSine(_timer / 60f));
            _shouldRetarget = false;
            cameraPosition.CameraPosition = cameraPosition.CameraPosition + _cameraOffset;
        }
    }
}

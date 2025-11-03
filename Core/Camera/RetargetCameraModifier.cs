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
        private Vector2 _lastTarget;
        private float _timer;
        public string UniqueIdentity { get; private set; }

        public bool Finished => false;

        public static Vector2 NewTarget;

        public void Update(ref CameraInfo cameraPosition)
        {
            if (NewTarget != Vector2.Zero)
                _lastTarget = NewTarget;
            if (NewTarget == Vector2.Zero)
            {
                _timer--;
            }
            else
            {
                _timer++;
            }

            if (_timer <= 0)
                _timer = 0;
            if (_timer >= 60f)
                _timer = 60f;


            Vector2 target = NewTarget == Vector2.Zero ? _lastTarget : NewTarget;
            Vector2 targetPosition = (target - cameraPosition.OriginalCameraPosition);
            Vector2 screenBounds = new Vector2(Main.screenWidth, Main.screenHeight);
            screenBounds *= 0.5f;
            targetPosition -= screenBounds;

            _cameraOffset = Vector2.Lerp(Vector2.Zero, targetPosition, EasingFunction.InOutSine(_timer / 60f));
            NewTarget = Vector2.Zero;
            cameraPosition.CameraPosition = cameraPosition.OriginalCameraPosition + _cameraOffset;
        }
    }
}

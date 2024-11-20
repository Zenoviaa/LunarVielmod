using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace Stellamod.Common.Camera
{
    internal class FocusCameraModifier : ICameraModifier
    {
        private float _timer;
        private float _duration;
        private Vector2? _focusTargetPosition;
        private Entity _entity;

        public string UniqueIdentity { get; private set; }
        public bool Finished { get; private set; }

        public FocusCameraModifier(Entity entity, float duration, string uniqueIdentity = null)
        {
            _timer = 0;
            _entity = entity;
            _duration = duration;
            UniqueIdentity = uniqueIdentity;
        }

        public FocusCameraModifier(Vector2 focusTargetPosition, float duration, string uniqueIdentity = null)
        {
            _timer = 0;
            _focusTargetPosition = focusTargetPosition;
            _duration = duration;
            UniqueIdentity = uniqueIdentity;
        }

        public void Update(ref CameraInfo cameraPosition)
        {
            _timer++;
            float transitionDuration = _duration / 4f;
            float exitStart = _duration - transitionDuration;
            float transitionProgress = 1;
            if (_timer < transitionDuration)
            {
                float enterProgress = _timer / transitionDuration;
                float easedEnterProgress = Easing.InOutCubic(enterProgress);
                transitionProgress = easedEnterProgress;

            }
            else if (_timer > exitStart)
            {
                float exitProgress = (_timer - exitStart) / transitionDuration;
                exitProgress = 1f - exitProgress;
                float easedExitProgress = Easing.InOutCubic(exitProgress);
                transitionProgress = easedExitProgress;
            }


            //Get the target position
            Vector2 targetPos = _entity != null ? _entity.Center : _focusTargetPosition.Value;
            targetPos.X -= Main.screenWidth * 0.5f;
            targetPos.Y -= Main.screenHeight * 0.5f;
            Vector2 focusCameraPosition = Vector2.Lerp(cameraPosition.CameraPosition, targetPos, transitionProgress);
            cameraPosition.CameraPosition = focusCameraPosition;


            if (_focusTargetPosition == null)
            {
                if (_entity == null || !_entity.active || _timer >= _duration)
                {
                    Finished = true;
                }
            }
            else if (_timer >= _duration)
            {
                Finished = true;
            }
        }
    }
}

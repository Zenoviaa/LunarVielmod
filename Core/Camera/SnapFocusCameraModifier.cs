using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace Stellamod.Core.Camera
{
    internal class SnapFocusCameraModifier : ICameraModifier
    {
        private float _timer;
        private float _duration;
        private Vector2? _focusTargetPosition;
        private Entity _entity;

        public string UniqueIdentity { get; private set; }
        public bool Finished { get; private set; }

        public SnapFocusCameraModifier(Entity entity, float duration, string uniqueIdentity = null)
        {
            _timer = 0;
            _entity = entity;
            _duration = duration;
            UniqueIdentity = uniqueIdentity;
        }

        public SnapFocusCameraModifier(Vector2 focusTargetPosition, float duration, string uniqueIdentity = null)
        {
            _timer = 0;
            _focusTargetPosition = focusTargetPosition;
            _duration = duration;
            UniqueIdentity = uniqueIdentity;
        }

        public void Update(ref CameraInfo cameraPosition)
        {
            _timer++;

            //Get the target position
            Vector2 targetPos = _entity != null ? _entity.Center : _focusTargetPosition.Value;
            targetPos.X -= Main.screenWidth * 0.5f;
            targetPos.Y -= Main.screenHeight * 0.5f;
            cameraPosition.CameraPosition = targetPos;


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

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using Terraria.ModLoader;

namespace Stellamod.Common.Camera
{
    internal class SmoothCameraModifier : ICameraModifier
    {
        private Vector2? _oldCameraPosition;
        public string UniqueIdentity => "crystalmoon_camerasmooth";
        public bool Finished => false;

        public void Update(ref CameraInfo cameraPosition)
        {
            _oldCameraPosition ??= cameraPosition.OriginalCameraPosition;
            float maxSmoothValue = 0.03f;
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            float smoothValue = MathHelper.Lerp(1f, maxSmoothValue, config.CameraSmoothness / 100f);
            float distance = Vector2.Distance(_oldCameraPosition.Value, cameraPosition.CameraPosition);
            if (distance >= Main.screenWidth)
            {

            }
            else
            {
                cameraPosition.CameraPosition = Vector2.Lerp(_oldCameraPosition.Value, cameraPosition.CameraPosition, smoothValue);
            }

            _oldCameraPosition = cameraPosition.CameraPosition;
        }
    }
}

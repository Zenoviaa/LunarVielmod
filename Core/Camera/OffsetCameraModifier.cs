using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace Stellamod.Core.Camera
{
    public class OffsetCameraModifier : ICameraModifier
    {
        private Vector2 _cameraOffset;
        public string UniqueIdentity { get; private set; }

        public bool Finished => false;

        public static Vector2 FocusTargetOffset;

        public void Update(ref CameraInfo cameraPosition)
        {
            _cameraOffset = Vector2.Lerp(_cameraOffset, FocusTargetOffset, 0.1f);
            FocusTargetOffset = Vector2.Zero;
            cameraPosition.CameraPosition = cameraPosition.OriginalCameraPosition + _cameraOffset; 
        }
    }
}

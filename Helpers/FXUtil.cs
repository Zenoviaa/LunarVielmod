using Microsoft.Xna.Framework;
using Stellamod.Common.Camera;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace Stellamod.Helpers
{
    internal static class FXUtil
    {
        public static void FocusCamera(Vector2 position, float duration, string uniqueIdentity = null)
        {
            FocusCameraModifier focusCameraModifier = new FocusCameraModifier(position, duration, uniqueIdentity);
            Main.instance.CameraModifiers.Add(focusCameraModifier);
        }

        public static void FocusCamera(Entity entity, float duration, string uniqueIdentity = null)
        {
            FocusCameraModifier focusCameraModifier = new FocusCameraModifier(entity, duration, uniqueIdentity);
            Main.instance.CameraModifiers.Add(focusCameraModifier);
        }
        public static void ShakeCamera(Vector2 position, float distance, float strength, string uniqueIdentity = null)
        {
            ScreenshakeCameraModifier screenshakeCameraModifier = new ScreenshakeCameraModifier(
                position,
                distance,
                strength,
                uniqueIdentity);
            Main.instance.CameraModifiers.Add(screenshakeCameraModifier);
        }

        public static void PunchCamera(Vector2 startPosition, Vector2 direction, float strength, float vibrationCyclesPerSecond, int frames, float distanceFallOff = -1, string uniqueIdentity = null)
        {
            PunchCameraModifier punchCameraModifier = new PunchCameraModifier(
                startPosition,
                direction,
                strength,
                vibrationCyclesPerSecond,
                frames,
                distanceFallOff,
                uniqueIdentity);
            Main.instance.CameraModifiers.Add(punchCameraModifier);
        }
    }
}

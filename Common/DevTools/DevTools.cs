using Microsoft.Xna.Framework.Input;
using Terraria;

namespace Stellamod.Common.DevTools;

public static class DevTools
{
    public static int spawnNPCType;
    public static float LightIntensity;
    public static GodMode godMode;
    public static bool infiniteFlight;
    public static bool infiniteReach;
    public static FreeCamera freeCamera;
    public static bool infiniteMana;
    public static bool showTimeSlider;
    public static Vector2 freeCameraPosition;
    public static void UpdateFreeCameraPosition()
    {
        if (DevTools.freeCamera == FreeCamera.Freeze)
        {

        }
        else if (DevTools.freeCamera == FreeCamera.Free && !Main.blockInput)
        {
            float speed = 30f;
            if (Main.keyState.IsKeyDown(Keys.LeftAlt))
                speed *= .3f;
            if (Main.keyState.IsKeyDown(Keys.LeftShift))
                speed *= 1.5f;
            if (DevToolPlayer.left)
                DevTools.freeCameraPosition.X -= speed;
            if (DevToolPlayer.down)
                DevTools.freeCameraPosition.Y += speed;
            if (DevToolPlayer.right)
                DevTools.freeCameraPosition.X += speed;
            if (DevToolPlayer.up)
                DevTools.freeCameraPosition.Y -= speed;
        }
        else
        {

            DevTools.freeCameraPosition = Main.screenPosition;
        }
    }
}

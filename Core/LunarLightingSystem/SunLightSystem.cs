using Stellamod.Common.Shaders;
using System;
using System.Threading;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem;

public static class LightingGlobals
{
    public static Vector2 ShadowDirection;
    public static Color ShadowColor;
    public static Color SunColor;
}

public class SunLightSystem : ModSystem
{
    private static float _overSunTimer;
    private static float _daylightFadeTimer;
    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();
        Update();
    }

    private static void Update()
    {
        Point point = Main.LocalPlayer.position.ToTileCoordinates();
        bool overworld = (double)point.Y <= Main.worldSurface;
        if (!overworld)
        {
            _overSunTimer--;
            if (_overSunTimer <= 0)
                return;
        }
        else
        {
            _overSunTimer++;
        }


        _overSunTimer = MathHelper.Clamp(_overSunTimer, 0, 120);
        float interpolant = _overSunTimer / 120f;
        Vector2 sunLeft = Main.Camera.Center + new Vector2(-Main.screenWidth / 2, -Main.screenHeight / 2);
        Vector2 sunRight = Main.Camera.Center + new Vector2(Main.screenWidth / 2, -Main.screenHeight / 2);

        float dayProgress = Main.dayTime ? (float)Main.time / (float)Main.dayLength : (float)Main.time / (float)Main.nightLength;
        float radians = MathHelper.Lerp(MathHelper.ToRadians(-45), MathHelper.ToRadians(45), dayProgress);
        Vector2 sunDirection = Vector2.UnitY.RotatedBy(radians) * 400;
        if (dayProgress <= 0.1f || dayProgress >= 0.9f)
        {

            _daylightFadeTimer--;
        }
        else
        {
            _daylightFadeTimer++;
        }


        _daylightFadeTimer = MathHelper.Clamp(_daylightFadeTimer, 0, 120);
        float shadowDaylightFadeInterpolant = _daylightFadeTimer / 120f;

        Vector2 sunPosition = Main.Camera.Center + new Vector2(0, 0);
        LightingGlobals.SunColor = Main.ColorOfTheSkies * interpolant;
        LightingGlobals.ShadowDirection = sunDirection.SafeNormalize(Vector2.Zero);
        LightingGlobals.ShadowColor = Color.Black * 0.05f * shadowDaylightFadeInterpolant;
    }
}

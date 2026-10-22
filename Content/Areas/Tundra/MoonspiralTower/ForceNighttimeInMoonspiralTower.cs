using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower;

[Autoload(Side = ModSide.Client)]
public class ForceNighttimeInMoonspiralTower : ModSystem
{
    double _realTime;
    bool _realDayTime;
    float _lerpTimer;
    float _lerpValue;
    float _scrollingTicks;
    public static bool isActive;

    public override void Load()
    {
        base.Load();
        On_Main.DoDraw += FakeTimeChange;
        On_Main.DrawInfernoRings += PutTimeChangeBack;
    }


    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        _lerpTimer += isActive ? 1 : -1;
        var ticks = 180f;
        _lerpTimer = MathHelper.Clamp(_lerpTimer, 0, ticks);
        _lerpValue = _lerpTimer / ticks;
        _scrollingTicks = (float)(Main.dayLength + Main.nightLength * 0.5D);
    }

    /// <summary>
    /// Returns a time value that clamps at daylength + nightlength
    /// </summary>
    /// <returns></returns>
    public double GetUnwrappedTime()
    {
        double unwrappedTime = 0.0D;
        if (Main.dayTime)
        {
            unwrappedTime += Main.time;
            return unwrappedTime;
        }
        else
        {
            unwrappedTime += Main.dayLength;
            unwrappedTime += Main.time;
        }
        return unwrappedTime;
    }

    public (double time, bool isDaytime) UnwrappedTimeToTimeOfDay(double unwrappedTime)
    {
        bool isDayTime = unwrappedTime < Main.dayLength;
        if (!isDayTime)
        {
            unwrappedTime -= Main.dayLength;
        }

        return (unwrappedTime, isDayTime);

    }
    private void FakeTimeChange(On_Main.orig_DoDraw orig, Main self, GameTime gameTime)
    {
        _realTime = Main.time;
        double unwrappedTime = GetUnwrappedTime();
        _realDayTime = Main.dayTime;
        var unwrappedTimeToScrollTo = _scrollingTicks;
        var newUnwrappedTime = MathHelper.Lerp((float)unwrappedTime, (float)unwrappedTimeToScrollTo, EasingFunction.InOutSine(_lerpValue));
        var timeOfDay = UnwrappedTimeToTimeOfDay(newUnwrappedTime);

        Main.time = timeOfDay.time;
        Main.dayTime = timeOfDay.isDaytime;

        orig(self, gameTime);


    }
    private void PutTimeChangeBack(On_Main.orig_DrawInfernoRings orig, Main self)
    {

        orig(self);
        Main.time = _realTime;
        Main.dayTime = _realDayTime;
    }
}

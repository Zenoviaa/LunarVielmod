using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Helpers;

public record struct ShockwaveParams(int rippleCount, int rippleSize, int rippleSpeed, float distortStrength, Vector2 rippleCenter)
{
    public static readonly ShockwaveParams Default = new ShockwaveParams(20, 5, 15, 300, Vector2.Zero);
}

public class EffectsHelper : ModSystem
{

    private static int _rippleCount;
    private static int _rippleSize;
    private static int _rippleSpeed;
    private static float _distortStrength;
    private static Vector2 _rippleCenter;
    private static int _bee;
    private static Filter ShockwaveFilter => Terraria.Graphics.Effects.Filters.Scene["Shockwave"];
    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        if (Main.netMode == NetmodeID.Server)
            return;


        if(_bee > 0)
        {
            if (!ShockwaveFilter.IsActive())
            {
                Terraria.Graphics.Effects.Filters.Scene.Activate("Shockwave", _rippleCenter).GetShader().UseColor(_rippleCount, _rippleSize, _rippleSpeed).UseTargetPosition(_rippleCenter);
            }

            if (ShockwaveFilter.IsActive())
            {
                float progress = (180f - _bee) / 60f; // Will range from -3 to 3, 0 being the point where the bomb explodes.
                ShockwaveFilter.GetShader().UseProgress(progress).UseOpacity(_distortStrength * (1 - progress / 3f));
            }

            _bee--;
        }
    }

    public static void StartShockwave(ShockwaveParams shockwaveParams)
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        _rippleCenter = shockwaveParams.rippleCenter;
        _rippleSize = shockwaveParams.rippleSize;
        _rippleSpeed = shockwaveParams.rippleSpeed; ;
        _distortStrength = shockwaveParams.distortStrength;
        _rippleCount = shockwaveParams.rippleCount;
        _bee = 180;
        if (ShockwaveFilter.IsActive())
        {
            ShockwaveFilter.Deactivate();
        }
    }
}

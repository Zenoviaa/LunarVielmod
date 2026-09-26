using System;
using Terraria;

namespace Stellamod.Core.Utilities;

public static class GoreAI
{
    /// <summary>
    /// Looks like it's being blown around basically, and slowly fades out
    /// </summary>
    /// <param name="gore"></param>
    public static void FeatherAI(Gore gore)
    {
        gore.velocity *= 0.93f;
        gore.velocity.Y = MathHelper.Lerp(gore.velocity.Y, 0.5f, 0.03f);
        gore.velocity.X += MathF.Sin(Main.GameUpdateCount * 0.03f) * 0.03f;
        gore.position += gore.velocity;
        gore.rotation = Utils.AngleLerp(gore.rotation, gore.velocity.ToRotation() - MathHelper.PiOver2, 0.03f);
        gore.timeLeft--;
        gore.alpha += 2;
        if (gore.timeLeft <= 0)
            gore.active = false;
    }
}

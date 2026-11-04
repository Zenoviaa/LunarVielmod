using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.WitchySystem;

public class PotionHealthPlayer : ModPlayer
{
    public Color bloodColor;
    public Color darkBloodColor;
    public float hueShift;
    public static float hurtTimer;
    public override void ResetEffects()
    {
        base.ResetEffects();
        bloodColor = Color.Red;
        darkBloodColor = Color.DarkRed;
        hueShift = 0;
    }

    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        if (hurtTimer > 0)
            hurtTimer--;
    }
    public override void PostHurt(Player.HurtInfo info)
    {
        base.PostHurt(info);
        hurtTimer = 30;
    }
}

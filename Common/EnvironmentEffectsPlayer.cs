using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common;

/// <summary>
/// Implements effects specific to the environment such as reduced breath thime
/// </summary>
public class EnvironmentEffectsPlayer : ModPlayer
{
    public const float REDUCED_BREATH_EFFECTIVENESS = 8;
    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        Player.breathEffectiveness /= REDUCED_BREATH_EFFECTIVENESS;
    }
}

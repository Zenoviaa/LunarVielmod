using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.BossesWS;

public class NeuroZap : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.pvpBuff[Type] = true;
        Main.buffNoTimeDisplay[Type] = false;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.lifeRegen -= 64;
        if (Main.rand.NextBool(4))
        {
            var z = ElectricZapParticle.Spawn(
                player.Center + Main.rand.NextVector2Circular(32, 32),
                Main.rand.NextVector2Circular(2, 2), Scale: Main.rand.NextFloat(0.3f, 0.6f));
            z.Scale *= 0.5f;
        }
        if (Main.rand.NextBool(8))
        {
            FXUtil.GlowCircleBoom(player.Center, Color.White, Color.SkyBlue, Color.DarkBlue, duration: 30f, baseSize: 0.16f);
        }
    }
}

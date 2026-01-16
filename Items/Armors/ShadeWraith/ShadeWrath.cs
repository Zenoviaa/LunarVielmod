using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.ShadeWraith
{
    public class ShadeWrath : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Melee) += 0.3f;
            player.moveSpeed += 0.3f;
            if (Main.rand.NextBool(3))
            {
                SmokeParticle sp = Particle<SmokeParticle>.Spawn(player.position + new Vector2(Main.rand.Next(0, player.width), Main.rand.Next(0, player.height)), -Vector2.UnitY, Color.OrangeRed, Main.rand.NextFloat(0.9f, 1.5f));
                sp.initialColor = Color.Lerp(Color.Purple, Color.Violet, Main.rand.NextFloat(0f, 1f)) * 0.4f;
                sp.expand = true;
            }
            if (Main.rand.NextBool(3))
            {
                var ember = LegacyParticle.NewParticle<EmberParticle>(player.position + new Vector2(Main.rand.Next(0, player.width), Main.rand.Next(0, player.height)), -Vector2.UnitY.RotatedByRandom(1.5f), Color.Purple, Main.rand.NextFloat(0.9f, 1.5f));
                ember.innerColor = Color.LightPink;
                ember.outerColor = Color.Purple;
            }
        }
    }

    public class ShadeWrathCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
    }
}

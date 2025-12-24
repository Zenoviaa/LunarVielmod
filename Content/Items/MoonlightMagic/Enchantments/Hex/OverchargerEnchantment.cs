using Microsoft.Xna.Framework;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Hex
{
    public class OverchargerEnchantment : BaseEnchantment
    {
        public override int GetElementType()
        {
            return ModContent.ItemType<HexElement>();
        }

        public override void AI_Charge(AdvancedMagicPlayer player, AdvancedMagicStaffHold hold)
        {
            base.AI_Charge(player, hold);
            if (hold.IsOvercharging())
            {
               
                float damagePerTick = 0.04f / 60f;
                player.chargeDamageBonus += damagePerTick;
                player.overchargingVisual = true;
                if (Main.rand.NextBool(8))
                {
                    Vector2 dustPos = hold.Projectile.Center + Main.rand.NextVector2CircularEdge(32, 32);
                    Vector2 vel = (hold.Projectile.Center - dustPos) * 0.05f;
                    dustPos += hold.Projectile.velocity * 64;
                    LegacyParticle.NewParticle<SparkleWindParticle>(dustPos, vel, Color.White, Scale: Main.rand.NextFloat(1f, 2f));
                }
            }
        }
    }
}

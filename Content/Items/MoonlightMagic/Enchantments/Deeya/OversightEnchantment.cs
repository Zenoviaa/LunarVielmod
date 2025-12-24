using Microsoft.Xna.Framework;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Deeya
{
    public class OversightEnchantment : BaseEnchantment
    {
        public override int GetElementType()
        {
            return ModContent.ItemType<DeeyaElement>();
        }

        public override void AI_Charge(AdvancedMagicPlayer player, AdvancedMagicStaffHold hold)
        {
            base.AI_Charge(player, hold);
            if (hold.IsOvercharging())
            {

                float damagePerTick = 0.02f / 60f;
                float widthPerTick = 0.1f / 60f;

                player.chargeDamageBonus += damagePerTick;
                player.chargeWidthBonus += widthPerTick;
                player.overchargingVisual = true;
                if (Main.rand.NextBool(8))
                {
                    Vector2 dustPos = hold.Projectile.Center + Main.rand.NextVector2CircularEdge(32, 32);
                    Vector2 vel = (hold.Projectile.Center - dustPos) * 0.05f;
                    dustPos += hold.Projectile.velocity * 64;
                    LegacyParticle.NewParticle<BloodSparkleParticle>(dustPos, vel, Color.White, Scale: Main.rand.NextFloat(1f, 2f));
                }
            }
        }
    }
}

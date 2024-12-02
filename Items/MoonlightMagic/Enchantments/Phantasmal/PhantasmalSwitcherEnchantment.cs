using Microsoft.Xna.Framework;
using Stellamod.Common.Particles;
using Stellamod.Items.MoonlightMagic.Elements;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Phantasmal
{
    internal class PhantasmalSwitcherEnchantment : BaseEnchantment
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            time = 45;
        }

        public override void AI()
        {
            base.AI();

            //Count up
            Countertimer++;

            //If greater than time then start homing, we'll just swap the movement type of the projectile
            if (Countertimer == time)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 spawnPoint = Projectile.Center + Main.rand.NextVector2Circular(8, 8);
                    Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                    Particle.NewParticle<GlowParticle>(spawnPoint, velocity, Color.Turquoise);
                }

                MagicProj.PrimaryElement = new PhantasmalElement();

            }
        }

        public override float GetStaffManaModifier()
        {
            return 0.2f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<PhantasmalElement>();
        }
    }
}

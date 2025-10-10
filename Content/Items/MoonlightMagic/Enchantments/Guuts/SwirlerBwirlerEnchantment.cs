using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Forms;
using Stellamod.Helpers;
using Stellamod.Core.Helpers.Math;
using Stellamod.Core.ItemTemplates;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Visual.Particles;
using Stellamod.Trails;
using Terraria;
using Terraria.ModLoader;
namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Guuts
{
    public class SwirlerBwirlerEnchantment : BaseEnchantment
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            time = 30;
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
                    Particle.NewParticle<GlowParticle>(spawnPoint, velocity, Color.White);
                }

                Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.TwoPi);
            }
        }

        public override float GetStaffManaModifier()
        {
            return 0.2f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<GuutElement>();
        }
    }
}

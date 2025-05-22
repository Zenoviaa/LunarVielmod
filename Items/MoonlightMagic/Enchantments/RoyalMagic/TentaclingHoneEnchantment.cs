using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.MoonlightMagic.Elements;
using Stellamod.Items.MoonlightMagic.Movements;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.RoyalMagic
{
    internal class TentaclingHoneEnchantment : BaseEnchantment
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            time = 15;
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
                    //   Particle.NewParticle<SparkleWindParticle>(spawnPoint, velocity, Color.White);
                }

                MagicProj.Movement = new TentaclingMovement();
            }
        }

        public override float GetStaffManaModifier()
        {
            return 0.4f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<RoyalMagicElement>();
        }
    }
}

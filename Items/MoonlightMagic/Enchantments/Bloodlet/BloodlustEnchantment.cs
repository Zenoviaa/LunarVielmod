using Stellamod.Items.MoonlightMagic.Elements;
using Stellamod.Common.Particles;
using Stellamod.Visual.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Bloodlet
{
    internal class BloodlustEnchantment : BaseEnchantment
    {
        
        public override void SetDefaults()
        {
            base.SetDefaults();
            time = 45;
        }

        public override void AI()
        {
            base.AI();
            Countertimer++;
            if(Countertimer == time)
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 spawnPoint = Projectile.Center + Main.rand.NextVector2Circular(8, 8);
                    Vector2 velocity = Main.rand.NextVector2Circular(8, 8);

                    Color color = Color.White;
                    color.A = 0;
                    Particle.NewBlackParticle<BloodSparkleParticle>(spawnPoint, velocity, color);
                }

                float damage = Projectile.damage;
                damage *= 1.25f;
                Projectile.damage = (int)damage;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Player player = Main.player[Projectile.owner];
            player.statLife -= 25;
            for (int i = 0; i < 12; i++)
            {
                Vector2 spawnPoint = player.Center + Main.rand.NextVector2Circular(8, 8);
                Vector2 velocity = Main.rand.NextVector2Circular(8, 8);

                Color color = Color.White;
                color.A = 0;
                Particle.NewBlackParticle<BloodSparkleParticle>(spawnPoint, velocity, color);
            }
            CombatText.NewText(player.getRect(), Color.Red, "-25", true);
            return true;
        }

        public override float GetStaffManaModifier()
        {
            return 0.13f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<BloodletElement>();
        }
    }
}

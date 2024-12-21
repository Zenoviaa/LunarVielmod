using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage.Tomes
{
    internal class ShinobiTome : BaseMagicTomeItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.shoot = ModContent.ProjectileType<ShinobiTomeTome>();
            Item.shootSpeed = 16f;
        }
    }

    internal class ShinobiTomeTome : BaseMagicTomeProjectile
    {
        private float _dustTimer;
        public override string Texture => this.PathHere() + "/ShinobiTome";
        public override void SetDefaults()
        {
            base.SetDefaults();
            //How often it shoots
            AttackRate = 12;

            //How fast it drains mana, better to change the mana use in the item instead of this tho
            ManaConsumptionRate = 4;

            //How far the tome is held from the player
            HoldDistance = 36;

            //The glow effect around it
            GlowDistanceOffset = 4;
            GlowRotationSpeed = 0.05f;
        }

        public override void AI()
        {
            base.AI();
            _dustTimer++;
            if (_dustTimer % 16 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.White, Main.rand.NextFloat(1f, 1.5f));
            }
        }

        protected override void Shoot(Player player, IEntitySource source, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Shoot(player, source, position, velocity, damage, knockback);
            if (Main.myPlayer == Projectile.owner)
            {
                float numberProjectiles = 5;
                float rotation = MathHelper.ToRadians(14);
                position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
                for (int i = 0; i < numberProjectiles; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // This defines the projectile roatation and speed. .4f == projectile speed
                    Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<ShinobiKnife>(), damage, knockback, player.whoAmI);
                }
            }
        }
    }
}
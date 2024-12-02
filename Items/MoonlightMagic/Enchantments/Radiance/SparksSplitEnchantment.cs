using Microsoft.Xna.Framework;
using Stellamod.Items.MoonlightMagic.Elements;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Radiance
{
    internal class SparksSplitEnchantment : BaseEnchantment
    {

        public override void SetDefaults()
        {
            base.SetDefaults();

        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //Spawn the explosion
            if (!MagicProj.IsClone)
            {
                //Count up


                //If greater than time then start homing, we'll just swap the movement type of the projectile

                int count = 6;
                for (float i = 0; i < count; i++)
                {
                    float progress = i / count;
                    float angle = MathHelper.PiOver4;
                    float fireRot = progress * angle;
                    Vector2 fireDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    Vector2 firePoint = Projectile.Center;
                    Vector2 fireVelocity = fireDirection.RotatedBy(fireRot - angle / 2f) * Projectile.velocity.Length() * 0.5f;
                    AdvancedMagicUtil.CloneMagicProjectile(MagicProj, firePoint, -fireVelocity, Projectile.damage / count, Projectile.knockBack / count,
                        MagicProj.TrailLength / count, MagicProj.Size / count);
                }

                Projectile.Kill();

            }
            return true;
        }

        public override void AI()
        {
            base.AI();

        }

        public override float GetStaffManaModifier()
        {
            return 0.3f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<RadianceElement>();
        }
    }
}

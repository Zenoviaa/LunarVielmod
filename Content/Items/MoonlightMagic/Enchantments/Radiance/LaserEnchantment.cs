using Microsoft.Xna.Framework;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Radiance
{
    public class LaserEnchantment : BaseEnchantment
    {

        public override void SetMagicDefaults()
        {
            base.SetMagicDefaults();
            MagicProj.TrailLength += 384;
            MagicProj.Projectile.timeLeft *= 2;
            MagicProj.Projectile.penetrate += 1000;
            MagicProj.extraScale += 0.4f;
            MagicProj.killTime = 460;
            MagicProj.tileHitCount += 1;
        }

        public override void AI()
        {
            base.AI();
             
            //Count up

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * 16;
            if (MagicProj.Projectile.extraUpdates == 0)
            {
                Projectile.extraUpdates += 16;
                MagicProj.damagingTrail = true;
                MagicProj.laserLike = true;
            }
            else
            {
                Projectile.extraUpdates += 1;
            }
        }

        public override float GetStaffManaModifier()
        {
            return 2f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<RadianceElement>();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if(MagicProj.tileHitCount <= 0)
            {
                Projectile.velocity *= 0f;
                return true;
            }
            /*
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;*/
         //   Projectile.velocity *= 0.01f;
            return base.OnTileCollide(oldVelocity);
        }
    }
}

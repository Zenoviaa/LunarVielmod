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
            MagicProj.TrailLength += 16;
            MagicProj.Projectile.timeLeft *= 2;
            MagicProj.Projectile.penetrate = -1;
            MagicProj.extraScale += 0.25f;
        }

        public override void AI()
        {
            base.AI();

            /*
             *  Turns your spell into a superfast laser (using your element) 
             *  and turning it through color and speed, making the trail do consistent damage as a largeish laser with a point,
             *  more laser enchantments extend that the laser is there 
             */
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
            return 0.4f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<RadianceElement>();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
    }
}

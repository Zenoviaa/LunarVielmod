using Microsoft.Xna.Framework;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic
{
    public static class AdvancedMagicUtil
    {

        public static void NewMagicProjectile(BaseStaff item, Projectile sourceProjectile, float charge)
        {
            Player player = Main.player[sourceProjectile.owner];
            float speed = sourceProjectile.velocity.Length();
            Vector2 velocity = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero) * speed;
            Projectile p = Projectile.NewProjectileDirect(
                                sourceProjectile.GetSource_FromThis(), player.Center, velocity,
                                ModContent.ProjectileType<AdvancedMagicProjectile>(), sourceProjectile.damage, sourceProjectile.knockBack, sourceProjectile.owner, 
                                ai1: charge);
            p.netUpdate = true;
        }


        public static void CloneMagicProjectile(AdvancedMagicProjectile sourceProjectile,
            Vector2 position, Vector2 velocity, int damage, float knockback, int trailLength, float size)
        {
            Projectile p = Projectile.NewProjectileDirect(
                                sourceProjectile.Projectile.GetSource_FromThis(), position, velocity,
                                ModContent.ProjectileType<AdvancedMagicProjectile>(), damage, knockback, sourceProjectile.Projectile.owner,
                                ai1: 1f);

            //Set Moonlight Defaults
            AdvancedMagicProjectile moonlightMagicProjectile = p.ModProjectile as AdvancedMagicProjectile;
            moonlightMagicProjectile.TrailLength = trailLength;
            moonlightMagicProjectile.Size = size;
            moonlightMagicProjectile.IsClone = true;
            moonlightMagicProjectile.SetMoonlightDefaults(sourceProjectile);
        }
    }
}

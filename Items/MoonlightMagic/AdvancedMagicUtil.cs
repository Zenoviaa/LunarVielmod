using Microsoft.Xna.Framework;
using Stellamod.Common.Players;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic
{
    internal static class AdvancedMagicUtil
    {
        public static void NewMagicProjectile(BaseStaff item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //
            ComboPlayer comboPlayer = player.GetModPlayer<ComboPlayer>();
            comboPlayer.ComboWaitTime = 0;

            int combo = comboPlayer.ComboCounter;
            int dir = comboPlayer.ComboDirection;
            Projectile staff = Projectile.NewProjectileDirect(
                source, position, velocity, ModContent.ProjectileType<AdvancedMagicStaffProjectile>(), damage, knockback, player.whoAmI,
                ai0: 0, ai1: dir, ai2: item.Item.useTime);
            comboPlayer.IncreaseCombo(maxCombo: 0);
            staff.netUpdate = true;
        }

        public static void NewMagicProjectile(BaseStaff item, Projectile sourceProjectile)
        {
            Player player = Main.player[sourceProjectile.owner];
            float speed = sourceProjectile.velocity.Length();
            Vector2 velocity = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero) * speed;
            Projectile p = Projectile.NewProjectileDirect(
                                sourceProjectile.GetSource_FromThis(), player.Center, velocity,
                                ModContent.ProjectileType<AdvancedMagicProjectile>(), sourceProjectile.damage, sourceProjectile.knockBack, sourceProjectile.owner);
            p.netUpdate = true;
        }

         
        public static void CloneMagicProjectile(AdvancedMagicProjectile sourceProjectile,
            Vector2 position, Vector2 velocity, int damage, float knockback, int trailLength, float size)
        {
            Projectile p = Projectile.NewProjectileDirect(
                                sourceProjectile.Projectile.GetSource_FromThis(), position, velocity,
                                ModContent.ProjectileType<AdvancedMagicProjectile>(), damage, knockback, sourceProjectile.Projectile.owner);

            //Set Moonlight Defaults
            AdvancedMagicProjectile moonlightMagicProjectile = p.ModProjectile as AdvancedMagicProjectile;
            moonlightMagicProjectile.TrailLength = trailLength;
            moonlightMagicProjectile.Size = size;
            moonlightMagicProjectile.IsClone = true;
            moonlightMagicProjectile.SetMoonlightDefaults(sourceProjectile);
        }
    }
}

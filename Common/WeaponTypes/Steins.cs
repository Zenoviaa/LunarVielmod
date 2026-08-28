using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.WeaponTypes;

public static class SteinHelper
{
    /// <summary>
    /// Attempts to teleport to a point but does not go through tiles.
    /// This also sends the teleport net message so it does not need to be manually synced with each call
    /// </summary>
    /// <param name="player">The player to teleport</param>
    /// <param name="projectile">The projectile that this teleport is attached to</param>
    /// <param name="targetPoint">The point to teleport to</param>
    /// <returns>True if the teleport succeeded, and false if not</returns>
    public static bool SteinDash(Player player, Projectile projectile, Vector2 targetPoint)
    {
        if (Collision.CanHitLine(player.Center, 1, 1, targetPoint, 1, 1))
        {
            player.Teleport(targetPoint, 6);
            NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, targetPoint.X, targetPoint.Y, 1);
            float speed = 5;
            projectile.velocity = projectile.DirectionTo(Main.MouseWorld) * speed;
            projectile.netUpdate = true;

            player.immune = true;
            player.immuneTime = 3;
            projectile.Center = player.Center;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Sets common defaults for a stein projectile
    /// </summary>
    /// <param name="projectile"></param>
    public static void DefaultToSteinProjectile(this Projectile projectile)
    {
        projectile.timeLeft = 60;
        projectile.penetrate = -1;
        projectile.usesLocalNPCImmunity = true;
        projectile.localNPCHitCooldown = -1;
        projectile.friendly = true;
        projectile.ignoreWater = true;
        projectile.tileCollide = false;
        projectile.height = 100;
        projectile.width = 100;

    }

    /// <summary>
    /// Sets common defaults for a fisting stein projectile
    /// </summary>
    /// <param name="projectile"></param>
    public static void DefaultToSteinFistProjectile(this Projectile projectile)
    {
        projectile.width = 64;
        projectile.height = 64;
        projectile.penetrate = -1;
        projectile.usesLocalNPCImmunity = true;
        projectile.localNPCHitCooldown = -1;
        projectile.friendly = true;
        projectile.tileCollide = false;
        projectile.ignoreWater = true;
        projectile.timeLeft = 12;
    }
}
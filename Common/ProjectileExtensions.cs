using Terraria;
using Terraria.ID;

namespace Stellamod.Common;

public static class ProjectileExtensions
{
    public static void DefaultToMinionProjectile(this Projectile projectile)
    {
        // These below are needed for a minion weapon
        // Only controls if it deals damage to enemies on contact (more on that later)
        projectile.friendly = true;
        // Only determines the damage type
        projectile.minion = true;
        // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
        projectile.minionSlots = 1f;
    }
    public static void StaticDefaultToMinionProjectile(this Projectile projectile)
    {
        Main.projPet[projectile.type] = true;
        ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
        ProjectileID.Sets.CultistIsResistantTo[projectile.type] = true;
    }
    extension(Projectile projectile)
    {
        public int WidthAndHeight
        {
            set
            {
                projectile.width = projectile.height = value;
            }
        }

        public bool LocalHitOnce
        {
            set
            {
                if(value)
                {
                    projectile.penetrate = -1;
                    projectile.usesLocalNPCImmunity = true;
                    projectile.localNPCHitCooldown = -1;
                }
            }
        }

        /// <summary>
        /// Sets penetrate to -1, enables static immunity, and sets cooldown to the given value
        /// </summary>
        public int StaticPiercingImmunityTime
        {
            set
            {
                projectile.penetrate = -1;
                projectile.usesIDStaticNPCImmunity = true;
                projectile.idStaticNPCHitCooldown = value;
            }
        }

        /// <summary>
        /// Sets penetrate to -1, enables local immunity and sets the cooldown to the given value
        /// </summary>
        public int LocalPiercingImmunityTime
        {
            set
            {

                // Needed so the minion doesn't despawn on collision with enemies or tiles
                projectile.penetrate = -1;
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = value;
            }
        }
        public Player PlayerOwner => Main.player[projectile.owner];
    }
}

using Terraria;

namespace Stellamod.Core.ProjectileHelpers
{
    public interface IProjectileNetID
    {
        int GetNetID();
    }
    public static class ProjectileNetIDHelper
    {
        private static int _lastID;
        public static int RegisterID()
        {
            int id = _lastID;
            _lastID++;
            return id;
        }

        public static bool TryFindProjectile(int netID, int playerWhoAmI, out Projectile result)
        {
            foreach (var projectile in Main.ActiveProjectiles)
            {
                if (projectile.owner != playerWhoAmI)
                    continue;

                if (projectile.ModProjectile is IProjectileNetID pNetID)
                {
                    int projectileNetID = pNetID.GetNetID();
                    if (projectileNetID == netID)
                    {
                        result = projectile;
                        return true;
                    }
                }
            }
            result = null;
            return false;
        }
    }
}

using Terraria.ID;

namespace Stellamod.Core.ProjectileHelpers
{
    public static class ProjectileSets
    {
        public static bool[] ResistedByFlamecrestShield = ProjectileID.Sets.Factory.CreateBoolSet(false);
        public static bool[] ResetBossMultihitDamageFalloff = ProjectileID.Sets.Factory.CreateBoolSet(false);
        public static bool[] BossMultihitDamageFalloff = ProjectileID.Sets.Factory.CreateBoolSet(false);
    }
}

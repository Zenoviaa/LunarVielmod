using Stellamod.Helpers;
using Terraria;

namespace Stellamod
{
    public class CustomConditions
    {
        public static readonly Condition PostFenix = new Condition("Defeated Fenix", () => DownedBossSystem.downedFenixBoss);
        public static readonly Condition PostSingularity = new Condition("Killed Singularity Fragment", () => DownedBossSystem.downedSOMBoss);
        public static readonly Condition PostDaedus = new Condition("Defeated Daedus", () => DownedBossSystem.downedDaedusBoss);
        public static readonly Condition PostGintzia = new Condition("Defeated Commander Gintzia", () => DownedBossSystem.downedCommanderGintziaBoss);
    }
}

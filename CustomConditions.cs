using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod
{
    internal class CustomConditions
    {
        public static readonly Condition PostFenix = new Condition("Defeated Fenix", () => DownedBossSystem.downedFenixBoss);
        public static readonly Condition PostSingularity = new Condition("Killed Singularity Fragment", () => DownedBossSystem.downedSOMBoss);
        public static readonly Condition PostDaedus = new Condition("Defeated Daedus", () => DownedBossSystem.downedDaedusBoss);
        public static readonly Condition SewingKitEquipped = new Condition("Sewing Kit must be Equipped", () => Main.LocalPlayer.GetModPlayer<SewingKitPlayer>().hasSewingKit);
    }
}

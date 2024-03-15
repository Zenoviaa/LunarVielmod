using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod
{
    internal class CustomConditions
    {
        public static readonly Condition SewingKitEquipped = new Condition("Sewing Kit must be Equipped", () => Main.LocalPlayer.GetModPlayer<SewingKitPlayer>().hasSewingKit);
    }
}

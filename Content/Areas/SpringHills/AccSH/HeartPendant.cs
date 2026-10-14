using Stellamod.Common.StaminaSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH;

public class HeartPendant : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Item.HeartPendant.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<StaminaNDashPlayer>().DashRegenerationBonus += 0.1f;
        player.GetModPlayer<StaminaNDashPlayer>().MaxDashCount++;
    }
}

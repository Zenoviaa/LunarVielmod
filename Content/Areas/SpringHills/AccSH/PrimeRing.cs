using Stellamod.Items.Accessories.Players;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH;

public class PrimeRing : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Item.PrimeRing.KEY;
    public override void SetDefaults()
    {
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<DashPlayer>().MaxDashCount += 1;
    }
}
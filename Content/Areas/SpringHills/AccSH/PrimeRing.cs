using Stellamod.Common.StaminaSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH;

public class PrimeRing : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Item.PrimeRing.KEY;
    public override void SetDefaults()
    {
        Item.DefaultToAccessory();
        Item.rare = ModContent.RarityType<ShopRarity>();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.MaxStamina += 1;
    }
}
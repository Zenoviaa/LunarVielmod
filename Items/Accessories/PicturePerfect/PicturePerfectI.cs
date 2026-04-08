using Stellamod.Content.Armors.Artisan;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.PicturePerfect
{
    public class PicturePerfectI : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Book of Wooden Illusion");
            /* Tooltip.SetDefault("Increased Regeneration!" +
				"\n +3% damage" +
				"\n Increases crit strike change by 5% "); */

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ItemRarityID.LightPurple;
            Item.accessory = true;


        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            Main.LocalPlayer.GetModPlayer<ArtisanPlayer>().Cameraaa = true;
            player.GetModPlayer<ArtisanPlayer>().PPDMG = 10;
            player.GetModPlayer<ArtisanPlayer>().PPDefense = 5;
            player.GetModPlayer<ArtisanPlayer>().PPCrit = 5;
            player.GetModPlayer<ArtisanPlayer>().PPSpeed = 0.5f;
            player.GetModPlayer<ArtisanPlayer>().PPPaintI = true;
            player.GetModPlayer<ArtisanPlayer>().PPPaintDMG = 0.5f;
            player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2 = 5;
            player.GetModPlayer<ArtisanPlayer>().PPPaintTime = 60;
            player.GetModPlayer<ArtisanPlayer>().PPFrameTime = 2;
        }




    }
}
using Stellamod.Content.Armors.Artisan;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.PicturePerfect
{
    public class PicturePerfectIII : ModItem
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
            Item.value = Item.sellPrice(gold: 20);
            Item.rare = ItemRarityID.Cyan;
            Item.accessory = true;


        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            Main.LocalPlayer.GetModPlayer<ArtisanPlayer>().Cameraaa = true;
            player.GetModPlayer<ArtisanPlayer>().PPDMG = 20;
            player.GetModPlayer<ArtisanPlayer>().PPDefense = 25;
            player.GetModPlayer<ArtisanPlayer>().PPCrit = 15;
            player.GetModPlayer<ArtisanPlayer>().PPSpeed = 1f;
            player.GetModPlayer<ArtisanPlayer>().PPPaintI = true;
            player.GetModPlayer<ArtisanPlayer>().PPPaintII = true;
            player.GetModPlayer<ArtisanPlayer>().PPPaintIII = true;
            player.GetModPlayer<ArtisanPlayer>().PPPaintDMG = 1.75f;
            player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2 = 50;
            player.GetModPlayer<ArtisanPlayer>().PPPaintTime = 240;
            player.GetModPlayer<ArtisanPlayer>().PPFrameTime = 8;
        }




    }
}
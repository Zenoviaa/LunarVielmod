using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.PicturePerfect
{
    public class PicturePerfectII : ModItem
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
            Item.value = Item.sellPrice(gold: 15);
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;


        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            Main.LocalPlayer.GetModPlayer<MyPlayer>().Cameraaa = true;
            player.GetModPlayer<MyPlayer>().PPDMG = 10;
            player.GetModPlayer<MyPlayer>().PPDefense = 10;
            player.GetModPlayer<MyPlayer>().PPCrit = 10;
            player.GetModPlayer<MyPlayer>().PPSpeed = 1f;
            player.GetModPlayer<MyPlayer>().PPPaintI = true;
            player.GetModPlayer<MyPlayer>().PPPaintII = true;
            player.GetModPlayer<MyPlayer>().PPPaintDMG = 1f;
            player.GetModPlayer<MyPlayer>().PPPaintDMG2 = 35;
            player.GetModPlayer<MyPlayer>().PPPaintTime = 120;
            player.GetModPlayer<MyPlayer>().PPFrameTime = 4;
        }




    }
}
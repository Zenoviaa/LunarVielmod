using Stellamod.Common.Players;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.AccessoriesUrdveil
{
    public class PrimeRing : ModItem
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
            Item.value = Item.sellPrice(silver: 25);
            Item.rare = ItemRarityID.LightPurple;
            Item.accessory = true;


        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ComboPlayer>().MaxStamina += 1;
        }
    }
}
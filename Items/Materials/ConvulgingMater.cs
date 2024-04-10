
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{

    public class ConvulgingMater : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Pure shadows conjured by the darkest of entities."); // The (English) text shown below your item's name
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 12));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
            ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of commonly used research amounts depending on item type.
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
        }

        public override void SetDefaults()
        {
            Item.width = 44; // The item texture's width
            Item.height = 42; // The item texture's height
            Item.maxStack = Item.CommonMaxStack; // The item's max stack value
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
		}
	}
}
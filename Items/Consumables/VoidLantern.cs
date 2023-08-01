
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.Audio;
using Stellamod.NPCs.Bosses.SunStalker;
using Stellamod.Items.Materials;
using Stellamod.NPCs.Bosses.Jack;
using Stellamod.NPCs.Bosses.singularityFragment;

namespace Stellamod.Items.Consumables
{
    internal class VoidLantern : ModItem
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Stalker's Stone");
            // Tooltip.SetDefault("Use this as a sun altar to unleash the solar power…"); // The (English) text shown below your item's name
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of commonly used research amounts depending on item type.
        }

        public override void SetDefaults()
        {
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.width = 20; // The item texture's width
            Item.height = 20; // The item texture's height
            Item.maxStack = 99; // The item's max stack value
            Item.value = Item.buyPrice(silver: 1);
            Item.useStyle = ItemUseStyleID.HoldUp;


            Item.noMelee = true;
            Item.consumable = true;
            Item.autoReuse = false;
            Item.UseSound = SoundID.Item43;
        }
        public override bool CanUseItem(Player player) => !NPC.AnyNPCs(ModContent.NPCType<SingularityFragment>());

        public override bool? UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<SingularityFragment>());
            return true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<ConvulgingMater>(), 20);
            recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 40);
            recipe.AddIngredient(ItemID.Bone, 15);
            recipe.AddTile(TileID.Furnaces);
            recipe.Register();
        }

    }
}

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

namespace Stellamod.Items.Consumables
{
    internal class GothiviasSeal : ModItem
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
            Item.value = Item.buyPrice(gold: 10);
            Item.useStyle = ItemUseStyleID.HoldUp;


            Item.noMelee = true;
            Item.consumable = false;
            Item.autoReuse = false;
            Item.UseSound = SoundID.Item43;
        }


    }
}
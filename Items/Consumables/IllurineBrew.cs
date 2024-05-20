using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Stellamod.Buffs;
using Microsoft.Xna.Framework;
using Stellamod.Helpers;

namespace Stellamod.Items.Consumables
{
    internal class IllurineBrew : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;

            // Dust that will appear in these colors when the item with ItemUseStyleID.DrinkLiquid is used
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
                ColorFunctions.Niivin,
                Color.LightGoldenrodYellow,
                Color.DeepSkyBlue
            };
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item3;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 1);
            Item.buffType = ModContent.BuffType<IlluriasBlessing>(); // Specify an existing buff to be applied when used.
            Item.buffTime = 28800;
        }
    }
}

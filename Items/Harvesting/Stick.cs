using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
    internal class Stick : ModItem
    {


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stick");
            Tooltip.SetDefault("Welp, its a stick" +
            "\nBest use for weapons and planting items!");

            Item.damage = 6;
            Item.DamageType = DamageClass.Melee;
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.autoReuse = true;
       



        }

        public override void SetDefaults()
        {

            Item.width = 20;
            Item.height = 20;

            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 2);
        }
    }


}

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
    internal class ViolinStick : ModItem
    {


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Violin Stick");
            Tooltip.SetDefault("Hmph, what is this used for?" +
            "\nBest use for weapons and musical items!");

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

            Item.value = Item.sellPrice(silver: 20);


        }

    }


}

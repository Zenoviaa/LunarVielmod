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
    internal class Hlos : ModItem
    {


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magical Handle");
            Tooltip.SetDefault("Magical Handle omg!" +
            "\nBest use for arcanal weapons");

            Item.damage = 4;
            Item.DamageType = DamageClass.Melee;
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.SeedlerThorn;
            Item.shootSpeed = 6;



        }

        public override void SetDefaults()
        {

            Item.width = 20;
            Item.height = 20;

            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 20);
        }
    }


}

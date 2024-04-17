using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;

namespace Stellamod.Items.Weapons.Mage
{ 
    class TheAurora : ClassSwapItem
    {
        //Alternate class you want it to change to
        public override DamageClass AlternateClass => DamageClass.Ranged;

        //Defaults for the other class
        public override void SetClassSwappedDefaults()
        {
            //Do if(IsSwapped) if you want to check for the alternate class
            //Stats to have when in the other class
            Item.mana = 0;
            Item.damage = 25;
        }
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.damage = 19;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = 5;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 20, 0);
            Item.rare = 4;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<AuroraStar>();
            Item.shootSpeed = 15f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.FallenStar, 5);
            recipe.AddIngredient(ItemType<DarkEssence>(), 10);
            recipe.AddIngredient(ItemType<AuroreanStarI>(), 15);
            recipe.AddIngredient(ItemID.FlareGun, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
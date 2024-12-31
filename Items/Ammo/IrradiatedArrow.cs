using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Stellamod.Projectiles.Ammo;
using Stellamod.Items.Ores;
using Stellamod.Items.Materials;

namespace Stellamod.Items.Ammo
{
    internal class IrradiatedArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.damage = 18; // The damage for projectiles isn't actually 12, it actually is the damage combined with the projectile and the item together.
            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
            Item.knockBack = 1.5f;
            Item.value = 10;
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ModContent.ProjectileType<IrradiatedArrowProj>(); // The projectile that weapons fire when using this item as ammunition.
            Item.shootSpeed = 16f; // The speed of the projectile.
            Item.ammo = AmmoID.Arrow; // The ammo class this ammo belongs to.
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe(100)
                .AddIngredient(ItemID.WoodenArrow, 100)
                .AddIngredient(ModContent.ItemType<GraftedSoul>(), 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

    }
}

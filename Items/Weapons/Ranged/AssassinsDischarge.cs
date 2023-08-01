using Terraria;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;
using Stellamod.Items.Materials;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class AssassinsDischarge : ModItem
    {
        public int WinterboundArrow;
        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Blue;
            Item.staff[Item.type] = true;
            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 36f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 28;
            Item.useTime = 28;
            Item.consumeAmmoOnLastShotOnly = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<TerrorFragments>(), 4);
            recipe.AddIngredient(ItemID.Leather, 8);
            recipe.AddIngredient(ItemID.IronBar, 5);
            recipe.AddTile(TileID.Furnaces);
            recipe.Register();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-20f, 0f);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }

    }
}

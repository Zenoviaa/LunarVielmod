using Stellamod.Projectiles.Thrown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;

namespace Stellamod.Items.Weapons.Thrown
{
    internal class IvythornShuriken : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Assassin's Shuriken");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 6;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 12;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.White;
            Item.shootSpeed = 13;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.DamageType = DamageClass.Throwing;
            Item.shoot = ModContent.ProjectileType<IvythornShurikenProj>();
            Item.shootSpeed = 20f;
            Item.useAnimation = 18;
            Item.useTime = 19;
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.UseSound = SoundID.Item1;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            Recipe recipe = CreateRecipe(50);
            recipe.AddIngredient(ItemID.Wood, 2);
            recipe.AddIngredient(ModContent.ItemType<Ivythorn>(), 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}

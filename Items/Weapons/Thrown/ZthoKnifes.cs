using Stellamod.Projectiles.Weapons.Magic;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using Stellamod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Projectiles.Weapons.Spears;
using Stellamod.Projectiles.Weapons.Magic;
using Stellamod.Projectiles.Weapons.Spears;
using System;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Stellamod.Projectiles.Weapons.Thrown;
using Stellamod.Items.Materials.Tech;

namespace Stellamod.Items.Weapons.Thrown
{
    internal class ZthoKnifes : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Assassin's Knife");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 8;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.DamageType = DamageClass.Throwing;
            Item.shoot = ModContent.ProjectileType<ZthoKnife>();
            Item.shootSpeed = 35f;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.consumable = true;
            Item.maxStack = 999;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(50);
            recipe.AddIngredient(ModContent.ItemType<UnknownCircuitry>(), 2);
            recipe.AddIngredient(ItemID.ThrowingKnife, 50);
            recipe.AddIngredient(ModContent.ItemType<DriveConstruct>(), 1);
            recipe.AddIngredient(ModContent.ItemType<MeleeDrive>(), 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }
    }
}

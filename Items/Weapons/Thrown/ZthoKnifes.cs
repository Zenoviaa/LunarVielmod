using Microsoft.Xna.Framework;
using Stellamod.Items.Materials.Tech;
using Stellamod.Projectiles.Thrown;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown
{
    internal class ZthoKnives : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Assassin's Knife");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
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
            Item.maxStack = 9999;
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

using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class VoidsGrasp : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Void's Grasp");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 36;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 9;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Green;
            Item.shootSpeed = 25;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<VoidHandSpawn>();
            Item.shootSpeed = 10f;
            Item.mana = 15;
            Item.useAnimation = 32;
            Item.useTime = 32;
            Item.consumeAmmoOnLastShotOnly = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<LuminullSpiritFragments>(), 10);
            recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 10);
            recipe.AddIngredient(ModContent.ItemType<ShadeHandTome>(), 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }


    }
}

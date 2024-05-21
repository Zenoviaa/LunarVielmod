using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class WintersStom : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Winter's Stom");
            // Tooltip.SetDefault("Every third shot will be a powerful winter shot today explodes into snowflakes that deal damage");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {

            Item.damage = 14;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 20;
            Item.autoReuse = true;
            Item.UseSound = SoundID.DD2_BookStaffCast;
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<WinterStormProg>();
            Item.shootSpeed = 10f;
            Item.mana = 5;
            Item.useAnimation = 34;
            Item.useTime = 34;

            Item.consumeAmmoOnLastShotOnly = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BorealWood, 13);
            recipe.AddIngredient(ModContent.ItemType<WinterbornShard>(), 8);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }



    }
}

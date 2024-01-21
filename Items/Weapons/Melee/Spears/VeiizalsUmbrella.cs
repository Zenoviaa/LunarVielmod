using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Spears;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee.Spears
{
    internal class VeiizalsUmbrella : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gladiator Spear");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void SetDefaults()
        {
            Item.damage = 58;
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

            Item.DamageType = DamageClass.Melee;
            Item.shoot = ModContent.ProjectileType<VeiizalsUmbrellaProj>();
            Item.shootSpeed = 20f;
  
            Item.useAnimation = 20;
            Item.useTime = 26;
            Item.consumeAmmoOnLastShotOnly = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<TerrorFragments>(), 8);
            recipe.AddIngredient(ModContent.ItemType<DreadFoil>(), 15);
            recipe.AddIngredient(ModContent.ItemType<ViolinStick>(), 1);
            recipe.AddIngredient(ItemID.Umbrella, 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.channel = true;

            }
            else
            {
                Item.channel = false;
            }

            return base.CanUseItem(player);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<VeiizalsUmbrellaProjOpen>(), damage, knockback, player.whoAmI);
                return false;
            }
            else
            {
                float numberProjectiles = 3;
                float rotation = MathHelper.ToRadians(12);
                position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
                for (int i = 0; i < numberProjectiles; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // This defines the projectile roatation and speed. .4f == projectile speed
                    Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X / 2, perturbedSpeed.Y / 3, ModContent.ProjectileType<VeiizalsUmbrellaFireProj>(), damage, Item.knockBack, player.whoAmI);
                }
            }


            return true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }
    }
}

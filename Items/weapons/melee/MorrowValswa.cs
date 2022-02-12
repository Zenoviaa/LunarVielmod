using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Stellamod.Projectiles;
using Stellamod.Items.Materials;

namespace Stellamod.Items.weapons.melee
{
    public class MorrowValswa : ModItem
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Morrow Valswa");
        }

        public override void SetDefaults()
        {
            Item.channel = true;
            Item.damage = 11;
            Item.width = 60;
            Item.height = 60;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.crit = 4;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.knockBack = 8;
            Item.useTurn = false;
            Item.value = Terraria.Item.sellPrice(0, 0, 1, 0);
            Item.rare = ItemRarityID.White;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Beachthrow>();
            Item.shootSpeed = 6f;
            Item.noUseGraphic = true;
            
        }
        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {


            position = new Vector2((float)position.X, (float)
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback));
            if (Item.shoot == ModContent.ProjectileType<Beachthrow>())
            {
                Item.shoot = ModContent.ProjectileType<MorrowValswaProj>();
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<Beachthrow>();
            }


           

        
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public override bool CanUseItem(Player player)
        {
            return base.CanUseItem(player);
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.WoodenSword, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 30);
            recipe.AddIngredient(ModContent.ItemType<OvermorrowWood>(), 15);
            recipe.AddIngredient(ItemID.Torch, 210);
        }

    }
}
using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using Stellamod.Projectiles.Swords;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Melee
{
    class CinderBraker : ClassSwapItem
    {

        public int dir;

        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 19;
            Item.mana = 3;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cinder Braker");
        }

        public override void SetDefaults()
        {
            Item.damage = 9;
            Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7;
            Item.value = Item.sellPrice(0, 3, 20, 14);
            Item.rare = ItemRarityID.Blue;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CBCustomSwingProjectile>();
            Item.shootSpeed = 8f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Using the shoot function, we override the swing projectile to set ai[0] (which attack it is)
            int Sound = Main.rand.Next(1, 3);

            float numberProjectiles = 3;
            float rotation = MathHelper.ToRadians(20);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // This defines the projectile roatation and speed. .4f == projectile speed
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<CinderBrakerSword>(), damage, Item.knockBack, player.whoAmI);
            }

            if (dir == -1)
            {
                dir = 1;
            }
            else if (dir == 1)
            {
                dir = -1;
            }
            else
            {
                dir = 1;
            }

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, dir);
            return false; // return false to prevent original projectile from being shot
        }
        public override void AddRecipes()
        {
            /*
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Gladius, 1);
            recipe.AddIngredient(ItemType<Cinderscrap>(), 25);
            recipe.AddIngredient(ItemType<WanderingFlame>(), 15);
            recipe.AddIngredient(ItemType<DarkEssence>(), 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();*/
        }
    }
}
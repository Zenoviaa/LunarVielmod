using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Projectiles.Bow;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class ArchariliteEnergyShot : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 40f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 31;
            Item.useTime = 31;
            Item.channel = true;
            Item.scale = 1f;
            Item.consumeAmmoOnLastShotOnly = true;
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0f);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<ArnchaliteBar>(), 12);
            recipe.AddIngredient(ModContent.ItemType<Cinderscrap>(), 50);
            recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ArchariliteEnergyShot"));
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ArchariliteEnergyShot2"));
            }

            if (player.GetModPlayer<MyPlayer>().ArchariliteSC)
            {
                Item.damage = 18;
                Item.useTime = 25;
                Item.shootSpeed = 55f;
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ArchariliteArrowSC>(), damage, knockback, player.whoAmI);
                float numberProjectiles = 2;
                float rotation = MathHelper.ToRadians(10);
                position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 25f;
                for (int i = 0; i < numberProjectiles; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
                    Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<ArchariliteArrowSmallSC>(), damage, knockback, player.whoAmI);
                }
            }
            else
            {
                Item.damage = 12;
                Item.useTime = 31;
                Item.shootSpeed = 40f;
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ArchariliteArrow>(), damage, knockback, player.whoAmI);
                float numberProjectiles = 2;
                float rotation = MathHelper.ToRadians(15);
                position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 25f;
                for (int i = 0; i < numberProjectiles; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
                    Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<ArchariliteArrowSmall>(), damage, knockback, player.whoAmI);
                }
            }

            return false;
        }



    }
}

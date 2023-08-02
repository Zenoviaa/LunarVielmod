using Stellamod.Projectiles.Spears;
using System;
using Stellamod.Projectiles.Spears;
using System;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Projectiles.Spears;
using Stellamod.Projectiles.Bow;
using Terraria.Audio;
using Terraria.DataStructures;
using Stellamod.NPCs.Bosses.DreadMire;
using Stellamod.Projectiles.Magic;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class ArchariliteEnergyShot : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Blue;

            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 40f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 31;
            Item.useTime = 31;
            Item.channel = true;
            Item.consumeAmmoOnLastShotOnly = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0f);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<ArnchaliteBar>(), 12);
            recipe.AddIngredient(ModContent.ItemType<LostScrap>(), 14);
            recipe.AddTile(TileID.HeavyWorkBench);
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
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ArchariliteArrow>(), damage, knockback, player.whoAmI);
            float numberProjectiles = 2;
            float rotation = MathHelper.ToRadians(15);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 25f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<ArchariliteArrowSmall>(), damage, knockback, player.whoAmI);
            }
            return false;
        }



    }
}

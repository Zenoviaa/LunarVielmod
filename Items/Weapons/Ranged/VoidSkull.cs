using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Spears;
using Terraria.GameContent.Creative;
using Terraria.DataStructures;
using Terraria.Audio;
using Stellamod.Projectiles.Bow;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class VoidSkull : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 52;
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
            Item.useAnimation = 27;
            Item.useTime = 27;
            Item.channel = true;
            Item.consumeAmmoOnLastShotOnly = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<TheRedSkull>(), 1);
            recipe.AddIngredient(ItemType<SingulariumBar>(), 12);
            recipe.AddIngredient(ItemType<Wingspand>(), 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0f);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Wingspand"));
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Wingspand2"));
            }
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            float numberProjectiles = 2;
            float rotation = MathHelper.ToRadians(8);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 25f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<VoidSkullProg>(), damage, knockback, player.whoAmI);
            }
            return false;
        }



    }
}

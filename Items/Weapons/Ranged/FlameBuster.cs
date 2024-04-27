using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class FlameBuster : ModItem
    {
        private int _comboCounter;
        public override void SetDefaults()
        {
            Item.width = 92;
            Item.height = 44;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 16;
            Item.value = Item.sellPrice(gold: 2);
            Item.useTime = 29;
            Item.useAnimation = 29;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.autoReuse = true;
            Item.shootSpeed = 15;
            Item.shoot = ProjectileID.Bullet;
            Item.useAmmo = AmmoID.Bullet;
            Item.noMelee = true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-24, 0);
        }
       
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            _comboCounter++;
            if(_comboCounter > 28)
            {
                //Reset
                Item.useTime = 29;
                Item.useAnimation = 29;
                _comboCounter = 0;
            }

            if(_comboCounter > 5)
            {
                Item.useTime--;
                Item.useAnimation--;
                float recoilStrength = 7;
                Vector2 targetVelocity = -velocity.SafeNormalize(Vector2.Zero) * recoilStrength;
                player.velocity = VectorHelper.VelocityUpTo(player.velocity, targetVelocity);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 16f);
                int numProjectiles = Main.rand.Next(4, 9);
                velocity *= 2.5f;
                damage *= 2;
                type = ModContent.ProjectileType<CinderFlameball>();
                for (int p = 0; p < numProjectiles; p++)
                {
                    // Rotate the velocity randomly by 30 degrees at max.
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
                }

                for (int p = 0; p < numProjectiles / 2; p++)
                {
                    // Rotate the velocity randomly by 30 degrees at max.
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    Projectile.NewProjectileDirect(source, position, newVelocity, 
                        ProjectileID.WandOfSparkingSpark, damage, knockback, player.whoAmI);
                }

                //Dust Burst Towards Mouse
                int count = _comboCounter;
                for (int k = 0; k < count; k++)
                {
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(7));
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    Dust.NewDust(position, 0, 0, DustID.Smoke, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
                }

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CinderBraker"), position);
                return false;
            }
            else
            {
                int numProjectiles = Main.rand.Next(3, 6);
                for (int p = 0; p < numProjectiles; p++)
                {
                    // Rotate the velocity randomly by 30 degrees at max.
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
                }

                SoundEngine.PlaySound(SoundID.Item38, position);
            }

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Shotgun, 1);
            recipe.AddIngredient(ModContent.ItemType<BasicGunParts>(), 1);
            recipe.AddIngredient(ModContent.ItemType<ArnchaliteBar>(), 18);
            recipe.AddIngredient(ModContent.ItemType<MoltenScrap>(), 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}

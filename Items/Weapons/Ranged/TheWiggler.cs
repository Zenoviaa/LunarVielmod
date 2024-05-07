using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Projectiles.Gun;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class TheWiggler : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 110;
            Item.height = 44;
            Item.damage = 25;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.LightRed;
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<WigglerShot>();
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.noMelee = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (!p.active)
                        continue;

                    if (p.type == ModContent.ProjectileType<WigglerStick>() || 
                        p.type == ModContent.ProjectileType<WigglerStick2>())
                    {
                        if (p.ai[2] == 0)
                        {
                            p.ai[1] = Main.rand.NextFloat(30, 90);
                            p.ai[2] = 1;
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/clickk"));
                        }
                    }
                }
                return false;
            }

            //Shooting Sound
            string soundPath;
            switch (Main.rand.Next(0, 3))
            {
                default:
                case 0:
                    soundPath = "Stellamod/Assets/Sounds/WigglerShot";
                    break;
                case 1:
                    soundPath = "Stellamod/Assets/Sounds/WigglerShot2";
                    break;
                case 2:
                    soundPath = "Stellamod/Assets/Sounds/WigglerShot3";
                    break;
            }

            SoundStyle soundStyle = new SoundStyle(soundPath) with { PitchVariance = 0.1f };
            SoundEngine.PlaySound(soundStyle, position);

            //Dust Burst Towards Mouse
            int count = 4;
            for (int k = 0; k < count; k++)
            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(7));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Dust.NewDust(position, 0, 0, DustID.GemSapphire, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
            }

            int numProjectiles = Main.rand.Next(0, 3);
            for (int p = 0; p < numProjectiles; p++)
            {
                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }

            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<BasicGunParts>(), 1);
            recipe.AddIngredient(ModContent.ItemType<LostScrap>(), 12);
            recipe.AddIngredient(ItemID.HallowedBar, 15);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 12);
            recipe.AddIngredient(ItemID.SoulofMight, 10);
            recipe.AddIngredient(ItemID.Gel, 100);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}

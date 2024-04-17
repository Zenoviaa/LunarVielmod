using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Gun;
using Stellamod.Projectiles.Magic;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class Shredder : ModItem
    {
        private int _attackStyle;
        public override void SetDefaults()
        {
            Item.damage = 55;
            Item.crit = 4;
            Item.knockBack = 3f;
            Item.width = 62;
            Item.height = 54;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ModContent.ProjectileType<ShredderProj>();
            Item.shootSpeed = 25;
            Item.autoReuse = true;
            Item.noMelee = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        private void ChangeForm(int newForm)
        {
            _attackStyle = newForm;
            if (_attackStyle == 1)
            {
                Item.damage = 60;
                Item.DamageType = DamageClass.Magic;
                Item.mana = 4;

                Item.useTime = 22;
                Item.useAnimation = 22;
                Item.autoReuse = false;
                Item.noUseGraphic = true;
                Item.mana = 15;
                Item.UseSound = SoundID.Item9;
                Item.shootSpeed = 2;
            }
            else if (_attackStyle == 0)
            {
                Item.damage = 60;
                Item.UseSound = SoundID.Item1;
                Item.DamageType = DamageClass.Ranged;
                Item.useTime = 15;
                Item.useAnimation = 15;
                Item.autoReuse = true;
                Item.noUseGraphic = false;
                Item.mana = 0;
                Item.shootSpeed = 25;
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");
            if (_attackStyle == 0)
            {
                line = new TooltipLine(Mod, "Brooch of the TaGo", "Right click to change form, requires a Sewing Kit")
                {
                    OverrideColor = Color.Magenta
                };
                tooltips.Add(line);
            }
            else
            {
                line = new TooltipLine(Mod, "Brooch of the TaGo", "Changed by Sewing Kit, effects may be incorrect...")
                {
                    OverrideColor = Color.Magenta
                };
                tooltips.Add(line);
            }

        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2 && player.GetModPlayer<SewingKitPlayer>().hasSewingKit)
            {
                if (_attackStyle == 0)
                {
                    ChangeForm(1);
                }
                else
                {
                    ChangeForm(0);
                }

                int sound = Main.rand.Next(0, 3);
                switch (sound)
                {
                    case 0:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia1"), player.position);
                        break;
                    case 1:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia2"), player.position);
                        break;
                    case 2:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia3"), player.position);
                        break;
                }
                return false;
            }

            return base.CanUseItem(player);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            DrawHelper.DrawGlowInInventory(Item, spriteBatch, position, Color.Purple);
            if (_attackStyle == 1)
            {
                Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Mage/RainbowRodMiracle").Value;
                Vector2 size = new Vector2(32, 30);
                Vector2 drawOrigin = size / 2;
                spriteBatch.Draw(iconTexture, position, null, drawColor, 0f, drawOrigin, scale * 2, SpriteEffects.None, 0);
                return false;
            }

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.GetModPlayer<SewingKitPlayer>().hasSewingKit && _attackStyle == 1)
            {
                ChangeForm(0);
            }

            if (_attackStyle == 1)
            {
                int numProjectiles = Main.rand.Next(3, 4);

                for (int p = 0; p < numProjectiles; p++)
                {
                    float speedMultiplier = Main.rand.NextFloat(0.5f, 1f);

                    // Rotate the velocity randomly by 30 degrees at max.
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    Projectile.NewProjectile(source, position + Main.rand.NextVector2Circular(24, 24), newVelocity * speedMultiplier, ModContent.ProjectileType<RainbowRodMiracleProj>(),
                        damage, knockback, player.whoAmI, ai0: Main.rand.NextFloat(10, 20));
                }
    
                return false;
            } 
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StormDragon_CloudBolt"), player.position);

                //Funny Screenshake
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 16);
                int numProjectiles = Main.rand.Next(6, 9);
        
                for (int p = 0; p < numProjectiles; p++)
                {
                    float direction = Main.rand.NextBool(2) ? -1 : 1;
                    float speedMultiplier = Main.rand.NextFloat(0.5f, 1f);
                    // Rotate the velocity randomly by 30 degrees at max.
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    Projectile.NewProjectileDirect(source, position, newVelocity * speedMultiplier, type, damage, knockback, player.whoAmI, direction);
                }
            }

            return false;
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.RainbowRod, 1)
                .AddIngredient(ModContent.ItemType<MiracleThread>(), 15)
                .AddIngredient(ModContent.ItemType<AlcaricMush>(), 4)
                .AddIngredient(ModContent.ItemType<EldritchSoul>(), 4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}

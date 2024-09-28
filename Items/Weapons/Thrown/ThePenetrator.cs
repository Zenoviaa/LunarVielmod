using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Projectiles.Bow;
using Stellamod.Projectiles.Swords;
using Stellamod.Projectiles.Thrown;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace Stellamod.Items.Weapons.Thrown
{
    internal class ThePenetrator : ModItem
    {
        private int _attackStyle;
        public override void SetDefaults()
        {
            Item.width = 96;
            Item.height = 96;
            Item.DamageType = DamageClass.Throwing;
            Item.damage = 42;
            Item.useTime = 100;
            Item.useAnimation = 100;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = 10000;
            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = false;
            Item.UseSound = SoundID.Item1;
    
            Item.shoot = ModContent.ProjectileType<ThePenetratorProj>();
            Item.shootSpeed = 1f;
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
        }
        private void ChangeForm(int newForm)
        {
            _attackStyle = newForm;
            if (_attackStyle == 1)
            {
                Item.damage = 196;
                Item.UseSound = null;
                Item.DamageType = DamageClass.Ranged;
                Item.useTime = 18;
                Item.useAnimation = 18;
                Item.noUseGraphic = true;
                Item.channel = false;
                Item.shoot = ModContent.ProjectileType<ThePenetratorMiracleProj>();
            }
            else if (_attackStyle == 0)
            {
                Item.damage = 100;
                Item.DamageType = DamageClass.Throwing;
                Item.useTime = 18;
                Item.useAnimation = 18;
                Item.noUseGraphic = true;
                Item.channel = true;
                Item.shoot = ModContent.ProjectileType<ThePenetratorProj>();
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
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
            if (!player.GetModPlayer<SewingKitPlayer>().hasSewingKit && _attackStyle == 1)
            {
                ChangeForm(0);
            }

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
                Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Ranged/PenetratorMiracle").Value;
                Vector2 size = new Vector2(52, 52);
                Vector2 drawOrigin = size / 2;
                spriteBatch.Draw(iconTexture, position, null, drawColor, 0f, drawOrigin, scale, SpriteEffects.None, 0);
                return false;
            }

            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedRepeater, 1)
                .AddIngredient(ModContent.ItemType<MiracleThread>(), 15)
                .AddIngredient(ModContent.ItemType<AlcaricMush>(), 4)
                .AddIngredient(ModContent.ItemType<EldritchSoul>(), 4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Materials;
using Stellamod.Particles;
using Stellamod.Projectiles.Summons.Sentries;
using Stellamod.Projectiles.Summons.VoidMonsters;
using Stellamod.Projectiles.Swords;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Summon
{
    public class XScissorComboPlayer : ModPlayer
	{
		public float speed = 1f;
		public float timer;
        public override void UpdateEquips()
        {
            base.UpdateEquips();
			timer++;
			if(timer >= 120)
			{
				speed = 1;
                timer = 0;
			}
        }
    }

	public class XScissor : ModItem
    {
		private int _attackStyle;
		private int _dir;
        public override void SetDefaults()
        {
			Item.damage = 100;
			Item.knockBack = 3f;
			Item.mana = 20;
			Item.width = 76;
			Item.height = 80;
			Item.useTime = 18;
			Item.useAnimation = 18;
			Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(0, 0, 33, 0);
            Item.rare = ItemRarityID.LightPurple;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/RipperSlashTelegraph");

			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;

            // No buffTime because otherwise the item tooltip would say something like "1 minute duration"
            Item.buffType = ModContent.BuffType<XScissorMinionBuff>();
			Item.shoot = ModContent.ProjectileType<XScissorMinionProj>();
		}

        private void ChangeForm(int newForm)
        {
            _attackStyle = newForm;
            if (_attackStyle == 1)
            {
                Item.damage = 166;
				Item.UseSound = null; 
				Item.DamageType = DamageClass.Melee;
                Item.mana = 4;
                Item.useTime = 5;
                Item.useAnimation = 5;
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.noUseGraphic = true;

                Item.mana = 0;

                Item.shoot = ModContent.ProjectileType<XScissorMiracleProj>();
                Item.shootSpeed = 1;
            }
            else if (_attackStyle == 0)
            {
      
                Item.damage = 100;
                Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/RipperSlashTelegraph");
                Item.DamageType = DamageClass.Summon;
                Item.useTime = 18;
                Item.useAnimation = 18;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.noUseGraphic = false;
                Item.mana = 20;

                Item.shoot = ModContent.ProjectileType<XScissorMinionProj>();
                Item.shootSpeed = 0;
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
                Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Melee/XScissorMiracle").Value;
                Vector2 size = new Vector2(52, 52);
                Vector2 drawOrigin = size / 2;
                spriteBatch.Draw(iconTexture, position, null, drawColor, 0f, drawOrigin, scale, SpriteEffects.None, 0);
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

            if (_attackStyle == 0)
			{
                //Spawn at the mouse cursor position
                position = Main.MouseWorld;
                player.AddBuff(Item.buffType, 2);
                var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
                projectile.originalDamage = Item.damage;

                player.UpdateMaxTurrets();
			}
			else
			{
                // Using the shoot function, we override the swing projectile to set ai[0] (which attack it is)
				float speed = player.GetModPlayer<XScissorComboPlayer>().speed;
				float speedProgress = speed / 3;
                //Sound
                if (Main.rand.NextBool(2))
                {
                    SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeProg");
                    soundStyle.PitchVariance = 0.15f;
					soundStyle.Pitch = 0.75f + speedProgress * 0.2f;
                    SoundEngine.PlaySound(soundStyle, player.position);
                }
                else
                {
                    SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeProg2");
                    soundStyle.PitchVariance = 0.15f;
                    soundStyle.Pitch = 0.75f + speedProgress * 0.2f;
                    SoundEngine.PlaySound(soundStyle, player.position);
                }

                if (_dir == -1)
                {
                    _dir = 1;
                }
                else if (_dir == 1)
                {
                    _dir = -1;
                }
                else
                {
                    _dir = 1;
                }

                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, _dir);
            }

			return false;
		}

        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient(ItemID.Excalibur, 1)
				.AddIngredient(ModContent.ItemType<MiracleThread>(), 12)
				.AddIngredient(ModContent.ItemType<AlcaricMush>(), 4)
				.AddIngredient(ModContent.ItemType<EldritchSoul>(), 4)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
    }
}

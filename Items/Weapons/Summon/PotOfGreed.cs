using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Buffs;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Materials;
using Stellamod.Particles;
using Stellamod.Projectiles.Summons.Sentries;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Summon
{
    public class PotOfGreed : ModItem
    {
		private int _attackStyle;
        public override void SetDefaults()
        {
			Item.damage = 61;
			Item.knockBack = 3f;
			Item.mana = 40;
			Item.width = 54;
			Item.height = 34;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.value = Item.sellPrice(0, 5, 0, 0);
			Item.rare = ItemRarityID.LightPurple;
			
			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;

            // No buffTime because otherwise the item tooltip would say something like "1 minute duration"
            Item.buffType = ModContent.BuffType<PotOfGreedMinionBuff>();
            Item.shoot = ModContent.ProjectileType<PotOfGreedMinionProj>();
		}

        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient(ItemID.CookingPot, 1)
                .AddIngredient(ModContent.ItemType<MiracleThread>(), 20)
                .AddIngredient(ModContent.ItemType<AlcaricMush>(), 4)
                .AddIngredient(ModContent.ItemType<EldritchSoul>(), 4)
                .AddTile(TileID.MythrilAnvil)
				.Register();
		}

        private void ChangeForm(int newForm)
        {
            _attackStyle = newForm;
            if (_attackStyle == 1)
            {
				Item.UseSound = SoundID.Item3;
				Item.DamageType = DamageClass.Default;
				Item.knockBack = 0;
                Item.mana = 200;
                Item.useStyle = ItemUseStyleID.DrinkLiquid;
				Item.shoot = 0;
                Item.buffType = 0;
            }
            else if (_attackStyle == 0)
            {
                Item.damage = 100;
                Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/RipperSlashTelegraph");
                Item.DamageType = DamageClass.Summon;
				Item.knockBack = 3;
                Item.useTime = 36;
                Item.useAnimation = 36;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.noUseGraphic = false;
                Item.mana = 20;
                Item.buffType = ModContent.BuffType<PotOfGreedMinionBuff>();
                Item.shoot = ModContent.ProjectileType<PotOfGreedMinionProj>();
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

			if(_attackStyle == 0)
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
                Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Summon/PotOfGreedMiracle").Value;
                Vector2 size = new Vector2(28, 42);
                Vector2 drawOrigin = size / 2;
                spriteBatch.Draw(iconTexture, position, null, drawColor, 0f, drawOrigin, scale, SpriteEffects.None, 0);
                return false;
            }

            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (!player.GetModPlayer<SewingKitPlayer>().hasSewingKit && _attackStyle == 1)
            {
                ChangeForm(0);
				return true;
            }

            if (_attackStyle == 1 && player.altFunctionUse == 0)
			{
                player.AddBuff(ModContent.BuffType<MiracleLiquid>(), 300);
                return true;
            }

			return base.UseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
   
            //Spawn at the mouse cursor position
            if (player.ownedProjectileCounts[type] > 0)
            {
				//Desummon it
				for(int p = 0; p< Main.maxProjectiles; p++)
                {
					Projectile projectile = Main.projectile[p];
					if (projectile.owner != player.whoAmI)
						continue;

					if(projectile.type == type && projectile.active)
                    {
						projectile.Kill();
                    }
                }
            }
			
            if(_attackStyle == 0)
            {
                player.AddBuff(Item.buffType, 2);
            }

			position = Main.MouseWorld;
			SoundEngine.PlaySound(SoundID.Item82, player.position);
			Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
			player.UpdateMaxTurrets();
			return false;
		}
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using Stellamod.Projectiles.Swords;
using Stellamod.Projectiles.Swords.Ripper;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{

	public class Ripper : ModItem
    {
		private int _attackStyle;
		public override void SetDefaults()
        {
			Item.damage = 54;
			Item.crit = 4;
			Item.knockBack = 3f;
			Item.width = 62;
			Item.height = 54;
			Item.useTime = 9;
			Item.useAnimation = 9;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.UseSound = SoundID.Item1;
			Item.value = Item.buyPrice(0, 30, 0, 0);
			Item.rare = ItemRarityID.LightPurple;
			Item.DamageType = DamageClass.Melee;
			Item.shoot = ModContent.ProjectileType<RipperSwordProj>();
			Item.shootSpeed = 16;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
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
				Item.damage = 36;
                Item.DamageType = DamageClass.Magic;
                Item.mana = 4;
                Item.useTime = 5;
                Item.useAnimation = 5;
                Item.useStyle = ItemUseStyleID.Shoot;
				Item.UseSound = SoundID.Item9;
				Item.shootSpeed = 2;
            }
            else if (_attackStyle == 0)
            {
                Item.damage = 70;
                Item.UseSound = SoundID.Item1;
                Item.DamageType = DamageClass.Melee;
                Item.useTime = 9;
                Item.useAnimation = 9;
                Item.mana = 0;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.shootSpeed = 16;
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
                Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Mage/SkyFractureMiracle").Value;
				Vector2 size = new Vector2(60, 68);
				Vector2 drawOrigin = size / 2;
                spriteBatch.Draw(iconTexture, position, null, drawColor, 0f, drawOrigin, scale, SpriteEffects.None, 0);
				return false;
            }

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if(!player.GetModPlayer<SewingKitPlayer>().hasSewingKit && _attackStyle == 1)
			{
				ChangeForm(0);
			}

			if(_attackStyle == 1)
			{
				for(int i = 0; i < 3; i++)
				{
                    int randy = Main.rand.Next(-50, 50);
                    int randx = Main.rand.Next(-50, 50);
					Vector2 offset = new Vector2(randx, randy);
					Vector2 spawnPos = position + offset;

                    Projectile.NewProjectile(player.GetSource_FromThis(), spawnPos, velocity, 
						ModContent.ProjectileType<SkyFractureMiracleProj>(), damage, knockback, player.whoAmI);

                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 speed = Main.rand.NextVector2CircularEdge(2f, 2f);
                        var d = Dust.NewDustPerfect(spawnPos, DustID.GemAmethyst, speed, Scale: 1f);
                        d.noGravity = true;
                    }
                }

				return false;
			}
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
	
		public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SkyFracture, 1)
                .AddIngredient(ModContent.ItemType<MiracleThread>(), 15)
                .AddIngredient(ModContent.ItemType<AlcaricMush>(), 4)
                .AddIngredient(ModContent.ItemType<EldritchSoul>(), 4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}

using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles.Safunai.Alcarish;
using Stellamod.Projectiles.Slashers.IshNYire;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Crossbows.Eckasect;
using Stellamod.Projectiles.Crossbows.Ultras;

using Terraria.GameContent.Creative;
using Stellamod.Projectiles.Magic;

namespace Stellamod.Items.Weapons.Melee.Greatswords.INY
{

	internal class IshNYirePlayer : ModPlayer
	{
		public bool Vire;
		public bool Ish;
		public bool Yire;
		public bool ISHNYIRE;


	}

	public class IshNYire : ModItem
	{
		public int combo;
		public int AttackCounter = 1;
		public int combowombo = 0;
		public int alternate;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Halhurish The Flamed"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			/* Tooltip.SetDefault("Whip your opponents in the air" +
				"\nHitting enemies will explode"); */
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{

			// Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
		
			var line = new TooltipLine(Mod, "", "");

			line = new TooltipLine(Mod, "Alcarishasd",  Helpers.LangText.Common("Greatsword"))
			{
				OverrideColor = ColorFunctions.GreatswordWeaponType
			};
			tooltips.Add(line);

			line = new TooltipLine(Mod, "Alcarish", "(S) Use your right click to switch between 4 forms!")
			{
				OverrideColor = new Color(220, 87, 24)

			};
			tooltips.Add(line);



		}
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useAnimation = 30;
			Item.useTime = 15;
			Item.shootSpeed = 1f;
			Item.knockBack = 4f;
			Item.UseSound = SoundID.Item116;
			Item.shoot = ModContent.ProjectileType<VireProj>();

			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Melee;
			Item.damage = 75;
			Item.value = Item.sellPrice(0, 2, 50, 0);
			Item.rare = ModContent.RarityType<SirestiasSpecialRarity>();
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				if (!player.HasBuff<VireShow>() && !player.HasBuff<IshShow>() && !player.HasBuff<YireShow>() && !player.HasBuff<ISHNYIREShow>())
				{
					player.AddBuff(ModContent.BuffType<VireShow>(), 100000);
					player.ClearBuff(ModContent.BuffType<IshShow>());
					player.ClearBuff(ModContent.BuffType<YireShow>());
					player.ClearBuff(ModContent.BuffType<ISHNYIREShow>());

					//Item.shoot = ModContent.ProjectileType<EckasectGenesisSwitch>();
					Item.noUseGraphic = true;
					Item.useStyle = ItemUseStyleID.Shoot;
					Item.noMelee = true; //so the Item's animation doesn't do damage
					Item.knockBack = 11;
					Item.autoReuse = true;
					Item.mana = 20;
					Item.useTurn = true;
					Item.DamageType = DamageClass.Melee;
					Item.shootSpeed = 1f;
					Item.useAnimation = 30;
					Item.useTime = 15;
					Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GallinLock2");
					Item.channel = false;
					return true;
				}


				else if (!player.HasBuff<VireShow>() && !player.HasBuff<IshShow>() && !player.HasBuff<YireShow>() || player.HasBuff<ISHNYIREShow>())
				{
					player.AddBuff(ModContent.BuffType<VireShow>(), 100000);
					player.ClearBuff(ModContent.BuffType<IshShow>());
					player.ClearBuff(ModContent.BuffType<YireShow>());
					player.ClearBuff(ModContent.BuffType<ISHNYIREShow>());

					//Item.shoot = ModContent.ProjectileType<EckasectGenesisSwitch>();
					Item.noUseGraphic = true;
					Item.useStyle = ItemUseStyleID.Shoot;
					Item.noMelee = true; //so the Item's animation doesn't do damage
					Item.knockBack = 11;
					Item.autoReuse = true;
					Item.mana = 20;
					Item.useTurn = true;
					Item.DamageType = DamageClass.Melee;
					Item.shootSpeed = 1f;
					Item.useAnimation = 30;
					Item.useTime = 15;

					Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GallinLock2");
					Item.channel = false;
					return true;
				}

				else if (player.HasBuff<VireShow>())
				{
					player.AddBuff(ModContent.BuffType<IshShow>(), 100000);
					player.ClearBuff(ModContent.BuffType<VireShow>());
					player.ClearBuff(ModContent.BuffType<YireShow>());
					player.ClearBuff(ModContent.BuffType<ISHNYIREShow>());

					//Item.shoot = ModContent.ProjectileType<IshThrow>();
					Item.noUseGraphic = true;
					Item.useTime = 100; // The Item's use time in ticks (60 ticks == 1 second.)
					Item.useAnimation = 30; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
					Item.useStyle = ItemUseStyleID.Shoot;
					Item.noMelee = true; //so the Item's animation doesn't do damage
					Item.knockBack = 11;

					Item.shootSpeed = 0f; // the speed of the projectile (measured in pixels per frame)
					Item.channel = true;

					return true;
				}

				else if (player.HasBuff<IshShow>())
				{
					player.AddBuff(ModContent.BuffType<YireShow>(), 100000);
					player.ClearBuff(ModContent.BuffType<VireShow>());
					player.ClearBuff(ModContent.BuffType<IshShow>());
					player.ClearBuff(ModContent.BuffType<ISHNYIREShow>());

					
					//Item.shoot = ModContent.ProjectileType<EckasectExecutorSwitch>();


					Item.noUseGraphic = true;
					Item.useStyle = ItemUseStyleID.Shoot;
					Item.noMelee = true; //so the Item's animation doesn't do damage
					Item.knockBack = 11;
					Item.autoReuse = true;
					Item.mana = 20;
					Item.useTurn = true;
					Item.DamageType = DamageClass.Melee;
					Item.shootSpeed = 10f;
					Item.useAnimation = 20;
					Item.useTime = 20;
					Item.consumeAmmoOnLastShotOnly = true;
					Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GallinLock2");
					//GallinLock2
					return true;
				}


				else if (player.HasBuff<YireShow>())
				{
					player.AddBuff(ModContent.BuffType<ISHNYIREShow>(), 100000);
					player.ClearBuff(ModContent.BuffType<VireShow>());
					player.ClearBuff(ModContent.BuffType<IshShow>());
					player.ClearBuff(ModContent.BuffType<YireShow>());

					//Item.shoot = ModContent.ProjectileType<EckasectExecutorSwitch>();
					Item.noUseGraphic = true;
					Item.useStyle = ItemUseStyleID.Shoot;
					Item.noMelee = true; //so the Item's animation doesn't do damage
					Item.knockBack = 11;
					Item.autoReuse = true;
					Item.mana = 20;
					Item.useTurn = true;
					Item.DamageType = DamageClass.Melee;
					Item.shootSpeed = 1f;
					Item.useAnimation = 30;
					Item.useTime = 30;
					Item.consumeAmmoOnLastShotOnly = true;
					Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GallinLock2");
					//GallinLock2
					return true;
				}

			}
			else
			{
				Item.mana = 0;
				Item.UseSound = null;
				Item.DamageType = DamageClass.Melee;
			}

			return true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.altFunctionUse == 2)
			{
				int count = 48;
				for (int k = 0; k < count; k++)
				{
					Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(7));
					newVelocity *= 1f - Main.rand.NextFloat(0.3f);
					Dust.NewDust(position, 0, 0, DustID.Smoke, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
				}

				return true;
			}



			if (player.altFunctionUse != 2)
			{


				int dir = AttackCounter;
				if (player.direction == 1)
				{
					player.GetModPlayer<CorrectSwing>().SwingChange = AttackCounter;
				}
				else
				{
					player.GetModPlayer<CorrectSwing>().SwingChange = AttackCounter * -1;

				}
				AttackCounter = -AttackCounter;


				if (!player.HasBuff<VireShow>() && !player.HasBuff<IshShow>() && !player.HasBuff<YireShow>() && !player.HasBuff<ISHNYIREShow>())
				{

					

					float distanceMult = Main.rand.NextFloat(0.5f, 1.2f);
					float curvatureMult = 0.5f;

					float distanceMult2 = Main.rand.NextFloat(0.5f, 1.2f);
					float curvatureMult2 = 0.7f;

					bool slam = combo % 5 == 4;

					Vector2 direction = velocity.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f));
					if (alternate == 0)
					{
						combo++;
						alternate = 1;

						Projectile proj = Projectile.NewProjectileDirect(source, position, direction, ModContent.ProjectileType<VireProj>(), damage, knockback, player.whoAmI);

						if (proj.ModProjectile is VireProj modProj)
						{
							modProj.SwingTime = (int)(Item.useTime * UseTimeMultiplier(player) * (slam ? 1.75f : 1));
							modProj.SwingDistance = player.Distance(Main.MouseWorld) * distanceMult;
							modProj.Curvature = 0.33f * curvatureMult;
							modProj.Flip = combo % 2 == 1;
							modProj.Slam = slam;
							modProj.PreSlam = combo % 5 == 3;
						}

					}
					else if (alternate == 1)
					{

						alternate = 0;

						Projectile proj2 = Projectile.NewProjectileDirect(source, position, direction, ModContent.ProjectileType<VireProj2>(), damage, knockback, player.whoAmI);

						if (proj2.ModProjectile is VireProj2 modProj2)
						{
							modProj2.SwingTime = (int)(Item.useTime * UseTimeMultiplier(player) * (slam ? 1.75f : 1));
							modProj2.SwingDistance = player.Distance(Main.MouseWorld) * distanceMult2;
							modProj2.Curvature = 0.33f * curvatureMult2;
							modProj2.Flip = combo % 2 == 1;
							modProj2.Slam = slam;
							modProj2.PreSlam = combo % 5 == 3;
						}

					}


                 

                    return base.Shoot(player, source, position, velocity, type, damage, knockback);
				}

				if (player.HasBuff<VireShow>())
				{



					float distanceMult = Main.rand.NextFloat(0.5f, 1.2f);
					float curvatureMult = 0.5f;

					float distanceMult2 = Main.rand.NextFloat(0.5f, 1.2f);
					float curvatureMult2 = 0.7f;

					bool slam = combo % 5 == 4;

					Vector2 direction = velocity.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f));
					if (alternate == 0)
					{
						combo++;
						alternate = 1;

						Projectile proj = Projectile.NewProjectileDirect(source, position, direction, ModContent.ProjectileType<VireProj>(), damage, knockback, player.whoAmI);

						if (proj.ModProjectile is VireProj modProj)
						{
							modProj.SwingTime = (int)(Item.useTime * UseTimeMultiplier(player) * (slam ? 1.75f : 1));
							modProj.SwingDistance = player.Distance(Main.MouseWorld) * distanceMult;
							modProj.Curvature = 0.33f * curvatureMult;
							modProj.Flip = combo % 2 == 1;
							modProj.Slam = slam;
							modProj.PreSlam = combo % 5 == 3;
						}

					}
					else if (alternate == 1)
					{

						alternate = 0;

						Projectile proj2 = Projectile.NewProjectileDirect(source, position, direction, ModContent.ProjectileType<VireProj2>(), damage, knockback, player.whoAmI);

						if (proj2.ModProjectile is VireProj2 modProj2)
						{
							modProj2.SwingTime = (int)(Item.useTime * UseTimeMultiplier(player) * (slam ? 1.75f : 1));
							modProj2.SwingDistance = player.Distance(Main.MouseWorld) * distanceMult2;
							modProj2.Curvature = 0.33f * curvatureMult2;
							modProj2.Flip = combo % 2 == 1;
							modProj2.Slam = slam;
							modProj2.PreSlam = combo % 5 == 3;
						}

					}


					return base.Shoot(player, source, position, velocity, type, damage, knockback);
				}


				if (player.HasBuff<IshShow>())
				{
					Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<IshThrow>(), damage / 3, knockback, player.whoAmI, 1, dir);
				}

				if (player.HasBuff<YireShow>())
				{
					Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<YireProj>(), damage * 3, knockback, player.whoAmI, 1, dir);
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwingyAr") { Pitch = Main.rand.NextFloat(-10f, 10f) }, player.Center);

                    float numberProjectiles = 3;
					float rotation = MathHelper.ToRadians(20);
					position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 15f;
					for (int i = 0; i < numberProjectiles; i++)
					{
						Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // This defines the projectile roatation and speed. .4f == projectile speed
						Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X * 8, perturbedSpeed.Y * 8, ModContent.ProjectileType<XexShot>(), damage / 2, Item.knockBack, player.whoAmI);



					}


					float rot = velocity.ToRotation();
					float spread = 2f;
				

					Vector2 offset = new Vector2(0.01f, -0.01f * player.direction).RotatedBy(rot);
					for (int k = 0; k < 15; k++)
					{
						Vector2 direction = offset.RotatedByRandom(spread);

						Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(15), 125, new Color(150, 80, 40), Main.rand.NextFloat(0.2f, 2f));
					}


					for (int k = 0; k < 15; k++)
					{
						Vector2 direction = offset.RotatedByRandom(spread);

						Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(8), 125, new Color(15, 8, 150), Main.rand.NextFloat(0.2f, 2f));
					}


					int numProjectiles = Main.rand.Next(1, 6);
					for (int p = 0; p < numProjectiles; p++)
					{


						Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(150, 80, 40), 2);
						Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));
					}

				}

				if (player.HasBuff<ISHNYIREShow>())
				{



		

					Vector2 direction = velocity.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f));
					if (alternate == 0)
					{
						
						alternate = 1;

					


						float numberProjectiles = 3;
						float rotation = MathHelper.ToRadians(20);
						position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 15f;
						for (int i = 0; i < 1; i++)
						{
							Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // This defines the projectile roatation and speed. .4f == projectile speed
							Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X * 6, perturbedSpeed.Y * 6, ModContent.ProjectileType<YireBoomer>(), damage, Item.knockBack, player.whoAmI);



						}

					}
					else if (alternate == 1)
					{

						alternate = 0;

					


						float numberProjectiles = 3;
						float rotation = MathHelper.ToRadians(20);
						position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 15f;
						for (int i = 0; i < 1; i++)
						{
							Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(rotation, -rotation, i / (numberProjectiles - 1))) * 1f; // This defines the projectile roatation and speed. .4f == projectile speed
							Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X * 6, perturbedSpeed.Y  * 6, ModContent.ProjectileType<IshBoomer>(), damage, Item.knockBack, player.whoAmI);



						}

					}
				}
			}


			return false;
		}



		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Player player = Main.player[Main.myPlayer];

			if (!player.HasBuff<VireShow>() && !player.HasBuff<IshShow>() && !player.HasBuff<YireShow>() && !player.HasBuff<ISHNYIREShow>())
			{
				Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Melee/Greatswords/INY/Vire").Value;
				spriteBatch.Draw(iconTexture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0);
				return false;
			}
			if (player.HasBuff<VireShow>())
			{
				Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Melee/Greatswords/INY/Vire").Value;
				spriteBatch.Draw(iconTexture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0);
				return false;
			}

			if (player.HasBuff<IshShow>())
			{
				Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Melee/Greatswords/INY/Ish").Value;
				spriteBatch.Draw(iconTexture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0);
				return false;
			}

			if (player.HasBuff<YireShow>())
			{
				Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Melee/Greatswords/INY/Yire").Value;
				spriteBatch.Draw(iconTexture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0);
				return false;
			}

			if (player.HasBuff<ISHNYIREShow>())
			{
				Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Melee/Greatswords/INY/IshNYire").Value;
				spriteBatch.Draw(iconTexture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0);
				return false;
			}


			return true;
		}

		public override float UseTimeMultiplier(Player player) => player.GetAttackSpeed(DamageClass.Melee); //Scale with melee speed buffs, like whips
		public override void NetSend(BinaryWriter writer) => writer.Write(combo);
		public override void NetReceive(BinaryReader reader) => combo = reader.ReadInt32();
	}


	public class VireShow : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Charm Buff!");
			// Description.SetDefault("A true warrior such as yourself knows no bounds");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}
		public override void Update(Player player, ref int buffIndex)
		{
			IshNYirePlayer IshNYirePlayer = player.GetModPlayer<IshNYirePlayer>();
			IshNYirePlayer.Vire = true;


		}
	}

	public class IshShow : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Charm Buff!");
			// Description.SetDefault("A true warrior such as yourself knows no bounds");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}
		public override void Update(Player player, ref int buffIndex)
		{
			IshNYirePlayer IshNYirePlayer = player.GetModPlayer<IshNYirePlayer>();
			IshNYirePlayer.Ish = true;


		}
	}

	public class YireShow : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Charm Buff!");
			// Description.SetDefault("A true warrior such as yourself knows no bounds");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}
		public override void Update(Player player, ref int buffIndex)
		{
			IshNYirePlayer IshNYirePlayer = player.GetModPlayer<IshNYirePlayer>();
			IshNYirePlayer.Yire = true;


		}
	}

	public class ISHNYIREShow : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Charm Buff!");
			// Description.SetDefault("A true warrior such as yourself knows no bounds");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}
		public override void Update(Player player, ref int buffIndex)
		{
			IshNYirePlayer IshNYirePlayer = player.GetModPlayer<IshNYirePlayer>();
			IshNYirePlayer.ISHNYIRE = true;


		}
	}
}

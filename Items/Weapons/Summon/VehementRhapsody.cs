
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.Items.Weapons.Summon.VehementRhapsody;
using static Terraria.ModLoader.ModContent;
using Stellamod.Trails;
using Terraria.Graphics.Shaders;


namespace Stellamod.Items.Weapons.Summon
{
	/*
	 * This file contains all the code necessary for a minion
	 * - ModItem
	 *     the weapon which you use to summon the minion with
	 * - ModBuff
	 *     the icon you can click on to despawn the minion
	 * - ModProjectile 
	 *     the minion itself
	 *     
	 * It is not recommended to put all these classes in the same file. For demonstrations sake they are all compacted together so you get a better overwiew.
	 * To get a better understanding of how everything works together, and how to code minion AI, read the guide: https://github.com/tModLoader/tModLoader/wiki/Basic-Minion-Guide
	 * This is NOT an in-depth guide to advanced minion AI
	 */

	public class VehementBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("IrradiatedCreeperBuff");
			// Description.SetDefault("The Creeper will fight for you");
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ProjectileType<VehementMinion>()] > 0)
			{
				player.buffTime[buffIndex] = 18000;
			}
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}

	public class VehementRhapsody : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Irradiated Creeper Staff");
			// Tooltip.SetDefault("Summons an Irradiated Creeper to fight with you");
			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller.
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 11;
			Item.knockBack = 6f;
			Item.mana = 10;
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(0, 0, 33, 0);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item44;

			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;
			Item.buffType = BuffType<VehementBuff>();
			// No buffTime because otherwise the item tooltip would say something like "1 minute duration"
			Item.shoot = ProjectileType<VehementMinion>();
		}
		
		

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			for (int i = 0; i < 1000; ++i)
			{
				if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == Item.shoot)
					return false;
			}
			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
			projectile.originalDamage = Item.damage;

			player.AddBuff(Item.buffType, 2);
			SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GSummon"), player.position);
			// Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.
			position = Main.MouseWorld;
			return false;
		}


		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse != 2)
			{
				for (int i = 0; i < 1000; ++i)
				{
					if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == Item.shoot)
					{
						Main.projectile[i].minionSlots += 1f;
						Main.projectile[i].originalDamage = Item.damage + (int)(3 * Main.projectile[i].minionSlots);
						if (Main.projectile[i].scale < 1.3f)
                        {
							Main.projectile[i].scale += .062f;
						}
							
					}
				}
			}
			return true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Cinderscrap>(), 50);
			recipe.AddIngredient(ModContent.ItemType<MoltenScrap>(), 10);
			recipe.AddIngredient(ModContent.ItemType<SingulariumBar>(), 5);
			recipe.AddTile(TileID.Hellforge);
			recipe.Register();
		}

		/*
		 * This minion shows a few mandatory things that make it behave properly. 
		 * Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
		 * If the player targets a certain NPC with right-click, it will fly through tiles to it
		 * If it isn't attacking, it will float near the player with minimal movement
		 */
		public class VehementMinion : ModProjectile
		{
			public override void SetStaticDefaults()
			{
				// DisplayName.SetDefault("Irradiated Creeper");
				ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
				ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
				// Sets the amount of frames this minion has on its spritesheet
				Main.projFrames[Projectile.type] = 1;
				// This is necessary for right-click targeting
				ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

				// These below are needed for a minion
				// Denotes that this projectile is a pet or minion
				Main.projPet[Projectile.type] = true;
				// This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
				ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
				// Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
				ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
			}

			public sealed override void SetDefaults()
			{
				Projectile.width = 30;
				Projectile.height = 30;
				Projectile.tileCollide = false; // Makes the minion go through tiles freely
												// These below are needed for a minion weapon
				Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)// Declares this as a minion (has many effects)
				Projectile.DamageType = DamageClass.Melee; // Declares the damage type (needed for it to deal damage) // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
				Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
				Projectile.timeLeft = 1500;
				Projectile.scale = 0.7f;
				Projectile.CloneDefaults(ProjectileID.DeadlySphere);
				AIType = ProjectileID.DeadlySphere;
			}

			// Here you can decide if your minion breaks things like grass or pots
			public override bool? CanCutTiles()
			{
				return false;
			}

			// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
			public override bool MinionContactDamage()
			{
				return true;
			}
			private float alphaCounter = 0;
			public override void AI()
			{
				Player player = Main.player[Projectile.owner];

				#region Active check
				// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
				if (player.dead || !player.active)
				{
					player.ClearBuff(BuffType<VehementBuff>());
				}
				if (player.HasBuff(BuffType<VehementBuff>()))
				{
					Projectile.timeLeft = 2;
				}
				#endregion

				
				// Some visuals here
				Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
			}

			public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
			{
				if (Main.rand.NextBool(3))
				{
					target.AddBuff(BuffID.OnFire, 180);
				}
				var EntitySource = Projectile.GetSource_Death();
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<CandleShotProj2>(), Projectile.damage, 1, Main.myPlayer, 0, 0);
				}


				int Sound = Main.rand.Next(1, 6);
				if (Sound == 1)
				{
					SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Rhap1"), Projectile.position);
				}
				if (Sound == 2)
				{
					SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Rhap2"), Projectile.position);
				}
				if (Sound == 3)
				{
					SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Rhap3"), Projectile.position);

				}

			

			}

			public override Color? GetAlpha(Color lightColor)
			{
				return Color.White;
			}

			public PrimDrawer TrailDrawer { get; private set; } = null;
			public float WidthFunction(float completionRatio)
			{
				float baseWidth = Projectile.scale * Projectile.width * 1.3f;
				return MathHelper.SmoothStep(baseWidth, 0.5f, completionRatio);
			}

			public Color ColorFunction(float completionRatio)
			{
				return Color.Lerp(Color.Goldenrod, Color.LightGoldenrodYellow, completionRatio) * 0.7f;
			}

			public override bool PreDraw(ref Color lightColor)
			{
				Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
				Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(35f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (5 + 0.6f), SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(35f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (5 + 0.6f), SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(35f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
				Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
				Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
				TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
				GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.BeamTrail2);
				TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.7f - Main.screenPosition, 155);
				return false;
			}
		}
	}
}
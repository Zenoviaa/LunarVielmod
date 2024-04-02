using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.Items.Weapons.Summon.SolMothStaff;
using static Terraria.ModLoader.ModContent;

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

    public class SolMothBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Gelatal Minion");
			// Description.SetDefault("The Jelly boi will fight for you");
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ProjectileType<SolMothMinion>()] > 0)
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

	public class SolMothStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Gelatal Slaff");
			// Tooltip.SetDefault("Summons an Jelly boi to fight for you");
			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller.
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 9;
			Item.knockBack = 3f;
			Item.mana = 10;
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(0, 0, 33, 0);
            Item.rare = ItemRarityID.Green;

			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;
			Item.buffType = BuffType<SolMothBuff>();
			// No buffTime because otherwise the item tooltip would say something like "1 minute duration"
			Item.shoot = ProjectileType<SolMothMinion>();
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
			player.AddBuff(Item.buffType, 2);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SoftSummon"), player.position);
            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.
            position = Main.MouseWorld;
			return true;
		}

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.Anvils);
            recipe.AddIngredient(ModContent.ItemType<AlcadizScrap>(), 9);
            recipe.Register();
        }
        /*
		 * This minion shows a few mandatory things that make it behave properly. 
		 * Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
		 * If the player targets a certain NPC with right-click, it will fly through tiles to it
		 * If it isn't attacking, it will float near the player with minimal movement
		 */
        public class SolMothMinion : ModProjectile
		{
			Player Owner => Main.player[Projectile.owner];
			ref float Timer => ref Projectile.ai[0];
			ref float TimerOffset => ref Projectile.ai[1];
            public float HuntrianColorX;
            public float HuntrianColorZ;
            public float HuntrianColorY;

            public float HuntrianColorOfset;
            public override void SetStaticDefaults()
			{
				// DisplayName.SetDefault("Jelly Minion");
				// Sets the amount of frames this minion has on its spritesheet
				Main.projFrames[Projectile.type] = 4;
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
				Projectile.width = 40;
				Projectile.height = 40;
				// Makes the minion go through tiles freely
				Projectile.tileCollide = false;

				// These below are needed for a minion weapon
				// Only controls if it deals damage to enemies on contact (more on that later)
				Projectile.friendly = true;
				// Only determines the damage type
				Projectile.minion = true;
				// Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
				Projectile.minionSlots = 1f;
				// Needed so the minion doesn't despawn on collision with enemies or tiles
				Projectile.penetrate = -1;
				Projectile.usesLocalNPCImmunity = true;
				Projectile.localNPCHitCooldown = 20;
				HuntrianColorOfset = Main.rand.NextFloat(-1f, 1f);

            }


            // Here you can decide if your minion breaks things like grass or pots
            public override bool? CanCutTiles()
			{
				return false;
            }
            public override void OnKill(int timeLeft)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Morrowsc1"), Projectile.position);
            }

            // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
            public override bool MinionContactDamage()
			{
				return true;
			}

            public override void PostDraw(Color lightColor)
			{
				Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
				Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(HuntrianColorX * 1), (int)(HuntrianColorY * 1), (int)(HuntrianColorZ * 1), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(HuntrianColorX * 1), (int)(HuntrianColorY * 1), (int)(HuntrianColorZ * 1), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(HuntrianColorX * 1), (int)(HuntrianColorY * 1), (int)(HuntrianColorZ * 1), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(HuntrianColorX * 1), (int)(HuntrianColorY * 1), (int)(HuntrianColorZ * 1), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(HuntrianColorX * 1), (int)(HuntrianColorY * 1), (int)(HuntrianColorZ * 1), 0), Projectile.rotation, new Vector2(32, 32), 0.07f * (7 + 0.6f), SpriteEffects.None, 0f);
                Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3() * 1.0f * Main.essScale);
            }

            public override void AI()
			{
				//Kill yoself if no buff
				if (!SummonHelper.CheckMinionActive<SolMothBuff>(Owner, Projectile))
					return;

				SummonHelper.SearchForTargets(Owner, Projectile, 
					out bool foundTarget, 
					out float distanceFromTarget, 
					out Vector2 targetCenter);

				if (foundTarget)
				{
					Timer++;
					if(Timer == 1)
					{
                        SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/SoftSummon");
                        soundStyle.PitchVariance = 0.15f;
                        SoundEngine.PlaySound(soundStyle);
						for(int i =0; i < 5; i++)
						{
                            Dust.NewDustPerfect(targetCenter, DustID.GoldFlame, (Vector2.One * Main.rand.Next(1, 5))
                                .RotatedByRandom(19.0), 0, Color.White, 1f).noGravity = true;
                        }
                    }

					if(Timer < 10)
					{
						Projectile.velocity *= 0.92f;
					}

					if(Timer == 10)
					{
						if(Main.myPlayer == Projectile.owner)
						{
							TimerOffset = Main.rand.Next(0, 30);
							Projectile.netUpdate = true;
						}

                        SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2");
                        soundStyle.PitchVariance = 0.15f;
                        SoundEngine.PlaySound(soundStyle);
                        Dust.QuickDustLine(Projectile.Center, targetCenter, 50, Color.Goldenrod);
                        for (int i = 0; i < 2; i++)
                        {
                            Dust.NewDustPerfect(targetCenter, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5))
								.RotatedByRandom(19.0), 0, Color.LightGoldenrodYellow, 1f).noGravity = true;
                        }
                        
						for (int i = 0; i < 2; i++)
                        {
                            Dust.NewDustPerfect(targetCenter, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5))
								.RotatedByRandom(19.0), 0, Color.LightGoldenrodYellow, 1f).noGravity = true;
                        }
                        
						for (int i = 0; i < 2; i++)
                        {
                            //Get a random velocity
                            Vector2 velocity = Main.rand.NextVector2Circular(4, 4);

                            //Get a random
                            float randScale = Main.rand.NextFloat(0.5f, 1.5f);
                            ParticleManager.NewParticle<StarParticle2>(targetCenter, velocity, 
								Color.DarkGoldenrod, randScale);
                        }

                        Projectile.Center = targetCenter;
					}
					else if(Timer < 30)
					{
                        Projectile.velocity *= 0.98f;
                    }
					else
					{
                        SummonHelper.CalculateIdleValues(Owner, Projectile,
                               out Vector2 vectorToIdlePosition,
                               out float distanceToIdlePosition);
                        SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
                    }
			
					if(Timer >= 60 + TimerOffset)
					{
						Timer = 0;
					}
				}
				else
				{
                    SummonHelper.CalculateIdleValues(Owner, Projectile,
                                out Vector2 vectorToIdlePosition,
                                out float distanceToIdlePosition);
                    SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
                }

				Visuals();
			}

			private void Visuals()
            {
                HuntrianColorZ = VectorHelper.Osc(15f, 60, 3, HuntrianColorOfset);
                HuntrianColorY = VectorHelper.Osc(45f, 60, 3, HuntrianColorOfset);
                HuntrianColorX = VectorHelper.Osc(85f, 15, 3, HuntrianColorOfset);
                // So it will lean slightly towards the direction it's moving
                Projectile.rotation = Projectile.velocity.X * 0.05f;

                // This is a simple "loop through all frames from top to bottom" animation
                int frameSpeed = 8;
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= frameSpeed)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= Main.projFrames[Projectile.type])
                    {
                        Projectile.frame = 0;
                    }
                }

                // Some visuals here
                Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
            }
		}
	}
}
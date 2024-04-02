

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.singularityFragment;
using Stellamod.NPCs.Bosses.singularityFragment.Phase1;
using Stellamod.Projectiles.Summons;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.Items.Weapons.Summon.VoidStaff;
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

    public class VoidMinionBuff : ModBuff
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
			if (player.ownedProjectileCounts[ProjectileType<VoidMinion>()] > 0)
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

	public class VoidStaff : ModItem
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
			Item.damage = 11;
			Item.knockBack = 3f;
			Item.mana = 10;
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(0, 1, 33, 0);
            Item.rare = ItemRarityID.Orange;

			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;
			Item.buffType = BuffType<VoidMinionBuff>();
			// No buffTime because otherwise the item tooltip would say something like "1 minute duration"
			Item.shoot = ProjectileType<VoidMinion>();
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
			player.AddBuff(Item.buffType, 2);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GSummon"), player.position);
            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.
            position = Main.MouseWorld;
			return true;
		}

		/*
		 * This minion shows a few mandatory things that make it behave properly. 
		 * Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
		 * If the player targets a certain NPC with right-click, it will fly through tiles to it
		 * If it isn't attacking, it will float near the player with minimal movement
		 */
		public class VoidMinion : ModProjectile
		{
			Player Owner => Main.player[Projectile.owner];
			ref float Timer => ref Projectile.ai[0];
			public override void SetStaticDefaults()
			{
				// DisplayName.SetDefault("Jelly Minion");
				// Sets the amount of frames this minion has on its spritesheet
				// This is necessary for right-click targeting
				ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
                ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
                ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
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
				Projectile.width = 18;
				Projectile.height = 28;
				// Makes the minion go through tiles freely
				Projectile.tileCollide = false;

				// These below are needed for a minion weapon
				// Only controls if it deals damage to enemies on contact (more on that later)
				Projectile.friendly = true;
				// Only determines the damage type
				Projectile.minion = true;
				// Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
				Projectile.minionSlots = 2f;
				// Needed so the minion doesn't despawn on collision with enemies or tiles
				Projectile.penetrate = -1;
				Projectile.usesLocalNPCImmunity = true;
				Projectile.localNPCHitCooldown = 20;
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
            int counter = 6;
            float alphaCounter = 6;
            public override Color? GetAlpha(Color lightColor)
            {
                return Color.White;
            }
            public PrimDrawer TrailDrawer { get; private set; } = null;
            public float WidthFunction(float completionRatio)
            {
                float baseWidth = Projectile.scale * Projectile.width * 1.3f;
                return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
            }
            public Color ColorFunction(float completionRatio)
            {
                return Color.Lerp(Color.RoyalBlue, Color.Transparent, completionRatio) * 0.7f;
            }
            public override bool PreDraw(ref Color lightColor)
            {
                Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
                Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(15f * alphaCounter), (int)(45f * alphaCounter), 0), Projectile.rotation, new Vector2(64 / 2, 64 / 2), 0.2f * (counter + 0.3f), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(05f * alphaCounter), (int)(05f * alphaCounter), (int)(55f * alphaCounter), 0), Projectile.rotation, new Vector2(64 / 2, 64 / 2), 0.2f * (counter + 0.3f * 2), SpriteEffects.None, 0f);

                Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
                GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.FadedStreak);
                TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);

                return false;
            }

            public override void AI()
			{
				if (!SummonHelper.CheckMinionActive<VoidMinionBuff>(Owner, Projectile))
					return;

				SummonHelper.SearchForTargets(Owner, Projectile,
					out bool foundTarget,
					out float distanceFromTarget,
					out Vector2 targetCenter);
				if (foundTarget)
				{
					Timer++;
					Vector2 directionToTarget = Projectile.Center.DirectionTo(targetCenter);
					Vector2 offset = -directionToTarget * 200;
					Projectile.velocity = VectorHelper.VelocitySlowdownTo(Projectile.Center, targetCenter + offset, 8);
					if(Timer == 1)
					{
                        NPC.NewNPC(Projectile.GetSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.Center.Y,
                         ModContent.NPCType<SingularitySpark>());
                    }

					if(Timer > 20  && Timer % 7 == 0 && Timer < 100)
					{
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
						Vector2 velocity = directionToTarget * 16;
						velocity = velocity.RotatedByRandom(MathHelper.PiOver4/2);
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
							ModContent.ProjectileType<VoidMinionSparkProj>(), Projectile.damage, Projectile.knockBack, Owner: Projectile.owner);
                    }

					if(Timer > 100 && Timer < 150)
					{
                        //Idle
                        SummonHelper.CalculateIdleValues(Owner, Projectile,
                            out Vector2 vectorToIdlePosition,
                            out float distanceToIdlePosition);
                        SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
                    }

					if(Timer >= 150)
					{
						Timer = 0;
					}
                }
                else
				{
					//Idle
					SummonHelper.CalculateIdleValues(Owner, Projectile,
						out Vector2 vectorToIdlePosition,
						out float distanceToIdlePosition);
					SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
				}
				Visuals();
            }

			private void Visuals()
			{
                // So it will lean slightly towards the direction it's moving
                Projectile.rotation = Projectile.velocity.X * 0.05f;

                // This is a simple "loop through all frames from top to bottom" animation
                int frameSpeed = 5;
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
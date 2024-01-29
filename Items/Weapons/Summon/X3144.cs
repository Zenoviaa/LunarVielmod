using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.Materials.Tech;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.Items.Weapons.Summon.X3144;
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

    public class ProbeBuff : ModBuff
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
			if (player.ownedProjectileCounts[ProjectileType<ProbeMinion>()] > 0)
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

	public class X3144 : ModItem
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
			Item.damage = 51;
			Item.knockBack = 3f;
			Item.mana = 10;
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(0, 1, 33, 0);
            Item.rare = ItemRarityID.Pink;

			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;
			Item.buffType = BuffType<ProbeBuff>();
			// No buffTime because otherwise the item tooltip would say something like "1 minute duration"
			Item.shoot = ProjectileType<ProbeMinion>();
		}
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<SummonerDrive>(), 1);
            recipe.AddIngredient(ItemID.SoulofMight, 20);
            recipe.AddTile(TileID.AdamantiteForge);
            recipe.Register();
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
		public class ProbeMinion : ModProjectile
		{
			public override void SetStaticDefaults()
            {
                ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
                ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
                // DisplayName.SetDefault("Jelly Minion");
                // Sets the amount of frames this minion has on its spritesheet
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
				Projectile.minionSlots = 1f;
				// Needed so the minion doesn't despawn on collision with enemies or tiles
				Projectile.penetrate = -1;
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
            public override bool PreDraw(ref Color lightColor)
            {

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(Color.Lerp(new Color(255, 8, 55), new Color(99, 39, 51), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                    Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
                }
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                return true;
            }
            public override void AI()
			{
				Player player = Main.player[Projectile.owner];

				#region Active check
				// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
				if (player.dead || !player.active)
				{
					player.ClearBuff(BuffType<ProbeBuff>());
				}
				if (player.HasBuff(BuffType<ProbeBuff>()))
				{
					Projectile.timeLeft = 2;
				}
				#endregion

				#region General behavior
				Vector2 idlePosition = player.Center;
				idlePosition.Y -= 1f; // Go up 48 coordinates (three tiles from the center of the player)

				// If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
				// The index is projectile.minionPos
				float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -player.direction;

				// All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

				// Teleport to player if distance is too big
				Vector2 vectorToIdlePosition = idlePosition - Projectile.Center;
				float distanceToIdlePosition = vectorToIdlePosition.Length();
				if (Main.myPlayer == player.whoAmI && distanceToIdlePosition > 1000f)
				{
					// Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
					// and then set netUpdate to true
					Projectile.position = idlePosition;
					Projectile.velocity *= 0.20f;
					Projectile.netUpdate = true;
				}

				// If your minion is flying, you want to do this independently of any conditions
				float overlapVelocity = 0.04f;
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					// Fix overlap with other minions
					Projectile other = Main.projectile[i];
					if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
					{
						if (Projectile.position.X < other.position.X) Projectile.velocity.X -= overlapVelocity;
						else Projectile.velocity.X += overlapVelocity;

						if (Projectile.position.Y < other.position.Y) Projectile.velocity.Y -= overlapVelocity;
						else Projectile.velocity.Y += overlapVelocity;
					}
				}
				#endregion


				#region Find target
				// Starting search distance
				float distanceFromTarget = 900f;
				Vector2 targetCenter = Projectile.position;
				bool foundTarget = false;

				// This code is required if your minion weapon has the targeting feature
				if (player.HasMinionAttackTargetNPC)
				{
					NPC npc = Main.npc[player.MinionAttackTargetNPC];
					float between = Vector2.Distance(npc.Center, Projectile.Center);
					// Reasonable distance away so it doesn't target across multiple screens
					if (between < 2000f)
					{
						distanceFromTarget = between;
						targetCenter = npc.Center;
						foundTarget = true;
					}
				}
				if (!foundTarget)
				{
					// This code is required either way, used for finding a target
					for (int i = 0; i < Main.maxNPCs; i++)
					{
						NPC npc = Main.npc[i];
						if (npc.CanBeChasedBy())
						{
							float between = Vector2.Distance(npc.Center, Projectile.Center);
							bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
							bool inRange = between < distanceFromTarget;
							bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
							// Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
							// The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
							bool closeThroughWall = between < 100f;
							if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
							{
								distanceFromTarget = between;
								targetCenter = npc.Center;
								foundTarget = true;
							}
						}
					}
				}

				// friendly needs to be set to true so the minion can deal contact damage
				// friendly needs to be set to false so it doesn't damage things like target dummies while idling
				// Both things depend on if it has a target or not, so it's just one assignment here
				// You don't need this assignment if your minion is shooting things instead of dealing contact damage
				Projectile.friendly = foundTarget;
				#endregion

				#region Movement

				// Default movement parameters (here for attacking)
				float speed = 11f;
				float inertia = 20f;

				if (foundTarget)
				{
                    Projectile.rotation = Projectile.DirectionTo(targetCenter).ToRotation() - MathHelper.PiOver2;
					if (distanceFromTarget > 100f)
					{
						// The immediate range around the target (so it doesn't latch onto it when close)
						Vector2 direction = targetCenter - Projectile.Center;
						direction.Normalize();
						direction *= speed;
						Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;

                        Projectile.ai[1]++;

                        if (Projectile.ai[1] >= 30)
                        {
          
                            var EntitySource = Projectile.GetSource_Death();
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, direction.X * 25, direction.Y * 25, ProjectileID.Bullet, 40, 1, Main.myPlayer, 0, 0);
                            Projectile.ai[1] = 0;
                        }
                    }
				}
				else
                {
                    Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                    // Minion doesn't have a target: return to player and idle
                    if (distanceToIdlePosition > 600f)
					{
						// Speed up the minion if it's away from the player
						speed = 30f;
						inertia = 60f;
					}
					else
					{
						// Slow down the minion if closer to the player
						speed = 4f;
						inertia = 80f;
					}
					if (distanceToIdlePosition > 20f)
					{
						// The immediate range around the player (when it passively floats about)

						// This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
						vectorToIdlePosition.Normalize();
						vectorToIdlePosition *= speed;
						Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
					}
					else if (Projectile.velocity == Vector2.Zero)
					{
						// If there is a case where it's not moving at all, give it a little "poke"
						Projectile.velocity.X = -0.15f;
						Projectile.velocity.Y = -0.05f;
					}
				}
				#endregion

				#region Animation and visuals
				// So it will lean slightly towards the direction it's moving

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
				#endregion
			}
		}
	}
}
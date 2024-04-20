
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Projectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.Items.Weapons.Summon.DripplerStaff;
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

    public class DripperBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dripper");
			// Description.SetDefault("The Dripper will fight for you");
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ProjectileType<Drippler>()] > 0)
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

	public class DripplerStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Drippler Staff");
			// Tooltip.SetDefault("Summons an Drippler to fight with you");
			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller.
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
		}

        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.knockBack = 3f;
            Item.mana = 10;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(0, 0, 32, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item44;

            // These below are needed for a minion weapon
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = BuffType<DripperBuff>();
            // No buffTime because otherwise the item tooltip would say something like "1 minute duration"
            Item.shoot = ProjectileType<Drippler>();
        }

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemType<TerrorFragments>(), 5);
			recipe.AddIngredient(ItemType<DarkEssence>(), 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
			player.AddBuff(Item.buffType, 2);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GSummon"), player.position);
            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.
            // position = Main.MouseWorld;
			return true;
		}

		/*
		 * This minion shows a few mandatory things that make it behave properly. 
		 * Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
		 * If the player targets a certain NPC with right-click, it will fly through tiles to it
		 * If it isn't attacking, it will float near the player with minimal movement
		 */
		public class Drippler : ModProjectile
		{
			public override void SetStaticDefaults()
			{
                // DisplayName.SetDefault("Drippler");
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
            private Vector2 _targetOffset;
            private int _targetNpc = -1;
            Player Owner => Main.player[Projectile.owner];
			private float Timer
			{
				get => Projectile.ai[0];
				set => Projectile.ai[0] = value;
			}

			private int StickToNPC
			{
				get => (int)Projectile.ai[1];
				set => Projectile.ai[1] = value;
			}

		
            private void AI_Movement(Vector2 targetCenter, float moveSpeed, float accel = 1f)
            {
                //This code should give quite interesting movement
                //Accelerate to being on top of the player

                float distX = targetCenter.X - Projectile.Center.X;
                if (Projectile.Center.X < targetCenter.X && Projectile.velocity.X < moveSpeed)
                {
                    Projectile.velocity.X += accel;
                }
                else if (Projectile.Center.X > targetCenter.X && Projectile.velocity.X > -moveSpeed)
                {
                    Projectile.velocity.X -= accel;
                }

                //Accelerate to being above the player.
                float distY = targetCenter.Y - Projectile.Center.Y;
                if (Projectile.Center.Y < targetCenter.Y && Projectile.velocity.Y < moveSpeed)
                {
                    Projectile.velocity.Y += accel;
                }
                else if (Projectile.Center.Y > targetCenter.Y && Projectile.velocity.Y > -moveSpeed)
                {
                    Projectile.velocity.Y -= accel;
                }
            }


            public override void AI()
            {
                Projectile.spriteDirection = Projectile.direction;
                if (!SummonHelper.CheckMinionActive<DripperBuff>(Owner, Projectile))
					return;

				SummonHelper.SearchForTargets(Owner, Projectile,
					out bool foundTarget,
					out float distanceFromTarget,
					out Vector2 targetCenter);

				if (foundTarget)
				{
            
					if(_targetNpc != -1)
					{
                        AI_Sticking();
					}
					else
					{
                        AI_Movement(targetCenter, 3);
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

			private void AI_Sticking()
			{
				if (_targetNpc == -1)
					return;

				NPC targetNpc = Main.npc[_targetNpc];
				if (!targetNpc.active)
				{
					_targetNpc = -1;
					return;
				}

				if (!targetNpc.CanBeChasedBy())
				{
					_targetNpc = -1;
					return;
				}
                Vector2 targetPos = targetNpc.position - _targetOffset;
                Vector2 directionToTarget = Projectile.position.DirectionTo(targetPos);
                float dist = Vector2.Distance(Projectile.position, targetPos);
                Projectile.velocity = (directionToTarget * dist) + new Vector2(0.001f, 0.001f);
				Timer++;
				if(Timer >= 90)
				{
                    float speedX = Main.rand.Next(-15, 15);
                    float speedY = Main.rand.Next(-15, -15);
                    Vector2 speed = new Vector2(speedX, speedY);

                    SoundEngine.PlaySound(SoundID.NPCHit18, targetNpc.Center);
                    SoundEngine.PlaySound(SoundID.Item171, targetNpc.Center);

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), (int)targetNpc.Center.X, (int)targetNpc.Center.Y, speed.X, speed.Y,
						ModContent.ProjectileType<BloodWaterProj>(), Projectile.damage / 2, 1f, Projectile.owner);
					Timer = 0;
                }
            }


            public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
            {
                base.OnHitNPC(target, hit, damageDone);
			
				if(_targetNpc == -1)
				{
                    _targetNpc = target.whoAmI;
                    _targetOffset = (target.position - Projectile.position) + new Vector2(0.001f, 0.001f);
                }
            }

            private void Visuals()
			{
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

                if (Main.rand.NextBool(3))
                {
                    int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.MeteorHead);
                    int dust1 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.MeteorHead);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust1].noGravity = true;
                }

                // Some visuals here
                Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
            }
		}
	}
}
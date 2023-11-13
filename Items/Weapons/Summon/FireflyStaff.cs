using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Items.Materials;
using Stellamod.Particles;
using Stellamod.Projectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Summon
{
    public class FireflyMinionBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ProjectileType<FireflyMinion>()] > 0)
			{
				int fireflyCount = player.ownedProjectileCounts[ProjectileType<FireflyMinion>()];
				int fireflyMinionType = ProjectileType<FireflyMinion>();
				player.buffTime[buffIndex] = 18000;

				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile other = Main.projectile[i];
					//Ignore projectiles that are not fireflies and are from a different owner.
					if (other.type != fireflyMinionType)
						continue;
					if (other.owner != player.whoAmI)
						continue;


					FireflyMinion fireflyMinion = other.ModProjectile as FireflyMinion;
					if (fireflyMinion.AttackStyle == FireflyMinion.AttackState.Defense_Mode)
					{
						player.statDefense += fireflyCount * 7;
						player.lifeRegen += fireflyCount * 3;
						player.nightVision = true;
						break;
					} else
					{
						//player.wingTime += fireflyCount * 1;
						player.moveSpeed += 0.1f * fireflyCount;
						player.wingRunAccelerationMult += 0.05f * fireflyCount;
						break;
                    }
				}
			}
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}

	public class FireflyStaff : ModItem
    {
		private FireflyMinion.AttackState _attackState;
		public override void SetDefaults()
		{
			Item.damage = 34;
			Item.knockBack = 3f;
			Item.mana = 10;
			Item.width = 48;
			Item.height = 72;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.buyPrice(0, 30, 0, 0);
			Item.rare = ItemRarityID.LightRed;

			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.UseSound = SoundID.Item46;
			Item.DamageType = DamageClass.Summon;
			Item.buffType = BuffType<FireflyMinionBuff>();
			Item.shoot = ProjectileType<FireflyMinion>();
		}

        public override bool? UseItem(Player player)
        {
			if(player.altFunctionUse == 2)
			{
				if (_attackState == FireflyMinion.AttackState.Defense_Mode)
				{
					_attackState = FireflyMinion.AttackState.Attack_Mode;
					SoundEngine.PlaySound(SoundID.Item46);

				}
				else
				{
					_attackState = FireflyMinion.AttackState.Defense_Mode;
					SoundEngine.PlaySound(SoundID.Item43);
				}
				

				int fireflyMinionType = ProjectileType<FireflyMinion>();
				//Loop over and swap the projectile attack states
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile other = Main.projectile[i];
					//Ignore projectiles that are not fireflies and are from a different owner.
					if (other.type != fireflyMinionType)
						continue;
					if (other.owner != player.whoAmI)
						continue;

				
					FireflyMinion fireflyMinion = other.ModProjectile as FireflyMinion;
					fireflyMinion.AttackStyle = _attackState;

					//Dust Burst in Circle at Muzzle
					int count = 32;
					float degreesPer = 360 / (float)count;
					for (int k = 0; k < count; k++)
					{
						float degrees = k * degreesPer;
						Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
						Vector2 vel = direction * 8;
						Dust.NewDust(other.Center, 0, 0, DustID.CopperCoin, vel.X * 0.5f, vel.Y * 0.5f);
					}
				}

				return true;
            }

            return base.UseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
			player.AddBuff(Item.buffType, 2);

			// Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
			projectile.originalDamage = Item.damage;

			FireflyMinion fireflyMinion = projectile.ModProjectile as FireflyMinion;
			fireflyMinion.AttackStyle = _attackState;
			// Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
			return false;
		}

        public override void AddRecipes()
        {
            base.AddRecipes();
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.AddIngredient(ItemType<PearlescentScrap>(), 12);
			recipe.AddIngredient(ItemType<LostScrap>(), 10);
			recipe.AddIngredient(ItemID.SoulofLight, 10);
			recipe.AddIngredient(ItemID.LifeCrystal, 1);
			recipe.AddIngredient(ItemID.Firefly, 1);
			recipe.Register();
		}
    }

	public class FireflyMinion : ModProjectile
	{
		private float _attackCooldown;
		private static float _orbitingOffset;
		public enum AttackState
        {
			Defense_Mode = 0,
			Attack_Mode = 1
        };

		public AttackState AttackStyle { get; set; }
		public override void SetStaticDefaults()
		{
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 4;
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

			Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
		}

		public sealed override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			Projectile.tileCollide = false; // Makes the minion go through tiles freely

			// These below are needed for a minion weapon
			Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.minion = true; // Declares this as a minion (has many effects)
			Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
			Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
		}

		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles()
		{
			return false;
		}

		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
		public override bool MinionContactDamage()
		{
			//Only have contact damage in defense mode.
			return AttackStyle == AttackState.Defense_Mode;
		}

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
			if(AttackStyle == AttackState.Defense_Mode)
            {
				//Boosted Contact Damage in defense mode
				modifiers.FinalDamage += 3.5f;
	
			}
        }

		public Color GlowColor;
		public float HuntrianColorX;
		public float HuntrianColorZ;
		public float HuntrianColorY;
		public float HuntrianColorOffset;
		public float Timer;

		public override void PostDraw(Color lightColor)
		{
			Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
			for(int i = 0; i < 4; i++)
			{
				Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(HuntrianColorX * 1), (int)(HuntrianColorY * 1), (int)(HuntrianColorZ * 1), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
			}

			Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(HuntrianColorX * 1), (int)(HuntrianColorY * 1), (int)(HuntrianColorZ * 1), 0), Projectile.rotation, new Vector2(32, 32), 0.07f * (7 + 0.6f), SpriteEffects.None, 0f);
			Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3() * 1.0f * Main.essScale);
		}

		// The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];
			if (!CheckActive(owner))
			{
				return;
			}

			HuntrianColorZ = VectorHelper.Osc(15f, 60, 3, HuntrianColorOffset);
			HuntrianColorY = VectorHelper.Osc(45f, 60, 3, HuntrianColorOffset);
			HuntrianColorX = VectorHelper.Osc(85f, 15, 3, HuntrianColorOffset);
			Timer++;
			if (Timer <= 2)
			{
				HuntrianColorOffset = Main.rand.NextFloat(-1f, 1f);
			}

			//Fierflies orbit faster the more you have.

			Vector2 circlePosition = CalculateCirclePosition(owner);
			Projectile.Center = circlePosition;
			float attackCooldown;
			switch (AttackStyle)
            {
				default:
				case AttackState.Defense_Mode:
					//Attack Slower in defense mode
					_orbitingOffset += 0.01f;
					attackCooldown = 80;
					break;
				case AttackState.Attack_Mode:
					_orbitingOffset += 0.33f;
					attackCooldown = 17;
					break;
            }

			SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
			_attackCooldown--;
			if (_attackCooldown <= 0 && foundTarget)
			{
				//Fire Projectile
				Vector2 vectorToTarget = targetCenter - Projectile.Center;
				Vector2 directionToTarget = vectorToTarget.SafeNormalize(Vector2.Zero);
				Vector2 velocity = directionToTarget * 30;
				Projectile.NewProjectileDirect(owner.GetSource_FromThis(), Projectile.Center, velocity, ProjectileType<FireflyBomb>(), Projectile.damage, Projectile.knockBack, owner.whoAmI);

				//How many ticks between attacks?
				_attackCooldown = attackCooldown;
				//_scaleOffset += 0.1f;
			}

			Visuals();
		}

		// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
		private bool CheckActive(Player owner)
		{
			if (owner.dead || !owner.active)
			{
				owner.ClearBuff(BuffType<FireflyMinionBuff>());
				return false;
			}

			if (owner.HasBuff(BuffType<FireflyMinionBuff>()))
			{
				Projectile.timeLeft = 2;
			}

			return true;
		}


		private Vector2 CalculateCirclePosition(Player owner)
        {
			int minionIndex = 0;
			// Fix overlap with other minions
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile other = Main.projectile[i];

				//Ignore projectiles that are not fireflies and are from a different owner.
				if (other.type != Projectile.type)
					continue;
				if (other.owner != Projectile.owner)
					continue;

				//If the project is not me, then increase the index.
				if(i != Projectile.whoAmI)
                {
					minionIndex++;
                }
                else
                {
					//We found ourselves, therefore we have our minion index.
					break;
                }
			}

			//Now we can calculate the circle position	
			int fireflyCount = owner.ownedProjectileCounts[ProjectileType<FireflyMinion>()];
			float degreesBetweenFirefly = 360 / (float)fireflyCount;
			float degrees = degreesBetweenFirefly * minionIndex;
			float circleDistance = AttackStyle == AttackState.Defense_Mode ? 48f : 80f;

			Vector2 circlePosition = owner.Center + new Vector2(circleDistance, 0).RotatedBy(MathHelper.ToRadians(degrees + _orbitingOffset));
			switch (AttackStyle)
            {
				case AttackState.Attack_Mode:

					break;
				case AttackState.Defense_Mode:
					//float factor =  / (float)fireflyCount;
					float t = _orbitingOffset + (minionIndex / (float)fireflyCount) * minionIndex;
					float offset = VectorHelper.Osc(5, 10);
					float x = 16 * MathF.Pow(MathF.Sin(t), 3);
					float y = 13f * MathF.Cos(t) - 5 * MathF.Cos(2 * t) - 2 * MathF.Cos(3 * t) - MathF.Cos(4 * t);
					circlePosition = owner.Center + new Vector2(x * offset, -y * offset);
					break;
            }

			return circlePosition;
		}


		private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
		{
			// Starting search distance
			distanceFromTarget = 700f;
			targetCenter = Projectile.position;
			foundTarget = false;
			if (owner.HasMinionAttackTargetNPC)
			{
				NPC npc = Main.npc[owner.MinionAttackTargetNPC];
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
                        if (((closest && inRange) || !foundTarget) && lineOfSight)
						{
							distanceFromTarget = between;
							targetCenter = npc.Center;
							foundTarget = true;
                        }
					}
				}
			}
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

            if (Main.rand.NextBool(12))
            {
				int count = 3; 
				for (int k = 0; k < count; k++)
				{
					Dust.NewDust(Projectile.position, 8, 8, DustID.CopperCoin);
				}
			}

			if (Main.rand.NextBool(12))
            {
				Vector2 randomOrigin = new Vector2(Main.rand.NextFloat(-8, 8f));
				Vector2 speed = Main.rand.NextVector2Circular(1f, 1f); 
				ParticleManager.NewParticle(Projectile.Center - randomOrigin, speed * 2, ParticleManager.NewInstance<SparkleTrailParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
			}

			// Some visuals here
			Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
		}
	}
}

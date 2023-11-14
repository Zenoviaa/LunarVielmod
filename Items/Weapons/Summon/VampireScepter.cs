using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Items.Materials;
using Stellamod.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;


namespace Stellamod.Items.Weapons.Summon
{
    public class VampirePlayer : ModPlayer
    {
		public bool lifesteal;
        public override void ResetEffects()
        {
            base.ResetEffects();
			lifesteal = false;
        }

        public override void PostUpdateBuffs()
        {
            base.PostUpdateBuffs();
			if (lifesteal)
			{
				float radius = 320;
				int count = 64;
				for (int i = 0; i < count; i++)
				{
					Vector2 position = Player.Center + new Vector2(radius, 0).RotatedBy((i * MathHelper.PiOver2 / count) * 4);
					Dust.NewDust(position, 1, 1, DustID.Blood, 0f, 0f, 0, default(Color), 1f);
				}
			}
		}

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPCWithProj(proj, target, hit, damageDone);
			if(lifesteal && proj.DamageType == DamageClass.Summon)
            {
				float distanceToTarget = Vector2.Distance(Player.position, target.position);
				//10 tile radius
				if(distanceToTarget <= 320)
                {
					//Life steal for 5% of the damage
					float healFactor = damageDone * 0.08f;
					int healthToHeal = (int)healFactor;
					healthToHeal = Math.Clamp(healthToHeal, 1, 20);
					Player.Heal(healthToHeal);

					int count = 8;
					float degreesPer = 360 / (float)count;
					for (int k = 0; k < count; k++)
					{
						float degrees = k * degreesPer;
						Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
						Vector2 vel = direction * 2;
						Dust.NewDust(target.Center, 0, 0, DustID.BloodWater, vel.X, vel.Y);
					}
					Dust.QuickDustLine(Player.Center, target.Center, 100f, Color.Red);
					SoundEngine.PlaySound(SoundID.NPCHit18);
				}
			}
        }
    }

	public class VampireTorchMinionBuff : ModBuff
	{
		private int _vampiricTimer;
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		private void SearchForTargets(Player player, out bool foundTarget, out float distanceFromTarget)
		{
			// Starting search distance
			distanceFromTarget = 700f;
			foundTarget = false;
			if (!foundTarget)
			{
				// This code is required either way, used for finding a target
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];
					float between = Vector2.Distance(npc.Center, player.Center);
					bool inRange = between < distanceFromTarget;
					if (inRange)
					{
						foundTarget = true;
						distanceFromTarget = between;
					}
				}
			}


		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ProjectileType<VampireTorchMinion>()] > 0)
			{
				player.buffTime[buffIndex] = 18000;
				player.lifeRegenCount = 0;

				//Health Loss
				SearchForTargets(player, out bool foundTarget, out float distanceFromTarget);
                if (foundTarget)
				{
					_vampiricTimer++;
					if (_vampiricTimer >= 9 && player.statLife > 10)
					{
						player.statLife += -1;
						_vampiricTimer = 0;
					}
				}

				player.GetDamage(DamageClass.Summon) += 0.25f;
				player.GetModPlayer<VampirePlayer>().lifesteal = true;
			}
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}

	public class VampireScepter : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 34;
			Item.knockBack = 3f;
			Item.mana = 10;
			Item.width = 40;
			Item.height = 48;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.buyPrice(0, 30, 0, 0);
			Item.rare = ItemRarityID.LightRed;

			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.UseSound = SoundID.Item46;
			Item.DamageType = DamageClass.Summon;
			Item.buffType = BuffType<VampireTorchMinionBuff>();
			Item.shoot = ProjectileType<VampireTorchMinion>();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			//Only allow one
			if (player.ownedProjectileCounts[ProjectileType<VampireTorchMinion>()] > 0)
				return false;
				// This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
			player.AddBuff(Item.buffType, 2);

			// Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
			projectile.originalDamage = Item.damage;
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
			recipe.AddIngredient(ItemID.SoulofNight, 10);
			recipe.AddIngredient(ItemID.BloodMoonStarter, 1);
			recipe.AddIngredient(ItemID.SanguineStaff, 1);
			recipe.Register();
		}
    }

	public class VampireTorchMinion : ModProjectile
	{
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
			Projectile.width = 20;
			Projectile.height = 42;
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
		// The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];
			if (!CheckActive(owner))
			{
				return;
			}

			//This minion doesn't attack
			Projectile.Center = owner.Center - new Vector2(0, 96);
			Visuals();
		}

		// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
		private bool CheckActive(Player owner)
		{
			if (owner.dead || !owner.active)
			{
				owner.ClearBuff(BuffType<VampireTorchMinionBuff>());
				return false;
			}

			if (owner.HasBuff(BuffType<VampireTorchMinionBuff>()))
			{
				Projectile.timeLeft = 2;
			}

			return true;
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

			//Bloody :3
			if (Main.rand.NextBool(12))
			{
				int count = 3;
				for (int k = 0; k < count; k++)
				{
					Dust.NewDust(Projectile.position, 8, 8, DustID.Blood);
				}
			}

			// Some visuals here
			Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
		}
	}
}
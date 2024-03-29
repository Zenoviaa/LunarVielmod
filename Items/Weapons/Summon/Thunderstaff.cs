using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Particles;
using Stellamod.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Summon
{
    public class Thunderstaff : ModItem
    {
        public override void SetDefaults()
        {
			Item.damage = 48;
			Item.knockBack = 3f;
			Item.mana = 10;
			Item.width = 48;
			Item.height = 62;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(0, 1, 33, 0);
            Item.rare = ItemRarityID.LightRed;

			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.UseSound = SoundID.Item46;
			Item.DamageType = DamageClass.Summon;
			Item.buffType = BuffType<CloudMinionBuff>();
			Item.shoot = ProjectileType<CloudMinion>();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
			player.AddBuff(Item.buffType, 2);

			// Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
			position = Main.MouseWorld;
			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
			projectile.originalDamage = Item.damage;

			// Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
			return false;
		}

        public override void AddRecipes()
        {
            base.AddRecipes();
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.AddIngredient(ItemType<StickOfWisdom>(), 1);
			recipe.AddIngredient(ItemID.SoulofFlight, 12);
			recipe.AddIngredient(ItemType<PearlescentScrap>(), 8);
			recipe.AddIngredient(ItemType<WinterbornShard>(), 6);
			recipe.Register();
		}
    }

	public class CloudMinionBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ProjectileType<CloudMinion>()] > 0)
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

	public class CloudMinion : ModProjectile
	{
		private static float _orbitCounter;
		public enum AttackState
        {
			Frost_Attack = 0,
			Lightning_Attack = 1,
			Tornado_Attack = 2
        }

		private ClimateTornadoProj _tornadoProj;
		private Vector2 _targetIdlePosition;
		private float _movementCounter;
		public AttackState State { get; set; }
		public override void SetStaticDefaults()
		{
			// Sets the amount of frames this minion has on its spritesheet
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

			Main.projFrames[Projectile.type] = 4;
			Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
		}

		public sealed override void SetDefaults()
		{
			Projectile.width = 34;
			Projectile.height = 34;
			Projectile.tileCollide = false; // Makes the minion go through tiles freely

			// These below are needed for a minion weapon
			Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.minion = true; // Declares this as a minion (has many effects)
			Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
			Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
		}

		private float _attackCounter;
		private void KillTornado()
        {
			if (_tornadoProj != null && _tornadoProj.Projectile.active)
            {
				_tornadoProj.Projectile.Kill();
				_tornadoProj = null;
			}
		}

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];
			if (!SummonHelper.CheckMinionActive<CloudMinionBuff>(owner, Projectile))
				return;

			//minion count
			int minionCount = owner.ownedProjectileCounts[ProjectileType<CloudMinion>()];
			if(minionCount <= 3)
            {
				State = AttackState.Frost_Attack;
            } else if (minionCount <= 6)
            {
				State = AttackState.Lightning_Attack;
            }
            else
            {
				State = AttackState.Tornado_Attack;
            }

			SummonHelper.CalculateIdleValues(owner, Projectile, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
			SummonHelper.SearchForTargets(owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
			switch (State)
            {
				case AttackState.Frost_Attack:
					KillTornado();
					SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);

					//Frost Nearby Enemies
					if (foundTarget)
                    {
						//Shoot
						_attackCounter++;
						if(_attackCounter > 90)
                        {
							Vector2 velocity = VectorHelper.VelocityDirectTo(Projectile.Center, targetCenter, 30);

							//Auroran Bullet Placeholder, it will be instanteous lightning projectile
							//Maybe just directly damage the target? idk
							Projectile projectile = Projectile.NewProjectileDirect(owner.GetSource_FromThis(), Projectile.Center, velocity, 
								ProjectileType<ClimateIceProj>(), Projectile.damage, Projectile.knockBack, owner.whoAmI);
							
							projectile.DamageType = DamageClass.Summon;
							_attackCounter = 0;
							SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Crysalizer1"), Projectile.position);
						}
                    }

					//Ice Particles

					//Turn into thunderclouds and zap nearby enemies for a few seconds
					break;
                
				case AttackState.Lightning_Attack:
					KillTornado();

					//I want  it to move around erratically
					_movementCounter++;
		
					if(_movementCounter >= Main.rand.Next(20, 30) || _targetIdlePosition == Vector2.Zero)
					{
						int range = 200;
						if (!foundTarget)
							targetCenter = owner.Center;

						_movementCounter = 0;
						//Get a new position
						_targetIdlePosition = new Vector2(targetCenter.X + Main.rand.NextFloat(-range, range),
							targetCenter.Y + Main.rand.NextFloat(-range, range));
                    }

					Projectile.velocity = VectorHelper.VelocityDirectTo(Projectile.position, _targetIdlePosition, 10f);

					//Zap Nearby Enemies
					if (foundTarget)
					{
						_attackCounter++;
						if (_attackCounter > 15)
						{
							Vector2 velocity = VectorHelper.VelocityDirectTo(Projectile.Center, targetCenter, 60);

							//Auroran Bullet Placeholder, it will be instanteous lightning projectile
							//Maybe just directly damage the target? idk
							Projectile projectile = Projectile.NewProjectileDirect(owner.GetSource_FromThis(), Projectile.Center, velocity,
								ProjectileID.DD2LightningBugZap, Projectile.damage / 2, Projectile.knockBack, owner.whoAmI);

							projectile.timeLeft = 60;
							projectile.friendly = true;
							projectile.hostile = false;
							projectile.DamageType = DamageClass.Summon;
							_attackCounter = 0;
							SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap, Projectile.position);
						}
					}
						
					//Merge above the player and do large thunderbolts
					break;

                case AttackState.Tornado_Attack:
					//OK SO
					//WHAT WE NEED TO DO IS.
					//Have the guys move in a n ellipse, how do we do tahat?
					_orbitCounter += 0.2f;
					Projectile.Center = CalculateCirclePosition(owner);
					if(_tornadoProj == null)
                    {
						Projectile projectile = Projectile.NewProjectileDirect(owner.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
							ProjectileType<ClimateTornadoProj>(), Projectile.damage * 2, Projectile.knockBack, owner.whoAmI);

						projectile.DamageType = DamageClass.Summon;
						_tornadoProj = projectile.ModProjectile as ClimateTornadoProj;
					}

					//Massive tornado :P
					_tornadoProj.Projectile.timeLeft = 32;
					_tornadoProj.Projectile.Center = Projectile.Center;
					break; 
            }

            Visuals();
		}

		private Vector2 CalculateCirclePosition(Player owner)
		{
			//Get the index of this minion
			int minionIndex = SummonHelper.GetProjectileIndex(Projectile);

			//Now we can calculate the circle position	
			int count = owner.ownedProjectileCounts[ProjectileType<CloudMinion>()];
			float between = 360 / (float)count;
			float degrees = between * minionIndex;
			float circleDistance = 256+16;
			Vector2 circlePosition = owner.Center + new Vector2(circleDistance, 0).RotatedBy(MathHelper.ToRadians(degrees + _orbitCounter));
			return circlePosition;
		}

		public Vector3 HuntrianColorXyz;

        public override bool PreDraw(ref Color lightColor)
		{
			switch (State)
			{
				case AttackState.Frost_Attack:
					DrawHelper.DrawAdditiveAfterImage(Projectile, Color.LightCyan, Color.White, ref lightColor);
					break;
				case AttackState.Lightning_Attack:
					DrawHelper.DrawAdditiveAfterImage(Projectile, Color.MediumPurple, Color.White, ref lightColor);
					break;
				case AttackState.Tornado_Attack:
					DrawHelper.DrawAdditiveAfterImage(Projectile, Color.LightGreen, Color.White, ref lightColor);
					break;
			}

			return true;
        }

        public override void PostDraw(Color lightColor)
		{
            switch (State)
            {
				case AttackState.Frost_Attack:
					DrawHelper.DrawDimLight(Projectile, HuntrianColorXyz.X, HuntrianColorXyz.Y, HuntrianColorXyz.Z, Color.LightCyan, lightColor, 2);
					break;
				case AttackState.Lightning_Attack:
					DrawHelper.DrawDimLight(Projectile, HuntrianColorXyz.X, HuntrianColorXyz.Y, HuntrianColorXyz.Z, Color.MediumPurple, lightColor, 2);
					break;
				case AttackState.Tornado_Attack:
					DrawHelper.DrawDimLight(Projectile, HuntrianColorXyz.X, HuntrianColorXyz.Y, HuntrianColorXyz.Z, Color.LightGreen, lightColor, 2);
					break;
			}
		}

        private void Visuals()
		{
			// So it will lean slightly towards the direction it's moving
			Projectile.rotation = Projectile.velocity.X * 0.05f;
			DrawHelper.AnimateTopToBottom(Projectile, 7);

			switch (State)
			{
				case AttackState.Frost_Attack:
					HuntrianColorXyz = DrawHelper.HuntrianColorOscillate(
						new Vector3(85, 45, 150),
						new Vector3(15, 60, 60),
						new Vector3(3, 3, 3), 0);


					if (Main.rand.NextBool(12))
					{
						int count = 2;
						for (int k = 0; k < count; k++)
						{
							Dust.NewDust(Projectile.position, 8, 8, DustID.Frost);
						}
					}

					if (Main.rand.NextBool(12)) 
					{ 
						for (int j = 0; j < 5; j++)
						{
							Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
							var particle = ParticleManager.NewParticle(Projectile.Center, speed, ParticleManager.NewInstance<IceyParticle>(), Color.White, Main.rand.NextFloat(.2f, .4f));
							particle.timeLeft = 12;
						}
					}
					break;
				case AttackState.Lightning_Attack:
					HuntrianColorXyz = DrawHelper.HuntrianColorOscillate(
						new Vector3(125, 100, 40),
						new Vector3(15, 60, 60),
						new Vector3(3, 3, 3), 0);

					if (Main.rand.NextBool(12))
					{
						int count = 2;
						for (int k = 0; k < count; k++)
						{
							Dust.NewDust(Projectile.position, 8, 8, DustID.Electric);
						}
					}

					break;
				case AttackState.Tornado_Attack:
					HuntrianColorXyz = DrawHelper.HuntrianColorOscillate(
						new Vector3(125, 150, 40),
						new Vector3(15, 60, 60),
						new Vector3(3, 3, 3), 0);

					if (Main.rand.NextBool(12))
					{
						int count = 2;
						for (int k = 0; k < count; k++)
						{
							Dust.NewDust(Projectile.position, 8, 8, DustID.Vortex);
						}
					}

					break;
			}

			// Some visuals here
			Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
		}
	}
}

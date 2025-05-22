using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles;
using Stellamod.Projectiles.IgniterExplosions.Stein;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Projectiles.Steins
{


	public class WOW : ModProjectile
	{
		private static float _orbitCounter;
		public enum AttackState
		{
			Frost_Attack = 0,
			Lightning_Attack = 1,
			Tornado_Attack = 2
		}

		public AttackState State { get; set; }
		public override void SetStaticDefaults()
		{
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 30;
			Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
		}

		public sealed override void SetDefaults()
		{
			Projectile.width = 132;
			Projectile.height = 36;
			Projectile.tileCollide = false; // Makes the minion go through tiles freely

			// These below are needed for a minion weapon
			Projectile.friendly = false; // Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.minion = true; // Declares this as a minion (has many effects)
			Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
			Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
		}

		private float _attackCounter;

		private static float _orbitingOffset;
		public override bool? CanCutTiles()
		{
			return false;
		}

		public override bool MinionContactDamage()
		{
			return false;
		}
		int Explosion = 0;

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];
			Explosion++;

			if (Explosion > 179)
			{

				Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 512f, 16f);
				for (int i = 0; i < 5; i++)
				{
					float speedX = Main.rand.Next(-9, 9);
					float speedY = Main.rand.Next(-9, 9);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, speedX, speedY, ModContent.ProjectileType<ShadingShot>(), (int)(Projectile.damage * 1), 0f, Projectile.owner, 0f, 0f);
					Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GunFlash>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Purple, 1f).noGravity = true;
				}
				for (int i = 0; i < 4; i++)
				{
					Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LumiDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(19.0), 170, Color.Purple, 1f).noGravity = true;
				}
				for (int i = 0; i < 4; i++)
				{
					Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Black, 0.5f).noGravity = true;
				}
				Projectile.Kill();
			}

			Vector2 circlePosition = CalculateCirclePosition(owner);
			float speed = 48;
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, VectorHelper.VelocitySlowdownTo(Projectile.Center, circlePosition, speed), 0.1f);



			Visuals();
		}

		private Vector2 CalculateCirclePosition(Player owner)
		{
			//Get the index of this minion
			int minionIndex = SummonHelper.GetProjectileIndexMulti(Projectile, ModContent.ProjectileType<WOW>(), ModContent.ProjectileType<AMAZING>(), ModContent.ProjectileType<GREAT>(), ModContent.ProjectileType<SOHOT>(), ModContent.ProjectileType<SEXY>());


			//Now we can calculate the circle position	
			int minionCount = owner.ownedProjectileCounts[ModContent.ProjectileType<WOW>()];
			minionCount += owner.ownedProjectileCounts[ModContent.ProjectileType<AMAZING>()];
			minionCount += owner.ownedProjectileCounts[ModContent.ProjectileType<GREAT>()];
			minionCount += owner.ownedProjectileCounts[ModContent.ProjectileType<SOHOT>()];
			minionCount += owner.ownedProjectileCounts[ModContent.ProjectileType<SEXY>()];

			float degreesBetweenFirefly = 360 / (float)minionCount;
			float degrees = degreesBetweenFirefly * minionIndex;
			float circleDistance = 96f;
			Vector2 circlePosition = owner.Center + new Vector2(circleDistance, 0).RotatedBy(MathHelper.ToRadians(degrees + _orbitingOffset));
			return circlePosition;
		}

		public Vector3 HuntrianColorXyz;


		private void Visuals()
		{
			// So it will lean slightly towards the direction it's moving
			Projectile.rotation = Projectile.velocity.X * 0.05f;
			DrawHelper.AnimateTopToBottom(Projectile, 1);




			// Some visuals here
			Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
		}
	}
}
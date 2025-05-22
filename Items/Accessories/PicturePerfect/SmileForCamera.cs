using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Accessories.PicturePerfect
{
    public class CameraMinBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ProjectileType<SmileForCamera>()] > 0)
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

	public class SmileForCamera : ModProjectile
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
			Main.projFrames[Projectile.type] = 60;
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
			Projectile.friendly = false; // Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.minion = true; // Declares this as a minion (has many effects)
			Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
			Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
		}

		private float _attackCounter;
		

		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];
			if (!SummonHelper.CheckMinionActive<CameraMinBuff>(owner, Projectile))
				return;

			//minion count
			int minionCount = owner.ownedProjectileCounts[ProjectileType<SmileForCamera>()];
			

			SummonHelper.CalculateIdleValues(owner, Projectile, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
			SummonHelper.SearchForTargets(owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);


			_orbitCounter += 0.2f;
			Projectile.Center = CalculateCirclePosition(owner);

			Visuals();
		}

		private Vector2 CalculateCirclePosition(Player owner)
		{
			//Get the index of this minion
			int minionIndex = SummonHelper.GetProjectileIndex(Projectile);

			//Now we can calculate the circle position	
			int count = owner.ownedProjectileCounts[ProjectileType<SmileForCamera>()];
			float between = 360 / (float)count;
			float degrees = between * minionIndex;
			float circleDistance = 96;
			Vector2 circlePosition = owner.Center + new Vector2(circleDistance, 0).RotatedBy(MathHelper.ToRadians(degrees + _orbitCounter));
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
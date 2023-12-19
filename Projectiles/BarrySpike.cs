using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Items.Accessories.Catacombs;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class BarrySpike : ModProjectile
	{
		private float _degrees;
		public float DegreesOffset;
		public const float Rotation_Speed = 1;
		public const float Distance_From_Owner = 64;
		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 30;
			Projectile.timeLeft = 18;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 30;
			Projectile.tileCollide = false;
		}

		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

        public override void AI()
        {
			Player player = Main.player[Projectile.owner];
            if (!player.GetModPlayer<BarryPlayer>().hasBarry)
            {
				Projectile.Kill();
				return;
            }


			_degrees += Rotation_Speed;
			float degrees = _degrees + DegreesOffset;
			float circleDistance = Distance_From_Owner;
			Vector2 circlePosition = player.Center + new Vector2(circleDistance, 0).RotatedBy(MathHelper.ToRadians(degrees));

			Projectile.timeLeft = 2;
			Projectile.Center = circlePosition;
			Projectile.rotation = player.Center.DirectionTo(circlePosition).ToRotation();

			Vector3 RGB = new(1.95f, 0.9f, 2.55f);
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			// The hitDirection is always set to hit away from the player
			modifiers.HitDirectionOverride = (Main.player[Projectile.owner].Center.X < target.Center.X).ToDirectionInt();
		}


		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			float count = 32;
			float degreesPer = 360 / count;
			for (int k = 0; k < count; k++)
			{
				float degrees = k * degreesPer;
				Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
				Vector2 vel = direction * 4;
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, vel.X, vel.Y);
			}

			ParticleManager.NewParticle(target.Center, Vector2.Zero, ParticleManager.NewInstance<SparkleTrailParticle>(),
								  Color.White, Main.rand.NextFloat(0.5f, 0.75f));
			SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit2"));
			switch (Main.rand.Next(0, 3))
			{
				case 0:
					target.AddBuff(BuffID.Poisoned, 120);
					break;
				case 1:
					target.AddBuff(BuffID.Venom, 120);
					break;
				case 2:
					target.AddBuff(BuffID.StardustMinionBleed, 120);
					break;
			}
		}
    }
}
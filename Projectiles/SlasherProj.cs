using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.UI.Systems;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public abstract class SlasherProj : ModProjectile
	{
		public readonly int ChargeTime;
		private readonly int DustType;
		private readonly int Size;
		private readonly int MinKnockback;
		private readonly int MaxKnockback;
		private readonly float Acceleration;
		private readonly float MaxSpeed;

		public SlasherProj(int chargetime, int mindamage, int maxdamage, int dusttype, int size, int minknockback, int maxknockback, float acceleration, float maxspeed)
		{
			ChargeTime = chargetime;
			DustType = dusttype;
			Size = size;
			MinKnockback = minknockback;
			MaxKnockback = maxknockback;
			Acceleration = acceleration;
			MaxSpeed = maxspeed;


		}
		public virtual void SafeAI() { }
		public virtual void SafeDraw(SpriteBatch spriteBatch, Color lightColor) { }
		public virtual void SafeSetDefaults() { }

		public sealed override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.width = 25;
			Projectile.height = 26;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.alpha = 255;
			ProjectileID.Sets.TrailingMode[Type] = 0; // Creates a trail behind the golf ball.
			ProjectileID.Sets.TrailCacheLength[Type] = 20; // Sets the length of the trail.
			SafeSetDefaults();
		}
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Main.player[Projectile.owner].Center, Projectile.Center) ? true : base.Colliding(projHitbox, targetHitbox);
		public virtual void Smash(Vector2 position) { }

		public bool released = false;
		public double radians = 0;

		private float _angularMomentum = 1;
		private int _lingerTimer = 0;
		private int _flickerTime = 0;

		public SpriteEffects Effects => ((Main.player[Projectile.owner].direction * (int)Main.player[Projectile.owner].gravDir) < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
		public float TrueRotation => ((float)radians + 3.9f) + ((Effects == SpriteEffects.FlipHorizontally) ? MathHelper.PiOver2 : 0);
		public Vector2 Origin => (Effects == SpriteEffects.FlipHorizontally) ? new Vector2(Size, Size) : new Vector2(0, Size);

		public sealed override bool PreDraw(ref Color lightColor)

		{
			Color color = lightColor;
			Main.spriteBatch.Draw((Texture2D)TextureAssets.Projectile[Projectile.type], Main.player[Projectile.owner].Center - Main.screenPosition, new Rectangle(0, 0, Size, Size), color, TrueRotation, Origin, Projectile.scale, Effects, 0);
			SafeDraw(Main.spriteBatch, lightColor);
			if (Projectile.ai[0] >= ChargeTime && !released && _flickerTime < 16)
			{
				_flickerTime++;
				color = Color.White;
				float flickerTime2 = (_flickerTime / 20f);
				float alpha = 1.5f - (((flickerTime2 * flickerTime2) / 2) + (2f * flickerTime2));
				if (alpha < 0)
					alpha = 0;

				Main.spriteBatch.Draw((Texture2D)TextureAssets.Projectile[Projectile.type], Main.player[Projectile.owner].Center - Main.screenPosition, new Rectangle(0, Size, Size, Size), color * alpha, TrueRotation, Origin, Projectile.scale, Effects, 1);
			}
			return false;
		}

		public sealed override bool PreAI()
		{
			SafeAI();

			Projectile.scale = Projectile.ai[0] < 10 ? (Projectile.ai[0] / 10f) : 1;
			Player player = Main.player[Projectile.owner];
			player.heldProj = Projectile.whoAmI;
			int degrees = (int)((player.itemAnimation * -2.1) + 55) * player.direction * (int)player.gravDir;
			if (player.direction == 1)
				degrees += 180;



			radians = degrees * (Math.PI / 180);
			if (player.channel && !released)
			{
				if (Projectile.ai[0] == 0)
				{
					player.itemTime = 1;
					player.itemAnimation = 1;
				}
				if (Projectile.ai[0] < ChargeTime)
				{
					Projectile.ai[0]++;
					float rot = Main.rand.NextFloat(MathHelper.TwoPi);
					if (DustType != -1)
						Dust.NewDustPerfect(Projectile.Center + Vector2.One.RotatedBy(rot) * 35, DustType, -Vector2.One.RotatedBy(rot) * 1.5f, 0, default, Projectile.ai[0] / 100f);
					if (Projectile.ai[0] < ChargeTime / 1.5f || Projectile.ai[0] % 2 == 0)
						_angularMomentum = -1;
					else
						_angularMomentum = 0;
				}
				else
				{
					if (Projectile.ai[0] == ChargeTime)
					{
						for (int k = 0; k <= 10; k++)
						{
							if (DustType != -1)
								Dust.NewDustPerfect(Projectile.Center, DustType, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2), 0, default, 1.5f);
						}
						SoundEngine.PlaySound(SoundID.NPCDeath7, Projectile.Center);
						Projectile.ai[0]++;
						ShakeModSystem.Shake = 12;
					}
					if (DustType != -1)
						Dust.NewDustPerfect(Projectile.Center, DustType, Vector2.One.RotatedByRandom(MathHelper.TwoPi));

					_angularMomentum = 0;
				}

				Projectile.knockBack = MinKnockback + (int)((Projectile.ai[0] / ChargeTime) * (MaxKnockback - MinKnockback));
			}
			else
			{
				Projectile.scale = 1;

				if (_angularMomentum < MaxSpeed)
					_angularMomentum += Acceleration;

				if (!released)
				{

					released = true;
					Projectile.friendly = true;
					SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);

				}
			}


			Projectile.position.Y = player.Center.Y - (int)(Math.Sin(radians * 0.96) * Size) - (Projectile.height / 2);
			Projectile.position.X = player.Center.X - (int)(Math.Cos(radians * 0.96) * Size) - (Projectile.width / 2);
			if (_lingerTimer == 0)
			{
				player.itemTime++;
				player.itemAnimation++;
				if (player.itemTime > _angularMomentum + 1)
				{
					player.itemTime -= (int)_angularMomentum;
					player.itemAnimation -= (int)_angularMomentum;
				}
				else
				{
					player.itemTime = 2;
					player.itemAnimation = 2;
				}
				//if (player.itemTime == 2 || (Main.tile[(int)Projectile.Center.X / 16, (int)((Projectile.Center.Y + 24) / 16)]).CollisionType == 1 && released)
				{
					_lingerTimer = 30;

					if (Projectile.ai[0] >= ChargeTime)
						Smash(Projectile.Center);

					//if (Main.tile[(int)Projectile.Center.X / 16, (int)((Projectile.Center.Y + 24) / 16)].CollisionType == 1)

					Projectile.friendly = false;
					SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);
					SoundEngine.PlaySound(SoundID.NPCHit42, Projectile.Center);
				}
			}
			else
			{
				_lingerTimer--;
				if (_lingerTimer == 1)
				{
					Projectile.active = false;
					player.itemTime = 2;
					player.itemAnimation = 2;
				}
				player.itemTime++;
				player.itemAnimation++;
			}
			return true;
		}
	}
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using Stellamod.UI.Systems;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Crossbows.Lasers
{
	public class TheTraumatizerHold : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float SwordRotation => ref Projectile.ai[1];
        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 1;//number of frames the animation has
		}
		public override void SetDefaults()
		{
			Projectile.damage = 0;
			Projectile.width = 1;
			Projectile.height = 1;
			Projectile.aiStyle = 595;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.ownerHitCheck = true;
			Projectile.timeLeft = int.MaxValue;
		}

		public override bool? CanDamage()
		{
			return false;
		}

		public override void AI()
		{
			Timer++;
			if (Timer > 200)
			{
				// Our timer has finished, do something here:
				// Main.PlaySound, Dust.NewDust, Projectile.NewProjectile, etc. Up to you.		
				Timer = 0;
			}

			if(Timer % 10 == 0)
			{
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/TraumatizerLaserFire") with { PitchVariance = 0.1f }, Projectile.position);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position + Projectile.velocity.SafeNormalize(Vector2.Zero) * 25, Projectile.velocity, 
					ModContent.ProjectileType<TraumatizingRay>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);
            }

			Player player = Main.player[Projectile.owner];
			if (player.noItems || player.CCed || player.dead || !player.active)
				Projectile.Kill();

			Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
			if (Main.myPlayer == Projectile.owner)
			{
				player.ChangeDir(Projectile.direction);
				SwordRotation = (Main.MouseWorld - player.Center).ToRotation();
				Projectile.netUpdate = true;
				if (!player.channel)
					Projectile.Kill();
			}

			Projectile.velocity = SwordRotation.ToRotationVector2();
			Projectile.spriteDirection = player.direction;
			if (Projectile.spriteDirection == 1)
			{
				float targetRotation = Projectile.velocity.ToRotation();
				Projectile.rotation = targetRotation;
            }
			else
            {
                float targetRotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
				Projectile.rotation = targetRotation;
            }

			if (Timer < 3)
			{
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/TraumatizerLaserStart"));
            }

			if (Timer > 1)
			{
				
				ShakeModSystem.Shake = 2;
			}


			Projectile.Center = playerCenter + new Vector2(40, 0).RotatedBy(SwordRotation);// customization of the hitbox position

			player.heldProj = Projectile.whoAmI;
			player.itemTime = 2;
			player.itemAnimation = 2;
			player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

			if (++Projectile.frameCounter >= 1)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 1)
				{
					Projectile.frame = 0;
				}
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			//Player player = Main.player[Projectile.owner];
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (Projectile.spriteDirection == -1)
				spriteEffects = SpriteEffects.FlipHorizontally;
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			int frameHeight = texture.Height / Main.projFrames[Projectile.type];
			int startY = frameHeight * Projectile.frame;
			Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
			Vector2 origin = sourceRectangle.Size() / 2f;
			origin.X = Projectile.spriteDirection == 1 ? sourceRectangle.Width - 30 : 30; // Customization of the sprite position

			Color drawColor = Projectile.GetAlpha(lightColor);
			Main.EntitySpriteDraw((Texture2D)TextureAssets.Projectile[Projectile.type], Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
			return false;
		}
	}
}
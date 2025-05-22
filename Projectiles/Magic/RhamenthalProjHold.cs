using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.UI.Systems;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
	public class RhamenthalProjHold : ModProjectile
    {
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
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
			Projectile.DamageType = DamageClass.Magic;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.ownerHitCheck = true;
			Projectile.timeLeft = 80;
		}

		public override bool? CanDamage()
		{
			return false;
		}

		public override void AI()
		{
			Timer++;
			if (Timer > 75)
			{
				// Our timer has finished, do something here:
				// Main.PlaySound, Dust.NewDust, Projectile.NewProjectile, etc. Up to you.


				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/CyroBolt2"), Projectile.position);
				Timer = 0;
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
				Projectile.rotation = Projectile.velocity.ToRotation();
			else
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;


			if (Timer == 1)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/CorsageRune1"), Projectile.position);
			}

			if (Timer == 1)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;

				
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY, ModContent.ProjectileType<RhamenthalProj>(), Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);
			}


			if (Timer == 75)
			{
				float speedX = Projectile.velocity.X * 30;
				float speedY = Projectile.velocity.Y * 30;
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 1f, speedY, ModContent.ProjectileType<RhamenthalShot>(), (int)(Projectile.damage * 12), 0f, Projectile.owner, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Rhamenthal"), Projectile.position);
				ShakeModSystem.Shake = 9;
			}


			Projectile.Center = playerCenter + Projectile.velocity * 1f;// customization of the hitbox position

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
		private void UpdatePlayerVisuals(Player player, Vector2 playerhandpos)
		{
			Projectile.Center = playerhandpos;
			Projectile.spriteDirection = Projectile.direction;

			// Constantly resetting player.itemTime and player.itemAnimation prevents the player from switching items or doing anything else.
			player.ChangeDir(Projectile.direction);
			player.heldProj = Projectile.whoAmI;
			player.itemTime = 3;
			player.itemAnimation = 3;

			player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

		}
		public override bool PreDraw(ref Color lightColor)
		{
			Player player = Main.player[Projectile.owner];

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
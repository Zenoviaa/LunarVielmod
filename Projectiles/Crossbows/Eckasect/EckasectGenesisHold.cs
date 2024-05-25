using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using Stellamod.UI.Systems;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Crossbows.Eckasect
{
	public class EckasectGenesisHold : ModProjectile
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
			Projectile.timeLeft = 320;
		}

		public override bool? CanDamage()
		{
			return false;
		}

		public override void AI()
		{
			Timer++;
			if (Timer > 319)
			{
				// Our timer has finished, do something here:
				// Main.PlaySound, Dust.NewDust, Projectile.NewProjectile, etc. Up to you.		
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

				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<GenesisBolt1>(), Projectile.damage * 1, Projectile.knockBack, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Genesis1"), Projectile.position);
				ShakeModSystem.Shake = 2;
			}

			if (Timer == 20)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;

				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<GenesisBolt1>(), Projectile.damage * 1, Projectile.knockBack, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Genesis1"), Projectile.position);
				ShakeModSystem.Shake = 2;
			}

			if (Timer == 40)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;

				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<GenesisBolt1>(), Projectile.damage * 1, Projectile.knockBack, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Genesis1"), Projectile.position);
				ShakeModSystem.Shake = 2;
			}
			if (Timer == 60)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;

				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<GenesisBolt1>(), Projectile.damage * 1, Projectile.knockBack, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Genesis1"), Projectile.position);
				ShakeModSystem.Shake = 2;
			}

			if (Timer == 80)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;

				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<GenesisBolt2>(), Projectile.damage * 2, Projectile.knockBack, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Genesis2"), Projectile.position);
				ShakeModSystem.Shake = 2;
			}

			if (Timer == 100)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;

				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<GenesisBolt2>(), Projectile.damage * 2, Projectile.knockBack, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Genesis2"), Projectile.position);
				ShakeModSystem.Shake = 2;
			}

			if (Timer == 120)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;

				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<GenesisBolt2>(), Projectile.damage * 2, Projectile.knockBack, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Genesis2"), Projectile.position);
				ShakeModSystem.Shake = 2;
			}

			if (Timer == 140)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;

				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<GenesisBolt2>(), Projectile.damage * 2, Projectile.knockBack, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Genesis2"), Projectile.position);
				ShakeModSystem.Shake = 2;
			}

			if (Timer == 160)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;

				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<GenesisBolt3>(), Projectile.damage * 3, Projectile.knockBack, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Genesis3"), Projectile.position);
				ShakeModSystem.Shake = 2;
			}

			if (Timer == 180)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;

				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<GenesisBolt3>(), Projectile.damage * 3, Projectile.knockBack, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Genesis3"), Projectile.position);
				ShakeModSystem.Shake = 2;
			}

			if (Timer == 200)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;

				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<GenesisBolt3>(), Projectile.damage * 3, Projectile.knockBack, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Genesis3"), Projectile.position);
				ShakeModSystem.Shake = 2;
			}

			if (Timer == 220)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;

				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<GenesisBolt3>(), Projectile.damage * 3, Projectile.knockBack, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Genesis3"), Projectile.position);
				ShakeModSystem.Shake = 2;
			}

			if (Timer == 240)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;

				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<GenesisBolt1>(), Projectile.damage * 1, Projectile.knockBack, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Genesis4"), Projectile.position);
				ShakeModSystem.Shake = 2;
			}

			if (Timer == 260)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;

				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<GenesisBolt2>(), Projectile.damage * 1, Projectile.knockBack, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Genesis4"), Projectile.position);
				ShakeModSystem.Shake = 2;
			}

			if (Timer == 280)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;

				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<GenesisBolt3>(), Projectile.damage * 4, Projectile.knockBack, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Genesis4"), Projectile.position);
				ShakeModSystem.Shake = 2;
			}

			if (Timer == 300)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;

				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<GenesisBolt2>(), Projectile.damage * 2, Projectile.knockBack, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Genesis4"), Projectile.position);
				ShakeModSystem.Shake = 2;
			}

			Projectile.Center = playerCenter + new Vector2(80, 0).RotatedBy(SwordRotation);
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
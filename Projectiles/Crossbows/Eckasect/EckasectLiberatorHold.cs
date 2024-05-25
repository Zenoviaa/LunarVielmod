using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.UI.Systems;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Crossbows.Eckasect
{
	public class EckasectLiberatorHold : ModProjectile
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
			Projectile.timeLeft = 480;
		}

		public override bool? CanDamage()
		{
			return false;
		}

		public override void AI()
		{
			Timer++;
			if (Timer > 481)
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
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 3f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 6f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 40f, ModContent.ProjectileType<EckasectLiberatorBolt1>(), Projectile.damage * 4, Projectile.knockBack * 4, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Liberator1"), Projectile.position);
				ShakeModSystem.Shake = 4;
			}

			if (Timer == 61)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 3f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 6f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 40f, ModContent.ProjectileType<EckasectLiberatorBolt1>(), Projectile.damage * 4, Projectile.knockBack * 4, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Liberator1"), Projectile.position);
				ShakeModSystem.Shake = 4;
			}

			if (Timer == 121)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 3f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 6f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 40f, ModContent.ProjectileType<EckasectLiberatorBolt1>(), Projectile.damage * 3, Projectile.knockBack * 4, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Liberator1"), Projectile.position);
				ShakeModSystem.Shake = 4;
			}

			if (Timer == 181)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 3f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 6f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 40f, ModContent.ProjectileType<EckasectLiberatorBolt1>(), Projectile.damage * 2, Projectile.knockBack * 4, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Liberator1"), Projectile.position);
				ShakeModSystem.Shake = 4;
			}

			if (Timer == 241)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 3f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 6f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 40f, ModContent.ProjectileType<EckasectLiberatorBolt1>(), Projectile.damage * 3, Projectile.knockBack * 4, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Liberator2"));
				ShakeModSystem.Shake = 4;
			}

			if (Timer == 301)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 3f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 6f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 40f, ModContent.ProjectileType<EckasectLiberatorBolt1>(), Projectile.damage * 2, Projectile.knockBack * 4, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Liberator2"), Projectile.position);
				ShakeModSystem.Shake = 4;
			}

			if (Timer == 361)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 3f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 6f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 40f, ModContent.ProjectileType<EckasectLiberatorBolt1>(), Projectile.damage * 7, Projectile.knockBack * 4, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Liberator2"), Projectile.position);
				ShakeModSystem.Shake = 4;
			}

			if (Timer == 421)
			{
				float speedX = Projectile.velocity.X * 10;
				float speedY = Projectile.velocity.Y * 7;
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 3f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 6f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 12f, ModContent.ProjectileType<Alvial>(), Projectile.damage * 0, Projectile.knockBack * 4, player.whoAmI);
				Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 40f, ModContent.ProjectileType<EckasectLiberatorBolt1>(), Projectile.damage * 3, Projectile.knockBack * 4, player.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Liberator2"), Projectile.position);
				ShakeModSystem.Shake = 4;
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
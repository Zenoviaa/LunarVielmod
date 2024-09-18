using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Dusts;
using Stellamod.Trails;
using Stellamod.Utilis;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Stellamod.Items.Accessories.Players;
using Stellamod.Projectiles.Visual;

namespace Stellamod.Projectiles.Paint
{
	public class InkingSProj : ModProjectile
	{
		public static bool swung = false;
		public int SwingTime = 60;
		public float holdOffset = 25f;
		public bool bounced = false;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Slasher");
			Main.projFrames[Projectile.type] = 1;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
		}
		public override void SetDefaults()
		{
			Projectile.damage = 10;
			Projectile.timeLeft = SwingTime;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.height = 20;
			Projectile.width = 20;
			Projectile.friendly = true;
			Projectile.scale = 1f;
		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public virtual float Lerp(float val)
		{
			return val == 1f ? 1f : (val == 1f ? 1f : (float)Math.Pow(2, val * 6.5f - 5f) / 2f);
		}
		public override void AI()
		{
			Vector3 RGB = new Vector3(1.45f, 2.55f, 0.94f);
			float multiplier = 1;
			float max = 2.25f;
			float min = 1.0f;
			RGB *= multiplier;
			if (RGB.X > max)
			{
				multiplier = 0.5f;
			}
			if (RGB.X < min)
			{
				multiplier = 1.5f;
			}
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10000;
			AttachToPlayer();
			Player player = Main.player[Projectile.owner];
			Timer++;
			if (Timer < 60)
			{
				int minImmuneTime = 5;
				player.immune = true;

				//Using Math.Max returns the highest value, so you won't override any existing immune times.
				//Just prevent it from going low
				player.immuneTime = Math.Max(player.immuneTime, minImmuneTime);

				//No clue why you need to set this, but if you don't you'll still sometimes take damage
				player.hurtCooldowns[0] = Math.Max(player.hurtCooldowns[0], minImmuneTime);
				player.hurtCooldowns[1] = Math.Max(player.hurtCooldowns[1], minImmuneTime);
				player.noKnockback = true;
			}

			if (Timer > 60)
			{
				player.noKnockback = false;
			}
		}

		public override bool ShouldUpdatePosition() => false;
		public void AttachToPlayer()
		{
			Player player = Main.player[Projectile.owner];
			Vector2 oldMouseWorld = Main.MouseWorld;
			Timer++;
			if (Timer < 45)
			{
				if (Main.rand.NextBool(2))
				{
					float speedXa = Main.rand.NextFloat(-60f, 60f);
					float speedYa = Main.rand.Next(-60, 60);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb1>(), (Projectile.damage * 2) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
					Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob3>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
				}

				if (Main.rand.NextBool(1))
				{
					float speedXa = Main.rand.NextFloat(-60f, 60f);
					float speedYa = Main.rand.Next(-60, 60);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb2>(), Projectile.damage + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
				}

				if (Main.rand.NextBool(4))
				{
					float speedXa = Main.rand.NextFloat(-60f, 60f);
					float speedYa = Main.rand.Next(-60, 60);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb3>(), (Projectile.damage * 3) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
					Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
				}


				if (Main.rand.NextBool(4))
				{
					float speedXa = Main.rand.NextFloat(-60f, 60f);
					float speedYa = Main.rand.Next(-60, 60);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb5>(), (Projectile.damage * 3) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
					Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
				}

				if (Main.rand.NextBool(4))
				{
					float speedXa = Main.rand.NextFloat(-60f, 60f);
					float speedYa = Main.rand.Next(-60, 60);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb4>(), (Projectile.damage * 3) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
					Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
				}

				if (Main.rand.NextBool(4))
				{
					float speedXa = Main.rand.NextFloat(-60f, 60f);
					float speedYa = Main.rand.Next(-60, 60);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb6>(), (Projectile.damage * 3) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
					Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
				}

				if (player.GetModPlayer<MyPlayer>().PPPaintI)
				{
					if (Main.rand.NextBool(4))
					{
						float speedXa = Main.rand.NextFloat(-60f, 60f);
						float speedYa = Main.rand.Next(-60, 60);
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb7>(), (Projectile.damage * 4) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
						Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob5>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
					}
				}

				if (player.GetModPlayer<MyPlayer>().PPPaintII)
				{
					if (Main.rand.NextBool(7))
					{
						float speedXa = Main.rand.NextFloat(-35f, 35f);
						float speedYa = Main.rand.Next(-35, 35);
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb8>(), (Projectile.damage * 3) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
						Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob1>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
					}
				}
			}

			if (Timer < 3 && Main.myPlayer == Projectile.owner)
			{
				player.velocity = Projectile.DirectionTo(oldMouseWorld) * 20f;
			}

			int dir = (int)Projectile.ai[1];
			float swingProgress = Lerp(Utils.GetLerpValue(0f, SwingTime, Projectile.timeLeft));
			// the actual rotation it should have
			float defRot = Projectile.velocity.ToRotation();
			// starting rotation
			float endSet = ((MathHelper.PiOver2) / 9f);
			float start = defRot - endSet;

			// ending rotation
			float end = defRot + endSet;
			// current rotation obv
			float rotation = dir == 1 ? start.AngleLerp(end, swingProgress) : start.AngleLerp(end, 0.2f - swingProgress);
			// offsetted cuz sword sprite
			Vector2 position = player.RotatedRelativePoint(player.MountedCenter);
			position += rotation.ToRotationVector2() * holdOffset;
			Projectile.Center = position;
			Projectile.rotation = (position - player.Center).ToRotation() + MathHelper.PiOver4;

			player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
			player.itemRotation = rotation * player.direction;
			player.itemTime = 2;
			player.itemAnimation = 2;
		}

		public override bool PreAI()
		{
			if (Main.rand.NextBool(3))
			{
				Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
				Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob3>(), speed * 2, 0, default(Color), 4f).noGravity = false;
			}

			if (Main.rand.NextBool(3))
			{

				Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
				Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob5>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;

			}

			if (Main.rand.NextBool(3))
			{
				Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
				Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob4>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;

			}
			return true;
		}

		public override void PostDraw(Color lightColor)
		{
			Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.75f * Main.essScale);
			if (Main.rand.NextBool(5))
			{
				int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<PaintBlob3>(), 0f, 0f, 150, Color.White, 1f);
				Main.dust[dustnumber].velocity *= 0.3f;
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Vector2 oldMouseWorld = Main.MouseWorld;
            for (int i = 0; i < Main.rand.Next(1, 3); i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<SplashProj>(), 0, 0, Projectile.owner);
            }

            for (int i = 0; i < 25; i++)
			{
				Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 8)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
				Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
				Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob3>(), speed * 2, 0, default(Color), 4f).noGravity = false;
			}

			for (int i = 0; i < 7; i++)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<PaintBlob1>());
				Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob5>(), (Vector2.One * Main.rand.Next(1, 8)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
			}

			if (target.lifeMax <= 500)
			{
				if (target.life < target.lifeMax / 2)
				{
					target.SimpleStrikeNPC(9999, 1, crit: false, 1);
					SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.Center);
				}
			}
		}
		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			overPlayers.Add(index);
		}

		public PrimDrawer TrailDrawer { get; private set; } = null;
		public float WidthFunction(float completionRatio)
		{
			float baseWidth = Projectile.scale * Projectile.width * 1.3f;
			return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
		}
		public Color ColorFunction(float completionRatio)
		{
			return Color.Lerp(Color.Turquoise, Color.Transparent, completionRatio) * 0.7f;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

			int frameHeight = texture.Height / Main.projFrames[Projectile.type];
			int startY = frameHeight * Projectile.frame;

			Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
			Vector2 origin = sourceRectangle.Size() / 2f;
			Color drawColor = Projectile.GetAlpha(lightColor);

			Main.EntitySpriteDraw(texture,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

			if (Main.rand.NextBool(5))
			{
				int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<PaintBlob2>(), 0f, 0f, 150, Color.White, 1f);
				Main.dust[dustnumber].velocity *= 0.3f;
				Main.dust[dustnumber].noGravity = true;
			}


			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
			TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
			GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
			TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
			return false;

		}



	}
}
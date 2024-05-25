using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.UI.Systems;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Nails
{
    public class BoneNailProj : ModProjectile
	{
		public static bool swung = false;
		public int SwingTime = 60;
		public float holdOffset = 15f;
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
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10000;
			AttachToPlayer();
		}
		public override bool ShouldUpdatePosition() => false;
		public void AttachToPlayer()
		{
			Player player = Main.player[Projectile.owner];
			if (!player.active || player.dead || player.CCed || player.noItems)
				return;

			Vector2 oldMouseWorld = Main.MouseWorld;
			Timer++;
			if (Timer < 5 && Main.myPlayer == Projectile.owner)
				player.velocity = Projectile.DirectionTo(oldMouseWorld) * 10f;


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
			//Projectile.netUpdate = true;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[Projectile.owner];
			Vector2 oldMouseWorld = Main.MouseWorld;
			if (!bounced)
			{
				
				player.velocity = Projectile.DirectionTo(oldMouseWorld) * -10f;
				bounced = true;
			}
			
			float speedX = Projectile.velocity.X * Main.rand.NextFloat(.2f, .3f) + Main.rand.NextFloat(-4f, 4f);
			float speedY = Projectile.velocity.Y * Main.rand.Next(20, 35) * 0.01f + Main.rand.Next(-10, 11) * 0.2f;
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 3, speedY * 3, ProjectileID.BoneGloveProj, Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 1, speedY * 1, ProjectileID.BoneGloveProj, Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 2, speedY * 2, ProjectileID.BoneGloveProj, Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);


			if (target.lifeMax <= 100)
			{
				if (target.life < target.lifeMax / 2)
				{
					target.SimpleStrikeNPC(9999, 1, crit: false, 1);

				}
			}
			
			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>(), 0f, 0f);
			Main.dust[dust].scale = 1.2f;
			ShakeModSystem.Shake = 5;

			SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Veriappear"));
			
		}
		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			overPlayers.Add(index);
		}
		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

			int frameHeight = texture.Height / Main.projFrames[Projectile.type];
			int startY = frameHeight * Projectile.frame;

			Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
			Vector2 origin = sourceRectangle.Size() / 2f;
			Color drawColor = Projectile.GetAlpha(lightColor);

			Main.EntitySpriteDraw(texture,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}
	}
}
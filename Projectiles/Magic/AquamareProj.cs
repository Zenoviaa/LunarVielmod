using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.NPCs.Bosses.DaedusRework;
using Stellamod.UI.Systems;
using static Terraria.ModLoader.ModContent;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trails;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Stellamod.NPCs.Bosses.singularityFragment;
using Stellamod.NPCs.Bosses.STARBOMBER.Projectiles;

namespace Stellamod.Projectiles.Magic
{
	public class AquamareProj : ModProjectile
	{
		float distance = 8;
		int rotationalSpeed = 4;
		int afterImgCancelDrawCount = 0;
		float t = 0;
		public override void SetStaticDefaults()
		{

			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 300;
			//projectile.extraUpdates = 1;
			Projectile.width = Projectile.height = 50;
			Projectile.hostile = false;
			Projectile.friendly = true;



		}
		bool initialized = false;
		float alphaCounter;
		Vector2 initialSpeed = Vector2.Zero;


		public override void AI()
		{
			Projectile.velocity *= 0.991f;
			alphaCounter += 0.04f;
			int rightValue = (int)Projectile.ai[1] - 1;
			if (rightValue < (double)Main.projectile.Length && rightValue != -1)
			{
				Projectile other = Main.projectile[rightValue];
				Vector2 direction9 = other.Center - Projectile.Center;
				int distance = (int)Math.Sqrt((direction9.X * direction9.X) + (direction9.Y * direction9.Y));
				direction9.Normalize();
			}
			if (!initialized)
			{
				initialSpeed = Projectile.velocity;
				initialized = true;
			}
			if (initialSpeed.Length() < 15)
				initialSpeed *= 1.01f;
			Projectile.spriteDirection = 1;
			if (Projectile.ai[0] > 0)
			{
				Projectile.spriteDirection = 0;
			}

			distance += 0.4f;
			Projectile.ai[0] += rotationalSpeed;

			Vector2 offset = initialSpeed.RotatedBy(Math.PI / 2);
			offset.Normalize();
			offset *= (float)(Math.Cos(Projectile.ai[0] * (Math.PI / 180)) * (distance / 3));
			Projectile.velocity = initialSpeed + offset;
			Projectile.rotation -= 0.5f;

			if (t > 2)
			{

				afterImgCancelDrawCount++;
			}

			t += 0.01f;

			Projectile.ai[0]++;
		}

		public PrimDrawer TrailDrawer { get; private set; } = null;
		public float WidthFunction(float completionRatio)
		{
			float baseWidth = 1 * (Projectile.width / 4) * 1.3f;
			return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
		}
		public Color ColorFunction(float completionRatio)
		{
			return Color.Lerp(Color.Aquamarine, Color.Transparent, completionRatio) * 0.7f;
		}


		Vector2 DrawOffset;
		public override bool PreDraw(ref Color lightColor)
		{
			
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
			GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
			TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);


			int frameHeight = texture.Height / Main.projFrames[Projectile.type];
			int startY = frameHeight * Projectile.frame;
			Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
			Vector2 origin = sourceRectangle.Size() / 2f;
			Color drawColor = new Color(255, 255, 255, 255);

			Main.EntitySpriteDraw(texture,
				Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
				sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale / 2, SpriteEffects.None, 0);


			

			return false;
		}
	}
}
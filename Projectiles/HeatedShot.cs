using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Particles;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Trails;
using Stellamod.Utilis;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Stellamod.UI.Systems;
using Stellamod.Buffs;
using Stellamod.Projectiles.Magic;

namespace Stellamod.Projectiles
{
	public class HeatedShot : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("SalfaCirle");
			Main.projFrames[Projectile.type] = 21;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 216;
			Projectile.height = 120;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.knockBack = 12.9f;
			Projectile.aiStyle = 1;
			AIType = ProjectileID.Bullet;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.tileCollide = false;

		}
		public override bool PreAI()
		{

			Projectile.tileCollide = false;

			return true;
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
		}

		public float nigga = 0;
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
			Vector3 RGB = new(1.00f, 0.37f, 0.30f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f);
			Main.dust[dust].scale = 0.6f;


			nigga++;

			Projectile.velocity *= 1.04f;
			if (nigga < 2)
			{
				ShakeModSystem.Shake = 5;
			}


			if (++Projectile.frameCounter >= 2)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 21)
				{
					Projectile.frame = 0;
				}
			}


		}

		public PrimDrawer TrailDrawer { get; private set; } = null;
		public float WidthFunction(float completionRatio)
		{
			float baseWidth = Projectile.scale * (Projectile.width / 4) * 1.3f;
			return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
		}
		public Color ColorFunction(float completionRatio)
		{
			return Color.Lerp(Color.OrangeRed, Color.Transparent, completionRatio) * 0.7f;
		}
		public override bool PreDraw(ref Color lightColor)
		{


			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
			GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.BeamTrail2);
			TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
			return true;
		}


		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Main.rand.NextBool(3))
			{
				target.AddBuff(BuffID.OnFire, 180);
			}
			var EntitySource = Projectile.GetSource_Death();

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<CandleShotProj2>(), Projectile.damage, 1, Main.myPlayer, 0, 0);
			}
		}

	}
}




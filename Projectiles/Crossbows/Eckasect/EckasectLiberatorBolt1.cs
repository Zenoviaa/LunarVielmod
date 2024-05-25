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

namespace Stellamod.Projectiles.Crossbows.Eckasect
{
	public class EckasectLiberatorBolt1 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("SalfaCirle");
			Main.projFrames[Projectile.type] = 20;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.width = 373;
			Projectile.height = 130;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.knockBack = 12.9f;
			Projectile.aiStyle = 1;
			AIType = ProjectileID.Bullet;
			Projectile.DamageType = DamageClass.Ranged;
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
				ShakeModSystem.Shake = 13;
			}

			if (++Projectile.frameCounter >= 2)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 20)
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
			return Color.Lerp(Color.Violet, Color.Transparent, completionRatio) * 0.7f;
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
			NPC npc = target;
			if (npc.active && !npc.HasBuff<Sected>())
			{
				target.AddBuff(ModContent.BuffType<Sected>(), 700);
				float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
				float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
				switch (Main.rand.Next(3))
				{
					case 0:
						target.AddBuff(ModContent.BuffType<Genesis>(), 640);
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<GenesisDebuff>(), (int)(Projectile.damage * 0), 0f, Projectile.owner, 0f, 0f);
						break;


					case 1:				
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<ExecutorDebuff>(), (int)(Projectile.damage * 0), 0f, Projectile.owner, 0f, 0f);
						target.AddBuff(ModContent.BuffType<Executor>(), 640);
						break;

					case 2:
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<LiberatorDebuff>(), (int)(Projectile.damage * 0), 0f, Projectile.owner, 0f, 0f);
						target.AddBuff(ModContent.BuffType<Liberator>(), 640);
						break;
				}
			}


			if (npc.active && npc.HasBuff<Liberator>())
            {
				npc.SimpleStrikeNPC(Projectile.damage * 12, 1, crit: false, Projectile.knockBack);

			}
		}

	}
}




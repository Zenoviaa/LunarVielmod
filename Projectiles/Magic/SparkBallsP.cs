using Microsoft.Xna.Framework;
using Stellamod.Projectiles.IgniterExplosions;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework.Graphics;
using Stellamod.Trails;

using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

using static Terraria.ModLoader.ModContent;
using Stellamod.Helpers;
using Stellamod.Dusts;


namespace Stellamod.Projectiles.Magic
{
	public class SparkBallsP : ModProjectile
	{
		private ref float Timer => ref Projectile.ai[0];
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("MeatBall");
			Main.projFrames[Projectile.type] = 1;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			Projectile.damage = 12;
			Projectile.width = 12;
			Projectile.height = 12;
			Projectile.light = 1.5f;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.ownerHitCheck = true;
			Projectile.timeLeft = 360;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
		}

		public PrimDrawer TrailDrawer { get; private set; } = null;
		public float WidthFunction(float completionRatio)
		{
			float baseWidth = Projectile.scale * Projectile.width * 1.3f;
			return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
		}
		public Color ColorFunction(float completionRatio)
		{
			return Color.Lerp(Color.LightPink, Color.Transparent, completionRatio) * 0.7f;
		}
		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
			TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
			GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.LoveTrail);
			TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
			return false;
		}

		public override void PostDraw(Color lightColor)
		{
			Texture2D texture2D4 = Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
			Color glowColor = Color.LightPink;
			glowColor.A = 0;
			glowColor *= Timer / 30f;
			for (int i = 0; i < 3; i++)
			{
				Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, glowColor, Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
			}
		}

        public override void AI()
        {
            base.AI();
			Timer++;
			if(Timer % 6 == 0)
			{
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.Pink, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }
            if (Main.rand.NextBool(10))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Pink, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }

			Projectile.velocity *= 1.02f;
            NPC nearest = ProjectileHelper.FindNearestEnemy(Projectile.position, 1024);
            if(nearest != null)
			{
				Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, 6);
			}

			Lighting.AddLight(Projectile.Center, Color.LightPink.ToVector3() * 1.0f * Main.essScale);
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<StarringBoom>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, 0f, 0f);
        }
    }

	public class StarringBoom : ModProjectile
	{
		private ref float Timer => ref Projectile.ai[0];
		public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
			Projectile.width = 64;
			Projectile.height = 64;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 15;
			Projectile.friendly = true;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            base.AI();
			Timer++;
			if(Timer == 1)
            {
                for (float f = 0; f < 6; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Pink, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.Pink,
                        outerGlowColor: Color.Black,
                        duration: Main.rand.NextFloat(12, 25),
                        baseSize: Main.rand.NextFloat(0.01f, 0.15f));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Starexplosion"), Projectile.position);
            }

        }
    }
}

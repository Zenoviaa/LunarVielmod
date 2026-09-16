using Stellamod.Assets;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.Dusts;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Effects.RekFlames;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;

public class STARPUNCH : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 8;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 80;
        Projectile.height = 80;
        Projectile.tileCollide = false;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 80;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            var sound = AssetReferences.Assets.Sounds.STARR.STARRPUNCH.Asset with { PitchVariance = 0.9f };
            SoundEngine.PlaySound(sound, Projectile.position);
            int dustType = ModContent.DustType<StarBitDust>();
            for(float f =0; f < 10; f++)
            {
                Vector2 spawnPosition = Projectile.Center;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);
                spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                Vector2 spawnVelocity = Main.rand.NextVector2Circular(16, 16);
                Dust.NewDustPerfect(spawnPosition, dustType, spawnVelocity);
            }
            for(float f = 0; f < 3; f++)
            {
                Vector2 spawnPosition = Projectile.Center;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);
                spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                Vector2 spawnVelocity = Main.rand.NextVector2Circular(2, 2);
                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                spawnScale *= 3;
                var fx = Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
                fx.Scale *= 2;
            }

            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            ShakeScreenPosition.Shake = 12;
            for(float f = 0; f < 32; f++)
            {
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = Projectile.Center,
                    velocity = Main.rand.NextVector2Circular(32, 32),
                    innerColor = Color.LightGoldenrodYellow.ToVector4(),
                    outerColor = Color.DarkGoldenrod.ToVector4(),
                });
            }

            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Gold,
                    outerGlowColor: Color.Black,
                    baseSize: Main.rand.NextFloat(0.02f, 0.16f),
                    duration: Main.rand.NextFloat(12, 24));
                particle.Scale *= 3;
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }
        Projectile.frame = (int)(Timer / 3f);
        if (Projectile.frame >= Main.projFrames[Type])
            Projectile.Kill();
    }
    private void DrawTorch()
    {
        {
            float attackProgress = Timer / 36f;
            RekTorchShader torchShader = ShaderContent.GetInstance<RekTorchShader>();
            torchShader.Time = EasingFunction.OutExpo(attackProgress);
            torchShader.Strength = MathHelper.Lerp(-0.5f, 0.5f, EasingFunction.OutSine(attackProgress));
            torchShader.NoiseTexture = AssetManager.Noise.PerlinBlurred.Value;
            torchShader.InnerColor = Color.Yellow;
            torchShader.BloomColor = Color.Gold;
            SpriteBatch spriteBatch = Main.spriteBatch;
            SpritebatchParams @params = SpritebatchParams.InWorldAndZoomed() with { effect = torchShader.Effect };
            using (new SpritebatchContext(Main.spriteBatch, Main.spriteBatch.Parameters with { effect = torchShader }))
            {
                SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(
                    AssetReferences.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles.VulcanEruption.Asset, Projectile.Center);
                drawer.rotation = Projectile.velocity.ToRotation();
                drawer.color = Color.Lerp(Color.White, Color.Gold, attackProgress);
                drawer.color.A = 0;
                drawer.LeftCenterOrigin();
                drawer.scale *= 2f;
                drawer.scale.Y *= MathHelper.SmoothStep(1.5f, 0f, EasingFunction.OutExpo(attackProgress));
                drawer.scale.X *= 2.8f;
                spriteBatch.Draw(drawer);

                drawer.color = Color.DarkGoldenrod;
                drawer.color.A = 0;
                drawer.scale *= 1.12f;
                spriteBatch.Draw(drawer);

                drawer.color = Color.DarkViolet;
                drawer.color.A = 0;
                drawer.scale *= 1.12f;
                drawer.scale.Y *= 0.8f;
                spriteBatch.Draw(drawer);
            }

        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
     //   DrawTorch();

        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.color = Color.Gold;
        drawer.scale *= 1.1f;
      //  Main.spriteBatch.Draw(drawer);

        drawer.scale = Vector2.One;
        drawer.color = Color.White;
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

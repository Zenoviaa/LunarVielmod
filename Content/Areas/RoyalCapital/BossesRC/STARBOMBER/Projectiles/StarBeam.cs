using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER.Projectiles;

public class StarBeam : ScarletProjectile
{
    private Vector2 _startPoint;
    private Vector2 _impactPoint;
    private bool _impactGround;
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        TrailCacheLength = 384;
        Projectile.width = 1;
        Projectile.height = 1;
        Projectile.hostile = true;
        Projectile.tileCollide = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 180 * 5;
        Projectile.extraUpdates = 4;
    }

    public override void AI()
    {
        Timer++;

        if (Timer < TrailCacheLength)
        {
            base.AI();
        }
        else
        {
            Projectile.velocity *= 0f;
        }


        if (_impactPoint != Vector2.Zero)
        {
            if (Timer % 2 == 0)
            {
                var part = FXUtil.GlowCircleDetailedBoom1(_impactPoint, Color.Yellow, Color.Orange, Color.DarkRed);
                part.Scale *= 0.5f;
                part.Rotation = Main.rand.NextFloat(-1f, 1f);
            }
        }

        if(Timer % 10 == 0)
        {
            Vector2 pos = _startPoint;
            DustParticleSpawnParams spawnparams = DustParticleSpawnParams.Default;
            DustParticle.Spawn(pos, Main.rand.NextVector2Circular(8, 8) * 2, spawnparams);

            spawnparams.scaleRange *= 0.5f;
            for (int i = 0; i < OldCenterPos.Length; i+=2)
            {
                Vector2 pos2 = OldCenterPos[i];
                if (Main.rand.NextBool(100))
                {
                    DustParticle.Spawn(pos2, Main.rand.NextVector2Circular(8, 8) * 2, spawnparams);
                }
                if (Main.rand.NextBool(50))
                {
                    pos2 += Main.rand.NextVector2Circular(64, 64);
                    SparkleParticle sp = SparkleParticle.Spawn(pos2, Vector2.Zero, Color.White, 0.3f);
                    sp.gravity = 0;
                }
            }
        }

        if (Timer == 1)
        {
            _startPoint = Projectile.Center;
            SoundStyle railgun = AssetRegistry.Sounds.STARBOMBER.STARRAILGUN;
            railgun.PitchVariance = 0.3f;
            SoundEngine.PlaySound(railgun, Projectile.position);

            SoundStyle chargeSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_LAZER");
            SoundEngine.PlaySound(chargeSound, Projectile.position);

            FXUtil.ShakeCamera(Projectile.position, 1024, 18);
            FXUtil.GlowCircleBoom(Projectile.Center, Color.Pink, Color.Purple, Color.Black);
            for (float f = 0; f < 8; f++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), velocity, newColor: Color.Pink);
            }
            for (float f = 0; f < 8; f++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(), velocity, newColor: Color.Pink);
            }
        }
    }

    private void ImpactEffect()
    {
        ShakeScreenPosition.Shake = 9;
        FXUtil.ShakeCamera(Projectile.position, 1024, 32);
        FXUtil.GlowCircleBoom(Projectile.Center, Color.Pink, Color.Purple, Color.Black);
        FXUtil.PunchCamera(Projectile.Center, Vector2.UnitY, 8, 8, 8);
        for (float f = 0; f < 8; f++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), velocity, newColor: Color.Pink);
        }
        for (float f = 0; f < 8; f++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(), velocity, newColor: Color.Pink);
        }
        for (float f = 0; f < 6; f++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
            LegacyParticle.NewParticle<ZapParticle>(Projectile.Center, velocity, Color.Pink);
        }
        for (float f = 0; f < 8; f++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
        }

        for (int i = 0; i < 1; i++)
        {
            var source = Projectile.GetSource_FromThis();
            Vector2 rvelocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
            rvelocity = rvelocity.RotatedByRandom(MathHelper.ToRadians(24));
            rvelocity *= 2;

            Gore.NewGore(source, Projectile.Center, rvelocity,
                ModContent.GoreType<FableRock1>());

            rvelocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
            rvelocity = rvelocity.RotatedByRandom(MathHelper.ToRadians(24));

            Gore.NewGore(source, Projectile.Center, rvelocity,
                ModContent.GoreType<FableRock2>());

            rvelocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
            rvelocity = rvelocity.RotatedByRandom(MathHelper.ToRadians(24));

            Gore.NewGore(source, Projectile.Center, rvelocity,
                ModContent.GoreType<FableRock3>());

            rvelocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
            rvelocity = rvelocity.RotatedByRandom(MathHelper.ToRadians(24));

            Gore.NewGore(source, Projectile.Center, rvelocity,
                ModContent.GoreType<FableRock4>());
        }
        var sear = LegacyParticle.NewParticle<SearParticle>(Projectile.Center, Vector2.Zero);


        SoundStyle crush = AssetRegistry.Sounds.STARBOMBER.HeavyCrush;
        crush.PitchVariance = 0.3f;
        SoundEngine.PlaySound(crush, Projectile.position);


        var p = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.UnitY);
        p.Scale *= 5;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (!_impactGround)
        {
            _impactPoint = Projectile.Center;
            ImpactEffect();
            _impactGround = true;
        }
        Projectile.velocity.X = oldVelocity.X;
        Projectile.velocity.Y = 0;
        return false;
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return ProjectileHelper.OldPosColliding(OldCenterPos, projHitbox, targetHitbox);
    }

    private Color ColorFunction(float completionRatio)
    {
        float osc = MathF.Sin(completionRatio * 16) * 0.5f + 0.5f;
        Color flickerColor = Color.Lerp(Color.Red, Color.Blue, ExtraMath.Osc(0f, 1f, speed: 32) + osc);
        return Color.Lerp(flickerColor, Color.White, completionRatio);
        //return Color.Lerp(Color.White, Color.Black, 0.5f);
    }

    private float WidthFunction(float completionRatio)
    {
        float inEasing = EasingFunction.OutExpo(Timer / 60f);
        float outEasing = (float)Projectile.timeLeft / 60f;
        outEasing = EasingFunction.InOutSine(outEasing);
        float w = 48 * inEasing * outEasing;
        float w2 = MathHelper.Lerp(0, 24, outEasing);
        return MathHelper.SmoothStep(w, w2, completionRatio);
    }

    private void DrawPixelatedBeam(GraphicsDevice graphicsDevice)
    {
        var shader = RichLaserShader.Instance;
        shader.LaserColor = Color.Pink;
        shader.InnerColor = Color.Lerp(Color.LightPink, Color.Blue, 0.75f);
        shader.OuterColor = Color.Violet;
        shader.LaserTexture = TrailRegistry.BeamTrail;
        shader.BloomTexture = TrailRegistry.CrystalTrail;

        TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, ColorFunction, WidthFunction, shader);
    }
    public void DrawPixelatedMuzzleFlash(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Asset<Texture2D> muzzleFlashTexture = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/MuzzleFlash");
        Vector2 drawOrigin = muzzleFlashTexture.Size() / 2f;
        Vector2 drawCenter = Projectile.Center - screenPos;
        Color drawColor = Color.Pink;
        drawColor.A = 0;

        float width = (float)Projectile.timeLeft / 30f;
        float outWidth = EasingFunction.InOutSine(width);
        float scale = outWidth;
        Vector2 flashScale = Vector2.One;
        flashScale.X *= 1.5f;
        flashScale.Y *= 1.2f;
        flashScale *= scale;
        spriteBatch.Draw(muzzleFlashTexture.Value, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, flashScale, SpriteEffects.None, 0);

        drawColor = Color.White;
        drawColor.A = 0;
        spriteBatch.Draw(muzzleFlashTexture.Value, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, flashScale * 0.6f, SpriteEffects.None, 0);

        Asset<Texture2D> impactTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/ZuiEffect");
        drawOrigin = impactTexture.Size() / 2f;

        Vector2 impactPoint = _startPoint;
        scale *= ExtraMath.Osc(0.66f, 1f, speed: 32);

        drawCenter = impactPoint - screenPos;
        drawColor = Color.Pink;
        drawColor.A = 0;

        float rot = Main.GlobalTimeWrappedHourly;
        spriteBatch.Draw(impactTexture.Value, drawCenter, null, drawColor, rot, drawOrigin, scale * 1.2f, SpriteEffects.None, 0);

        drawColor = Color.White;
        drawColor.A = 0;
        spriteBatch.Draw(impactTexture.Value, drawCenter, null, drawColor, rot, drawOrigin, scale * 0.8f, SpriteEffects.None, 0);

        impactTexture = AssetManager.GlowMask.SpiralVortex;
        scale = 0.4f;
        drawOrigin = impactTexture.Size() * 0.5f;
        rot += Main.GlobalTimeWrappedHourly * 4;

        float outEasing = (float)Projectile.timeLeft / 60f;
        outEasing = EasingFunction.InOutSine(outEasing);
        scale *= outEasing;
        for (int i = 0; i < OldCenterPos.Length; i+= 2)
        {
            Vector2 pos = OldCenterPos[i];
            pos -= screenPos;


            drawColor = Color.Lerp(Color.Pink, Color.Blue, ExtraMath.Osc(0f, 1f, speed: 8, offset: i));
            drawColor *= 0.08f;
            drawColor.A = 0;
            spriteBatch.Draw(impactTexture.Value, pos, null, drawColor, rot + i * 0.2f, drawOrigin, scale * 2f, SpriteEffects.None, 0);
        }
    }

    private void DrawEndPoint()
    {
        Texture2D endPoint = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
        Vector2 drawOrigin = endPoint.Size() / 2f;
        Vector2 drawPosition = Projectile.Center - Main.screenPosition;
        SpriteBatch spriteBatch = Main.spriteBatch;
        Color glowColor = Color.Pink;
        glowColor.A = 0;
        float outEasing = (float)Projectile.timeLeft / 60f;
        outEasing = EasingFunction.InOutSine(outEasing);
        for (float f = 0; f < 4; f++)
        {
            spriteBatch.Draw(endPoint, drawPosition, null, glowColor, f / 4f * MathHelper.TwoPi, drawOrigin, ExtraMath.Osc(0.5f, 1f, speed: 32, offset: f) * outEasing, SpriteEffects.None, 0);
            spriteBatch.Draw(endPoint, _startPoint - Main.screenPosition, null, glowColor, f / 4f * MathHelper.TwoPi, drawOrigin, ExtraMath.Osc(0.5f, 1f, speed: 32, offset: f) * 0.5f * outEasing, SpriteEffects.None, 0);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedMuzzleFlash);
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedBeam);
        DrawEndPoint();
        return false;
    }
}

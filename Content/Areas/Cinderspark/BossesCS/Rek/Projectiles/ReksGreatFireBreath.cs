using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.DrawEffects;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Core.Palettes;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.ProjectileHelpers;
using Stellamod.Core.Rendering;
using Stellamod.Effects.Generic;
using Stellamod.Effects.RekFlames;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;


public class ReksGreatFireBreathRenderer : ModSystem
{
    private RenderTargetProvider _greatFireRT = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
    private RenderTargetProvider _greatFireRT2 = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
    public static readonly List<Action<SpriteBatch>> FirebreathDrawActions = new List<Action<SpriteBatch>>();

    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += RenderFireBreath;
    }

    private void RenderFireBreath(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (FirebreathDrawActions.Count <= 0)
            return;
        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
        graphicsDevice.SetRenderTarget(_greatFireRT2);
        graphicsDevice.Clear(Color.Transparent);

        SpritebatchParams startParams = SpritebatchParams.InWorldAndZoomed();
        spriteBatch.Begin(startParams);
        foreach (var drawAction in FirebreathDrawActions)
        {
            drawAction(spriteBatch);
        }
        spriteBatch.End();
        FirebreathDrawActions.Clear();
        graphicsDevice.SetRenderTarget(_greatFireRT);
        graphicsDevice.Clear(Color.Transparent);

        var palette = DitheredColorPaletteShader.PrepareForDrawing(PaletteAssets.FromPaletteFile(PaletteAssets.FIREBREATH).Value.ColorAtlas, _greatFireRT.Size);
        startParams.effect = palette.Effect;
        spriteBatch.Begin(startParams);
        spriteBatch.Draw(_greatFireRT2, Vector2.Zero, Color.White);
        spriteBatch.End();
        graphicsDevice.SetRenderTarget(null);
        PixelationManager.QueueSpritebatchDrawAction(DrawToScreen);
    }

    private void DrawToScreen(SpriteBatch sb, Vector2 sp)
    {
        sb.Draw(_greatFireRT, Vector2.Zero, Color.White);

    }

    public override void PreUpdateProjectiles()
    {
        base.PreUpdateProjectiles();

    }

}
public class ReksGreatFireBreath : ModProjectile,
    IDrawToRenderTarget
{
    private struct FireBreathParticleData : IParticleData
    {
        public Vector2 localPosition;
        public Vector2 localVelocity;
        public float timeLeft;
        public bool IsActive => timeLeft > 0;
    }

    private Vector2[] _beamPoints = new Vector2[64];
    private Asset<Texture2D> _ringsTextureAsset;
    private ParticleBuffer<FireBreathParticleData> _backFlameParticles = new ParticleBuffer<FireBreathParticleData>(100);
    private NPC Parent => Main.npc[(int)Projectile.ai[0]];
    private ref float Timer => ref Projectile.ai[1];
    private ref float ShouldDie => ref Projectile.ai[2];

    private Vector2 ImpactPoint => Projectile.Center + Projectile.velocity;
    private Asset<Texture2D> BeamTextureAsset => TextureAssets.Projectile[ModContent.ProjectileType<BigVulcanFireball>()];
    private Asset<Texture2D> BeamMaskTextureAsset => ModContent.Request<Texture2D>(ModContent.GetInstance<BigVulcanFireball>().Texture + "_Mask");

    private float EasingInOut
    {
        get
        {
            float inEasing = EasingFunction.InOutSine(Timer / 60f);
            float outEasing = EasingFunction.InOutSine(Projectile.timeLeft / 60f);
            return inEasing * outEasing;
        }
    }

    private float EasingOut
    {
        get
        {
            float outEasing = EasingFunction.InOutSine(Projectile.timeLeft / 60f);
            return outEasing;
        }
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float lineWidth = 64;
        float collisionPoint = 0;
        Vector2 position = Projectile.Center;
        Vector2 previousPosition = position + Projectile.velocity;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, lineWidth, ref collisionPoint);
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        this.AddCommonDebuff(DebuffFlags.Burning_Serpent);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.light = 0.78f;
        Projectile.timeLeft = 600;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    private void ShootEffects()
    {
        Color innerColor = Color.Yellow;
        Color outerColor = Color.Red;
        Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
        Vector2 position = Projectile.Center;


        if (Timer % 3 == 0)
        {
            MuzzleFlashParticle flashParticle = MuzzleFlashParticle.Spawn(position + velocity * 64, velocity * 5, innerColor);
            flashParticle.innerColor = innerColor;
            flashParticle.bloomColor = outerColor;
            flashParticle.Scale *= Main.rand.NextFloat(0.2f, 0.4f) * 4;

        }
        Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
        {
            position = ImpactPoint + Main.rand.NextVector2Circular(32, 32),
            velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(5f, 25f),
            timeLeft = 45,
            innerColor = Color.Yellow.ToVector4(),
            outerColor = Color.Red.ToVector4()
        });
    }

    public override void AI()
    {
        base.AI();
        if (!Parent.active || Parent.ModNPC is not RekBoss)
        {
            Projectile.active = false;
            return;
        }
        Timer++;

        if (Timer == 1)
        {
            var sound = AssetRegistry.Sounds.Rek.BigLaserRek;
            SoundEngine.PlaySound(sound, Main.LocalPlayer.position);
            if (this.OwnedByLocalClient())
            {
                ProjFirer firer = ProjFirer.From<MeteorBoom>(Projectile);
                firer.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 1024;
                firer.New();
            }
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            for (int i = 0; i < 32; i++)
            {
                _backFlameParticles.Spawn(new FireBreathParticleData
                {
                    localPosition = Main.rand.NextVector2Circular(32, 32) + Vector2.Lerp(Vector2.Zero, Vector2.UnitX * 512, Main.rand.NextFloat(0f, 1f)),
                    localVelocity = Vector2.UnitX * Main.rand.NextFloat(25, 45),
                    timeLeft = Main.rand.NextFloat(30, 90)
                });
            }

            for (int i = 0; i < 64; i++)
            {
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
                {
                    position = Projectile.Center + Main.rand.NextVector2Circular(64, 64),
                    velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(5f, 45),
                    timeLeft = 45,
                    innerColor = Color.White.ToVector4(),
                    outerColor = Color.Red.ToVector4()
                });
            }
            for (int i = 0; i < 32; i++)
            {
                Particles.BitDust.Spawn(BitDustFactory.Default with
                {
                    position = Projectile.Center + Main.rand.NextVector2Circular(64, 64),
                    velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(5f, 45),
                    timeLeft = 45,
                    innerColor = Color.White.ToVector4(),
                    outerColor = Color.Red.ToVector4(),
                    scale = new Vector2(2)
                });
            }
        }

        if (Timer == 2)
        {
            ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            screenShaderSystem.TintScreen(Color.Red, 0.18f, 120);
        }
        ShakeScreenPosition.Shake = 4;
        if (Timer % 6 == 0)
        {
            ShootEffects();
        }
        if (ModContent.GetInstance<LunarVeilClientConfig>().DramaticEffects)
        {
            SpecialEffectsPlayer effectsPlayer = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
            effectsPlayer.darknessCurve = MathHelper.Lerp(0.75f, 0f, EasingFunction.InOutExpo(Timer / (60f)));
        }
        for (int i = 0; i < _backFlameParticles.length; i++)
        {
            ref var particle = ref _backFlameParticles._particles[i];
            particle.localPosition += particle.localVelocity;
            float ySign = MathF.Sign(particle.localPosition.Y);
            float lerpValue = Utils.GetLerpValue(0, 120, particle.timeLeft);
            float rotationDirection = ySign * 0.0125f * (1f - lerpValue);
            particle.localVelocity = particle.localVelocity.RotatedBy(rotationDirection);
            particle.localVelocity *= MathHelper.Lerp(1f, 0.96f, lerpValue);
            particle.timeLeft--;
        }

        for (int i = 0; i < _backFlameParticles.length; i++)
        {
            ref var particle = ref _backFlameParticles._particles[i];
            if (Main.rand.NextBool(4))
            {
                Vector2 pos = particle.localPosition.RotatedBy(Projectile.rotation) + Projectile.Center;
                pos += Main.rand.NextVector2Circular(32, 32);
                Color color = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0f, 1f));
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
                {
                    position = pos,
                    velocity = -particle.localVelocity * 0.47f,
                    timeLeft = 45,
                    innerColor = color.ToVector4(),
                    outerColor = Color.Red.ToVector4()
                });
            }
        }

        if (ShouldDie > 0)
        {
            if (Projectile.timeLeft > 60)
                Projectile.timeLeft = 60;
        }

        if (Timer % 2 == 0 && Timer < 180)
        {
            _backFlameParticles.Spawn(new FireBreathParticleData
            {
                localPosition = Main.rand.NextVector2Circular(32, 32),
                localVelocity = Vector2.UnitX * Main.rand.NextFloat(25, 80),
                timeLeft = Main.rand.NextFloat(30, 90)
            });
        }

        if (Main.rand.NextBool(4))
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
            {
                position = ImpactPoint + Main.rand.NextVector2Circular(32, 32),
                velocity = -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(5f, 25f),
                timeLeft = 45,
                innerColor = Color.Yellow.ToVector4(),
                outerColor = Color.Red.ToVector4()
            });
        }
        if (Timer > 45)
        {
            ShouldDie = 1;
        }

        float length = ProjectileHelper.PerformBeamHitscan(Projectile, 2400);
        Projectile.Center = Parent.Center;
        Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * length;
        Projectile.velocity = Parent.rotation.ToRotationVector2() * Projectile.velocity.Length();
        Projectile.rotation = Parent.rotation;
        if (Timer % 2 == 0)
        {
            float numSteam = 2;
            for (float n = 0; n < numSteam; n++)
            {
                Vector2 spawnPosition = ImpactPoint;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);
                spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                Vector2 spawnVelocity = Main.rand.NextVector2Circular(2, 2);

                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                var p = Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
                p.Scale *= 1.5f;
            }
        }

        DrawUtilities.InterpolateBetweenPointsNonAlloc(ref _beamPoints, Projectile.Center, Projectile.Center + Projectile.velocity * 1.05f);
        ParticlesHelper.CheckForAndKillParticles(_backFlameParticles);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawImpactGlow();
        DrawStarBomb(Main.spriteBatch);
        CommonDrawEffects.DrawFenixLaserCircles(Main.spriteBatch, Timer, Projectile.Center, Projectile.velocity, num: 3);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    private void DrawImpactGlow()
    {
        var drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, ImpactPoint);
        drawer.color = Color.OrangeRed * 0.5f;
        drawer.color.A = 0;
        drawer.scale *= 0.9f * EasingInOut;
        drawer.scale.X *= 0.64f;
        drawer.scale.Y *= 1.2f;
        drawer.rotation = Projectile.rotation;
        Main.spriteBatch.Draw(drawer);

        drawer.color = Color.Gold * 0.5f;
        drawer.color.A = 0;
        drawer.scale *= 0.84f;
        drawer.rotation = Projectile.rotation;
        Main.spriteBatch.Draw(drawer);

        drawer.color = Color.White;
        drawer.color.A = 0;
        drawer.scale *= 0.84f;
        drawer.rotation = Projectile.rotation;
        Main.spriteBatch.Draw(drawer);
    }

    public void DrawToRenderTargets()
    {
        ReksGreatFireBreathRenderer.FirebreathDrawActions.Add(RenderFlamethrower);
        PixelationManager.QueuePrimitivesDrawAction(DrawFlamingBeam);
        PixelationManager.QueueSpritebatchDrawAction(DrawFlameImpact, DrawLayer.OverNPCsAdditive);
    }


    private void DrawFlamingBeam(GraphicsDevice graphicsDevice)
    {
        Color GetBeamColor(float ratio)
        {
            return Color.Lerp(Color.Yellow, Color.OrangeRed, ratio) * EasingInOut;
        }
        float GetBeamWidth(float ratio)
        {
            return MathHelper.SmoothStep(196, 296, ratio) * 0.85f;
        }

        FixedRichLaserShader richShader = ShaderContent.GetInstance<FixedRichLaserShader>();
        richShader.LaserTexture = BeamTextureAsset;
        richShader.OuterColor = Color.Red;
        richShader.LaserColor = Color.White;
        richShader.InnerColor = Color.Orange;
        richShader.Time = Main.GlobalTimeWrappedHourly * 128;
        TrailDrawer.Draw(_beamPoints, GetBeamColor, GetBeamWidth, richShader, Vector2.Zero);
    }

    private void DrawFlameImpact(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        BigRekFireballShader shader = ShaderContent.GetInstance<BigRekFireballShader>();
        shader.Time = Main.GlobalTimeWrappedHourly * -64;
        shader.NoiseTexture = AssetManager.Noise.SharpPerlinNoise;
        shader.InnerColor = Color.Yellow;
        shader.BloomColor = Color.DarkRed;
        shader.Strength = 3f;
        shader.MaskTexture = BeamMaskTextureAsset.Value;
        var sbParams = SpritebatchParams.InWorldAndZoomed();
        sbParams.effect = shader.Effect;
        sbParams.blendState = BlendState.Additive;
        float y = MathHelper.Lerp(1.5f, 0.2f, Projectile.velocity.Length() / 80);
        using (new SpritebatchContext(spriteBatch, sbParams))
        {
            SpritebatchDrawer impactDrawer = SpritebatchDrawer.FromTextureAsset(BeamTextureAsset, ImpactPoint);
            impactDrawer.color = Color.White;
            impactDrawer.worldPosition -= Projectile.velocity.SafeNormalize(Vector2.Zero) * 144;
            impactDrawer.scale *= 1.2f * EasingInOut;
            impactDrawer.scale.X *= 1.5f;
            impactDrawer.rotation = Projectile.rotation;
            spriteBatch.Draw(impactDrawer);

            impactDrawer.color = Color.Orange * 1f;
            spriteBatch.Draw(impactDrawer);
        }
    }

    private void DrawStarBomb(SpriteBatch sb)
    {
        float divisor = 888;
        Vector2 _bounceOffset = Vector2.Lerp(Projectile.velocity.SafeNormalize(Vector2.Zero) * 444, Vector2.Zero, EasingFunction.OutSine(Timer / 60f));
        float _scale = MathHelper.SmoothStep(3f, 1f, Timer / 60f);
        SpritebatchDrawer circleDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.WhiteCircle, Projectile.Center + _bounceOffset);
        Color color = Color.Lerp(Color.OrangeRed, Color.Red, Timer / 30);
        circleDrawer.color = color * 0.75f * MathHelper.Lerp(1f, 0f, EasingFunction.Clamp(Timer / 30)) * EasingOut;
        circleDrawer.color.A = 0;
        circleDrawer.scale = Vector2.Lerp(Vector2.Zero, Vector2.One * 12, Timer / 30f);
        Main.spriteBatch.Draw(circleDrawer);

        SpritebatchDrawer glowBall2 = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare3, Projectile.Center + _bounceOffset);
        glowBall2.color = Color.White * 0.9f * ExtraMath.Osc(0.5f, 1f, speed: 6) * EasingOut;
        glowBall2.color.A = 0;
        glowBall2.scale *= 2 * _scale;
        glowBall2.scale.Y *= 1.3f;
        sb.Draw(glowBall2);

        SpritebatchDrawer glowBall = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center + _bounceOffset);
        glowBall.color = Color.Lerp(Color.Blue, Color.Magenta, ExtraMath.Osc(0f, 1f, speed: 3)) * 0.1f;
        glowBall.color.A = 0;
        glowBall.scale *= 2 * _scale;
        sb.Draw(glowBall);


        glowBall = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center + _bounceOffset);
        glowBall.color = Color.DarkRed * 0.75f * (_bounceOffset.Length() / 115f) * EasingOut;
        glowBall.color.A = 0;
        glowBall.scale *= 2 * _scale;
        sb.Draw(glowBall);



        glowBall = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center + _bounceOffset);
        glowBall.color = Color.White * 0.75f * MathHelper.Lerp(0f, 1f, EasingFunction.InExpo((_bounceOffset.Length() / divisor))) * EasingOut;
        glowBall.color.A = 0;
        glowBall.scale *= 2 * _scale;
        sb.Draw(glowBall);


        glowBall = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SolarRing, Projectile.Center + _bounceOffset);
        glowBall.color = Color.Lerp(Color.Orange, Color.Red, ExtraMath.Osc(0f, 1f, speed: 3)) * 0.08f * EasingOut;
        glowBall.color.A = 0;
        glowBall.scale *= 2 * _scale * MathHelper.Lerp(0f, 2f, EasingFunction.InExpo((_bounceOffset.Length() / divisor)));
        sb.Draw(glowBall);


        glowBall = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare3, Projectile.Center + _bounceOffset);
        glowBall.color = Color.White * 0.92f * EasingOut;
        glowBall.color.A = 0;
        glowBall.scale *= 2 * _scale * MathHelper.Lerp(0, 6.4f, EasingFunction.InExpo((_bounceOffset.Length() / divisor)));
        sb.Draw(glowBall);
    }
    private void RenderFlamethrower(SpriteBatch spriteBatch)
    {
        var shader = ShaderContent.GetInstance<RekFlamethrowerShader>();
        shader.Time = Main.GlobalTimeWrappedHourly * 4;
        shader.Distortion = 0.25f;
        shader.DistortionNoise = AssetManager.Noise.FlamethrowerNoise.Value;
        SpritebatchParams flamethrowerParams = SpritebatchParams.InWorldAndZoomed();
        flamethrowerParams.effect = shader.Effect;
        flamethrowerParams.blendState = BlendState.AlphaBlend;
        using (new SpritebatchContext(spriteBatch, flamethrowerParams))
        {
            for (int i = 0; i < _backFlameParticles.length; i++)
            {
                ref var particle = ref _backFlameParticles._particles[i];
                Vector2 position = particle.localPosition.RotatedBy(Projectile.rotation) + Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 84;

                float lerpValue = Utils.GetLerpValue(0f, 120, particle.timeLeft);
                SpritebatchDrawer flamethrowerDrawer = SpritebatchDrawer.FromProjectile(Projectile);
                flamethrowerDrawer.worldPosition = position;
                flamethrowerDrawer.color = Color.OrangeRed * lerpValue * EasingInOut * 0.85f;
                flamethrowerDrawer.color.A = 0;
                flamethrowerDrawer.scale *= 1.5f + MathHelper.Lerp(1f, 0f, lerpValue);
                flamethrowerDrawer.scale *= EasingFunction.QuadraticBump(lerpValue);

                flamethrowerDrawer.rotation = -particle.timeLeft * 0.05f;
                spriteBatch.Draw(flamethrowerDrawer);
            }
            for (int i = 0; i < _backFlameParticles.length; i++)
            {
                ref var particle = ref _backFlameParticles._particles[i];
                Vector2 position = particle.localPosition.RotatedBy(Projectile.rotation) + Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 84;

                float lerpValue = Utils.GetLerpValue(0f, 120, particle.timeLeft);
                //      lerpValue = 1f - lerpValue;
                SpritebatchDrawer flamethrowerDrawer = SpritebatchDrawer.FromProjectile(Projectile);
                flamethrowerDrawer.worldPosition = position;
                flamethrowerDrawer.color = Color.White * lerpValue * 0.8f * EasingInOut * 0.85f;
                flamethrowerDrawer.color.A = 0;
                flamethrowerDrawer.scale *= EasingFunction.QuadraticBump(lerpValue);

                flamethrowerDrawer.rotation = particle.timeLeft * 0.05f;
                spriteBatch.Draw(flamethrowerDrawer);
            }
        }

        var shader2 = ShaderContent.GetInstance<RekFlamethrowerBeamShader>();
        shader2.Time = Main.GlobalTimeWrappedHourly * 4;
        shader2.Distortion = 0.05f;
        shader2.DistortionNoise = AssetManager.Noise.FlamethrowerNoise.Value;
        SpritebatchParams flamethrowerBeanParams = SpritebatchParams.InWorldAndZoomed();
        flamethrowerBeanParams.effect = shader2.Effect;
        flamethrowerBeanParams.blendState = BlendState.AlphaBlend;

        _ringsTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Rings");
        using (new SpritebatchContext(spriteBatch, flamethrowerBeanParams))
        {

            //Draw detailing and texturing within the beam
            SpritebatchDrawer flamethrowerDrawer = SpritebatchDrawer.FromTextureAsset(TrailRegistry.DirnTrail,
                Projectile.Center);
            flamethrowerDrawer.LeftCenterOrigin();
            flamethrowerDrawer.color = Color.Yellow * EasingInOut * 0.85f;
            flamethrowerDrawer.color.A = 0;
            flamethrowerDrawer.rotation = Projectile.velocity.ToRotation();
            flamethrowerDrawer.scale.X = Projectile.velocity.Length() / TrailRegistry.DirnTrail.Width();
            flamethrowerDrawer.scale.Y *= 1.3f;
            //flamethrowerDrawer.worldPosition += Projectile.rotation.ToRotationVector2() * 512;
            spriteBatch.Draw(flamethrowerDrawer);

            flamethrowerDrawer.color = Color.White * EasingInOut * 0.85f;
            flamethrowerDrawer.color.A = 0;
            flamethrowerDrawer.scale.Y *= 0.72f;
            spriteBatch.Draw(flamethrowerDrawer);
        }

        shader2.Time = Main.GlobalTimeWrappedHourly * 32;
        using (new SpritebatchContext(spriteBatch, flamethrowerBeanParams))
        {

            var flamethrowerDrawer = SpritebatchDrawer.FromTextureAsset(_ringsTextureAsset,
                Projectile.Center);
            flamethrowerDrawer.LeftCenterOrigin();
            flamethrowerDrawer.color = Color.LightGoldenrodYellow * EasingInOut * 0.85f;
            flamethrowerDrawer.color.A = 0;
            flamethrowerDrawer.rotation = Projectile.velocity.ToRotation();
            flamethrowerDrawer.scale.X = Projectile.velocity.Length() / _ringsTextureAsset.Width();
            flamethrowerDrawer.scale.Y *= 1.3f;
            //flamethrowerDrawer.worldPosition += Projectile.rotation.ToRotationVector2() * 512;
            spriteBatch.Draw(flamethrowerDrawer);
        }


        SpritebatchDrawer glowCircle = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowCircle.color = Color.White;
        glowCircle.color.A = 0;
        glowCircle.scale *= 0.24f * EasingInOut;
        spriteBatch.Draw(glowCircle);

    }
}

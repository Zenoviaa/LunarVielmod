using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.MoonspiralTower.VerliaBoss;
using Stellamod.Content.Areas.MoonspiralTower.VerliaBoss.Projectiles;
using Stellamod.Content.Areas.PunkerTown.BossesPT.Steamroller;
using Stellamod.Content.Areas.Snow.WeaponsSN;
using Stellamod.Core.Palettes;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.EveroseVillage.CelestiaBoss.Projectiles;

public class BigCelestialBow : ModProjectile,
    IDrawToRenderTarget
{
    private int _growthIndex;
    private float _growthTimer;
   
    private Vector2 _mirageOffset;
    private Vector2 _pullScale;
    private Vector2 _chargeScale;
    private Vector2 _arrowOffset;
    private Vector2 _bowOffset;
    private float _arrowAlpha;
    private float _alphaTimer;
    private ref float Timer => ref Projectile.ai[0];
    private NPC Parent => Main.npc[(int)Projectile.ai[1]];
    private ref float Attack => ref Projectile.ai[2];

    public bool ready;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 10;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _pullScale = Vector2.One;
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 600;
        Projectile.hostile = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        base.AI(); 
        Player target = Main.player[Parent.target];
        _alphaTimer++;
        if (Main.rand.NextBool(4))
        {
            _mirageOffset = Main.rand.NextVector2Circular(4, 4);
        }

        Vector2 offset = Parent.Center + Vector2.UnitX * MathF.Sign(Parent.velocity.X) * 64; ;
        offset -= Projectile.rotation.ToRotationVector2() * 128 * EasingFunction.OutExpo(Timer / 25f);
        if(Timer >= 25f)
        {
            Projectile.Kill();
        }
        Projectile.Center = offset;
        if (_alphaTimer % 3 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(96, 96);
            var d = Dust.NewDustPerfect(pos, DustID.GemEmerald, Scale: 1f);
            d.noGravity = true;
        }
        if (Attack == 1)
        {
            Timer++;
            if(Timer == 1)
            {
                if (this.OwnedByLocalClient())
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.rotation.ToRotationVector2(),
                        ModContent.ProjectileType<BigCelestialArrow>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 
                        ai1: target.whoAmI);
                }
            }

            if(Projectile.frame < 9)
            {
                Projectile.frameCounter += 1;
                if (Projectile.frameCounter >= 5)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }
            }
        }

        float offsetDistance = 0;
        float targetArrowAlpha=0;
 
        Vector2 targetScale = Vector2.One;
        switch (_growthIndex)
        {
            case 0:
                {
                    Projectile.frame = 0;
                    _growthTimer++;
                    if(_growthTimer >= 60)
                    {
                        GrowEffect();
                        _growthTimer = 0;
                        _growthIndex++;
                    }
                    offsetDistance = 196;
                    targetArrowAlpha = 0.2f;
                    targetScale = Vector2.One * 0.5f;
                }
                break;
            case 1:
                {
                
                    _growthTimer++;
                    if (_growthTimer >= 60)
                    {
                        Projectile.frame = 1;
                        GrowEffect();
                        _growthTimer = 0;
                        _growthIndex++;
                    }
                    offsetDistance = 128;
                    targetArrowAlpha = 0.5f;
                    targetScale = Vector2.One * 0.4f;
                }
                break;
            case 2:
                {
                
                    _growthTimer++;
                    if (_growthTimer >= 60)
                    {
                        Projectile.frame = 2;
                        BigGrowEffect();
                        _growthTimer = 0;
                        _growthIndex++;
                    }
                    offsetDistance = 64;
                    targetArrowAlpha = 0.75f;
                    targetScale = Vector2.One * 0.6f;
                }
                break;
            case 3:
                {
                    if (Projectile.frame < 6)
                    {
                        Projectile.frameCounter += 1;
                        if (Projectile.frameCounter >= 5)
                        {
                            Projectile.frameCounter = 0;
                            Projectile.frame++;
                        }
                    }

                    _growthTimer++;
                    if(_growthTimer >= 60f)
                    {
                        ready = true;
                    }
                    offsetDistance = 0;
                    targetArrowAlpha = 1f;
                    targetScale = Vector2.One * 1.15f;
                }
                break;
        }
        _chargeScale = Vector2.Lerp(_chargeScale, targetScale, 0.1f);

        Vector2 targetArrowOffset = Projectile.rotation.ToRotationVector2() * offsetDistance;
        _arrowOffset = Vector2.Lerp(_arrowOffset, targetArrowOffset, 0.1f);
        _arrowAlpha = MathHelper.Lerp(_arrowAlpha, targetArrowAlpha, 0.1f);
      //  _pullScale = Vector2.Lerp(_pullScale, _targetPullScale, 0.1f);
 
        if(Timer < 10)
        {
            Vector2 aimingDirection = (target.Center - Projectile.Center);
            _bowOffset = Vector2.Lerp(-aimingDirection.SafeNormalize(Vector2.Zero) * 96, Vector2.Zero, 
                EasingFunction.InOutSine(_alphaTimer / 240f));

            Vector2 start = -aimingDirection.SafeNormalize(Vector2.Zero);
            float startArcHoldOffset = start.ToRotation();
            float endArcHoldOffset = aimingDirection.ToRotation();

            float ratio = _alphaTimer / 240f;
            float ease = EasingFunction.InOutSine(ratio);
            float dir = aimingDirection.X > 0 ? 0 : 1;
            float arcHoldOffset = MathHelper.Lerp(startArcHoldOffset, endArcHoldOffset - MathHelper.TwoPi * dir, ease);
            _bowOffset += arcHoldOffset.ToRotationVector2() * 32;


            Vector2 outPosition = Parent.Center + _bowOffset;
            float outRotation = (outPosition - Parent.Center).ToRotation();


            float aimingRotation = aimingDirection.ToRotation();
            float rotOffset = MathHelper.Lerp(-MathHelper.Pi + MathHelper.PiOver4, 0, EasingFunction.OutCirc(_alphaTimer / 60f));
            float targetRot = aimingRotation + rotOffset;
            targetRot = Utils.AngleLerp(outRotation, targetRot, EasingFunction.InOutSine(_alphaTimer / 240f));
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, targetRot, 0.1f);
        }

        if (Timer > 0)
        {
            Projectile.velocity = Projectile.rotation.ToRotationVector2() * 0.2f;
        }
    }

    private void BigGrowEffect()
    {
        for (float f = 0; f < 8; f++)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(384, 384);
            Vector2 vel = (Projectile.Center - pos);
            vel *= 0.1f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Color.Turquoise;
            fx.Scale *= 0.5f;
        }

        if (Main.netMode != NetmodeID.Server)
        {
            var screenShader = ModContent.GetInstance<ScreenShaderSystem>();
            screenShader.TintScreen(Color.Turquoise, 0.1f, 15f);
            PixelPrimitiveCircleFactory.CreateCelestiaInwardBoom(Projectile.Center);
        }

        for (float f = 0; f < 12; f++)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(384, 384);
            Vector2 vel = (Projectile.Center - pos);
            vel *= 0.02f;

            DustParticleSpawnParams spawnparams = DustParticleSpawnParams.Default;
            spawnparams.innerColor = Color.Lerp(Color.White, Color.Turquoise, Main.rand.NextFloat(0f, 1f));
            spawnparams.outerColor = Color.Turquoise;
           
            var dp = DustParticle.Spawn(pos, vel, spawnparams);
            dp.dampening = 0.05f;
            dp.gravity = 0;
            dp.Scale *= 0.5f;
        }
        SoundStyle growSound = AssetRegistry.Sounds.Celestia.BigBowFullyGrown with { PitchVariance = 0.3f };
        SoundEngine.PlaySound(growSound, Projectile.position);
    }

    private void GrowEffect()
    {
        for(float f = 0; f < 8; f++)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(384, 384);
            Vector2 vel = (Projectile.Center - pos);
            vel *= 0.1f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Color.Turquoise;
            fx.Scale *= 0.5f;
        }

        if(Main.netMode != NetmodeID.Server)
        {
            var screenShader = ModContent.GetInstance<ScreenShaderSystem>();
            screenShader.TintScreen(Color.Turquoise, 0.1f, 15f);
            PixelPrimitiveCircleFactory.CreateCelestiaInwardBoom(Projectile.Center);
        }

        for(float f = 0; f < 12; f++)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(384, 384);
            Vector2 vel = (Projectile.Center - pos);
            vel *= 0.1f;

            DustParticleSpawnParams spawnparams = DustParticleSpawnParams.Default;
            spawnparams.innerColor = Color.Lerp(Color.White, Color.Turquoise, Main.rand.NextFloat(0f, 1f));
            spawnparams.outerColor = Color.Turquoise;
            var dp = DustParticle.Spawn(pos, vel, spawnparams);
            dp.dampening = 0.05f;
            dp.gravity = 0;
            dp.Scale *= 0.5f;
        }

        SoundStyle growSound = AssetRegistry.Sounds.Celestia.BigBowCharge with { PitchVariance = 0.3f };
        SoundEngine.PlaySound(growSound, Projectile.position);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        float alpha = EasingFunction.InSine(_alphaTimer / 30f);
        alpha *= (float)(EasingFunction.Clamp(Projectile.timeLeft / 30f));
        Vector2 pullScale = _pullScale;
        pullScale *= MathHelper.Lerp(1.45f, 1f, EasingFunction.InSine(_alphaTimer / 60f));
        pullScale *= _chargeScale;

        Vector2 bowOffset = _bowOffset;
        float come = EasingFunction.InSine(_alphaTimer / 70f);

        SpritebatchDrawer backGlowDrawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BasicGlow"), Projectile.Center); ;
        backGlowDrawer.scale *= pullScale * 2.5f;
        backGlowDrawer.color = Color.Black * 0.5f * alpha;
        backGlowDrawer.worldPosition += bowOffset;
        Main.spriteBatch.Draw(backGlowDrawer);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center); ;
        glowDrawer.scale *= pullScale * 0.5f;
        glowDrawer.color = Color.Lerp(Color.Teal, Color.LightGreen, ExtraMath.Osc(0f, 1f, 0, Projectile.whoAmI)) * 0.2f * alpha;
        glowDrawer.color.A = 0;
        glowDrawer.worldPosition += bowOffset;
        Main.spriteBatch.Draw(glowDrawer);

        SpritebatchDrawer spiralVortexDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center); ;
        spiralVortexDrawer.scale *= pullScale * 0.5f;
        spiralVortexDrawer.color = Color.Lerp(Color.Teal, Color.LightGreen, ExtraMath.Osc(0f, 1f, 0, Projectile.whoAmI)) * 0.1f * alpha;
        spiralVortexDrawer.color.A = 0;
        spiralVortexDrawer.rotation = Main.GlobalTimeWrappedHourly;
        spiralVortexDrawer.worldPosition += bowOffset;
        Main.spriteBatch.Draw(spiralVortexDrawer);

        GlowingSwordMaskShader shader = GlowingSwordMaskShader.Instance;
        shader.TrailTexture = TrailRegistry.BulbTrail;
        shader.Distortion = 0.02f;
        shader.DistortionTexture = TrailRegistry.WhispyTrail;
        shader.Time = Main.GlobalTimeWrappedHourly * 16;
        shader.Bloom = 0.8f;
        shader.Tiling = Vector2.One * 0.75f;
        shader.InnerColor = Color.Lerp(Color.LightGreen, Color.DarkTurquoise, ExtraMath.Osc(0f, 1f, 12));
        shader.OuterColor = Color.DarkTurquoise;
        Main.spriteBatch.Restart(effect: shader.Effect);


        SpritebatchDrawer bowDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        bowDrawer.scale *= pullScale;
        bowDrawer.color = Color.Lerp(Color.Teal, Color.LightGreen, ExtraMath.Osc(0f, 1f, 16, Projectile.whoAmI)) * 0.5f * alpha;
        bowDrawer.color.A = 0;
        bowDrawer.worldPosition += bowOffset;
        Main.spriteBatch.Draw(bowDrawer);


        Main.spriteBatch.RestartDefaults();

        bowDrawer.worldPosition -= Projectile.rotation.ToRotationVector2() * 8;
        bowDrawer.worldPosition += _mirageOffset;
        bowDrawer.color =
            Color.Lerp(Color.DarkTurquoise, Color.DarkGreen, ExtraMath.Osc(0f, 1f, 16, Projectile.whoAmI)) * 0.2f * alpha;
        bowDrawer.color.A = 0;
        bowDrawer.scale *= 1.3f;
        Main.spriteBatch.Draw(bowDrawer);

        bowDrawer.color *= MathHelper.Lerp(0f, 0.1f, EasingFunction.InExpo(_growthIndex / 3f));
        for (float f = 0f; f < MathHelper.TwoPi; f+= 0.2f)
        {
            bowDrawer.worldPosition += Main.rand.NextVector2Circular(4, 4);
     
            Main.spriteBatch.Draw(bowDrawer);

        }

        float lineOut = Timer / 30f;
        lineOut = EasingFunction.InOutSine(lineOut);
        float lineOutAlpha = MathHelper.Lerp(1f, 0f, lineOut);
        SpritebatchDrawer bloomlineDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        bloomlineDrawer.color = Color.Teal * come * alpha * lineOutAlpha;
        bloomlineDrawer.color.A = 0;

        Player target = Main.player[Parent.target];
        float dist = Vector2.Distance(Projectile.Center, target.Center);
        float bloomLineSize = dist / (float)bloomlineDrawer.texture.Width;
        bloomlineDrawer.scale.X *= bloomLineSize;
        bloomlineDrawer.scale.Y *= 0.025f;
        bloomlineDrawer.scale *= _chargeScale;
        bloomlineDrawer.LeftCenterOrigin();
        bloomlineDrawer.drawOrigin.X += 64;
        bloomlineDrawer.rotation = Projectile.rotation;
        bloomlineDrawer.worldPosition += bowOffset;
        Main.spriteBatch.Draw(bloomlineDrawer);

        SpritebatchDrawer arrowDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Projectile[ModContent.ProjectileType<CelestialArrow>()], Projectile.Center);
        arrowDrawer.rotation = Projectile.rotation;
        arrowDrawer.scale.Y *= 0.5f;
        arrowDrawer.scale.X *= MathHelper.Lerp(0.5f, 1f, come);
        arrowDrawer.scale *= 2;
        arrowDrawer.scale *= _chargeScale;
        arrowDrawer.color = Color.LightGreen * come * alpha * lineOutAlpha * _arrowAlpha;
        arrowDrawer.color.A = 0;
        arrowDrawer.worldPosition += _arrowOffset + _bowOffset;

        Main.spriteBatch.Draw(arrowDrawer);

        return false;
    }
    private MagicCircleRenderer _magicCircleRenderer;
    private void DrawPixelatedPrims(GraphicsDevice graphicsDevice)
    {
        _magicCircleRenderer ??= new MagicCircleRenderer(AssetManager.GlowMask.MagicCircle2);
        Vector2 vel = Projectile.rotation.ToRotationVector2() * 100;
        Vector2 auraPos = Projectile.Center + vel;

        float alpha = 1f;


        float come = EasingFunction.InSine(_alphaTimer / 70f);
        float lineOut = Timer / 30f;
        lineOut = EasingFunction.InOutSine(lineOut);
        float lineOutAlpha = MathHelper.Lerp(1f, 0f, lineOut);
        alpha *= lineOutAlpha * come;
        _magicCircleRenderer.DrawRing(auraPos, vel, 0, 1, Color.Lerp(Color.Transparent, Color.Turquoise * 0.75f, alpha), Main.GlobalTimeWrappedHourly * 4);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Turquoise, Color.DarkGreen, 35);
        fx.Scale *= 2;
        for (float f = 0; f < 32f; f++)
        {
            Vector2 vel = -Projectile.rotation.ToRotationVector2() * 3;
            vel *= Main.rand.NextFloat(0.5f, 6);
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(127, 127);
            DustParticle dp = DustParticle.Spawn(pos, vel);
            dp.outerColor = Color.Turquoise;
            dp.gravity = 0;
            dp.dampening = 0.05f;
            dp.noTileCollide = true;
            dp.Scale *= 1.5f;
            dp.innerColor = Color.Lerp(Color.White, Color.Turquoise, Main.rand.NextFloat(0f, 1f));
        }
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedPrims);
    }
}

public class BigCelestialArrow : ModProjectile
{
    private Vector2 _stretchScale;
    private Vector2 _mirageOffset;
    public override string Texture => ModContent.GetInstance<CelestialArrow>().Texture;
    private ref float Timer => ref Projectile.ai[0];
    private Player Target => Main.player[(int)Projectile.ai[1]];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 1;
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        _stretchScale = Vector2.One;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 1200;
        Projectile.tileCollide = true;
        Projectile.extraUpdates = 16;
    }

    public override void AI()
    {
        base.AI();

        Timer++;
        if (Timer == 1)
        {
            _stretchScale = Vector2.One;
            for (float f = 0; f < 8f; f++)
            {
                Vector2 vel = Projectile.velocity;
                vel = vel.RotatedByRandom(MathHelper.PiOver4 / 2f);
                vel *= Main.rand.NextFloat(5f, 15f);
                DustParticle dp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(16, 16), vel);
                dp.outerColor = Color.Turquoise;
                dp.gravity = 0;
                dp.dampening = 0.05f;
                dp.noTileCollide = true;
            }

            GlowDonutParticle d = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity * 2);
            d.outerColor = Color.Turquoise;
            d.fadeToColor = Color.DarkTurquoise;
            d.Scale *= 0.3f;

            GlowDonutParticle d2 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity * 4);
            d2.outerColor = Color.Turquoise;
            d2.fadeToColor = Color.DarkTurquoise;
            d2.Scale *= 0.15f;


            GlowDonutParticle d3 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity * 1);
            d3.outerColor = Color.Turquoise;
            d3.fadeToColor = Color.DarkTurquoise;
            d3.Scale *= 1f;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            Projectile.velocity *= 5;
        }

        if(Timer % 4 == 0)
        {
            var fx = FXUtil.GlowStretch(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Projectile.velocity);
            fx.OuterGlowColor = Color.Turquoise;
            fx.Scale *= 0.66f;
        }
        if (Timer % 4 == 0)
        {
            //Visual effect purely, doesn't need to be net synced.
            _mirageOffset = Main.rand.NextVector2Circular(3, 3);
        }
        if (Timer % 2 == 0)
        {
            for (float f = 0; f < 3; f++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(48, 48);
                var d = Dust.NewDustPerfect(pos, DustID.GemEmerald, Scale: 1f);
                d.noGravity = true;
            }

        }


        if (Timer % 6 == 0)
        {
            DustParticle dp = DustParticle.Spawn(Projectile.Center, Projectile.velocity * 0.1f);
            dp.outerColor = Color.Turquoise;
            dp.gravity = 0;
            dp.dampening = 0.05f;
            dp.noTileCollide = true;
            dp.Scale *= 0.66f;
        }

        if (Timer % 12 == 0)
        {
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(30) * 0.1f));
            sp.Scale *= 0.5f;
            sp.flickering = true;
            sp.outerColor = Color.Turquoise;
            sp.noTileCollide = true;
            sp.gravity = 0;
            sp.dampening = 0.05f;
        }
        if (Timer > 80)
        {
            Projectile.tileCollide = true;
        }

        Vector2 targetScale = Vector2.Lerp(Vector2.One, new Vector2(1.5f, 0.6f), Projectile.velocity.Length() / 25f);
        _stretchScale = Vector2.Lerp(_stretchScale, targetScale, 0.1f);
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    private void DrawTrails(GraphicsDevice gDevice)
    {
        RichLaserShader laserShader = RichLaserShader.Instance;
        laserShader.LaserColor = Color.White;
        laserShader.InnerColor = Color.Turquoise;
        laserShader.OuterColor = Color.Turquoise;
        laserShader.LaserTexture = TrailRegistry.StarTrail;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, laserShader, Projectile.Size * 0.5f);

        BloomTrailShader b = BloomTrailShader.Instance;
        b.InnerColor = Color.Turquoise;
        b.OuterColor = Color.DarkTurquoise;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth2, b, Projectile.Size * 0.5f);
    }
    private float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 2;
    }
    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(10, 0, ratio);
    }
    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.LightGreen, Color.Turquoise, ratio) * 0.3f;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawTrails);
        SpritebatchDrawer celestialArrowDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        celestialArrowDrawer.scale *= _stretchScale;
        celestialArrowDrawer.color = Color.Lerp(Color.Teal, Color.Turquoise, ExtraMath.Osc(0f, 1f, speed: 6)) * 0.35f;
        celestialArrowDrawer.color.A = 0;
        celestialArrowDrawer.scale *= 2;
        Main.spriteBatch.Draw(celestialArrowDrawer);


        celestialArrowDrawer.worldPosition -= Projectile.rotation.ToRotationVector2() * 8;
        celestialArrowDrawer.worldPosition += _mirageOffset;
        celestialArrowDrawer.color =
            Color.Lerp(Color.DarkTurquoise, Color.DarkGreen, ExtraMath.Osc(0f, 1f, 0, Projectile.whoAmI)) * 0.2f;
        celestialArrowDrawer.color.A = 0;
        celestialArrowDrawer.scale *= 1.3f;
     
        Main.spriteBatch.Draw(celestialArrowDrawer);

        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i];
            pos += Projectile.Size * 0.5f;
            celestialArrowDrawer.worldPosition = pos;
            celestialArrowDrawer.color = Color.Lerp(Color.Turquoise, Color.Black, (float)i / (float)Projectile.oldPos.Length) * 0.1f;
            celestialArrowDrawer.color.A = 0;
            Main.spriteBatch.Draw(celestialArrowDrawer);

        }

        SpritebatchDrawer flareDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare3, Projectile.Center);
        flareDrawer.color = Color.Lerp(Color.Turquoise, Color.Black, EasingFunction.InSine(Timer / 30f)) * 0.6f;
        flareDrawer.scale = Vector2.Lerp(Vector2.One * 0.65f, Vector2.Zero, EasingFunction.InSine(Timer / 30f));
        flareDrawer.color.A = 0;
        Main.spriteBatch.Draw(flareDrawer);
        return false;
        //return base.PreDraw(ref lightColor);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
        Projectile.Kill();
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        base.OnKill(timeLeft);
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Turquoise, Color.DarkTurquoise);
        fx.Scale *= 0.66f;
        float numDust = 4;
        for (float n = 0; n < numDust; n++)
        {
            Vector2 vel = -Projectile.velocity;
            vel = vel.RotatedByRandom(MathHelper.ToRadians(60));
            vel = vel.SafeNormalize(Vector2.Zero);
            vel *= Main.rand.NextFloat(6, 12);
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.Turquoise;
            var dp = DustParticle.Spawn(Projectile.Center, vel, spawnParams);
            dp.fast = true;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
            dp.gravity = 0;
            dp.Scale *= 0.5f;
        }
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<BigCelestialBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}

public class BigCelestialBoom : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 196;
        Projectile.height = 196;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 120;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
    }

    public override void AI()
    {
        base.AI();
        if (Timer > 24)
            Projectile.hostile = false;
        Timer++;
        if (Timer == 1)
        {

            // BigCrackParticle.Spawn(Projectile.Center, Vector2.Zero, color: Color.Turquoise, Scale: 1.4f);
            ShockwavePlayer shockwavePlayer = Main.LocalPlayer.GetModPlayer<ShockwavePlayer>();
            shockwavePlayer.Bee = 120;
            shockwavePlayer.shockwavePosition = Projectile.Center;
            shockwavePlayer.rippleSize = 5;
            PixelPrimitiveCircleFactory.CreateCelestiaBoom(Projectile.Center);
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Turquoise, Color.DarkTurquoise, duration: 45);
            fx.Scale *= 3f;

            var fx2 = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Turquoise, Color.DarkTurquoise, duration: 45);
            fx2.Scale *= 1.8f;


            var fx3 = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Turquoise, Color.DarkTurquoise, duration: 45);
            fx3.Scale *= 1.8f;
            fx3.VectorScale.X *= 8;
            fx3.VectorScale.Y *= 0.5f;

            for (float f = 0; f < 64; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(24, 24);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.innerColor = Color.Lerp(Color.White, Color.Turquoise, Main.rand.NextFloat(0f, 1f));
                spawnParams.outerColor = Color.Turquoise;
                var d = DustParticle.Spawn(Projectile.Center, vel, spawnParams);
                d.dampening = 0.05f;
                d.gravity = 0;
                d.noTileCollide = true;
                d.Scale *= 1.5f;
               
            }

            if(Main.netMode != NetmodeID.Server)
            {
                ScreenShaderSystem e = ModContent.GetInstance<ScreenShaderSystem>();
                e.TintScreen(Color.Turquoise, 0.1f, 20);
            }
        
            FXUtil.ShakeCamera(Projectile.Center, 1024, 64);
            SoundStyle spawnSound = AssetRegistry.Sounds.Celestia.ArrowCrash with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(spawnSound, Projectile.position);
        }

        if (Timer == 45 && Main.netMode != NetmodeID.Server)
        {
            for (int i = 0; i < 16; i++)
            {
                Vector2 spawnPosition = Projectile.Center;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);
                spawnPosition.Y += Main.rand.NextFloat(-50, 0);

                Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -15);
                ModContent.GetInstance<FlyingSoilSystem>().NewSoil(spawnPosition, spawnVelocity);
            }
        }

        ShakeScreenPosition.Shake = MathHelper.Lerp(18, 1f, EasingFunction.InSine(Timer / 60f));
        if (Timer % 12 == 0)
        {
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(64, 64), Vector2.Zero, Color.White, Scale: 0.5f);
            sp.fast = true;
            sp.gravity = 0;
        }
    }


    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelSprites);

        return false;
    }


    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    private void DrawPixelSprites(SpriteBatch spriteBatch, Vector2 screenPos)
    {


        float outRatio = Timer / 60f;
        RadialShearShader shearShader = RadialShearShader.Instance;
        shearShader.Time = outRatio * 1.4f;

        Asset<Texture2D> magicCircle = AssetManager.GlowMask.SpiralVortex;
        SpritebatchDrawer waveDrawer = SpritebatchDrawer.FromTextureAsset(magicCircle, Projectile.Center);
        waveDrawer.rotation += Main.GlobalTimeWrappedHourly * 4;
        waveDrawer.scale = Vector2.Lerp(Vector2.One * 0.8f, Vector2.One * 1.6f, EasingFunction.OutExpo(outRatio)) * 1.5f;
        waveDrawer.color = Color.Turquoise;
        waveDrawer.color *= MathHelper.SmoothStep(1f, 0f, outRatio);
        waveDrawer.color.A = 0;

        Main.spriteBatch.Restart(effect: shearShader.Effect);
        Main.spriteBatch.Draw(waveDrawer);

        SpritebatchDrawer backGlowDrawwer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        backGlowDrawwer.color = Color.DarkTurquoise * 0.5f;
        backGlowDrawwer.color.A = 0;
        backGlowDrawwer.scale = Vector2.One * 2f;
        Main.spriteBatch.Draw(backGlowDrawwer);

        waveDrawer.color = Color.Lerp(Color.Black, Color.White, EasingFunction.InOutSine(outRatio));
        waveDrawer.color.A = 0;
        Main.spriteBatch.Draw(waveDrawer);


        Main.spriteBatch.RestartDefaults();


        shearShader.Time = MathHelper.Lerp(0f, 1f, EasingFunction.InExpo(Timer / 120f));
        Main.spriteBatch.Restart(effect: shearShader.Effect);

        SpritebatchDrawer crackDrawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>("Stellamod/Visual/Particles/BigCrackParticle"), Projectile.Center);
        crackDrawer.color = Color.Lerp(Color.White, Color.Turquoise, EasingFunction.InExpo(Timer / 60f));
        crackDrawer.color.A = 0;
        crackDrawer.scale = Vector2.One * 2f;
        Main.spriteBatch.Draw(crackDrawer);


        Main.spriteBatch.RestartDefaults();


        float Time = 90f;
        float target = -Timer * 0.02f + 0.8f;
        VerliaShockwaveShader shockwaevShader = VerliaShockwaveShader.Instance;
        shockwaevShader.Time = MathHelper.Lerp(0, target, EasingFunction.InExpo(Timer / Time));
     
        SpriteBatch sb = Main.spriteBatch;
        sb.Restart(effect: shockwaevShader.Effect);
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.BlastPillar, Projectile.Center);
        sbDrawer.BottomCenterOrigin();
        sbDrawer.scale.X *= MathHelper.Lerp(0f, 5f, EasingFunction.OutExpo(Timer / Time));
        sbDrawer.scale.Y += MathHelper.Lerp(8f, 0f, EasingFunction.InOutExpo(Timer / Time));
        sbDrawer.scale.Y *= MathHelper.Lerp(0.2f, 2f, EasingFunction.QuadraticBump(Timer / Time));
        sbDrawer.color = Color.Turquoise;
        sbDrawer.color.A = 0;

        int height = 16;
        sbDrawer.worldPosition.Y += height;
        Main.spriteBatch.Draw(sbDrawer);


        sbDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.BlastPillar, Projectile.Center);
        sbDrawer.BottomCenterOrigin();
        sbDrawer.scale.X *= MathHelper.Lerp(0f, 3f, EasingFunction.OutExpo(Timer / Time));
        sbDrawer.scale.Y += MathHelper.Lerp(4f, 0f, EasingFunction.InOutExpo(Timer / Time));
        sbDrawer.scale.Y *= MathHelper.Lerp(0.2f, 2f, EasingFunction.QuadraticBump(Timer / Time));
        sbDrawer.color = Color.White;
        sbDrawer.color.A = 0;
        sbDrawer.worldPosition.Y += height;
        Main.spriteBatch.Draw(sbDrawer);



        sb.RestartDefaults();


        SpritebatchDrawer glowLineDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowLineDrawer.worldPosition.Y += height;
        glowLineDrawer.scale.X *= MathHelper.Lerp(1f, 4f, EasingFunction.OutExpo(Timer / Time));
        glowLineDrawer.scale.Y *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / Time)) * 0.2f;
        glowLineDrawer.color = Color.Turquoise;
        glowLineDrawer.color *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / Time));
        glowLineDrawer.color.A = 0;
        Main.spriteBatch.Draw(glowLineDrawer);
        glowLineDrawer.scale.X *= 0.5f;
        glowLineDrawer.color = Color.White;
        glowLineDrawer.color *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / Time));
        glowLineDrawer.color.A = 0;
        Main.spriteBatch.Draw(glowLineDrawer);

    }

}
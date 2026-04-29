using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Animations;
using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Palettes;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MoonspiralTower.VerliaBoss;

//VERLIA TODO:
//Two bouncing moons attack
//NEW scrolling moon and glowing moon shader very cool
//NEW moon particle, make some cool like space-y dust

//Blue Magic Sword
//NEW magic sword glow mask
//Should be blue

//Spinning little moons
//Tiny little moons with the same new moon shader
//Cool explosion using the new radial shear shader that we made
//Sparkling little moon dusts would be super cool

//Blade dance
//Basically the same as legacy, not much has to change

//Clone summon
//She creates a really cool mirror and her clones pop out

//Phase 2 transition
//Desperation big moon
//Really cool blue moon
//Moon blade sprites and afterimage effects when it explodes
//When she starts this there's a cool circle that comes in and pulls you inward once she teleports to her center

//Sword Fall
//Blue magical swords spiral in and then glisten/bright/flash up, and then they come down with a bit of easing
//Camera will slightly pan upward so you can see them better like Bishinine's rain


//Running
//basic afterimage and trailing effects
//cool moon magic

//Dash Slash
//Very fast, uses the slash animations we already have just with a bit more

//Blade Dance V2
//Goes into the big sword, shouldn't look like the other one it'll be a lot more elegant, blue, magical, and moony

//Dying
//Verlia slows, down, falls down and looks into the mirror
//She sees a clone of herself before the mirror cracks and she dies
//and her sword drops on the ground (you can't pick it up it's just visual)

public class VerlianWingsShader : CrystalShader<VerlianWingsShader>
{
    private EffectParameter _scrollOffsetParam;
    private EffectParameter _maskSizeParam;
    private EffectParameter _perlinNoiseSizeParam;
    private EffectParameter _scrollingTextureSizeParam;
    private EffectParameter _tilingParam;
    private EffectParameter _distortionStrengthParam;
    private EffectParameter _bloomColorStartParam;
    private EffectParameter _bloomColorEndParam;
    private EffectParameter _frequencyParam;

    public float Frequency
    {
        set
        {
            _frequencyParam ??= Effect.Parameters["frequency"];
            _frequencyParam.SetValue(value);
        }
    }
    public Vector2 MaskSize
    {
        set
        {
            _maskSizeParam ??= Effect.Parameters["maskSize"];
            _maskSizeParam.SetValue(value);
        }
    }
    public Texture2D ScrollingTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[1] = value;
            _scrollingTextureSizeParam ??= Effect.Parameters["scrollingTextureSize"];
            _scrollingTextureSizeParam.SetValue(value.Size());
        }
    }

    public Texture2D PerlinNoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[2] = value;
            _perlinNoiseSizeParam ??= Effect.Parameters["perlinNoiseSize"];
            _perlinNoiseSizeParam.SetValue(value.Size());
        }
    }

    public Vector2 ScrollOffset
    {
        set
        {
            _scrollOffsetParam ??= Effect.Parameters["scrollOffset"];
            _scrollOffsetParam.SetValue(value);
        }
    }

    public Vector2 Tiling
    {
        set
        {
            _tilingParam ??= Effect.Parameters["tiling"];
            _tilingParam.SetValue(value);
        }
    }

    public float DistortionStrength
    {
        set
        {
            _distortionStrengthParam ??= Effect.Parameters["distortionStrength"];
            _distortionStrengthParam.SetValue(value);
        }
    }

    public Color BloomColorStart
    {
        set
        {
            _bloomColorStartParam ??= Effect.Parameters["bloomColorStart"];
            _bloomColorStartParam.SetValue(value.ToVector3());
        }
    }

    public Color BloomColorEnd
    {
        set
        {
            _bloomColorEndParam ??= Effect.Parameters["bloomColorEnd"];
            _bloomColorEndParam.SetValue(value.ToVector3());
        }
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
}
public class VerliaShockwaveShader : CrystalShader<VerliaShockwaveShader>
{
    private EffectParameter _timeParam;
    private EffectParameter _frequencyParam;
    private EffectParameter _amplitudeParam;
    public float Time
    {
        set
        {
            _timeParam ??= Effect.Parameters["time"];
            _timeParam.SetValue(value);
        }
    }

    public float Frequency
    {
        set
        {
            _frequencyParam ??= Effect.Parameters["frequency"];
            _frequencyParam.SetValue(value);
        }
    }

    public float Amplitude
    {
        set
        {
            _amplitudeParam ??= Effect.Parameters["amplitude"];
            _amplitudeParam.SetValue(value);
        }
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Amplitude = 0.5f;
        Frequency = 4.0f;
    }
}
public class ScrollingMoonShader : CrystalShader<ScrollingMoonShader>
{
    private EffectParameter _scrollOffsetParam;
    private EffectParameter _imageSizeParam;
    private EffectParameter _maskSizeParam;
    private EffectParameter _bendStrengthParam;
    private EffectParameter _tilingParam;
    public Vector2 Tiling
    {
        set
        {
            _tilingParam ??= Effect.Parameters["tiling"];
            _tilingParam.SetValue(value);
        }
    }
    public float BendStrength
    {
        set
        {
            _bendStrengthParam ??= Effect.Parameters["bendStrength"];
            _bendStrengthParam.SetValue(value);
        }
    }
    public Vector2 ScrollOffset
    {
        set
        {
            _scrollOffsetParam ??= Effect.Parameters["scrollOffset"];
            _scrollOffsetParam.SetValue(value);
        }
    }

    public Vector2 ImageSize
    {
        set
        {
            _imageSizeParam ??= Effect.Parameters["imageSize"];
            _imageSizeParam.SetValue(value);
        }
    }

    public Vector2 MaskSize
    {
        set
        {
            _maskSizeParam ??= Effect.Parameters["maskSize"];
            _maskSizeParam.SetValue(value);
        }
    }


    /// <summary>
    /// Sets the Sampler 1 state to Point Clamp and Texture 1 to the passed value, while also automatically setting the ImageSize parameter of the shader
    /// </summary>
    public Texture2D ScrollingTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[1] = value;
            ImageSize = value.Size();
        }
    }
}
public class VerliaGreatBlade : ModProjectile
{
    private enum SwingState
    {
        Charge,
        Swing,
        Out
    }

    private float _stretchAlpha;
    private float _inScale;
    private int _growthIndex;
    private float _flashAlpha;
    private float _swingTrailAlpha;
    private Asset<Texture2D> _smallBladeTextureAsset;
    private Asset<Texture2D> _bigBladeTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    private SwingState State
    {
        get => (SwingState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    private NPC Parent => Main.npc[(int)Projectile.ai[2]];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 128;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {

        //Check if the sword is colliding, this does a line check instead of terraria default box.
        float length = 512 * 1.5f;
        float rotation = Projectile.rotation;
        //  rotation -= MathHelper.PiOver4;
        Vector2 start = Projectile.Center;
        Vector2 end = Projectile.Center + rotation.ToRotationVector2() * length;
        float collisionPoint = 0f;
        bool check = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 16, ref collisionPoint);
        return check;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _stretchAlpha = 1f;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 6000;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.extraUpdates = 8;
        Projectile.hostile = true;
    }
    private float Fixer => Projectile.extraUpdates + 1;
    private float SwingTime => 60f * Fixer;
    private float ChargeTime => 100 * Fixer;
    private float OutTime => 30f * Fixer;
    private Vector2 AimingDirection => Projectile.velocity.X > 0 ? Vector2.UnitX : -Vector2.UnitX;
    public override void AI()
    {
        base.AI();
        ProjectileID.Sets.TrailCacheLength[Type] = 128;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        switch (State)
        {
            case SwingState.Charge:
                AI_Charge();
                break;
            case SwingState.Swing:
                AI_Swing();
                break;
            case SwingState.Out:
                AI_Out();
                break;
        }
        _flashAlpha = MathHelper.Lerp(_flashAlpha, 1f, 0.1f);
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    private void Grow()
    {
        if (_growthIndex >= 1)
            return;

        _flashAlpha = 0;
        _growthIndex++;
    }
    private void SwitchState(SwingState state)
    {
        if (this.OwnedByLocalClient())
        {
            Timer = 0;
            State = state;
            Projectile.netUpdate = true;
        }
    }

    private void AI_Charge()
    {
        Projectile.hostile = false;
        Timer++;
        if (Timer == 1)
        {
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Huhhuh"), Projectile.position);
            // SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/StarCharge"), Projectile.position);
        }
        int growth1 = (int)ChargeTime / 2;
        if (Timer == growth1)
        {
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordSheethe"), Projectile.position);
            Grow();
        }


        float rotation = AimingDirection.ToRotation();
        float ratio = Timer / ChargeTime;

        if (Timer > growth1)
        {

            Smoke();
            ShakeModSystem.Shake = 2;
            _stretchAlpha = MathHelper.Lerp(1f, 1.05f, EasingFunction.QuadraticBump((Timer - growth1) / (ChargeTime / 2)));
        }
        else
        {
            ShakeModSystem.Shake = 1;
        }


        _inScale = MathHelper.Lerp(0f, 1f, EasingFunction.OutCirc(ratio));
        float ease = EasingFunction.InOutExpo(ratio);
        float startRotation = -Vector2.UnitY.ToRotation();
        float endRotation = rotation - MathHelper.ToRadians(208 * AimingDirection.X);
        float interpolatedRotation = Utils.AngleLerp(startRotation, endRotation, ease);
        Projectile.rotation = interpolatedRotation;
        Projectile.Center = Parent.Center;
        if (Timer >= ChargeTime + 30)
        {
            SwitchState(SwingState.Swing);
        }
    }

    private void Smoke()
    {
        if (Timer % 8 == 0)
        {
            Vector2 offset = Projectile.rotation.ToRotationVector2() * MathHelper.Lerp(0, 384f, Main.rand.NextFloat(0f, 1f));
            var sp = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center + offset, Main.rand.NextVector2Circular(15, 15));
            sp.behindLayer = true;
            sp.fadeToColor = Color.Black;
            sp.color = Color.Lerp(Color.LightSkyBlue, Color.DarkBlue, Main.rand.NextFloat(0f, 1f));
            //    sp.flickering = true;
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
        }
    }
    private void AI_Swing()
    {
        _swingTrailAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(Timer / SwingTime));
        _stretchAlpha = 1f;
        Projectile.hostile = true;
        _inScale = MathHelper.Lerp(_inScale, 1f, 0.1f);
        Timer++;
        if (Timer % 8 == 0 && Timer > 120)
        {
            Vector2 offset = Projectile.rotation.ToRotationVector2() * MathHelper.Lerp(0, 384f, Main.rand.NextFloat(0f, 1f));
            var sp = SparkleParticle.Spawn(Projectile.Center + offset, Main.rand.NextVector2Circular(15, 15));
            //sp.flickering = true;
            sp.dampening = 0.05f;
            sp.noTileCollide = true;
            //    sp.flickering = true;
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
            sp.gravity = 0;
            //  sp.Scale *= 2;
            sp.outerColor = Color.Blue;
        }
        Smoke();


        if (Timer % 8 == 0)
        {
            Vector2 offset = Projectile.rotation.ToRotationVector2() * 444f;
            var sp = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center + offset, Main.rand.NextVector2Circular(15, 15));
            sp.behindLayer = true;
            sp.fadeToColor = Color.Black;
            sp.color = Color.Lerp(Color.LightSkyBlue, Color.DarkBlue, Main.rand.NextFloat(0f, 1f));
            sp.color = Color.Lerp(sp.color, Color.Black, 0.5f);
            //    sp.flickering = true;
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
        }
        float ratio = Timer / SwingTime;
        float ease = EasingFunction.InOutExpo(ratio);
        float midRotation = AimingDirection.ToRotation();
        float startRotation = midRotation - MathHelper.ToRadians(208 * AimingDirection.X);
        float endRotation = midRotation + MathHelper.ToRadians(160 * AimingDirection.X);
        float interpolatedRotation = MathHelper.Lerp(startRotation, endRotation, ease);
        Projectile.rotation = interpolatedRotation;
        Projectile.Center = Parent.Center;
        if (Timer >= SwingTime - 120)
        {
            SwitchState(SwingState.Out);
        }
    }

    private void AI_Out()
    {
        Projectile.hostile = false;
        Projectile.Kill();
    }

    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.White, Color.Black, ratio);
    }

    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(192, 0, ratio) * _swingTrailAlpha;
    }
    private float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 1.05f;
    }
    private float GetTrailWidth3(float ratio)
    {
        return GetTrailWidth(ratio) * 3;
    }
    private float GetTrailWidth4(float ratio)
    {
        return GetTrailWidth3(ratio) * 1.05f;
    }
    private void DrawTrails(GraphicsDevice gDevice)
    {
        Vector2[] swingPos = new Vector2[Projectile.oldRot.Length];
        for (int i = 0; i < swingPos.Length; i++)
        {
            swingPos[i] = Projectile.oldRot[i].ToRotationVector2() * 484 * 2f + Projectile.Center;
        }

        RichLaserShader laserShader = RichLaserShader.Instance;
        laserShader.LaserColor = Color.White;
        laserShader.BloomTexture = AssetManager.LaserTextures.Bloom;
        laserShader.LaserTexture = TrailRegistry.StarTrail;
        TrailDrawer.Draw(Main.spriteBatch, swingPos, GetTrailColor, GetTrailWidth, laserShader);

        BloomTrailShader b = BloomTrailShader.Instance;
        b.InnerColor = Color.Blue;
        b.OuterColor = Color.DarkBlue;
        TrailDrawer.Draw(Main.spriteBatch, swingPos, GetTrailColor, GetTrailWidth2, b);


        swingPos = new Vector2[Projectile.oldRot.Length];
        for (int i = 0; i < swingPos.Length; i++)
        {
            swingPos[i] = Projectile.oldRot[i].ToRotationVector2() * 484 * 1.25f + Projectile.Center;
        }

        TrailDrawer.Draw(Main.spriteBatch, swingPos, GetTrailColor, GetTrailWidth3, laserShader);
        TrailDrawer.Draw(Main.spriteBatch, swingPos, GetTrailColor, GetTrailWidth4, b);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (State == SwingState.Swing)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawTrails);
        }

        _smallBladeTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_0");
        _bigBladeTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_1");

        SpriteBatch spriteBatch = Main.spriteBatch;
        GlowingSwordMaskShader shader = GlowingSwordMaskShader.Instance;
        shader.TrailTexture = TrailRegistry.WhispyTrail;
        shader.Distortion = 0.04f;
        shader.DistortionTexture = TrailRegistry.WhispyTrail;
        shader.Time = Main.GlobalTimeWrappedHourly * 16;
        shader.Bloom = 0.8f;
        shader.Tiling = Vector2.One * 1;
        shader.InnerColor = Color.LightBlue;
        shader.OuterColor = Color.Blue;
        spriteBatch.Restart(effect: shader.Effect);


        Asset<Texture2D> bladeAsset;
        switch (_growthIndex)
        {
            default:
            case 0:
                bladeAsset = _smallBladeTextureAsset;
                break;
            case 1:
                bladeAsset = _bigBladeTextureAsset;
                break;
        }

        float scale = 1.5f;
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromTextureAsset(bladeAsset, Projectile.Center);
        sbDrawer.LeftCenterOrigin();
        sbDrawer.rotation = Projectile.rotation;
        sbDrawer.color.A = 0;
        sbDrawer.scale = Projectile.scale * Vector2.Lerp(Vector2.One * 0.5f, Vector2.One, _flashAlpha) * _inScale;
        sbDrawer.scale.X *= _stretchAlpha;
        sbDrawer.scale *= scale;
        spriteBatch.Draw(sbDrawer);
        sbDrawer.scale *= 1.25f;
        sbDrawer.scale.X *= _stretchAlpha;
        sbDrawer.scale *= scale;
        spriteBatch.Draw(sbDrawer);
        spriteBatch.RestartDefaults();

        Asset<Texture2D> glowCircle = AssetManager.GlowMask.SimpleGlowCircle;
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(glowCircle, Projectile.Center);
        glowDrawer.color = Color.Blue * ExtraMath.Osc(0.5f, 1f, speed: 2);
        glowDrawer.color.A = 0;
        glowDrawer.rotation = Projectile.rotation;
        glowDrawer.scale.X *= 2 * _inScale;
        glowDrawer.scale.Y *= 0.5f;
        glowDrawer.scale *= scale;
        glowDrawer.worldPosition += Projectile.rotation.ToRotationVector2() * 384;
        spriteBatch.Draw(glowDrawer);
        return false;
        //return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        float numDust = 32;
        for (float n = 0; n < numDust; n++)
        {
            Vector2 offset = Projectile.rotation.ToRotationVector2() * MathHelper.Lerp(0, 384f, Main.rand.NextFloat(0f, 1f));
            var sp = DustParticle.Spawn(Projectile.Center + offset, Main.rand.NextVector2Circular(15, 15));
            //sp.flickering = true;
            sp.dampening = 0.05f;
            sp.noTileCollide = true;
            //    sp.flickering = true;
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
            sp.gravity = 0;
            sp.Scale *= 2;
            sp.outerColor = Color.Blue;
        }
        for (int i = 0; i < Projectile.oldRot.Length; i++)
        {
            if (!Main.rand.NextBool(8))
                continue;
            Vector2 pos = Projectile.oldRot[i].ToRotationVector2() * 484 + Projectile.Center;
            FXUtil.GlowStretch(pos, (pos - Parent.Center).SafeNormalize(Vector2.Zero) * 8);
        }
    }
}
public class VerliaBouncingMoonBoom : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 2048;
        Projectile.height = 32;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.hostile = true;
    }

    public override void AI()
    {
        base.AI();
        if (Timer > 10)
            Projectile.hostile = false;
        Timer++;
        if (Timer == 1)
        {
            for (float f = 0; f < 5f; f++)
            {
                Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
                Vector2 spawnVelocity = Vector2.Zero;
                spawnVelocity.Y = Main.rand.NextFloat(-10, -1f);

                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                var steamParticle = Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(12, 12);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.DarkGray;
                spawnParams.scaleRange *= 0.5f;
                spawnParams.innerColor = Color.White;
                DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
            }

            for (int i = 0; i < 8; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(12, 12);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.DarkGray;
                spawnParams.scaleRange *= 0.5f;
                spawnParams.innerColor = Color.White;
                DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
            }

            var fx = FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.LightSkyBlue,
                outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.24f);
            fx.Scale *= 1f;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            FXUtil.PunchCamera(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero), 4, 4, 4);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        if (Timer < 2)
            return false;
        float outRatio = Timer / 60f;
        RadialShearShader shearShader = RadialShearShader.Instance;
        shearShader.Time = outRatio * 1.4f;

        Asset<Texture2D> magicCircle = AssetManager.GlowMask.SpiralVortex;
        SpritebatchDrawer waveDrawer = SpritebatchDrawer.FromTextureAsset(magicCircle, Projectile.Center);
        // waveDrawer.rotation += Main.GlobalTimeWrappedHourly * 4;
        waveDrawer.scale = Vector2.Lerp(Vector2.One * 0.8f, Vector2.One * 1.6f, EasingFunction.OutExpo(outRatio)) * 1.5f;
        waveDrawer.scale.Y *= 0.5f;
        waveDrawer.color = Color.SkyBlue;
        waveDrawer.color *= MathHelper.SmoothStep(1f, 0f, outRatio);
        waveDrawer.color.A = 0;

        Main.spriteBatch.Restart(effect: shearShader.Effect);
        Main.spriteBatch.Draw(waveDrawer);

        SpritebatchDrawer backGlowDrawwer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        backGlowDrawwer.color = Color.DarkBlue * 0.5f;
        backGlowDrawwer.color.A = 0;
        backGlowDrawwer.scale = Vector2.One * 2f;
        Main.spriteBatch.Draw(backGlowDrawwer);

        waveDrawer.color = Color.Lerp(Color.Black, Color.White, EasingFunction.InOutSine(outRatio));
        waveDrawer.color.A = 0;
        Main.spriteBatch.Draw(waveDrawer);
        Main.spriteBatch.RestartDefaults();
        return false;
    }
}
public class VerliaBouncingMoonShockwave : ModProjectile
{
    private float Time => 120f;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 512;
        Projectile.height = 64;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = (int)Time;
    }

    public override void AI()
    {
        base.AI();
        if(Timer > 12)
        {
            Projectile.hostile = false;
        }
        Timer++;
        if (Timer == 1)
        {
            float numDust = 32;
            for (float f = 0; f < numDust; f++)
            {
                Vector2 spawnPos = Projectile.Center;
                spawnPos.X += Main.rand.NextFloat(-128, 128);
                spawnPos.Y += Main.rand.NextFloat(-2f, 2f);
                Vector2 velocity = -Vector2.UnitY * 2;
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(35));
                velocity *= Main.rand.NextFloat(1f, 15f);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.LightSkyBlue;
                var dp = DustParticle.Spawn(spawnPos, velocity, spawnParams);
                dp.gravity = 0;
                dp.dampening = 0.05f;
                dp.Scale *= 2;
            }

            float numSparkles = 16;
            for (float f = 0; f < numSparkles; f++)
            {
                Vector2 spawnPos = Projectile.Center;
                spawnPos.X += Main.rand.NextFloat(-128, 128);
                spawnPos.Y += Main.rand.NextFloat(-2f, 2f);
                Vector2 velocity = -Vector2.UnitY * 2;
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(35));
                velocity *= Main.rand.NextFloat(1f, 15f);

                var dp = SparkleParticle.Spawn(spawnPos, velocity);
                dp.gravity = 0;
                dp.dampening = 0.05f;
                dp.Scale *= 1.5f;
                dp.outerColor = Color.Blue;
                dp.flickering = true;
            }

            if (ModContent.GetInstance<LunarVeilClientConfig>().DramaticEffects)
            {
                SpecialEffectsPlayer effectsPlayer = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
                effectsPlayer.darknessCurve = MathHelper.Lerp(0.5f, 0f, EasingFunction.InExpo(Timer / Time));
            }
            SoundStyle impactSound = AssetRegistry.Sounds.Verlia.MoonDuoHitGround;
            SoundEngine.PlaySound(impactSound);

            ShakeModSystem.Shake = 16;
            FXUtil.ShakeCamera(Projectile.Center, 2048, 32);
            //     FXUtil.PunchCamera(Projectile.Center, Vector2.UnitY, 32, 2, 32);
        }

    }
    public override bool PreDraw(ref Color lightColor)
    {
        VerliaShockwaveShader shockwaevShader = VerliaShockwaveShader.Instance;
        shockwaevShader.Time = -Timer * 0.02f + 0.8f;
        SpriteBatch sb = Main.spriteBatch;
        sb.Restart(effect: shockwaevShader.Effect);
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.BottomCenterOrigin();
        sbDrawer.scale.X *= MathHelper.Lerp(0f, 3.8f, EasingFunction.OutExpo(Timer / Time));
        sbDrawer.scale.Y += MathHelper.Lerp(8f, 0f, EasingFunction.InOutExpo(Timer / Time));
        sbDrawer.scale.Y *= MathHelper.Lerp(0.2f, 2f, EasingFunction.QuadraticBump(Timer / Time));
        sbDrawer.color *= 0.5f;
        sbDrawer.color.A = 0;
        sbDrawer.worldPosition.Y += Projectile.height;
        Main.spriteBatch.Draw(sbDrawer);


        sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.BottomCenterOrigin();
        sbDrawer.scale.X *= MathHelper.Lerp(0f, 1.9f, EasingFunction.OutExpo(Timer / Time));
        sbDrawer.scale.Y += MathHelper.Lerp(4f, 0f, EasingFunction.InOutExpo(Timer / Time));
        sbDrawer.scale.Y *= MathHelper.Lerp(0.2f, 2f, EasingFunction.QuadraticBump(Timer / Time));
        sbDrawer.color *= 0.5f;
        sbDrawer.color.A = 0;
        sbDrawer.worldPosition.Y += Projectile.height;
        Main.spriteBatch.Draw(sbDrawer);

        sb.RestartDefaults();

        SpritebatchDrawer glowLineDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowLineDrawer.worldPosition.Y += Projectile.height;
        glowLineDrawer.scale.X *= MathHelper.Lerp(1f, 8f, EasingFunction.OutExpo(Timer / Time));
        glowLineDrawer.scale.Y *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / Time)) * 0.2f;
        glowLineDrawer.color *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / Time));
        glowLineDrawer.color.A = 0;
        Main.spriteBatch.Draw(glowLineDrawer);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class VerliaMiniMoon : ModProjectile
{

    private float _flashAlpha;
    private Vector2 _squishScale;
    private Vector2 _targetScale;
    private ref float Timer => ref Projectile.ai[0];
    private Asset<Texture2D> _scrollingMoonTextureAsset;
    private Asset<Texture2D> _shadowTextureAsset;
    private Asset<Texture2D> _outlineMoonTextureAsset;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;

    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 130;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.hostile = true;
    }

    public override void AI()
    {
        base.AI();

        Timer++;
        if (Timer == 1)
        {
            SoundStyle spawnSound = new SoundStyle($"Stellamod/Assets/Sounds/SoftSummon");
            spawnSound.PitchVariance = 0.4f;
            SoundEngine.PlaySound(spawnSound, Projectile.position);
            _flashAlpha = 1f;
            _squishScale = new Vector2(0.9f, 1.2f);
        }
        _targetScale = Vector2.Lerp(_targetScale, Vector2.One, 0.1f);
        _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.1f);
        _flashAlpha = MathHelper.Lerp(_flashAlpha, 0f, 0.1f);

        if (Timer < 40)
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.Pi / 40f);
        }
        else if (Timer < 70)
        {
            Projectile.velocity *= 0.8f;
        }
        else
        {
            Player player = PlayerHelper.FindClosestPlayer(Projectile.Center, 1024);
            if (Timer == 72 && player != null)
            {
                SoundStyle spawnSound = new SoundStyle($"Stellamod/Assets/Sounds/StarFlower2");
                spawnSound.PitchVariance = 0.4f;
                spawnSound.Volume = 0.3f;
                SoundEngine.PlaySound(spawnSound, Projectile.position);
                Vector2 redirectVelocity = player.Center - Projectile.Center;
                redirectVelocity = redirectVelocity.SafeNormalize(Vector2.Zero);
                redirectVelocity *= 2;
                Projectile.velocity = redirectVelocity;
            }
            if (Projectile.velocity.Length() < 25)
            {
                if (Timer % 7 == 0)
                {
                    Vector2 pos = Projectile.Center;
                    pos += Main.rand.NextVector2Circular(48, 48);
                    Vector2 vel = -Projectile.velocity;
                    vel = vel.SafeNormalize(Vector2.Zero);
                    vel *= 15;
                    FXUtil.GlowStretch(pos, vel);
                }

                if (Timer % 7 == 0)
                {
                    var sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Vector2.Zero);
                    sp.flickering = true;
                    sp.outerColor = Color.Blue;
                    sp.fast = true;
                    sp.behindLayer = true;
                    sp.gravity = 0;
                }

                Projectile.velocity *= 1.1f;
            }
        }
    }

    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.White, Color.Transparent, ratio);
    }

    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(64, 0, ratio);
    }
    private void DrawPixelatedTrails(GraphicsDevice gDevice)
    {
        RichLaserShader laserShader = RichLaserShader.Instance;
        laserShader.LaserTexture = TrailRegistry.CorkscrewTrail;
        laserShader.InnerColor = Color.White;
        laserShader.OuterColor = Color.Lerp(Color.LightSkyBlue, Color.Black, 0.6f);
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, laserShader, Projectile.Size * 0.5f);
    }
    private void DrawPixelatedMoon(SpriteBatch sb, Vector2 screenPos)
    {
        SpritebatchDrawer moonSprite = SpritebatchDrawer.FromProjectile(Projectile);
        _scrollingMoonTextureAsset ??= ModContent.Request<Texture2D>(ModContent.GetInstance<VerliaBouncingMoon>().Texture + "_ScrollingMoon");
        moonSprite = SpritebatchDrawer.FromProjectile(Projectile);
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.3f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.7f;
        glowDrawer.scale *= _squishScale * _targetScale;
        Main.spriteBatch.Draw(glowDrawer);


        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.2f;
        glowDrawer.color.A = 0;
        glowDrawer.scale.X *= 1.2f;
        glowDrawer.scale.Y *= 0.6f;
        glowDrawer.scale *= _squishScale * _targetScale;
        Main.spriteBatch.Draw(glowDrawer);


        ScrollingMoonShader scrollingMoonShader = ScrollingMoonShader.Instance;
        scrollingMoonShader.ScrollingTexture = _scrollingMoonTextureAsset.Value;
        scrollingMoonShader.MaskSize = TextureAssets.Projectile[Type].Value.Size();

        float time = Main.GlobalTimeWrappedHourly * 1f;
        time += Projectile.whoAmI * 0.5f;
        scrollingMoonShader.ScrollOffset = new Vector2(time, 0f);
        scrollingMoonShader.BendStrength = 1.8f;
        scrollingMoonShader.Tiling = new Vector2(0.13f, 0.45f);


        //Draw the moon itself
        sb.Restart(effect: scrollingMoonShader.Effect);
        moonSprite.rotation = MathHelper.ToRadians(-12);
        moonSprite.color = Color.Lerp(Color.White, Color.LightSkyBlue, ExtraMath.Osc(0f, 0.3f, speed: 8));
        moonSprite.scale *= _squishScale * _targetScale;
        Main.spriteBatch.Draw(moonSprite);
        sb.RestartDefaults();

        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SolarRing, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.6f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.5f;
        glowDrawer.scale *= _squishScale * _targetScale * 0.75f;
        Main.spriteBatch.Draw(glowDrawer);


        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SolarEye, Projectile.Center);
        glowDrawer.color = Color.Blue * 0.16f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 1f;
        glowDrawer.rotation = Main.GlobalTimeWrappedHourly;
        glowDrawer.scale *= _squishScale * _targetScale * 0.75f;
        Main.spriteBatch.Draw(glowDrawer);


        moonSprite.color = Color.Lerp(Color.Transparent, Color.White, _flashAlpha);
        Main.spriteBatch.Draw(moonSprite);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        _outlineMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedMoon);
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrails);

        _shadowTextureAsset ??= ModContent.Request<Texture2D>(ModContent.GetInstance<VerliaBouncingMoon>().Texture + "_Shadow");
        SpritebatchDrawer shadowDrawer = SpritebatchDrawer.FromTextureAsset(_shadowTextureAsset, Projectile.Center);
        shadowDrawer.color *= 0.45f;
        shadowDrawer.scale *= _squishScale * _targetScale * 0.5f;
        Main.spriteBatch.Draw(shadowDrawer);


        SpritebatchDrawer outlineDrawer = SpritebatchDrawer.FromTextureAsset(_outlineMoonTextureAsset, Projectile.Center);
        outlineDrawer.color = Color.Red;
        outlineDrawer.scale *= _squishScale * _targetScale;
        Main.spriteBatch.Draw(outlineDrawer);

        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for (float f = 0; f < 5f; f++)
        {
            Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
            Vector2 spawnVelocity = Vector2.Zero;
            spawnVelocity.Y = Main.rand.NextFloat(-10, -1f);

            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            var steamParticle = Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
        }

        for (int i = 0; i < 4; i++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(12, 12);
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.DarkGray;
            spawnParams.scaleRange *= 0.5f;
            spawnParams.innerColor = Color.White;
            DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
        }

        for (int i = 0; i < 8; i++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(12, 12);
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.DarkGray;
            spawnParams.scaleRange *= 0.5f;
            spawnParams.innerColor = Color.White;
            DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
        }

        var fx = FXUtil.GlowCircleBoom(Projectile.Center,
            innerColor: Color.White,
            glowColor: Color.LightSkyBlue,
            outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.24f);
        fx.Scale *= 1f;
        FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
    }
}
public class VerliaBouncingMoon : ModProjectile
{
    private float _flashAlpha;
    private Asset<Texture2D> _shadowMoonTextureAsset;
    private Asset<Texture2D> _outlineMoonTextureAsset;
    private Asset<Texture2D> _scrollingMoonTextureAsset;
    private Vector2 _startPosition;
    private enum BounceState
    {
        Spawn,
        Bounce_1,
        Bounce_2,
        Bounce_Out
    }

    private Color _outlineColor;
    private ref float Timer => ref Projectile.ai[0];
    private BounceState State
    {
        get => (BounceState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }

    private ref float Direction => ref Projectile.ai[2];

    private Vector2 _squishScale;
    private Vector2 _scale;
    private Vector2 _targetScale;
    public Vector2 BounceUp1Distance => new Vector2(128, 75);
    public Vector2 BounceUp2Distance => new Vector2(256, 50);
    public Vector2 BounceOutDistance => new Vector2(384, 50);

    public float SpawnTime => 60;
    public float BounceUp1Time => 60;
    public float BounceUp2Time => 60;
    public float BounceOutTime => 120;
    public override void SendExtraAI(BinaryWriter writer)
    {

        base.SendExtraAI(writer);
        writer.WriteVector2(_startPosition);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _startPosition = reader.ReadVector2();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.penetrate = -1;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.light = 0.7f;
        Projectile.timeLeft = 600;
    }

    public override void AI()
    {
        base.AI();
        _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.1f);
        if (Timer % 24 == 0)
        {
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(48, 48), Vector2.Zero);
            sp.outerColor = Color.LightBlue;
            sp.gravity = 0;
            sp.Scale *= 0.5f;
            sp.behindLayer = true;
        }


        _flashAlpha = MathHelper.Lerp(_flashAlpha, 0f, 0.1f);


        _targetScale = Vector2.One;
        switch (State)
        {
            case BounceState.Spawn:
                AI_Spawn();
                break;
            case BounceState.Bounce_1:
                AI_Bounce1();
                break;
            case BounceState.Bounce_2:
                AI_Bounce2();
                _targetScale *= 1.25f;
                break;
            case BounceState.Bounce_Out:
                AI_BounceOut();
                _targetScale *= 1.5f;
                break;
        }
        if (Projectile.hostile)
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Red, 0.1f);
        }
        else
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Yellow, 0.1f);
        }
        _scale = Vector2.Lerp(_scale, _targetScale, 0.1f);
    }

    private void SwitchState(BounceState state)
    {
        if (this.OwnedByLocalClient())
        {
            Timer = 0;
            State = state;
            Projectile.netUpdate = true;
        }
    }

    /// <summary>
    /// Controls movement for the arcing motion that the moons do
    /// </summary>
    /// <param name="time"></param>
    /// <param name="distance"></param>
    private void Bounce(float time, Vector2 distance)
    {
        if (Timer == 1)
        {
            _startPosition = Projectile.Center;
        }

        float ratio = Timer / time;
        float ease = EasingFunction.InExpo(ratio);
        Vector2 targetPosition = _startPosition + -Vector2.UnitY * distance.Y;

        float bounceD = 90;
        if (State == BounceState.Bounce_2)
        {
            bounceD = 0;
        }
        
        targetPosition.X += Direction * bounceD;
        Vector2 interpolatedPosition = Vector2.Lerp(_startPosition, targetPosition, ease);
        float ease2 = EasingFunction.QuadraticBump(ratio);
        interpolatedPosition.X += MathHelper.Lerp(0f, distance.X * Direction, ease2);

        Vector2 velocity = interpolatedPosition - Projectile.Center;
        Projectile.velocity = velocity;
        if (Timer >= time)
        {
            BounceEffect();
        }
    }

    private void BounceEffect()
    {
        ShakeModSystem.Shake = 2;
        FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
        _flashAlpha = 1f;
        _squishScale = new Vector2(0.8f, 1.4f);
        for(int i = 0; i < 3; i++)
        {
            var donut = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.UnitX * -Direction, Color.LightSkyBlue);
            donut.Scale *= 2;

        }

        SoundStyle bounceSound = AssetRegistry.Sounds.Verlia.MoonBounceOnce;
        if (State == BounceState.Bounce_2)
        {
            bounceSound = AssetRegistry.Sounds.Verlia.MoonBounceTwo;
        }
        SoundEngine.PlaySound(bounceSound, Projectile.position);
    }

    private void AI_Spawn()
    {
        SwitchState(BounceState.Bounce_1);
    }

    private void AI_Bounce1()
    {
        Projectile.hostile = false;
        Timer++;
        if (Timer == 1)
        {
            _flashAlpha = 1f;
            SoundStyle bounceSound = new SoundStyle("Stellamod/Assets/Sounds/Veripulse");
            bounceSound.PitchVariance = 0.5f;
            bounceSound.MaxInstances = 1;
            SoundEngine.PlaySound(bounceSound, Projectile.position);
        }
        Bounce(BounceUp1Time, BounceUp1Distance);
        if (Timer >= BounceUp1Time)
        {
            SwitchState(BounceState.Bounce_2);
        }
    }

    private void AI_Bounce2()
    {
        Projectile.hostile = false;
        Timer++;
        Bounce(BounceUp2Time, BounceUp2Distance);
        if (Timer >= BounceUp2Time)
        {
            SwitchState(BounceState.Bounce_Out);
        }
    }

    private void AI_BounceOut()
    {
        Projectile.hostile = true;
        if (Timer < 80)
        {
            Vector2 pos = Projectile.Center;
            pos.Y += 384;
        //    CameraTargetSystem.AddTarget(pos);
        }

        Timer++;
        if(Timer == 1)
        {
            Projectile.velocity.X *= -1;
        }

        Player telegraphPlayer = Main.LocalPlayer;
        Point tile = telegraphPlayer.Center.ToTileCoordinates();
        tile.Y -= 8;
        tile = TileUtilities.FallToSolidTile(tile);
        Vector2 worldPosition = tile.ToWorldCoordinates();

        if (Timer % 4 == 0)
        {
            Vector2 pos = worldPosition;
            pos.X += Main.rand.NextFloat(-384f, 384f);
            Vector2 vel = -Vector2.UnitY;
            vel *= Main.rand.NextFloat(2f, 15f);
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.VectorScale *= 0.3f;
        }


        if (Timer == 1)
        {
            Projectile.velocity.Y -= 15;
        }
        Projectile.velocity.X *= 0.98f;

        if (Projectile.velocity.Y > 0)
        {
            if (Timer % 8 == 0)
            {
                var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Bottom, -Projectile.velocity);
                p2.Scale *= 1.5f;
            }
            ShakeModSystem.Shake = 2;
            Projectile.velocity.Y *= 1.05f;
        }
        else
        {
            Projectile.velocity.Y += 0.75f;
        }
        Player player = PlayerHelper.FindClosestPlayer(Projectile.position, 2000);
        if (player == null)
            return;

        if (Projectile.Bottom.Y > player.Top.Y)
        {
            Projectile.tileCollide = true;
        }

    }


    private void DrawAfterImage(SpriteBatch sb, Vector2 screenPos)
    {
        SpritebatchDrawer moonSprite = SpritebatchDrawer.FromProjectile(Projectile);
        _scrollingMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_ScrollingMoon");
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            float ratio = i / (float)Projectile.oldPos.Length;
            moonSprite.color = Color.Lerp(Color.Blue, Color.DarkBlue, ratio);
            moonSprite.color *= MathHelper.Lerp(1f, 0f, ratio) * 0.4f;
            moonSprite.worldPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            moonSprite.scale = Vector2.Lerp(Vector2.One, Vector2.One * 0.5f, ratio) * 0.75f;
            Main.spriteBatch.Draw(moonSprite);
        }

    }
    private void DrawPixelatedMoon(SpriteBatch sb, Vector2 screenPos)
    {
        SpritebatchDrawer moonSprite = SpritebatchDrawer.FromProjectile(Projectile);
        _scrollingMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_ScrollingMoon");
        moonSprite = SpritebatchDrawer.FromProjectile(Projectile);
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.3f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.7f;
        glowDrawer.scale *= _squishScale * _targetScale;
        Main.spriteBatch.Draw(glowDrawer);


        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.2f;
        glowDrawer.color.A = 0;
        glowDrawer.scale.X *= 1.2f;
        glowDrawer.scale.Y *= 0.6f;
        glowDrawer.scale *= _squishScale * _targetScale;
        Main.spriteBatch.Draw(glowDrawer);


        ScrollingMoonShader scrollingMoonShader = ScrollingMoonShader.Instance;
        scrollingMoonShader.ScrollingTexture = _scrollingMoonTextureAsset.Value;
        scrollingMoonShader.MaskSize = TextureAssets.Projectile[Type].Value.Size();

        float time = Main.GlobalTimeWrappedHourly * 0.6f * Direction;
        time += Projectile.whoAmI * 0.5f;
        scrollingMoonShader.ScrollOffset = new Vector2(time, 0f);
        scrollingMoonShader.BendStrength = 1.8f;
        scrollingMoonShader.Tiling = new Vector2(0.13f, 0.45f);


        //Draw the moon itself
        sb.Restart(effect: scrollingMoonShader.Effect);
        moonSprite.rotation = MathHelper.ToRadians(-12);
        moonSprite.color = Color.Lerp(Color.White, Color.Black, 0.18f);
        moonSprite.scale *= _squishScale * _targetScale;
        Main.spriteBatch.Draw(moonSprite);
        sb.RestartDefaults();



        Player player = Main.LocalPlayer;
        Point tile = player.Center.ToTileCoordinates();
        tile.Y -= 8;
        tile = TileUtilities.FallToSolidTile(tile);
        Vector2 worldPosition = tile.ToWorldCoordinates();

        if (State == BounceState.Bounce_Out)
        {
            glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.MuzzleFlash, Projectile.Center);
            glowDrawer.color = Color.Yellow * MathHelper.Lerp(0f, 1f, Timer / 120f) * 0.5f;
            glowDrawer.color.A = 0;
            glowDrawer.scale.Y *= 32f;
            glowDrawer.scale.X *= 0.3f;
            glowDrawer.rotation = MathHelper.PiOver2;
            glowDrawer.worldPosition = worldPosition;
            Main.spriteBatch.Draw(glowDrawer);

        }


        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SolarRing, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.6f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.5f;
        glowDrawer.scale *= _squishScale * _targetScale * 1.5f;
        Main.spriteBatch.Draw(glowDrawer);


        moonSprite.color = Color.Lerp(Color.Transparent, Color.White, _flashAlpha);
        Main.spriteBatch.Draw(moonSprite);
    }
    private void DrawPixelatedTrails(GraphicsDevice gDevice)
    {
        BlackFireShader blackFireShader = BlackFireShader.Instance;
        blackFireShader.SetDefaults();
        blackFireShader.InnerColor = Color.White;
        blackFireShader.OuterColor = Color.White;


        blackFireShader.PrimaryTexture = TrailRegistry.BeamTrail;
        blackFireShader.PrimaryTexture2 = TrailRegistry.DottedTrail;
        blackFireShader.BloomTexture = TrailRegistry.VortexTrail;
        blackFireShader.NoiseTexture = TrailRegistry.WhispyTrail;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos,
            GetTrailColor, GetTrailWidth, blackFireShader, Projectile.Size * 0.5f);
    }

    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(64, 0, ratio) * _scale.X * 1.5f;
    }

    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.White, Color.SkyBlue, ratio);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawAfterImage, DrawLayer.BehindTiles);
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrails, DrawLayer.OverNPCs);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedMoon);
        _outlineMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");


        _shadowMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Shadow");
        SpritebatchDrawer shadowDrawer = SpritebatchDrawer.FromTextureAsset(_shadowMoonTextureAsset, Projectile.Center);
        shadowDrawer.color *= 0.58f;
        shadowDrawer.scale *= _squishScale * _targetScale;
        Main.spriteBatch.Draw(shadowDrawer);


        SpritebatchDrawer outlineDrawer = SpritebatchDrawer.FromTextureAsset(_outlineMoonTextureAsset, Projectile.Center);
        outlineDrawer.color = _outlineColor;
        outlineDrawer.scale *= _squishScale * _targetScale;
        Main.spriteBatch.Draw(outlineDrawer);

        return false;
        //return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<VerliaBouncingMoonShockwave>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<VerliaBouncingMoonBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
public class VerliaClone : ModNPC,
    IDrawOutlines
{
    private Color _outlineColor;
    private bool _warning;
    private bool _attacking;

    private Vector2 _startVelocity;
    private float _dir;
    public string RootTexture => ModContent.GetInstance<Verlia>().Texture;
    public override string Texture => TextureRegistry.EmptyTexture;
    private enum AIState
    {
        Idle,
        Moon_Slash_Copy
    }

    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private ref float AttackCycle => ref NPC.ai[2];
    private Animator _animatorBackingField;
    private Animator Animator
    {
        get
        {
            if (_animatorBackingField == null)
            {

                _animatorBackingField = ModContent.GetInstance<Verlia>().CreateAnimator();
                _animatorBackingField.PlayAnimation(Verlia.ANIM_UNSUMMON);
            }
            return _animatorBackingField;
        }
    }
    private Player MyTarget => Main.player[NPC.target];
    private int Moon_Slash_Damage => 50;

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_startVelocity);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _startVelocity = reader.ReadVector2();
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.TrailCacheLength[NPC.type] = 16;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 32;
        NPC.height = 64;
        NPC.damage = 1;
        NPC.defense = 15;
        NPC.dontCountMe = true;
        NPC.dontTakeDamage = true;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.lifeMax = 6750;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.value = Item.buyPrice(gold: 12);
        NPC.npcSlots = 10f;
        NPC.scale = 1f;
        NPC.aiStyle = -1;
    }

    public override void AI()
    {
        base.AI();
        _attacking = false;
        _warning = false;
        switch (State)
        {
            case AIState.Moon_Slash_Copy:
                AI_MoonSlashCopy();
                break;
        }
        if (_attacking)
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Red, 0.1f);
        }
        else if (_warning)
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Yellow, 0.1f);
        }
        else
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Transparent, 0.1f);
        }
    }
    private void FaceTarget()
    {
        NPC.spriteDirection = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
    }
    private void AI_MoonSlashCopy()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        _startVelocity = NPC.velocity;
                        _dir = Main.rand.NextBool(2) ? -1 : 1;
                    }

                    float time = 60;
                    float ratio = Timer / time;
                    float ease = EasingFunction.InOutSine(ratio);
                    Vector2 pos = MyTarget.Center + Vector2.UnitX * _dir * 128;
                    Vector2 targetVelocity = (pos - NPC.Center);
                    NPC.velocity = Vector2.Lerp(_startVelocity, targetVelocity, ease);
                    FaceTarget();
                    Animator.PlayAnimation(Verlia.ANIM_TELEPORTIN);
                    if (Animator.IsFinished() && Timer >= time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    //    CameraTargetSystem.AddTarget(NPC.Center);
                    if (Timer == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordSheethe"), NPC.position);
                        NPC.TargetClosest();
                        _startVelocity = NPC.velocity;

                    }

                    FaceTarget();

                    if (Timer < 90f)
                    {
                        float ratio = Timer / 90f;
                        float ease = EasingFunction.InOutExpo(ratio);
                        Vector2 inverseDir = MyTarget.Center.X > NPC.Center.X ? -Vector2.UnitX : Vector2.UnitX;
                        Vector2 targetPosition = MyTarget.Center + inverseDir * 128;
                        Vector2 targetVelocity = targetPosition - NPC.Center;
                        Vector2 interpolatedVelocity = Vector2.Lerp(_startVelocity, targetVelocity, ease);
                        NPC.velocity = interpolatedVelocity;
                    }
                    else
                    {
                        NPC.velocity *= 0.9f;
                    }


                    _warning = true;
                    Animator.PlayAnimation(Verlia.ANIM_SWORD);
                    if (Animator.IsFinished() && Timer > 140)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    if (Timer == 1)
                    {
                        Vector2 dir2 = MyTarget.Center.X > NPC.Center.X ? Vector2.UnitX : -Vector2.UnitX;
                        NPC.velocity = dir2;
                        if (MultiplayerHelper.IsHost)
                        {
                            float dir = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
                            Vector2 velocity = Vector2.UnitX * dir;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity, ModContent.ProjectileType<MoonSlash>(), Moon_Slash_Damage, 1, Main.myPlayer);
                        }

                        FXUtil.ShakeCamera(NPC.Center, 1024, 16);
                    }
                    if (ModContent.GetInstance<LunarVeilClientConfig>().DramaticEffects)
                    {
                        SpecialEffectsPlayer effectsPlayer = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
                        effectsPlayer.darknessCurve = MathHelper.Lerp(0.75f, 0f, EasingFunction.InExpo(Timer / 30f));
                    }
                    if (NPC.velocity.Length() < 25)
                        NPC.velocity *= 1.5f;

                    _attacking = true;
                    Animator.PlayAnimation(Verlia.ANIM_SWORDSLASH);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                        NPC.active = false;
                    }
                }
                break;
        }
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit)
    {
        base.OnHitNPC(target, hit);
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        Animator.Update();
        NPC.frame.Y = Animator.GetFrameY(184);
        NPC.frame.Height = 184;
        NPC.frame.Width = 266;
    }

    private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        string path = $"{ModContent.GetInstance<Verlia>().Texture}_{Animator.GetAnimation()}";

        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.texture = ModContent.Request<Texture2D>(path).Value;

        drawer.sourceRect = NPC.frame;
        drawer.drawOrigin = Animator.GetDrawOrigin().Value;
        if (NPC.spriteDirection == -1)
            drawer.drawOrigin.X = NPC.frame.Size().X - drawer.drawOrigin.X;
        drawer.color = drawColor * ExtraMath.Osc(0.25f, 0.75f, speed: 12, offset: NPC.whoAmI);
        drawer.worldPosition += screenPos;
        spriteBatch.Draw(drawer);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (!Animator.GetDrawOrigin().HasValue)
            return false;
        DrawSprite(spriteBatch, Vector2.Zero, drawColor);
        return false;
    }

    public override void OnKill()
    {
        base.OnKill();
    }

    public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        Vector2 v = Vector2.UnitX * 2;
        Vector2 h = Vector2.UnitY * 2;

        DrawSprite(spriteBatch, v, _outlineColor);
        DrawSprite(spriteBatch, -v, _outlineColor);
        DrawSprite(spriteBatch, h, _outlineColor);
        DrawSprite(spriteBatch, -h, _outlineColor);
        //      throw new System.NotImplementedException();
    }
}
public class MoonSnipe : ModProjectile
{
    private float _inScale;
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.timeLeft = 120;
        Projectile.penetrate = -1;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            var softSummon = new SoundStyle("Stellamod/Assets/Sounds/SoftSummon");
            softSummon.PitchVariance = 0.5f;
            SoundEngine.PlaySound(softSummon, Projectile.position);
        }
        if (Timer < 30)
        {
            if (Projectile.velocity.Length() > 0.2f)
                Projectile.velocity *= 0.2f;
        }
        else if (Timer == 31)
        {
            var softSummon = new SoundStyle("Stellamod/Assets/Sounds/StarFlower1");
            softSummon.PitchVariance = 0.5f;
            SoundEngine.PlaySound(softSummon, Projectile.position);
            Projectile.velocity *= 10;
        }
        else if (Projectile.velocity.Length() < 25)
        {
            Projectile.velocity *= 1.2f;
            if (Projectile.velocity.Length() > 25)
            {
                LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity * 0.5f);
            }
        }
        else
        {
            Projectile.extraUpdates = 2;
        }
        _inScale = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(Timer / 60f));
    }

    private void DrawTrails(GraphicsDevice gDevice)
    {
        RichLaserShader laserShader = RichLaserShader.Instance;
        laserShader.LaserColor = Color.White;
        laserShader.LaserTexture = TrailRegistry.StarTrail;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, laserShader, Projectile.Size * 0.5f);

        BloomTrailShader b = BloomTrailShader.Instance;
        b.InnerColor = Color.Blue;
        b.OuterColor = Color.DarkBlue;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth2, b, Projectile.Size * 0.5f);
    }
    private float GetTrailWidth2(float ratio)
    {
        return MathHelper.SmoothStep(32, 0, ratio) * _inScale * 2;
    }
    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(32, 0, ratio) * _inScale;
    }
    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.White, Color.Blue, ratio);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawTrails);
        float globalScale = 0.4f * _inScale;
        SpritebatchDrawer flareDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        flareDrawer.color = Color.Blue;
        flareDrawer.color.A = 0;
        flareDrawer.scale *= globalScale;
        flareDrawer.rotation = Main.GlobalTimeWrappedHourly;
        Main.spriteBatch.Draw(flareDrawer);

        flareDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare2, Projectile.Center);
        flareDrawer.color = Color.LightSkyBlue;
        flareDrawer.color.A = 0;
        flareDrawer.scale *= globalScale * 0.8f;
        flareDrawer.rotation = -Main.GlobalTimeWrappedHourly;
        Main.spriteBatch.Draw(flareDrawer);

        flareDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare3, Projectile.Center);
        flareDrawer.color = Color.White;
        flareDrawer.color.A = 0;
        flareDrawer.scale *= globalScale;
        flareDrawer.rotation = Main.GlobalTimeWrappedHourly * 4;
        Main.spriteBatch.Draw(flareDrawer);
        return false;
        //   return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
        fx.Scale *= 1.5f;
        float numDust = 5;
        for (float n = 0; n < numDust; n++)
        {
            Vector2 vel = -Projectile.velocity;
            vel = vel.RotatedByRandom(MathHelper.ToRadians(60));
            vel = vel.SafeNormalize(Vector2.Zero);
            vel *= Main.rand.NextFloat(6, 75);
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.Blue;
            var dp = DustParticle.Spawn(Projectile.Center, vel, spawnParams);
            dp.fast = true;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
            dp.gravity = 0;
        }
        FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
    }
}
public class VerliaDesperationMoonBoom : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
    public override void AI()
    {
        base.AI();
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class VerliaDesperationMoon : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private float _scale;
    private float _flashAlpha;
    private Asset<Texture2D> _outlineTextureAsset;
    private Asset<Texture2D> _scrollingMoonTextureAsset;
    private Asset<Texture2D> _shadowMoonTextureAsset;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        _flashAlpha = 1f;
        Projectile.width = 192;
        Projectile.height = 192;
        Projectile.hostile = true;
        Projectile.timeLeft = 1800;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            SoundStyle inSound = new SoundStyle($"Stellamod/Assets/Sounds/StarCharge");
         //   inSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(inSound, Projectile.position);
        }
        if (Timer >= 60 && Timer < 600)
        {
            int divisor = (int)MathHelper.Lerp(30, 10, EasingFunction.InOutSine(Timer / 400));
            if (Timer % divisor == 0)
            {
                _flashAlpha = 1f;
                if (this.OwnedByLocalClient())
                {
                    Player player = PlayerHelper.FindClosestPlayer(Projectile.Center, 4000);
                    if (player != null)
                    {
                        Vector2 velocity = player.Center - Projectile.Center;
                        velocity = velocity.SafeNormalize(Vector2.Zero);

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + velocity * 192, velocity * 15,
                            ModContent.ProjectileType<MoonSnipe>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }

                }
            }
        }
        else if (Timer > 600)
        {
            CameraTargetSystem.AddTarget(Projectile.Center);
            ShakeModSystem.Shake = 2;
            Projectile.tileCollide = true;
            if (Projectile.velocity.Y < 5)
            {
                Projectile.velocity.Y += 0.2f;
            }
            _flashAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine((Timer - 600f) / 60f));
        }
        _scale = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 60f));
        _flashAlpha = MathHelper.Lerp(_flashAlpha, 0f, 0.1f);
    }
    private void DrawPixelatedMoon(SpriteBatch sb, Vector2 screenPos)
    {
        Vector2 scale = Vector2.One * _scale;
        SpritebatchDrawer moonSprite = SpritebatchDrawer.FromProjectile(Projectile);
        _scrollingMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_ScrollingMoon");
        moonSprite = SpritebatchDrawer.FromProjectile(Projectile);
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.Blue * 0.8f * ExtraMath.Osc(0.5f, 1f, speed: 6);
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 1.8f;
        glowDrawer.scale *= scale;
        Main.spriteBatch.Draw(glowDrawer);



        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.2f;
        glowDrawer.color.A = 0;
        glowDrawer.scale.X *= 1.2f;
        glowDrawer.scale.Y *= 0.6f;
        glowDrawer.scale *= scale;
        Main.spriteBatch.Draw(glowDrawer);


        ScrollingMoonShader scrollingMoonShader = ScrollingMoonShader.Instance;
        scrollingMoonShader.ScrollingTexture = _scrollingMoonTextureAsset.Value;
        scrollingMoonShader.MaskSize = TextureAssets.Projectile[Type].Value.Size();

        float time = Main.GlobalTimeWrappedHourly * 0.6f * 1;
        time += Projectile.whoAmI * 0.5f;
        scrollingMoonShader.ScrollOffset = new Vector2(time, 0f);
        scrollingMoonShader.BendStrength = 1.8f;
        scrollingMoonShader.Tiling = new Vector2(0.13f, 0.45f);


        //Draw the moon itself
        sb.Restart(effect: scrollingMoonShader.Effect);
        moonSprite.rotation = MathHelper.ToRadians(-12);
        moonSprite.color = Color.Lerp(Color.White, Color.LightSkyBlue, ExtraMath.Osc(0f, 0.3f, speed: 8));
        moonSprite.scale *= scale;
        Main.spriteBatch.Draw(moonSprite);
        sb.RestartDefaults();


        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SolarRing, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.6f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.5f;
        glowDrawer.scale *= scale * 3f;
        Main.spriteBatch.Draw(glowDrawer);


        moonSprite.color = Color.Lerp(Color.Transparent, Color.White, _flashAlpha);
        Main.spriteBatch.Draw(moonSprite);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Vector2 scale = Vector2.One * _scale;
        _outlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");
        _scrollingMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_ScrollingMoon");
        _shadowMoonTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Shadow");

        SpritebatchDrawer shadowDrawer = SpritebatchDrawer.FromTextureAsset(_shadowMoonTextureAsset, Projectile.Center);
        shadowDrawer.color *= 0.45f;
        Main.spriteBatch.Draw(shadowDrawer);

        SpritebatchDrawer outlineDrawer = SpritebatchDrawer.FromTextureAsset(_outlineTextureAsset, Projectile.Center);
        outlineDrawer.color = Color.Red;
        outlineDrawer.scale *= scale;
        Main.spriteBatch.Draw(outlineDrawer);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedMoon);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            float numBlades = 12;
            for (float f = 0; f < numBlades; f++)
            {
                float ratio = f / numBlades;
                Vector2 vel = (ratio * MathHelper.TwoPi).ToRotationVector2();
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + vel * 128, vel * 15, ModContent.ProjectileType<MoonBlade>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<VerliaBouncingMoonShockwave>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<VerliaBouncingMoonBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
        fx.Scale *= 8f;
        float numDust = 32;
        for (float f = 0; f < numDust; f++)
        {
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.Blue;
            spawnParams.scaleRange *= 2;
            var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(16, 16), spawnParams);
            dp.fast = true;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
        }
    }
}

public class MoonBlade : ModProjectile
{
    private bool _lodged;
    private float _randScale;
    private float _rotOffset;
    private Vector2 _pullOffset;
    private Vector2 _startPullOffset;
    private Vector2 _scale;
    private Vector2 _outScale;
    private Vector2 _initialVelocity;
    private Asset<Texture2D> _outlineTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_pullOffset);
        writer.WriteVector2(_startPullOffset);
        writer.WriteVector2(_initialVelocity);
        writer.WriteVector2(_scale);
        writer.Write(_lodged);
        writer.Write(_randScale);
        writer.Write(_rotOffset);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _pullOffset = reader.ReadVector2();
        _startPullOffset = reader.ReadVector2();
        _initialVelocity = reader.ReadVector2();
        _scale = reader.ReadVector2();
        _lodged = reader.ReadBoolean();
        _randScale = reader.ReadSingle();
        _rotOffset = reader.ReadSingle();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.hostile = true;
        Projectile.timeLeft = 600;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (!_lodged)
        {
            float numDust = 8;
            for (float n = 0; n < numDust; n++)
            {
                Vector2 vel = -oldVelocity;
                vel = vel.RotatedByRandom(MathHelper.ToRadians(60));
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(6, 12f);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.Blue;
                var dp = DustParticle.Spawn(Projectile.Center, vel, spawnParams);
                dp.fast = true;
                dp.noTileCollide = true;
                dp.dampening = 0.05f;
            }
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            _lodged = true;
        }

        return false;
    }
    public override void AI()
    {
        base.AI();
        _outScale = Vector2.Lerp(Vector2.Zero, Vector2.One, EasingFunction.InOutSine(Projectile.timeLeft / 30f));
        if (_lodged)
        {
            Projectile.extraUpdates = 0;
            Projectile.velocity *= 0f;
            return;
        }
        Timer++;
        if (Timer == 1)
        {
            SoundStyle inSound = new SoundStyle($"Stellamod/Assets/Sounds/SoftSummon2");
            inSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(inSound, Projectile.position);
            if (this.OwnedByLocalClient())
            {
                float dist = Main.rand.NextFloat(140, 400);
                float dir = Main.rand.NextBool(2) ? -1 : 1;
                _startPullOffset = Vector2.UnitX * dist * dir;//Vector2.Lerp(-Vector2.UnitX * 128, Vector2.UnitX * 128, Main.rand.NextFloat(0f,
                _randScale = Main.rand.NextFloat(0.66f, 1.5f);
                Projectile.netUpdate = true;
            }
            _initialVelocity = Projectile.velocity;
            //MoonSpiralParticle.Spawn(Projectile.Center, Vector2.Zero);
        }

        if (Timer < 70f)
        {
            _pullOffset = Vector2.Lerp(_startPullOffset, Vector2.Zero, EasingFunction.InOutSine(Timer / 70f));
            _rotOffset = MathHelper.Lerp(MathHelper.TwoPi * 2, 0, EasingFunction.OutExpo(Timer / 70f));
            if (Projectile.velocity.Length() > 0.2f)
                Projectile.velocity *= 0.2f;
        }
        else if (Timer == 71)
        {
            Projectile.velocity = _initialVelocity * 0.5f;
            SoundStyle outSound = new SoundStyle($"Stellamod/Assets/Sounds/StarFlower1");
            outSound.PitchVariance = 0.3f;
            outSound.Volume = 0.3f;
            SoundEngine.PlaySound(outSound, Projectile.position);
        }
        else
        {
            if (Projectile.velocity.Length() < _initialVelocity.Length())
            {
                Projectile.velocity *= 1.1f;
                if (Projectile.velocity.Length() >= _initialVelocity.Length())
                {
                    LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity);
                }
            }
            else
            {
                if (Timer % 24 == 0)
                {
                    var sp = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2));
                    sp.Scale *= Main.rand.NextFloat(0.125f, 0.25f);
                    sp.behindLayer = true;
                    sp.noShrink = true;
                    sp.fadeToColor = Color.Black;
                    sp.color = Color.Lerp(Color.Blue, Color.Black, 0.75f);

                }
                Projectile.extraUpdates = 2;
            }
        }
        float ratio = Timer / 60f;
        float ease = EasingFunction.OutExpo(ratio);
        _scale = Vector2.Lerp(Vector2.Zero, new Vector2(1f, 0.46f), ease);
        Projectile.rotation = Projectile.velocity.ToRotation() + _rotOffset;
    }
    private void DrawPixelatedSwords(SpriteBatch sb, Vector2 screenPos)
    {
        _outlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.color = Color.Lerp(Color.Blue, Color.DarkBlue, ExtraMath.Osc(0f, 1f, 0, Projectile.whoAmI));
        sbDrawer.color.A = 0;
        sbDrawer.scale *= _scale * _outScale * _randScale;
        sbDrawer.worldPosition += _pullOffset;
        Main.spriteBatch.Draw(sbDrawer);
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            sbDrawer.worldPosition = pos;
            float ratio = i / (float)Projectile.oldPos.Length;
            sbDrawer.color = Color.Lerp(Color.Blue, Color.DarkBlue, ratio);
            sbDrawer.color *= MathHelper.SmoothStep(1f, 0f, EasingFunction.OutExpo(ratio));
            sbDrawer.color.A = 0;
            //   sbDrawer.scale *= _scale;
            sbDrawer.worldPosition += _pullOffset;
            Main.spriteBatch.Draw(sbDrawer);
        }
        sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.color = Color.White * ExtraMath.Osc(0.35f, 0.6f, speed: 6, Projectile.whoAmI);
        sbDrawer.color.A = 0;
        sbDrawer.scale *= _scale * _outScale * _randScale;
        sbDrawer.scale *= 0.9f;
        sbDrawer.worldPosition += _pullOffset;
        Main.spriteBatch.Draw(sbDrawer);

        sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.scale *= _scale * _outScale * _randScale;
        sbDrawer.color = Color.White * ExtraMath.Osc(0f, 1f, speed: 12, offset: Projectile.whoAmI);
        sbDrawer.color.A = 0;
        sbDrawer.texture = _outlineTextureAsset.Value;
        sbDrawer.worldPosition += _pullOffset;
        Main.spriteBatch.Draw(sbDrawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedSwords);

        //Main.spriteBatch.Draw(sbDrawer);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

#region Blade Dance
public class MoonShot : ModProjectile
{
    private float _inScale;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Style => ref Projectile.ai[1];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 128;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.ignoreWater = true;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 480;
        Projectile.tileCollide = false;
        Projectile.extraUpdates = 1;
    }

    public override void AI()
    {
        base.AI();
        //   ProjectileID.Sets.TrailCacheLength[Type] = 128;
        Timer++;
        if (Timer == 1)
        {
            float numDust = 12;
            for (float n = 0; n < numDust; n++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(8, 8);
                DustParticleSpawnParams spawnparams = DustParticleSpawnParams.Default;
                spawnparams.outerColor = Color.Blue;
                var d = DustParticle.Spawn(Projectile.Center, vel, spawnparams);
                d.dampening = 0.05f;
                d.gravity = 0;
                d.fast = true;
                d.noTileCollide = true;
            }
            var boom = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
            boom.Scale *= 2f;
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/FrostShot{Style}"), Projectile.position);
        }
        if (Timer % 18 == 0)
        {
            var sp = CrescentMoonParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2));
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
            sp.gravity = 0;
        }

        if (Timer % 14 == 0)
        {
            var sp = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2));
            sp.Scale *= Main.rand.NextFloat(0.125f, 0.25f);
            sp.behindLayer = true;
            sp.noShrink = true;
            sp.fadeToColor = Color.Black;
            sp.color = Color.Lerp(Color.Blue, Color.Black, 0.75f);

        }
        float targetScale = 1f * EasingFunction.InOutSine(Projectile.timeLeft / 30f);
        _inScale = MathHelper.Lerp(_inScale, targetScale, 0.1f);
        Player player = PlayerHelper.FindClosestPlayer(Projectile.Center, 1024);
        if (player != null)
        {
            Vector2 targetVelocity = (player.Center - Projectile.Center);
            targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
            targetVelocity *= MathHelper.Lerp(12f, 25f, EasingFunction.InExpo(Timer / 180f));
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.01f);
        }
    }
    private void DrawTrails(GraphicsDevice gDevice)
    {
        RichLaserShader laserShader = RichLaserShader.Instance;
        laserShader.LaserColor = Color.White;
        laserShader.LaserTexture = TrailRegistry.StarTrail;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, laserShader, Projectile.Size * 0.5f);

        BloomTrailShader b = BloomTrailShader.Instance;
        b.InnerColor = Color.Blue;
        b.OuterColor = Color.DarkBlue;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth2, b, Projectile.Size * 0.5f);
    }
    private float GetTrailWidth2(float ratio)
    {
        return MathHelper.SmoothStep(32, 0, ratio) * _inScale * 2;
    }
    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(32, 0, ratio) * _inScale;
    }
    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.White, Color.Blue, ratio);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawTrails);
        float globalScale = 0.4f * _inScale;
        SpritebatchDrawer flareDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        flareDrawer.color = Color.Blue;
        flareDrawer.color.A = 0;
        flareDrawer.scale *= globalScale;
        flareDrawer.rotation = Main.GlobalTimeWrappedHourly;
        Main.spriteBatch.Draw(flareDrawer);

        flareDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare2, Projectile.Center);
        flareDrawer.color = Color.LightSkyBlue;
        flareDrawer.color.A = 0;
        flareDrawer.scale *= globalScale * 0.8f;
        flareDrawer.rotation = -Main.GlobalTimeWrappedHourly;
        Main.spriteBatch.Draw(flareDrawer);

        flareDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare3, Projectile.Center);
        flareDrawer.color = Color.White;
        flareDrawer.color.A = 0;
        flareDrawer.scale *= globalScale;
        flareDrawer.rotation = Main.GlobalTimeWrappedHourly * 4;
        Main.spriteBatch.Draw(flareDrawer);
        return false;
        //   return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class MoonSlash : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 7;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 256;
        Projectile.height = 256;
        Projectile.hostile = true;
        Projectile.timeLeft = 24;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {


            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Hyuh"), Projectile.position);
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordSlice"), Projectile.position);
        }
        Projectile.spriteDirection = Projectile.velocity.X < 0 ? 1 : -1;
        if (++Projectile.frameCounter >= 2)
        {
            Projectile.frameCounter = 0;
            if (++Projectile.frame >= 7)
            {
                //Projectile.frame = 0;
            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.color.A = 0;
        Main.spriteBatch.Draw(sbDrawer);
        Main.spriteBatch.Draw(sbDrawer);
        return false;
        //  return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class MoonSlashHold : ModProjectile
{
    private NPC Parent => Main.npc[(int)Projectile.ai[0]];
    private ref float Timer => ref Projectile.ai[1];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 10;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hostile = true;
        Projectile.width = 300;
        Projectile.height = 300;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 40;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Moaning"), Projectile.position);
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordHoldVerlia"), Projectile.position);
        }
        Projectile.Center = Parent.Center;
        if (++Projectile.frameCounter >= 1)
        {
            Projectile.frameCounter = 0;
            if (++Projectile.frame >= 10)
            {
                Projectile.frame = 0;
            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.color.A = 0;
        Main.spriteBatch.Draw(sbDrawer);
        Main.spriteBatch.Draw(sbDrawer);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
#endregion


public class Verlia : ScarletBoss,
    IDrawOutlines
{
    private enum AIState
    {
        Spawn,
        Idle,

        //Phase 1
        Two_Bouncing_Moons,
        Blue_Magic_Sword,
        Spining_Little_Moons,
        Blade_Dance,
        Clone_Summon,

        //Phase 2
        Phase_2_Transition,
        Desperation_Big_Moon,
        Sword_Fall,
        Running,
        Dash_Slash,
        Blade_Dance_V2,

        Death
    }

    private Asset<Texture2D> _wingTextureAsset;
    private Asset<Texture2D> _wingOutlineTextureAsset;

    private Color _outlineColor;
    private bool _warning;
    private bool _attacking;
    private bool _showTrail;
    private bool _bladeDanceV2;
    private bool _magicSwordV2;
    private bool _contactDamage;
    private bool _showWings;
    private bool _showMagicCircle;
    private float _magicCircleAlpha;
    private float _trailAlpha;

    private bool _phase2;
    private float _wingScale;
    private Vector2 _startVelocity;
    private Vector2 _runningVelocity;
    private Vector2 _teleportPosition;
    private PatternManager<AIState> _patternManager;
    private PatternManager<AIState> PatternManager
    {
        get
        {
            _patternManager ??= new PatternManager<AIState>(
                new Tuple<AIState, float>(AIState.Two_Bouncing_Moons, 1.0f),
                new Tuple<AIState, float>(AIState.Spining_Little_Moons, 1.0f),
                new Tuple<AIState, float>(AIState.Blue_Magic_Sword, 1.0f),
                new Tuple<AIState, float>(AIState.Clone_Summon, 1.0f),
                new Tuple<AIState, float>(AIState.Blade_Dance, 1.0f),
                new Tuple<AIState, float>(AIState.Blade_Dance_V2, 1.0f),
                new Tuple<AIState, float>(AIState.Desperation_Big_Moon, 1.0f),
                new Tuple<AIState, float>(AIState.Running, 1.0f),
                new Tuple<AIState, float>(AIState.Sword_Fall, 1.0f));
            return _patternManager;
        }
    }


    private Animator _animatorBackingField;
    private Animator Animator
    {
        get
        {
            if (_animatorBackingField == null)
            {
                _animatorBackingField = CreateAnimator();
                _animatorBackingField.PlayAnimation(ANIM_SUMMON);
            }

            return _animatorBackingField;
        }
    }
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    private ref float AttackCycle => ref NPC.ai[2];
    private ref float AttackCounter => ref NPC.ai[3];

    private int Twin_Bouncing_Moon_Damage => 50;

    private int Mini_Moon_Damage => 20;
    private int Moon_Slash_Damage => 35;
    private int Moon_Shot_Damage => 20;
    private int Moon_Blade_damage = 30;
    private int Great_Blade_Damage => 50;
    private int Desperation_Moon_Damage => 50;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_teleportPosition);
        writer.WriteVector2(_startVelocity);
        writer.WriteVector2(_runningVelocity);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _teleportPosition = reader.ReadVector2();
        _startVelocity = reader.ReadVector2();
        _runningVelocity = reader.ReadVector2();
    }

    #region Defaults
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.TrailCacheLength[NPC.type] = 16;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 32;
        NPC.height = 64;
        NPC.damage = 50;
        NPC.defense = 15;
        NPC.lifeMax = 6750;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.value = Item.buyPrice(gold: 12);
        NPC.npcSlots = 10f;
        NPC.scale = 1f;
        NPC.aiStyle = -1;

        // The following code assigns a music track to the boss in a simple way.
        if (!Main.dedServ)
        {
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/VerliaOfTheMoon");
        }
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        // Sets the description of this NPC that is listed in the bestiary
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Verlia, The Empress of the Stars and moon, Vixyl's sister and a master magic swordswoman."))
            });
    }

    #endregion

    public override void AI()
    {
        base.AI();
        _showTrail = false;
        _attacking = false;
        _warning = false;
        _bladeDanceV2 = false;
        _contactDamage = false;
        _showWings = false;
        _showMagicCircle = false;
        if (_teleportPosition != Vector2.Zero)
        {
            NPC.Center = _teleportPosition;
            _teleportPosition = Vector2.Zero;
        }
        switch (State)
        {
            case AIState.Spawn:
                AI_Spawn();
                break;

            case AIState.Idle:
                AI_Idle();
                break;

            case AIState.Two_Bouncing_Moons:
                AI_TwoBouncingMoons();
                _showMagicCircle = true;
                break;

            case AIState.Spining_Little_Moons:
                AI_SpinningLittleMoons();
                _showMagicCircle = true;
                break;

            case AIState.Blade_Dance:
                AI_BladeDance();
                _showMagicCircle = true;
                break;

            case AIState.Blue_Magic_Sword:
                AI_BlueMagicSword();
                _showMagicCircle = true;
                break;

            case AIState.Blade_Dance_V2:
                _bladeDanceV2 = true;
                AI_BladeDance();
                break;

            case AIState.Clone_Summon:
                AI_CloneSummon();
                _showMagicCircle = true;
                break;

            case AIState.Running:
                AI_Running();
                break;

            case AIState.Sword_Fall:
                AI_SwordFall();
                _showMagicCircle = true;
                break;

            case AIState.Desperation_Big_Moon:
                AI_DesperationBigMoon();
                _showMagicCircle = true;
                break;
        }
        if (State != AIState.Idle && Animator.GetAnimation() != "Explode") 
            _showWings = true;


        float targetMagicCircleAlpha = _showMagicCircle ? 1f : 0f;
        _magicCircleAlpha = MathHelper.Lerp(_magicCircleAlpha, targetMagicCircleAlpha, 0.1f);

        float targetWingScale = _showWings ? 1f : 0f;
        _wingScale = MathHelper.Lerp(_wingScale, targetWingScale, 0.1f);
        _trailAlpha = MathHelper.Lerp(_trailAlpha, _showTrail ? 1f : 0f, 0.1f);
        if (_attacking)
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Red, 0.1f);
        }
        else if (_warning)
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Yellow, 0.1f);
        }
        else
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Transparent, 0.1f);
        }
    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            AttackCounter = 0;
            AttackCycle = 0;
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }

    private bool IsBlacklisted(AIState state)
    {
        if (_phase2)
        {
            switch (state)
            {
                case AIState.Blade_Dance:
                case AIState.Clone_Summon:
                    return true;
            }
        }
        else
        {
            switch (state)
            {
                case AIState.Desperation_Big_Moon:
                case AIState.Blade_Dance_V2:
                case AIState.Sword_Fall:
                    return true;
            }
        }
        return false;
    }
    private void ChooseAttack()
    {
        if (MultiplayerHelper.IsHost)
        {
            AIState state;
            if (!_phase2 && NPC.life < NPC.lifeMax * 0.5f)
            {
                state = AIState.Desperation_Big_Moon;
            }
            else
            {
                state = PatternManager.NextPattern();
                while (IsBlacklisted(state))
                    state = PatternManager.NextPattern();
            }
            SwitchState(state);
        }


        SwitchState(AIState.Blue_Magic_Sword);
    }

    private void Teleport(Vector2 pos)
    {
        if (MultiplayerHelper.IsHost)
        {
            _teleportPosition = pos;
            NPC.netUpdate = true;
        }
    }

    private void FaceTarget()
    {
        NPC.spriteDirection = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
    }

    private void TeleportsBehindYou(float dist = 64)
    {
        if (Timer == 1)
        {
            SoundStyle inSound = new SoundStyle($"Stellamod/Assets/Sounds/VDisappear");
            inSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(inSound, NPC.position);
            var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
            fx.Scale *= 1.8f;
            NPC.TargetClosest();
            _startVelocity = NPC.velocity;
            float dir = Main.rand.NextBool(2) ? -1 : 1;
            Teleport(MyTarget.Center + dir * Vector2.UnitX * dist);
        }

        NPC.velocity = Vector2.Zero;
        FaceTarget();
        Animator.PlayAnimation(ANIM_TELEPORTIN);
        if (Animator.IsFinished())
        {
            Timer = 0;
            AttackCycle++;
        }
    }

    private void TeleportsAboveYou()
    {
        if (Timer == 1)
        {
            SoundStyle inSound = new SoundStyle($"Stellamod/Assets/Sounds/VDisappear");
            inSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(inSound, NPC.position);
            var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
            fx.Scale *= 1.8f;
            NPC.TargetClosest();
            _startVelocity = NPC.velocity;
            float dir = Main.rand.NextBool(2) ? -1 : 1;
            Teleport(MyTarget.Center + -Vector2.UnitY * 100);
        }

        NPC.velocity = Vector2.Zero;
        FaceTarget();
        Animator.PlayAnimation(ANIM_TELEPORTIN);
        if (Animator.IsFinished())
        {
            Timer = 0;
            AttackCycle++;
        }
    }

    private void TeleportAbovePlayer(float height)
    {
        NPC.TargetClosest();
        Vector2 playerPosition = MyTarget.Center;// + -Vector2.UnitY * 384;
        Point tile = playerPosition.ToTileCoordinates();
        tile = TileUtilities.FallToSolidTile(tile);
        Vector2 bottomWorld = tile.ToWorldCoordinates();
        bottomWorld.Y -= height;
        Teleport(bottomWorld);
    }

    private void TeleportsFarAboveYou()
    {
        if (Timer == 1)
        {
            SoundStyle inSound = new SoundStyle($"Stellamod/Assets/Sounds/VDisappear");
            inSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(inSound, NPC.position);
         
            _startVelocity = NPC.velocity;
            float dir = Main.rand.NextBool(2) ? -1 : 1;
            TeleportAbovePlayer(144);

           
        }
        if(Timer == 5)
        {
            var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
            fx.Scale *= 1.8f;
        }

        NPC.velocity = Vector2.Zero;
        FaceTarget();
        Animator.PlayAnimation(ANIM_TELEPORTIN);
        if (Animator.IsFinished())
        {
            Timer = 0;
            AttackCycle++;
        }
    }


    private void AI_DesperationBigMoon()
    {
        _phase2 = true;
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    _warning = true;
                    TeleportsFarAboveYou();
                }
                break;
            case 1:
                {
                    NPC.velocity *= 0.9f;
                    CameraTargetSystem.AddTarget(NPC.Center);
                    Animator.PlayAnimation(ANIM_HOLDUP);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    NPC.velocity *= 0.9f;
                    if (Timer == 1)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, NPC.Center + -Vector2.UnitY * 100, Vector2.Zero, ModContent.ProjectileType<VerliaDesperationMoon>(), Desperation_Moon_Damage, 1, Main.myPlayer);
                        }
                    }
                    Animator.PlayAnimation(ANIM_PULSE);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    NPC.velocity *= 0.9f;
                    Animator.PlayAnimation(ANIM_IDLESUMMON);
                    if (Timer >= 800)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {
                    ExplodeOut();
                }
                break;
        }
    }
    private void AI_SwordFall()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    _warning = true;
                    TeleportsAboveYou();
                }
                break;
            case 1:
                {
                    FaceTarget();
                    _warning = true;
                    if (Timer % 4 == 0)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 pos = NPC.Center;
                            pos.X += Main.rand.NextFloat(-256, 256);
                            pos.Y -= 256;
                            Vector2 vel = (NPC.Center - pos);
                            vel = vel.SafeNormalize(Vector2.Zero);
                            vel *= 15;
                            Projectile.NewProjectile(SourceFromThis, pos, vel, ModContent.ProjectileType<MoonBlade>(), Moon_Blade_damage, 1, Main.myPlayer);
                        }
                    }
                    Animator.PlayAnimation(ANIM_HOLDUP);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    ExplodeOut();
                }
                break;
        }
    }

    private void ExplodeOut()
    {
        if(Timer == 1)
        {
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/VTeleportOut"), NPC.position);
        }
        if (Timer == 1)
        {

            var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.LightSkyBlue, Color.DarkBlue);
            fx.Scale *= 2f;

            for(float f = 0; f < 4f; f++)
            {
                var sp = FaintSmokeParticle.SpawnInAlphaLayer(NPC.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2));
                sp.Scale *= Main.rand.NextFloat(0.5f, 0.75f);
          //      sp.behindLayer = true;
                sp.noShrink = true;
                sp.fadeToColor = Color.Black;
                sp.color = Color.Lerp(Color.Blue, Color.Black, 0.75f);
            }
 
        }


        NPC.velocity *= 0.9f;
        Animator.PlayAnimation(ANIM_EXPLODE);
        if (Animator.IsFinished())
        {
            SwitchState(AIState.Idle);
        }
    }

    private void AI_Running()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    _warning = true;
                    TeleportsBehindYou(256);
                }
                break;
            case 1:
                {
                    if(Timer == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordSheethe"), NPC.position);
                    }
                    FaceTarget();
                    _warning = true;
                    Animator.PlayAnimation(ANIM_READYDASH);
                    if (Animator.IsFinished() && Timer >= 90f)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    _contactDamage = true;
                    if (Timer == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Moaning"), NPC.position);
                        _startVelocity = NPC.velocity;
                        _runningVelocity = MyTarget.Center.X > NPC.Center.X ? Vector2.UnitX : -Vector2.UnitX;
                        _runningVelocity *= 20;
                        FaceTarget();
                    }
                    DustTrailEffects();

                    _runningVelocity *= 1.02f;
                    float time = 30;
                    float ratio = Timer / time;
                    float ease = EasingFunction.InOutSine(ratio);
                    NPC.velocity = Vector2.Lerp(_startVelocity, _runningVelocity, ease);
                    _attacking = true;
                    _showTrail = true;
                    Animator.PlayAnimation(ANIM_RUN);
                    if (Timer >= 45)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {

                    if (AttackCounter < 7)
                    {
                        var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
                        fx.Scale *= 2;
                        float numDust = 16;
                        for (float n = 0; n < numDust; n++)
                        {
                            Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                            spawnParams.outerColor = Color.Blue;
                            var dp = DustParticle.Spawn(NPC.Center, vel, spawnParams);
                            dp.gravity = 0;
                            dp.dampening = 0.05f;
                            dp.noTileCollide = true;
                        }

                        NPC.velocity *= 0.9f;
                        if (MultiplayerHelper.IsHost)
                        {
                            _startVelocity = NPC.velocity;
                            float dir = _runningVelocity.X > 0 ? 1 : -1;
                            Teleport(MyTarget.Center + dir * Vector2.UnitX * 256);
                        }

                        Timer = 0;
                        AttackCycle--;
                        AttackCounter++;
                    }
                    else
                    {
                        ExplodeOut();
                    }


                }
                break;

        }
    }

    private void AI_CloneSummon()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    _warning = true;
                    TeleportsAboveYou();
                }
                break;
            case 1:
                {
                    if(Timer == 1)
                    {
                        _startVelocity = NPC.velocity;
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SoftSummon"), NPC.position);
                    }
                    _warning = true;
                    if (Timer % 18 == 0)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 spawnPos = NPC.Center;
                            NPC.NewNPC(SourceFromThis, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<VerliaClone>(), ai1: 1);
                            // Vector2 forwardVector = MyTarget.Center.X > NPC.Center.X ? Vector2.UnitX : -Vector2.UnitX;
                            // Projectile.NewProjectile(SourceFromThis, NPC.Center, forwardVector, ModContent.ProjectileType<VerliaGreatBlade>(), Great_Blade_Damage, 1, Main.myPlayer, ai2: NPC.whoAmI);
                        }
                    }

                    Vector2 targetPosition = MyTarget.Center - Vector2.UnitY * 80;
                    Vector2 targetVelocity = targetPosition - NPC.Center;
                    float time = 60f;
                    float ratio = Timer / time;
                    float ease = EasingFunction.InOutSine(ratio);
                    NPC.velocity = Vector2.Lerp(_startVelocity, targetVelocity, ease);
                   // NPC.velocity *= 0.9f;
                    Animator.PlayAnimation(ANIM_PULSE);
                    if (Animator.IsFinished() && Timer >= 100)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    ExplodeOut();
                }
                break;
        }
    }
    private void DustTrailEffects()
    {
        if (Timer % 12 == 0)
        {
            var sp = MoonSpiralParticle.Spawn(NPC.Center, Main.rand.NextVector2Circular(2, 2));
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
        }
        if (Timer % 6 == 0)
        {
            var sp = SparkleParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2));
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
            sp.flickering = true;
            sp.gravity = 0;
        }
        if (Timer % 4 == 0)
        {
            var sp = FaintSmokeParticle.SpawnInAlphaLayer(NPC.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2));
            sp.Scale *= Main.rand.NextFloat(0.5f, 0.75f);
            sp.behindLayer = true;
            sp.noShrink = true;
            sp.fadeToColor = Color.Black;
            sp.color = Color.Lerp(Color.Blue, Color.Black, 0.75f);
        }
    }
    private void AI_BlueMagicSword()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {

                    if (_magicSwordV2)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                    else
                    {
                        _warning = true;
                        TeleportsFarAboveYou();
                    }
                
                }
                break;
            case 1:
                {
                    //CameraTargetSystem.AddTarget(NPC.Center);
                    _warning = true;
                    if (Timer == 1)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 forwardVector = MyTarget.Center.X > NPC.Center.X ? Vector2.UnitX : -Vector2.UnitX;
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, forwardVector, ModContent.ProjectileType<VerliaGreatBlade>(), Great_Blade_Damage, 1, Main.myPlayer, ai2: NPC.whoAmI);
                        }
                    }
                    NPC.velocity *= 0.9f;
                    Animator.PlayAnimation(ANIM_SWORD);
                    if (Animator.IsFinished() && Timer >= 100)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    CameraTargetSystem.AddTarget(NPC.Center);
                    if (Timer == 1)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            float dir = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
                            Vector2 velocity = Vector2.UnitX * dir;
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity, ModContent.ProjectileType<MoonSlash>(), Moon_Slash_Damage, 1, Main.myPlayer);
                        }
                    }
                    _attacking = true;
                    NPC.velocity *= 0.9f;
                    Animator.PlayAnimation(ANIM_SWORDSLASH);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    NPC.velocity *= 0.9f;
                    Animator.PlayAnimation(ANIM_EXPLODE);
                    if (Animator.IsFinished())
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }
    private void AI_BladeDance()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    TeleportsBehindYou();
                }
                break;
            case 1:
                {
                    CameraTargetSystem.AddTarget(NPC.Center);
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        _startVelocity = NPC.velocity;
                        SoundStyle inSound = new SoundStyle($"Stellamod/Assets/Sounds/Moaning");
                        inSound.PitchVariance = 0.3f;
                        SoundEngine.PlaySound(inSound, NPC.position);
                    }

                    FaceTarget();


                    float ratio = Timer / 90f;
                    float ease = EasingFunction.InOutExpo(ratio);
                    Vector2 inverseDir = MyTarget.Center.X > NPC.Center.X ? -Vector2.UnitX : Vector2.UnitX;
                    Vector2 targetPosition = MyTarget.Center + inverseDir * 128;
                    Vector2 targetVelocity = targetPosition - NPC.Center;
                    Vector2 interpolatedVelocity = Vector2.Lerp(_startVelocity, targetVelocity, ease);
                    NPC.velocity = interpolatedVelocity;

                    _warning = true;
                    Animator.PlayAnimation(ANIM_SWORD);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    if (Timer == 1)
                    {
                        Vector2 dir2 = MyTarget.Center.X > NPC.Center.X ? Vector2.UnitX : -Vector2.UnitX;
                        NPC.velocity = dir2;
                        if (MultiplayerHelper.IsHost)
                        {
                            float dir = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
                            Vector2 velocity = Vector2.UnitX * dir;
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity, ModContent.ProjectileType<MoonSlash>(), Moon_Slash_Damage, 1, Main.myPlayer);
                        }

                        FXUtil.ShakeCamera(NPC.Center, 1024, 16);
                    }
                    if (ModContent.GetInstance<LunarVeilClientConfig>().DramaticEffects)
                    {
                        SpecialEffectsPlayer effectsPlayer = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
                        effectsPlayer.darknessCurve = MathHelper.Lerp(0.75f, 0f, EasingFunction.InExpo(Timer / 30f));
                    }
                    if(Timer < 15)
                    {
                        if (NPC.velocity.Length() < 25)
                            NPC.velocity *= 1.5f;
                    }
                    else
                    {
                        NPC.velocity *= 0.9f;
                    }
         

                    _attacking = true;
                    Animator.PlayAnimation(ANIM_SWORDSLASH);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {

                    Vector2 targetVelocity = (MyTarget.Center - NPC.Center);
                    targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
                    float speed = 14;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity * speed, 0.5f);
                    FaceTarget();
                    if (Timer == 1)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            float dir = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
                            Vector2 velocity = Vector2.UnitX * dir;
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity,
                                ModContent.ProjectileType<MoonSlashHold>(), Moon_Slash_Damage, 1, Main.myPlayer, ai0: NPC.whoAmI);
                        }
                    }
                    if (Timer % 3 == 0)
                    {
                        var sp = DustParticle.Spawn(NPC.Center, Main.rand.NextVector2Circular(15, 15));
                        sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
                        sp.outerColor = Color.Blue;
                        sp.fast = true;
                        sp.gravity = 0;
                        sp.dampening = 0.05f;
                    }
                    if (Timer % 12 == 0)
                    {
                        var sp = MoonSpiralParticle.Spawn(NPC.Center, Main.rand.NextVector2Circular(2, 2));
                        sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
                    }
                    if (Timer % 6 == 0)
                    {
                        var sp = SparkleParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2));
                        sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
                        sp.flickering = true;
                        sp.gravity = 0;
                    }
                    if (Timer % 4 == 0)
                    {
                        var sp = FaintSmokeParticle.SpawnInAlphaLayer(NPC.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2));
                        sp.Scale *= Main.rand.NextFloat(0.5f, 0.75f);
                        sp.behindLayer = true;
                        sp.noShrink = true;
                        sp.fadeToColor = Color.Black;
                        sp.color = Color.Lerp(Color.Blue, Color.Black, 0.75f);
                    }


                    _showTrail = true;
                    _attacking = true;


                    Animator.PlayAnimation(ANIM_DOWN);
                    if (Animator.IsFinished())
                    {
                        if (_bladeDanceV2)
                        {
                            _magicSwordV2 = true;
                            SwitchState(AIState.Blue_Magic_Sword);
                        }
                        else
                        {
                            Timer = 0;
                            AttackCycle++;
                        }
                    }
                }
                break;
            case 4:
                {
                    NPC.velocity *= 0.9f;
                    int style = 1;
                    if (Timer == 45)
                        style = 2;
                    if (Timer == 65)
                        style = 3;
                    if (Timer == 25 || Timer == 45 || Timer == 65)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 pos = NPC.Center;
                            pos.Y -= 32;
                            pos += Main.rand.NextVector2Circular(32, 32);


                            Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, Vector2.Zero, ModContent.ProjectileType<MoonShot>(), Moon_Shot_Damage, 0f, Owner: Main.myPlayer,
                            ai1: style);
                        }

                    }
                    _attacking = true;
                    Animator.PlayAnimation(ANIM_PULSE);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 5:
                {
                    ExplodeOut();
                }
                break;
        }
    }
    private void AI_SpinningLittleMoons()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    TeleportsFarAboveYou();
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();

                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Hyuh"), NPC.position);
                    }

                    Vector2 targetVelocity = (MyTarget.Center - NPC.Center);
                    targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
                    NPC.velocity += targetVelocity * MathHelper.Lerp(0f, 0.33f, Timer / 60f);
                    NPC.velocity *= 0.9f;
                    Animator.PlayAnimation(ANIM_PULSE);

                    _warning = true;
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    _showTrail = true;
                    if (Timer == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Moaning"), NPC.position);
                        _startVelocity = NPC.velocity;
                    }
                    DustTrailEffects();

                    if (Timer > 60 && Timer % 25 == 0)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 awayFromPlayer = (NPC.Center - MyTarget.Center);
                            awayFromPlayer = awayFromPlayer.SafeNormalize(Vector2.Zero);
                            awayFromPlayer *= 15;
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, awayFromPlayer,
                                ModContent.ProjectileType<VerliaMiniMoon>(), Mini_Moon_Damage, 1, Main.myPlayer);
                        }
                    }

                    _attacking = true;

                    float revolutionTime = 180f;
                    float radiansToOffset = MathHelper.TwoPi * 2f / revolutionTime;
                    Vector2 offset = -Vector2.UnitY;
                    offset *= 196;
                    offset = offset.RotatedBy(radiansToOffset * Timer);
                    Vector2 targetPosition = MyTarget.Center + offset;
                    Vector2 targetVelocity = targetPosition - NPC.Center;

                    float ease = EasingFunction.InOutExpo(Timer / 100f);
                    Vector2 interpolatedVelocity = Vector2.Lerp(_startVelocity, targetVelocity, ease);
                    NPC.velocity = interpolatedVelocity;
                    NPC.rotation = NPC.velocity.X * 0.025f;
                    FaceTarget();
                    Animator.PlayAnimation(ANIM_SPIN);
                    if (Timer >= revolutionTime)
                    {
                        AttackCycle++;
                        Timer = 0;
                    }
                }
                break;
            case 3:
                {
                    Animator.PlayAnimation(ANIM_READYDASH);
                    NPC.velocity *= 0.9f;
                    NPC.rotation = NPC.velocity.X * 0.025f;
                    if (Timer >= 90)
                    {
                        Timer = 0;
                        AttackCycle++;
                        //     SwitchState(AIState.Idle);
                    }
                }
                break;
            case 4:
                {
                    ExplodeOut();
                }
                break;
        }
    }
    private void AI_TwoBouncingMoons()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    _warning = true;
                    TeleportsFarAboveYou();
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Moaning"), NPC.position);
                    }

                    NPC.velocity *= 0.9f;
                    NPC.rotation *= 0.9f;

                    Animator.PlayAnimation(ANIM_HOLDUP);
                    if (Timer == 10)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero,
                                ModContent.ProjectileType<VerliaBouncingMoon>(), Twin_Bouncing_Moon_Damage, 1, Main.myPlayer, ai2: 1);
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero,
                                ModContent.ProjectileType<VerliaBouncingMoon>(), Twin_Bouncing_Moon_Damage, 1, Main.myPlayer, ai2: -1);
                        }
                    }

                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                    _warning = true;
                }
                break;
            case 2:
                if (Timer == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Huhhuh"), NPC.position);
                    var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.LightSkyBlue, Color.DarkBlue);
                    fx.Scale *= 2f;
                }

                if(Timer == 60)
                {
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/VerliaSONATO"));
                }
                _attacking = true;
                Animator.PlayAnimation(ANIM_PULSE);
                if (Animator.IsFinished())
                {
                    Timer = 0;
                    AttackCycle++;
                }
                break;
            case 3:
   
                
                ExplodeOut();
                break;
        }
    }
    private void AI_Spawn()
    {
        ShowNamePlate();
        SwitchState(AIState.Idle);
    }

    private void AI_Idle()
    {
        _magicSwordV2 = false;
        Timer++;
        if (Timer >= 100)
        {
            ChooseAttack();
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }
    #region Drawing


    public const string ANIM_SUMMON = "Summon";
    public const string ANIM_IDLESUMMON = "IdleSummon";
    public const string ANIM_UNSUMMON = "UnSummon";
    public const string ANIM_HOLDUP = "HoldUp";
    public const string ANIM_SWORD = "Sword";
    public const string ANIM_SWORDSLASH = "SwordSlash";
    public const string ANIM_DOWN = "Down";
    public const string ANIM_PULSE = "Pulse";
    public const string ANIM_EXPLODE = "Explode";
    public const string ANIM_TELEPORTIN = "TeleportIn";
    public const string ANIM_RUN = "Run";
    public const string ANIM_READYDASH = "ReadyDash";
    public const string ANIM_DASHFRAME = "DashFrame";
    public const string ANIM_SPIN = "Spin";
    public Animator CreateAnimator()
    {
        var animator = new Animator();
        Vector2 drawOrigin = new Vector2(100, 114);
        var idle = new SpriteAnimation(0, 1, isLooping: true, drawOrigin);
        animator.AddAnimation(ANIM_SUMMON, idle);

        var swordHold = new SpriteAnimation(0, 4, isLooping: true, drawOrigin);
        animator.AddAnimation(ANIM_IDLESUMMON, swordHold);

        var handOut = new SpriteAnimation(0, 3, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_UNSUMMON, handOut);

        var lookOver = new SpriteAnimation(0, 7, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_HOLDUP, lookOver);

        var morph = new SpriteAnimation(0, 10, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_SWORD, morph);

        var swimming = new SpriteAnimation(0, 4, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_SWORDSLASH, swimming);

        var battle = new SpriteAnimation(0, 9, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_DOWN, battle);

        var forwardSlash = new SpriteAnimation(0, 21, isLooping: false, drawOrigin, frameSpeed: 0.22f);
        animator.AddAnimation(ANIM_PULSE, forwardSlash);

        var backSlash = new SpriteAnimation(0, 8, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_EXPLODE, backSlash);

        var foundYou = new SpriteAnimation(0, 4, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_TELEPORTIN, foundYou);

        var holding = new SpriteAnimation(0, 8, isLooping: true, drawOrigin, frameSpeed: 0.3f);
        animator.AddAnimation(ANIM_RUN, holding);

        var bigSlash = new SpriteAnimation(0, 7, isLooping: false, drawOrigin, frameSpeed: 0.25f);
        animator.AddAnimation(ANIM_READYDASH, bigSlash);

        var running = new SpriteAnimation(0, 0, isLooping: true, drawOrigin, frameSpeed: 0.35f);
        animator.AddAnimation(ANIM_DASHFRAME, running);

        var running2 = new SpriteAnimation(0, 8, isLooping: true, drawOrigin, frameSpeed: 0.35f);
        animator.AddAnimation(ANIM_SPIN, running2);
        return animator;
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        Animator.Update();
        NPC.frame.Y = Animator.GetFrameY(184);
        NPC.frame.Height = 184;
        NPC.frame.Width = 266;
    }

    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(0, 16, EasingFunction.QuadraticBump(ratio)) * _trailAlpha;
    }
    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.White, Color.Transparent, ratio) * _trailAlpha;
    }
    private void DrawTrail(GraphicsDevice gDevice)
    {
        RichLaserShader laserShader = RichLaserShader.Instance;
        laserShader.LaserTexture = TrailRegistry.CorkscrewTrail;
        laserShader.InnerColor = Color.White;
        laserShader.OuterColor = Color.White;
        TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, GetTrailColor, GetTrailWidth, laserShader, NPC.Size * 0.5f);
    }

    private void DrawAfterImage(SpriteBatch spriteBatch)
    {
        SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;
        spriteBatch.Restart(effect: whiteShader.Effect);

        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.texture = ModContent.Request<Texture2D>($"{Texture}_{Animator.GetAnimation()}").Value;
        drawer.sourceRect = NPC.frame;
        drawer.drawOrigin = Animator.GetDrawOrigin().Value;
        if (NPC.spriteDirection == -1)
            drawer.drawOrigin.X = NPC.frame.Size().X - drawer.drawOrigin.X;
        for (int i = 0; i < NPC.oldPos.Length; i++)
        {
            Vector2 pos = NPC.oldPos[i];
            Vector2 oldCenter = pos + NPC.Size * 0.5f;
            drawer.worldPosition = oldCenter;
            drawer.color = Color.Lerp(Color.LightBlue, Color.Blue, i / (float)NPC.oldPos.Length);
            drawer.color *= MathHelper.Lerp(1f, 0f, i / (float)NPC.oldPos.Length);
            drawer.color *= _trailAlpha;
            spriteBatch.Draw(drawer);

        }
        spriteBatch.RestartDefaults();
    }
    private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.texture = ModContent.Request<Texture2D>($"{Texture}_{Animator.GetAnimation()}").Value;
        drawer.sourceRect = NPC.frame;
        drawer.drawOrigin = Animator.GetDrawOrigin().Value;
        if (NPC.spriteDirection == -1)
            drawer.drawOrigin.X = NPC.frame.Size().X - drawer.drawOrigin.X;
        drawer.color = drawColor;
        drawer.worldPosition += screenPos;
        spriteBatch.Draw(drawer);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (!Animator.GetDrawOrigin().HasValue)
            return false;


        PixelationManager.QueuePrimitivesDrawAction(DrawTrail, DrawLayer.BehindNPCsWithOutline);
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, NPC.Center);
        glowDrawer.scale *= 0.35f * _trailAlpha * ExtraMath.Osc(0.66f, 1f, speed: 2);
        glowDrawer.color = Color.LightSkyBlue * _trailAlpha;
        glowDrawer.color.A = 0;
        glowDrawer.rotation = Main.GlobalTimeWrappedHourly;
        spriteBatch.Draw(glowDrawer);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedSpellCircle, DrawLayer.BehindNPCsWithOutline);
        DrawWings(spriteBatch, screenPos, drawColor);
        DrawAfterImage(spriteBatch);
        DrawSprite(spriteBatch, Vector2.Zero, drawColor);
        return false;
    }

    private Vector2 LeftWingScale
    {
        get
        {
            Vector2 leftWingScale = Vector2.One;
            leftWingScale.X = MathHelper.Lerp(1f, 0.7f, EasingFunction.Clamp(NPC.velocity.X / -15f));
            leftWingScale *= _wingScale;
            return leftWingScale;
        }
    }
    private float LeftWingRotation
    {
        get
        {
            float rot = MathHelper.Lerp(0, MathHelper.ToRadians(12), EasingFunction.Clamp(NPC.velocity.X / -15f));
            return rot;
        }
    }
    private Vector2 RightWingScale
    {
        get
        {
            Vector2 leftWingScale = Vector2.One;
            leftWingScale.X = MathHelper.Lerp(1f, 0.7f, EasingFunction.Clamp(NPC.velocity.X / 15f));
            leftWingScale *= _wingScale;
            return leftWingScale;
        }
    }
    private float RightWingRotation
    {
        get
        {
            float rot = MathHelper.Lerp(0, MathHelper.ToRadians(-12), EasingFunction.Clamp(NPC.velocity.X / -15f));
            return rot;
        }
    }

    private void DrawPixelatedSpellCircle(SpriteBatch sb, Vector2 screenPos)
    {
        DrawSpellCircle(sb);
    }
    private void DrawSpellCircle(SpriteBatch sb)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>(Texture + "_MagicCircle"), NPC.Center);
        drawer.color = Color.Lerp(Color.LightBlue, Color.DarkBlue, ExtraMath.Osc(0f, 1f, speed: 4)) * _magicCircleAlpha;
        drawer.color.A = 0;
        drawer.rotation = Main.GlobalTimeWrappedHourly;
        drawer.scale *= 0.8f;
        drawer.scale *= MathHelper.Lerp(1.5f, 1f, _magicCircleAlpha);
        sb.Draw(drawer);
    }
    private void DrawWings_Inner(SpriteBatch spriteBatch)
    {

        float degrees = -MathHelper.Lerp(8, 15, ExtraMath.Osc(0f, 1f, speed: 3));
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, NPC.Center);
        glowDrawer.color = Color.DeepSkyBlue * 0.4f;
        glowDrawer.color.A = 0;
        glowDrawer.scale.Y *= 0.66f;
        glowDrawer.scale *= 0.6f;
        glowDrawer.scale *= _wingScale;
        glowDrawer.rotation = MathHelper.ToRadians(degrees);

        glowDrawer.drawOrigin = new Vector2(AssetManager.GlowMask.SimpleGlowCircle.Width() * 0.2f, AssetManager.GlowMask.SimpleGlowCircle.Height() * 0.5f);
        spriteBatch.Draw(glowDrawer);



        glowDrawer.rotation = MathHelper.ToRadians(-degrees);
        glowDrawer.drawOrigin.X = glowDrawer.texture.Size().X - glowDrawer.drawOrigin.X;
    //    glowDrawer.drawOrigin = new Vector2(AssetManager.GlowMask.SimpleGlowCircle.Width() * 0.2f, AssetManager.GlowMask.SimpleGlowCircle.Height() * 0.5f);
        spriteBatch.Draw(glowDrawer);


        SpritebatchDrawer wingDrawer = SpritebatchDrawer.FromTextureAsset(_wingTextureAsset, NPC.Center);
        wingDrawer.LeftCenterOrigin();
        wingDrawer.worldPosition.Y -= 32;
        wingDrawer.color = Color.DarkBlue;

        wingDrawer.scale = RightWingScale;
        wingDrawer.rotation = MathHelper.ToRadians(degrees) + RightWingRotation;
        spriteBatch.Draw(wingDrawer);

        wingDrawer.scale = LeftWingScale;
        wingDrawer.rotation = MathHelper.ToRadians(-degrees) + LeftWingRotation;
        wingDrawer.drawOrigin.X = wingDrawer.texture.Size().X - wingDrawer.drawOrigin.X;
        wingDrawer.spriteEffects = SpriteEffects.FlipHorizontally;
        spriteBatch.Draw(wingDrawer);
    }
    private void DrawPixelatedWings(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        DrawWings(spriteBatch, screenPos, Color.White);
    }
    private void DrawWings(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        _wingTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Wing");
        _wingOutlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_WingOutline");

    
        VerlianWingsShader wingShader = VerlianWingsShader.Instance;
        wingShader.BloomColorStart = Color.White;
        wingShader.BloomColorEnd = Color.Lerp(Color.Lerp(Color.Blue, Color.Black, 0.35f), Color.DarkBlue, ExtraMath.Osc(0f, 1f, speed: 2));
        wingShader.PerlinNoiseTexture = AssetManager.Noise.Whirly.Value;
        wingShader.ScrollingTexture = TrailRegistry.WaterTrail.Value;
        wingShader.DistortionStrength = 0.15f;
        wingShader.MaskSize = _wingTextureAsset.Size();
        wingShader.Frequency = 1f;
        wingShader.Tiling = Vector2.One * 2.5f;
        wingShader.ScrollOffset = new Vector2(-Main.GlobalTimeWrappedHourly * 0.4f, 0.0f);

        DrawWings_Inner(spriteBatch);
        float degrees = -MathHelper.Lerp(8, 15, ExtraMath.Osc(0f, 1f, speed: 3));
        spriteBatch.Restart(effect: wingShader.Effect, sortMode: SpriteSortMode.Immediate);
        SpritebatchDrawer wingDrawer;
      
        //Draw main wings
        wingDrawer = SpritebatchDrawer.FromTextureAsset(_wingTextureAsset, NPC.Center);
        wingDrawer.LeftCenterOrigin();
        wingDrawer.worldPosition.Y -= 32;
        wingDrawer.color = Color.Lerp(Color.White, Color.Black, 0.5f) * 0.6f;
        wingDrawer.rotation = MathHelper.ToRadians(degrees) + RightWingRotation; 
        wingDrawer.scale = RightWingScale;
        spriteBatch.Draw(wingDrawer);

        wingDrawer.rotation = MathHelper.ToRadians(-degrees) + LeftWingRotation;
        wingDrawer.drawOrigin.X = wingDrawer.texture.Size().X - wingDrawer.drawOrigin.X;
        wingDrawer.scale = LeftWingScale;
        wingDrawer.spriteEffects = SpriteEffects.FlipHorizontally;
        spriteBatch.Draw(wingDrawer);

        //Draw stars in wings
        spriteBatch.Restart(effect: wingShader.Effect, sortMode: SpriteSortMode.Immediate);
        wingShader.Tiling = Vector2.One *16f;
        wingShader.ScrollingTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BlurryPerlinNoise2").Value;
        wingDrawer = SpritebatchDrawer.FromTextureAsset(_wingTextureAsset, NPC.Center);
        wingDrawer.LeftCenterOrigin();
        wingDrawer.worldPosition.Y -= 32;
        wingDrawer.color = Color.Lerp(Color.White, Color.Black, 0.35f) * 0.45f;
        wingDrawer.color.A = 0;
        wingDrawer.scale = RightWingScale;
        wingDrawer.rotation = MathHelper.ToRadians(degrees) + RightWingRotation;
        spriteBatch.Draw(wingDrawer);

        wingDrawer.rotation = MathHelper.ToRadians(-degrees) + LeftWingRotation;
        wingDrawer.drawOrigin.X = wingDrawer.texture.Size().X - wingDrawer.drawOrigin.X;
        wingDrawer.spriteEffects = SpriteEffects.FlipHorizontally;
        wingDrawer.scale = LeftWingScale;
        spriteBatch.Draw(wingDrawer);


        wingShader.BloomColorEnd = Color.White;
        wingDrawer = SpritebatchDrawer.FromTextureAsset(_wingOutlineTextureAsset, NPC.Center);
        wingDrawer.LeftCenterOrigin();
        wingDrawer.worldPosition.Y -= 32;
        wingDrawer.color = Color.White;
        wingDrawer.scale = RightWingScale;
        wingDrawer.rotation = MathHelper.ToRadians(degrees) + RightWingRotation;
        spriteBatch.Draw(wingDrawer);

        wingDrawer.rotation = MathHelper.ToRadians(-degrees) + LeftWingRotation;
        wingDrawer.drawOrigin.X = wingDrawer.texture.Size().X - wingDrawer.drawOrigin.X;
        wingDrawer.spriteEffects = SpriteEffects.FlipHorizontally;
        wingDrawer.scale = LeftWingScale;
        spriteBatch.Draw(wingDrawer);

        spriteBatch.RestartDefaults();

    }
    #endregion
    public override void OnKill()
    {
        base.OnKill();
    }

    public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        Vector2 v = Vector2.UnitX * 2;
        Vector2 h = Vector2.UnitY * 2;

        DrawSprite(spriteBatch, v, _outlineColor);
        DrawSprite(spriteBatch, -v, _outlineColor);
        DrawSprite(spriteBatch, h, _outlineColor);
        DrawSprite(spriteBatch, -h, _outlineColor);
        //      throw new System.NotImplementedException();
    }
}

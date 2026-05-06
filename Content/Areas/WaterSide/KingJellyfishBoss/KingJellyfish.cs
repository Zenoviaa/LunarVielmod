using rail;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core;
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
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Biomes.Desert;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Content.Areas.WaterSide.KingJellyfishBoss;

/*
 * 
 * sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

#define PI 3.1415926535897931;
float time;
float frequency;
float amplitude;

 */

public class ZapLightningShader : CrystalShader<ZapLightningShader>
{
    private EffectParameter _tilingParam;
    private EffectParameter _laserTextureParam;
    private EffectParameter _levelsParam;
    private EffectParameter _amplitudeParam;
    private EffectParameter _timeParam;
    private EffectParameter _matrixParam;
    private EffectParameter _bloomParam;
    public Asset<Texture2D> LaserTexture
    {
        set
        {
            _laserTextureParam ??= Effect.Parameters["laserTexture"];
            _laserTextureParam.SetValue(value.Value);
        }
    }

    public Color BloomColor
    {
        set
        {
            _bloomParam ??= Effect.Parameters["bloomColor"];
            _bloomParam.SetValue(value.ToVector4());
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

    public Matrix TransformMatrix
    {
        set
        {
            _matrixParam ??= Effect.Parameters["transformMatrix"];
            _matrixParam.SetValue(value);
        }
    }
    public float Time
    {
        set
        {
            _timeParam ??= Effect.Parameters["time"];
            _timeParam.SetValue(value);
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
    public float Levels
    {
        set
        {
            _levelsParam ??= Effect.Parameters["levels"];
            _levelsParam.SetValue(value);
        }
    }

    public Texture2D Laser
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[0] = value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
        }
    }

    public Texture2D Noise
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
        }
    }

    public Texture2D Gradient
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[2] = value;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;
        }
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        TransformMatrix = TrailDrawer.WorldViewPoint2;
    }
}


public class ZapShockwaveShader : CrystalShader<ZapShockwaveShader>
{
    private EffectParameter _amplitudeParam;
    private EffectParameter _frequencyParam;
    private EffectParameter _timeParam;
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

    public Texture2D Gradient
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
        }
    }
}


public class ZapShockwave : ModProjectile
{
    private Asset<Texture2D> _gradientTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 256;
        Projectile.height = 256;
        Projectile.hostile = true;
        Projectile.timeLeft = 60;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer > 24)
        {
            Projectile.hostile = false;
        }
        if(Timer == 1)
        {
            SoundStyle explosionSound = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Bomb");
            SoundEngine.PlaySound(explosionSound, Projectile.position);
            PixelPrimitiveCircleFactory.CreateElectricBoom(Projectile.Center);
            FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
            var fx = FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.SkyBlue,
                outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.28f);
            fx.Scale *= 2f;

            for (float f = 0; f < 64; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(24, 24);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.innerColor = Color.Lerp(Color.White, Color.SkyBlue, Main.rand.NextFloat(0f, 1f));
                spawnParams.outerColor = Color.Turquoise;
                var d = DustParticle.Spawn(Projectile.Center, vel, spawnParams);
                d.dampening = 0.05f;
                d.gravity = 0;
                d.noTileCollide = true;
                d.Scale *= 1.5f;

            }

            if (Main.netMode != NetmodeID.Server)
            {
                ScreenShaderSystem e = ModContent.GetInstance<ScreenShaderSystem>();
                e.TintScreen(Color.SkyBlue, 0.1f, 20);
            }

            FXUtil.ShakeCamera(Projectile.Center, 1024, 16);
        }
        ShakeModSystem.Shake = MathHelper.Lerp(4f, 0f, Timer / 60f);
        if(Timer == 30)
        {
            var fx = FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.SkyBlue,
                outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.28f);
        }
        if(Timer >= 60)
        {
            Projectile.Kill();
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        _gradientTextureAsset ??= ModContent.Request<Texture2D>(ModContent.GetInstance<ZapShockwave>().Texture + "_Gradient");
        ZapShockwaveShader zapShockwaveShader = ZapShockwaveShader.Instance;
        zapShockwaveShader.Time =Timer * 0.15f;
        zapShockwaveShader.Frequency = 8f;
        zapShockwaveShader.Amplitude = 0.02f;
        zapShockwaveShader.Gradient = _gradientTextureAsset.Value;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.scale = Vector2.Lerp(Vector2.Zero, Vector2.One * 3f, EasingFunction.OutExpo(Timer / 60f));
        drawer.color = Color.SkyBlue;
        drawer.color *= MathHelper.Lerp(1f, 0f, EasingFunction.OutExpo(Timer / 30f));
        drawer.color.A = 0;



        Main.spriteBatch.Restart(effect: zapShockwaveShader.Effect);
        Main.spriteBatch.Draw(drawer);

        drawer.scale *= 0.9f;
        drawer.color = Color.White;
        drawer.color *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / 60f));
        drawer.color.A = 0;

        Main.spriteBatch.Draw(drawer);
        Main.spriteBatch.RestartDefaults();
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class BabyRichochetZap : ModProjectile
{
    private float _bounceTimer;
    private float _bounceCount;
    private Vector2 _startPosition;
    private Vector2 _midPosition;
    private Asset<Texture2D> _gradientTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    private int Target
    {
        get => (int)Projectile.ai[1];
        set => Projectile.ai[1] = value;
    }
    private int Style
    {
        get => (int)Projectile.ai[2];
        set => Projectile.ai[2] = value;
    }

    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_bounceCount);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _bounceCount = reader.ReadInt32();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 32;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hostile = false;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 700;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.extraUpdates = 2;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return base.Colliding(projHitbox, targetHitbox);
    }

    public override bool ShouldUpdatePosition()
    {
        return true;
    }

    private void FindNewTarget()
    {
        if (!this.OwnedByLocalClient())
            return;
        List<int> pool = new List<int>(7);
        foreach(NPC npc in Main.ActiveNPCs)
        {
            if (npc.type == ModContent.NPCType<BabyJellyfish>())
                pool.Add(npc.whoAmI);
        }

        if (pool.Count <= 0)
            return;

        int newTarget = pool[Main.rand.Next(pool.Count)];
        Target = newTarget;
        _startPosition = Projectile.Center;
        _midPosition = Projectile.Center + Main.rand.NextVector2CircularEdge(256, 256);
     //   _startPosition = Projectile.Center;
        Projectile.netUpdate = true;
    }
    public override void AI()
    {
        base.AI();
        if(Timer == 0)
        {
            SoundStyle zapSound = SoundID.DD2_LightningBugZap with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(zapSound, Projectile.position);

            if(Style == 1)
            {
                string path = $"Stellamod/Assets/Sounds/Dreadmire_Pentagram_Skull1";
                SoundStyle sound = new SoundStyle(path) with { PitchVariance = 0.3f };
                SoundEngine.PlaySound(sound, Projectile.position);
            }
            FindNewTarget();
            Timer++;

        }
        //   Main.NewText(Projectile.position);
        switch (Style)
        {
            case 0:
                {
                    Projectile.hostile = true;
                    LightningCharge();
                }
                break;
            case 1:
                {
                    LightningZap();
                }
                break;
        }

    }

    private void LightningZap()
    {
     
        float bounces = 3;
        if (_bounceCount >= bounces)
        {
            Projectile.extraUpdates = 2;
            Projectile.hostile = true;
            if (Projectile.velocity.Length() < 12)
                Projectile.velocity *= 1.1f;
            return;
        }
        Projectile.extraUpdates = 4;
        if (Main.rand.NextBool(16))
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Electric, Scale: 0.5f);
        }
        _bounceTimer++;
        NPC baby = Main.npc[Target];
        NPC parent = Main.npc[(int)baby.ai[2]];

        float time = 100;

        if (_bounceTimer >= time)
        {
            string path = $"Stellamod/Assets/Sounds/Dreadmire__LightingRain{_bounceCount + 1}";
            SoundStyle sound = new SoundStyle(path) with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(sound, Projectile.position);
            FindNewTarget();
            _bounceTimer = 0;
            _bounceCount++;

            var fx = FXUtil.GlowCircleBoom(baby.Center, Color.White, Color.SkyBlue, Color.Black);
            fx.Scale *= 0.66f;
            float numDust = 4;
            for (float n = 0; n < numDust; n++)
            {
                Vector2 vel = -Projectile.velocity;
                vel = vel.RotatedByRandom(MathHelper.ToRadians(60));
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(6, 12);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.SkyBlue;
                var dp = DustParticle.Spawn(Projectile.Center, vel, spawnParams);
                dp.fast = true;
                dp.noTileCollide = true;
                dp.dampening = 0.05f;
                dp.gravity = 0;
                dp.Scale *= 0.5f;
            }

            if (_bounceCount >= bounces)
            {
                Player target = Main.player[parent.target];
                Vector2 vel = (target.Center - Projectile.Center);
                vel = vel.SafeNormalize(Vector2.Zero);
                Projectile.velocity = vel;
                var fx2 = FXUtil.GlowCircleBoom(baby.Center, Color.White, Color.SkyBlue, Color.Black);
                fx2.Scale *= 2f;


                for (float f = 0; f < 4; f++)
                {
                    Particle<DustParticle>.Spawn(target.Center,
                        Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(6, 8f), Scale: Main.rand.NextFloat(0.5f, 1f));
                }

                for (float f = 0; f < 8f; f++)
                {
                    vel = Projectile.velocity;
                    vel = vel.RotatedByRandom(MathHelper.PiOver4 / 2f);
                    vel *= Main.rand.NextFloat(5f, 15f);
                    DustParticle dp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(16, 16), vel);
                    dp.outerColor = Color.Turquoise;
                    dp.gravity = 0;
                    dp.dampening = 0.05f;
                    dp.noTileCollide = true;
                }

                GlowDonutParticle d = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity * 2);
                d.outerColor = Color.SkyBlue;
                d.fadeToColor = Color.DeepSkyBlue;
                d.Scale *= 0.3f;

            }
        } 
        else if(_bounceCount < bounces)
        {
            float ratio = _bounceTimer / time;
            float ease = EasingFunction.InExpo(ratio);
            _midPosition = _midPosition.RotatedBy(0.015f, parent.Center);
            Vector2 lerp1 = Vector2.Lerp(_startPosition, _midPosition, EasingFunction.OutExpo(ratio));
            Vector2 lerp2 = Vector2.Lerp(_midPosition, baby.Center, ease);
            Vector2 targetPosition = Vector2.Lerp(lerp1, lerp2, ease);
            Vector2 targetVelocity = (targetPosition - Projectile.Center);
            Projectile.velocity = targetVelocity;
        }

    }

    private void LightningCharge()
    {
        if (_bounceCount >= 7)
        {
            Timer++;

            NPC baby = Main.npc[Target];
            NPC parent = Main.npc[(int)baby.ai[2]];
            Vector2 shootVelocity = (baby.Center - parent.Center).SafeNormalize(Vector2.Zero);
            shootVelocity *= 384;
            if (Timer % 10 == 0)
            {
                var fx = FXUtil.GlowCircleBoom(baby.Center, Color.White, Color.SkyBlue, Color.Black, 7);
                fx.Scale *= MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 120f));
            }

            Projectile.velocity = baby.Center - Projectile.Center;
            if (Timer >= 120)
            {
                if (this.OwnedByLocalClient())
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), baby.Center, shootVelocity,
                        ModContent.ProjectileType<BabyZap>(), Projectile.damage, 1, Projectile.owner);
                }
                Projectile.Kill();
            }
        }
        else
        {
            if (Main.rand.NextBool(16))
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Electric, Scale: 0.5f);
            }

            NPC npc = Main.npc[Target];
            Vector2 targetCenter = npc.Center;
            Vector2 velocity = (targetCenter - Projectile.Center);
            velocity = velocity.SafeNormalize(Vector2.Zero);
            float speed = 12;
            float dist = Vector2.Distance(Projectile.Center, npc.Center);
            if (dist < speed)
            {
                SoundStyle zapSound = SoundID.DD2_LightningBugZap with { PitchVariance = 0.5f };
                SoundEngine.PlaySound(zapSound, Projectile.position);
                speed = dist;

                //Change 
                FindNewTarget();
                _bounceCount++;
            }

            velocity *= speed;
            Projectile.velocity = velocity;
        }
    }
    private Color GetTrailColor(float ratio)
    {
        Color bloomColor = Color.Lerp(Color.White, Color.SkyBlue, 0.8f);
        Color bloomColor2 = Color.Lerp(Color.Transparent, bloomColor, EasingFunction.QuadraticBump(ratio));
        return Color.Lerp(Color.DeepSkyBlue, Color.White, EasingFunction.QuadraticBump(ratio));
    }

    private float GetTrailWidth(float ratio)
    {
        float w = MathHelper.Lerp(2f, 10, _bounceCount / 7f);
        if (Style == 1)
        {
            w = MathHelper.Lerp(2f, 12, _bounceCount / 3f);
        }
   
        float outEasing = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        return MathHelper.SmoothStep(w * 0.85f, w, EasingFunction.QuadraticBump(ratio)) * outEasing;
    }

    private Color GetTrailColor2(float ratio)
    {
        Color bloomColor = Color.Lerp(Color.White, Color.SkyBlue, 0.8f);
        Color bloomColor2 = Color.Lerp(Color.Transparent, bloomColor, EasingFunction.QuadraticBump(ratio));
        return Color.Lerp(Color.DeepSkyBlue, Color.White, EasingFunction.QuadraticBump(ratio));
    }

    private float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 1.6f;
    }

    private void DrawPixelatedLightning(GraphicsDevice gDevice)
    {
        Vector2[] lightningPoints = Projectile.oldPos;
        ZapLightningShader lightingShader = ZapLightningShader.Instance;
        lightingShader.Amplitude = 0.8f;

        float time = Main.GlobalTimeWrappedHourly * 16;
        float levels = 4;
        time = MathF.Floor(time * levels) / levels;
        lightingShader.Time = time;
        Asset<Texture2D> laserTexture = AssetManager.LaserTextures.TexturedLaser;
        lightingShader.LaserTexture = laserTexture;
        lightingShader.Noise = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BlurryPerlinNoise").Value;
        lightingShader.Gradient = _gradientTextureAsset.Value;
        lightingShader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        lightingShader.Levels = 64;
        lightingShader.Tiling = new Vector2(2f);
        TrailDrawer.Draw(Main.spriteBatch, lightningPoints, GetTrailColor, GetTrailWidth, lightingShader, Projectile.Size * 0.5f);

        BloomTrailShader bloom = BloomTrailShader.Instance;
        bloom.InnerColor = Color.SkyBlue;
        bloom.OuterColor = Color.DeepSkyBlue;
        TrailDrawer.Draw(Main.spriteBatch, lightningPoints, GetTrailColor2, GetTrailWidth2, bloom, Projectile.Size * 0.5f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        _gradientTextureAsset ??= ModContent.Request<Texture2D>(ModContent.GetInstance<ZapShockwave>().Texture + "_Gradient");
        if (Timer > 2)
            return false;
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedLightning);
        SpritebatchDrawer sb = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        //Main.spriteBatch.Draw(sb);
        return false;
    }
}

public class BabyZap : ModProjectile
{
    private float _widthMultiplier;
    private float _zapTime;
    private float _flashTimer;
    private Vector2 _controlPoint1;
    private Vector2 _controlPoint2;

    private Vector2 _controlPoint3;
    private Vector2 _controlPoint4;
    private Asset<Texture2D> _gradientTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    private Vector2 EndPoint => Projectile.Center + Projectile.velocity;
    private Vector2 EndPoint2 => Projectile.Center - Projectile.velocity;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();

    }
    
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hostile = true;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 30;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return base.Colliding(projHitbox, targetHitbox);
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            SoundStyle lightningSoundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_LightingZap");
            lightningSoundStyle.PitchVariance = 0.4f;
            SoundEngine.PlaySound(lightningSoundStyle, Projectile.position);

            SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
            hitSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(hitSound, Projectile.position);

            FXUtil.ShakeCamera(Projectile.Center, 1024, 32);

            for (float f = 0; f < 10; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(9, 9);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.innerColor = Color.Lerp(Color.White, Color.SkyBlue, Main.rand.NextFloat(0f, 1f));
                spawnParams.outerColor = Color.DarkBlue;
                var d = DustParticle.Spawn(EndPoint, vel, spawnParams);
                d.dampening = 0.05f;
                d.gravity = 0;
                d.noTileCollide = true;
                d.Scale *= 0.5f;
             }
            _controlPoint1 = Projectile.Center + Main.rand.NextVector2CircularEdge(192, 192);
            _controlPoint2 = Projectile.Center + Main.rand.NextVector2CircularEdge(192, 192);
            _controlPoint3 = Projectile.Center + Main.rand.NextVector2Circular(192, 192);
            _controlPoint4 = Projectile.Center + Main.rand.NextVector2Circular(192, 192);
            _widthMultiplier = Main.rand.NextFloat(0.5f, 1f);
            var fx = FXUtil.GlowCircleBoom(EndPoint, Color.White, Color.SkyBlue, Color.Blue);
            fx.Scale *= 2;
        }

        if(Timer % 5 == 0 && Timer < 30)
        {
            FXUtil.GlowCircleBoom(EndPoint, Color.SkyBlue, Color.DeepSkyBlue, Color.Black);
        }

        if(Timer % 3 == 0)
        {


        }
        if(Timer % 10 == 0)
        {
            _zapTime = Main.rand.NextFloat(0, 100);

        }

        if(Timer % 40 == 0)
        {
            _flashTimer = 28;
        }
        _flashTimer--;
    }
    
    private Color GetTrailColor(float ratio)
    {
        Color bloomColor = Color.Lerp(Color.White, Color.SkyBlue, 0.8f);
        Color bloomColor2 = Color.Lerp(Color.Transparent, bloomColor,  EasingFunction.QuadraticBump(ratio));
        return Color.Lerp(Color.DeepSkyBlue, Color.White, EasingFunction.QuadraticBump(ratio));
    }
    private float GetTrailWidth(float ratio)
    {
        float ease = EasingFunction.InOutSine(_flashTimer / 30f);
        float w = 20 * _widthMultiplier;
        float outEasing = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        return MathHelper.SmoothStep(w * 0.85f, w, EasingFunction.QuadraticBump(ratio)) * outEasing;
    }
    private Color GetTrailColor2(float ratio)
    {
        Color bloomColor = Color.Lerp(Color.White, Color.SkyBlue, 0.8f);
        Color bloomColor2 = Color.Lerp(Color.Transparent, bloomColor, EasingFunction.QuadraticBump(ratio));
        return Color.Lerp(Color.DeepSkyBlue, Color.White, EasingFunction.QuadraticBump(ratio));
    }
    private float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 1.6f;
    }
    private void DrawPixelatedLightning(GraphicsDevice gDevice)
    {
        int numPoints = 64;
        List<Vector2> trailPoints = new List<Vector2>(numPoints);
        Vector2 up = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);

        float outEase = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        Vector2 cp1 = Vector2.Lerp(_controlPoint1, _controlPoint3, outEase);
        Vector2 cp2 = Vector2.Lerp(_controlPoint2, _controlPoint4, outEase);
        for (float f = 0;  f < numPoints; f++)
        {
            float ratio = (float)f / (float)numPoints;
            Vector2 startPoint = Projectile.Center;
            Vector2 trailPoint = ExtraMath.CubicBezier(startPoint, 
                cp1, cp2, EndPoint2, ratio);
           // Vector2 trailPoint = Vector2.Lerp(startPoint, endPoint, ratio);
            trailPoint += up * MathF.Sin(ratio * 16 + _zapTime) * 32;
            trailPoints.Add(trailPoint);
        }
        for(int i = 0; i < 4; i++)
            trailPoints.Add(trailPoints[trailPoints.Count - 1]);
        //trailPoints.Add(trailPoints[trailPoints.Count - 1]);

        Vector2[] lightningPoints = trailPoints.ToArray();
        ZapLightningShader lightingShader = ZapLightningShader.Instance;
        lightingShader.Amplitude = 0.8f;

        float time  = Main.GlobalTimeWrappedHourly * 16;
        float levels = 4;
        time = MathF.Floor(time * levels) / levels;
        lightingShader.Time = time;
        Asset<Texture2D> laserTexture = Timer > 15 ? AssetManager.LaserTextures.TexturedLaser : AssetManager.LaserTextures.TexturedLaser2;
        lightingShader.LaserTexture = laserTexture;
        lightingShader.Noise = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BlurryPerlinNoise").Value;
        lightingShader.Gradient = _gradientTextureAsset.Value;
        lightingShader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        lightingShader.Levels = 64;
        lightingShader.Tiling = new Vector2(2f);
     //   lightingShader.BloomColor= Main.DiscoColor;
        TrailDrawer.Draw(Main.spriteBatch, lightningPoints, GetTrailColor, GetTrailWidth, lightingShader, Projectile.Size * 0.5f);
        BloomTrailShader bloom = BloomTrailShader.Instance;
        bloom.InnerColor = Color.SkyBlue;
        bloom.OuterColor = Color.DeepSkyBlue;
        TrailDrawer.Draw(Main.spriteBatch, lightningPoints, GetTrailColor2, GetTrailWidth2, bloom, Projectile.Size * 0.5f);
    }
    
    public override bool PreDraw(ref Color lightColor)
    {
        _gradientTextureAsset ??= ModContent.Request<Texture2D>(ModContent.GetInstance<ZapShockwave>().Texture + "_Gradient");
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedLightning);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class BabyJellyfish  : ModNPC
{
    private Asset<Texture2D> _glowTextureAsset;
    private enum AIState
    {
        Idle_Orbit,
        Zap,
        Motherless_Panic
    }

    private ref float Timer => ref NPC.ai[0];

    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private Vector2 _panicDirection;
    private NPC Mommy => Main.npc[(int)NPC.ai[2]];
    private ref float Offset => ref NPC.ai[3];
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[NPC.type] = 4;
        NPCID.Sets.TrailCacheLength[NPC.type] = 16;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 100;
        NPC.height = 100;
        NPC.damage = 50;
        NPC.defense = 15;
        NPC.lifeMax = 30;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.dontTakeDamage = true;
        NPC.value = Item.buyPrice(gold: 12);
        NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Morrowsc1");
        NPC.npcSlots = 10f;
        NPC.scale = 1f;
        NPC.aiStyle = -1;
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_panicDirection);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _panicDirection = reader.ReadVector2();
    }
    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter += 0.25f;
        NPC.frameCounter %= Main.npcFrameCount[NPC.type];
        int frame = (int)NPC.frameCounter;
        NPC.frame.Y = frame * frameHeight;
    }

    public override void AI()
    {
        base.AI();

    
        switch (State)
        {
            case AIState.Idle_Orbit:
                AI_IdleOrbit();
                break;
            case AIState.Zap:
                break;
            case AIState.Motherless_Panic:
                AI_MotherlessPanic();
                break;
        }
    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }
    private void AI_IdleOrbit()
    {
        Timer++;
        if (Main.rand.NextBool(32))
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, Scale: 0.5f);
        }

        Vector2 orbitPosition = Mommy.Center;
        Vector2 up = Vector2.UnitY;
        up *= 128;

        float offset = Offset / 7f * MathHelper.TwoPi;

        up = up.RotatedBy(Timer / 450f * MathHelper.TwoPi + offset);

        orbitPosition += up;

        Vector2 positionToMoveTo = NPC.Center.MoveTowards(orbitPosition, 12);
        NPC.velocity = positionToMoveTo - NPC.Center;
        NPC.rotation = Mommy.rotation;
        if (!Mommy.active)
        {
            SwitchState(AIState.Motherless_Panic);
        }
    }


    private void AI_MotherlessPanic()
    {
        Timer++;
        if (Timer >= Offset && MultiplayerHelper.IsHost)
        {
            Offset = Main.rand.Next(30, 210);
            _panicDirection = Main.rand.NextVector2Circular(1, 1);
            _panicDirection = _panicDirection.SafeNormalize(Vector2.Zero);
            NPC.netUpdate = true;
        }

        //Get the direction to the player
        Vector2 targetVelocity = _panicDirection * 4f;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation() + MathHelper.PiOver2, 0.1f);
        NPC.dontTakeDamage = false;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        SpritebatchDrawer jellyGlowDrawer = SpritebatchDrawer.FromNPC(NPC);
        spriteBatch.Draw(jellyGlowDrawer);

        _glowTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Glow");
        SpritebatchDrawer glowingDrawer = SpritebatchDrawer.FromNPC(NPC);
        glowingDrawer.texture = _glowTextureAsset.Value;
        glowingDrawer.color *= ExtraMath.Osc(0f, 1f, offset: NPC.whoAmI);
        glowingDrawer.color.A = 0;
        spriteBatch.Draw(glowingDrawer);
        return false;
    }

    public override void OnKill()
    {
        base.OnKill();
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }
}

//oh right i was supposed to design this boss
//well there's three attacks
//1. Commands all the glows to orbit around her, conduct electricity and zap at you
//2. King Jelly fish swims towards you while the orbiting little jelly fish fire tiny lightning bolts
//3. Charges up and sends a pulsing out shockwave from the mother
//4. Once the mother is dead, the little guys go wild and you can just kill them


public class KingJellyfish : ScarletBoss
{
    private enum AIState
    {
        Spawn,
        Despawn,
 
        Idle,
        Death,

        Electric_Orbit,
        Swimming_Lightning,
        Shockwave
    }


    private float _magicCircleChargeProgress;
    private float _magicCircleChargeProgress2;
    private float _magicCircleChargeProgress3;
    private float _electricAlpha;
    private bool _electrified;
    private Vector2 _startPoint;
    private Vector2 _controlPoint1;
    private Vector2 _controlPoint2;
    private Vector2 _targetPoint;
    private Outliner _outliner;
    private Asset<Texture2D> _magicCircleTextureAsset;
    private Asset<Texture2D> _glowTextureAsset;
    private Asset<Texture2D> _smallChainTextureAsset;
    private Asset<Texture2D> _chainTextureAsset;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private ref float AttackCycle => ref NPC.ai[2];
  
    private Chain _chain;
    private Chain Chain
    {
        get
        {
            _chain ??= new Chain(NPC.Center, 8, 128);
            return _chain;
        }
    }

    private Chain[] _tentacleChains;
    private Chain[] TentacleChains
    {
        get
        {
            if(_tentacleChains == null)
            {
                _tentacleChains = new Chain[5];
                for(int i = 0; i < _tentacleChains.Length; i++)
                {
                    _tentacleChains[i] = new Chain(NPC.Center, 24, 42);
                }
            }
            return _tentacleChains;
        }
    }

    private PatternManager<AIState> _patternManager;
    private PatternManager<AIState> PatternManager
    {
        get
        {
            if (_patternManager == null)
            {
                _patternManager = new();
                _patternManager.AddPattern(AIState.Electric_Orbit, 1.0f);
                _patternManager.AddPattern(AIState.Swimming_Lightning, 1.0f);
                _patternManager.AddPattern(AIState.Shockwave, 1.0f);
            }

            return _patternManager;
        }
    }

    private float IdleTime => 360;
    private int Electric_Orbit_Damage => 20;
    private int Zap_Shockwave_Damage => 40;
    public override BossLevel GetBossLevel()
    {
        return BossLevel.Miniboss;
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
        NPC.width = 100;
        NPC.height = 100;
        NPC.damage = 50;
        NPC.defense = 10;
        NPC.lifeMax = 2000;
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
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/NostalgicFoe");
        }
    }
    
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            AttackCycle = 0;
            NPC.netUpdate = true;
        }
    }

    public override void AI()
    {
        base.AI();
        if(!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if(!NPC.HasValidTarget && State != AIState.Despawn)
            {
                SwitchState(AIState.Despawn);
            }
        }

        _electrified = false;
        _outliner.SetDefaults();
        _magicCircleChargeProgress *= 0.98f;
        _magicCircleChargeProgress2 *= 0.96f;
        _magicCircleChargeProgress3 *= 0.94f;
        switch (State)
        {
            case AIState.Despawn:
                AI_Despawn();
                break;
            case AIState.Spawn:
                AI_Spawn();
                break;
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Death:
                AI_Death();
                break;
            case AIState.Electric_Orbit:
                AI_ElectricOrbit();
                break;
            case AIState.Swimming_Lightning:
                AI_SwimmingLightning();
                break;
            case AIState.Shockwave:
                AI_Shockwave();
                break;
        }

        float targetElectricAlpha = _electrified ? 1f : 0f;
        _electricAlpha = MathHelper.Lerp(_electricAlpha, targetElectricAlpha, 0.1f);
        _outliner.Update();
        SimulateTentacles();
    }

    private void ChooseAttack()
    {
        if (MultiplayerHelper.IsHost)
        {
            SwitchState(PatternManager.NextPattern());
        }
    //    SwitchState(AIState.Shockwave);
    }

    public List<Entity> GetElectricalPath()
    {
        List<Entity> entitiesToTraverse = new List<Entity>();
        return entitiesToTraverse;
    }

    private void MoveTowardsPlayer()
    {
        Vector2 velocity = (MyTarget.Center - NPC.Center);
        velocity = velocity.SafeNormalize(Vector2.Zero);
        NPC.velocity = Vector2.Lerp(NPC.velocity, velocity, 0.1f);
    }

    private void AI_Shockwave()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        SoundStyle telegraphSound = new SoundStyle("Stellamod/Assets/Sounds/Dreadmire__PreDash") with { PitchVariance = 0.5f };
                        SoundEngine.PlaySound(telegraphSound, NPC.position);
                    }

                    if (NPC.velocity.Length() > 1)
                        NPC.velocity *= 0.98f;
                    Vector2 targetVelocity = (MyTarget.Center - NPC.Center);
                    targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
                    NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
                    if (Timer % 8 == 0)
                    {
                        Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(128, 128);
                        Vector2 vel = (NPC.Center - pos);
                        vel *= 0.1f;
                        var fx = FXUtil.GlowStretch(pos, vel);
                        fx.VectorScale *= 0.5f;
                    }
                    if (Timer % 4 == 0)
                    {
                        Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(384, 384);
                        Vector2 vel = (NPC.Center - pos);
                        vel *= 0.05f;
                        var fx = FXUtil.GlowStretch(pos, vel);
                        fx.VectorScale *= 0.25f;
                    }
                    if (Timer % 60 == 0 && Timer < 239)
                    {
                        PixelPrimitiveCircleFactory.CreateElectricInwardBoom(NPC.Center);
                        SoundStyle inSound = new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_TP_In") with { Pitch = MathHelper.Lerp(0f, 0.75f, Timer / 180f) };
                        SoundEngine.PlaySound(inSound, NPC.position);
                    }

                    _magicCircleChargeProgress = EasingFunction.Clamp((Timer - 60) / 60f);
                    _magicCircleChargeProgress2 = EasingFunction.Clamp((Timer - 120) / 60f);
                    _magicCircleChargeProgress3 = EasingFunction.Clamp((Timer - 180) / 60f);
                    _electrified = true;


                    if (Timer == 160)
                    {
                        SoundStyle inSound = new SoundStyle("Stellamod/Assets/Sounds/StormKnight_Rechage");
                        SoundEngine.PlaySound(inSound, NPC.position);
                    }
                    _magicCircleChargeProgress = EasingFunction.Clamp(Timer / 180f);
                    ShakeModSystem.Shake = MathHelper.Lerp(0f, 4f, Timer / 180f);
                    _outliner.warning = true;
                    if (Timer >= 269)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    _electrified = true;
                    NPC.velocity *= 0;
                    //NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
                    if(Timer == 1)
                    {
                        Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero, 
                            ModContent.ProjectileType<ZapShockwave>(), Zap_Shockwave_Damage, 1, Main.myPlayer);
                    }
                
                    _outliner.attacking = true;
                    if (Timer >= 80f)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    SwitchState(AIState.Idle);
                }
                break;
        }
    }
    private void AI_SwimmingLightning()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        SoundStyle telegraphSound = new SoundStyle("Stellamod/Assets/Sounds/Dreadmire__LightingRain") with { PitchVariance = 0.5f };
                        SoundEngine.PlaySound(telegraphSound, NPC.position);
                        PixelPrimitiveCircleFactory.CreateElectricInwardBoom(NPC.Center);
                    }
                    if (NPC.velocity.Length() > 1)
                        NPC.velocity *= 0.98f;
                    _outliner.warning = true;
                    if (Timer >= 120)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    _electrified = true;
                    if (NPC.velocity.Length() > 1)
                        NPC.velocity *= 0.98f;
                    Vector2 targetVelocity = (MyTarget.Center - NPC.Center);
                    targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
                    if (Timer % 70 == 0)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, -Vector2.UnitY * 12, ModContent.ProjectileType<BabyRichochetZap>(), Electric_Orbit_Damage, 1, Main.myPlayer, ai2: 1);
                        }
                    }

                    // MoveTowardsPlayer();
                    NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
                    _outliner.attacking = true;
                    if (Timer >= 500)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    SwitchState(AIState.Idle);
                }
                break;
        }
    }
    private void AI_ElectricOrbit()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        SoundStyle telegraphSound = new SoundStyle("Stellamod/Assets/Sounds/Dreadmire__LightingRain") with { PitchVariance = 0.5f };
                        SoundEngine.PlaySound(telegraphSound, NPC.position);
                        PixelPrimitiveCircleFactory.CreateElectricInwardBoom(NPC.Center);
                    }
                    if (NPC.velocity.Length() > 1)
                        NPC.velocity *= 0.98f;
                    _outliner.warning = true;
                    if(Timer >= 120)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    _electrified = true;
                    if(NPC.velocity.Length() > 1)
                        NPC.velocity *= 0.98f;
                    Vector2 targetVelocity = (MyTarget.Center - NPC.Center);
                    targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
                    if(Timer % 30 == 0)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, -Vector2.UnitY * 12, ModContent.ProjectileType<BabyRichochetZap>(), Electric_Orbit_Damage, 1, Main.myPlayer);
                        }
                    }

                   // MoveTowardsPlayer();
                    NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
                    _outliner.attacking = true;
                  if(Timer >= 300)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    SwitchState(AIState.Idle);
                }
                break;
        }
    }
    private void AI_Death()
    {
        Timer++;
        if(Timer % 8 == 0)
        {
            Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(128, 128);
            Vector2 vel = (NPC.Center - pos);
            vel *= 0.06f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Color.SkyBlue;
        }

        if(Timer % 8 == 0)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, Scale: 0.5f);
        }

        ShakeModSystem.Shake = MathHelper.Lerp(0f, 4f, Timer / 160f);
        NPC.velocity *= 0.95f;
        NPC.rotation *= 0.95f;
        if(Timer >= 160)
        {
            if(Main.netMode != NetmodeID.Server)
            {
                int bodyGore1 = Mod.Find<ModGore>($"{Name}_Gore_0").Type;
                int bodyGore2 = Mod.Find<ModGore>($"{Name}_Gore_1").Type;
                int headGore = Mod.Find<ModGore>($"{Name}_Gore_2").Type;

                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(-16, 34), NPC.velocity, bodyGore1, 1f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(16, 34), NPC.velocity, bodyGore2);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore);
            }

            var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.Pink, Color.Purple, 40);
            fx.Scale *= 1.5f;
            for(int i = 0; i < 10; i++)
            {
                var spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.Pink;
                var dp = DustParticle.Spawn(NPC.Center, Main.rand.NextVector2Circular(16, 16), spawnParams);
                dp.noTileCollide = true;
                dp.dampening = 0.05f;
                dp.gravity = 0;
                dp.Scale *= 0.5f;
            }

            SoundStyle wetDeath = new SoundStyle("Stellamod/Assets/Sounds/WetDeath") with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(wetDeath, NPC.position);
            NPC.Kill();
        }
    }

    private void MoveTowardPlayer(float speed = 5f)
    {
        float distance = 24;
        Vector2 directionToTarget = NPC.Center.DirectionTo(MyTarget.Center);
        Vector2 initialSpeed = directionToTarget * speed;
        Vector2 offset = initialSpeed.RotatedBy(Math.PI / 2);
        offset.Normalize();
        offset *= (float)(Math.Cos(Timer * 3 * (Math.PI / 180)) * (distance / 3));

        Vector2 targetVelocity = initialSpeed + offset;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.04f);
    }
    private void AI_Idle()
    {
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
            _startPoint = NPC.Center;
            _controlPoint1 = _startPoint + new Vector2(384, 384);
            _controlPoint2 = MyTarget.Center + new Vector2(-384, -384);
            _targetPoint = MyTarget.Center;

        }



        MoveTowardPlayer(speed: 5);
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2 ;


        //NPC.velocity.X *= 0.98f;
        //float hoverVelocity = MathF.Sin(Timer * 0.5f) * 1f;
        //NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, hoverVelocity, 0.1f);
        if(Timer >= IdleTime)
        {
            ChooseAttack();
        }
    }
    private void AI_Despawn()
    {
        Timer++;
        NPC.velocity.Y -= 0.05f;
        NPC.rotation *= 0.9f;
        if(Timer >= 90)
        {
            NPC.active = false;
        }
    }
    private void AI_Spawn()
    {
        Timer++;
        if(Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                for(int i = 0; i < 7; i++)
                {
                    NPC.NewNPC(SourceFromThis, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BabyJellyfish>(), ai2: NPC.whoAmI, ai3: i);
                }
                
            }
            ShowNamePlate();
        }
        
        if(Timer >= 90)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void SimulateTentacles()
    {
        Chain.segmentLength = 1;
        Chain.points[0] = NPC.Center;
        Chain.pinned[0] = true;
        Chain.ResolveBackToRoot();

        for(int i = 0; i < TentacleChains.Length; i++)
        {
            var chain = TentacleChains[i];
            Vector2 right = NPC.rotation.ToRotationVector2();
            chain.segmentLength = 12;
            chain.points[0] = NPC.Center + right * ExtraMath.Osc(-32, 32, offset: i * 1.5f);
            chain.pinned[0] = true;
            chain.ResolveBackToRoot();
        }
    }

    private void DrawPixelatedTentacle(SpriteBatch sb, Vector2 screenPos)
    {
        DrawTentaclesInner(sb);
    }

    private float GetHairWidth(float ratio)
    {
        return MathHelper.SmoothStep(64, 48, ratio);
    }

    private Color GetHairColor(float ratio)
    {
        Color hairColor = Color.Lerp(Color.White, Main.DiscoColor, 0.2f);// * EasingFunction.OutExpo(ratio + 0.5f);
        hairColor *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(ratio));
        hairColor.A = 0;
        return hairColor;
    }

    private void DrawHair(GraphicsDevice gDevice)
    {
        HairShader shader = ShaderContent.GetInstance<HairShader>();
        shader.LaserTexture = TrailRegistry.StringTrail;
        shader.Time = Main.GlobalTimeWrappedHourly * 0.2f;
        shader.WaveFrequency = 4;
        shader.XOffset = 12;
        shader.WaveAmplitude = 0.2f;
        shader.BlendState = BlendState.AlphaBlend;
        TrailDrawer.Draw(Main.spriteBatch, Chain.points, GetHairColor, GetHairWidth, shader);
        
        shader.Time = Main.GlobalTimeWrappedHourly * 0.2f + 0.5f;
        TrailDrawer.Draw(Main.spriteBatch, Chain.points, GetHairColor, GetHairWidth, shader);
    }

    private void DrawTentaclesInner(SpriteBatch spriteBatch)
    {
        SpritebatchDrawer tentacleDraewr = SpritebatchDrawer.FromTextureAsset(_smallChainTextureAsset, NPC.Center);
        for (int j = 0; j < TentacleChains.Length; j++)
        {
            var chain = TentacleChains[j];
            for (int i = chain.points.Length - 1; i >= 1; i--)
            {
                float ratio = (float)i / (float)chain.points.Length;
                Vector2 worldPos = chain.points[i];
                tentacleDraewr.worldPosition = worldPos;
                float rot = (chain.points[i - 1] - chain.points[i]).ToRotation();
                tentacleDraewr.rotation = rot;
                Vector2 scale = Vector2.Lerp(Vector2.One * 1.7f, Vector2.One * 0.5f, ratio);
                tentacleDraewr.scale = scale;
                spriteBatch.Draw(tentacleDraewr);
            }
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        _glowTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Glow");
        _smallChainTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_TentacleSmall");
        _chainTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Tentacle");
        _magicCircleTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_MagicCircle");

        float dir = 1f;
        void DrawCircle(float progress, Vector2 size, Color color)
        {
            SpritebatchDrawer magicCircleDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.GothinMagicCircle, NPC.Center);
            magicCircleDrawer.color = Color.Lerp(Color.Black, color, progress) * 0.3f;
            magicCircleDrawer.color.A = 0;
            magicCircleDrawer.rotation = Main.GlobalTimeWrappedHourly * 0.4f * dir;
            magicCircleDrawer.scale = size * MathHelper.Lerp(1.5f, 1f, EasingFunction.InSine(progress));
            spriteBatch.Draw(magicCircleDrawer);

        }

        GlowingSwordMaskShader shader = GlowingSwordMaskShader.Instance;
        shader.TrailTexture = TrailRegistry.BulbTrail;
        shader.Distortion = 0.02f;
        shader.DistortionTexture = TrailRegistry.WhispyTrail;
        shader.Time = Main.GlobalTimeWrappedHourly * 16;
        shader.Bloom = 0.8f;
        shader.Tiling = Vector2.One * 0.75f;
        shader.InnerColor = Color.Lerp(Color.SkyBlue, Color.DarkBlue, ExtraMath.Osc(0f, 1f, 12)) * _magicCircleChargeProgress;
        shader.OuterColor = Color.DarkBlue;
        Main.spriteBatch.Restart(effect: shader.Effect);



        DrawCircle(_magicCircleChargeProgress, Vector2.One, Color.White);


        Main.spriteBatch.RestartDefaults();

        dir *= -1.5f;
        DrawCircle(_magicCircleChargeProgress2, Vector2.One * 1.5f, Color.SkyBlue);
        dir *= -1.5f;
        DrawCircle(_magicCircleChargeProgress3, Vector2.One * 2f, Color.DeepSkyBlue);

        PixelationManager.QueuePrimitivesDrawAction(DrawHair, DrawLayer.BehindNPCsWithOutline);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedTentacle, DrawLayer.BehindNPCsWithOutline);
        OutlineRenderer.Queue(DrawWhite);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, NPC.Center);
        glowDrawer.scale *= 0.5f;
        glowDrawer.color = Color.LightSkyBlue;
        glowDrawer.color *= 0.3f;
        glowDrawer.color.A = 0;
        spriteBatch.Draw(glowDrawer);
        
        SpritebatchDrawer headDrawer = SpritebatchDrawer.FromNPC(NPC);
        headDrawer.scale = Vector2.Lerp(Vector2.One, new Vector2(0.9f, 1.2f), ExtraMath.Osc(0f, 1f, 3));
        headDrawer.spriteEffects = SpriteEffects.None;
        spriteBatch.Draw(headDrawer);

        headDrawer.color *= ExtraMath.Osc(0.1f, 0.3f);
        headDrawer.color.A = 0;
        spriteBatch.Draw(headDrawer);

        SpritebatchDrawer glowLineDrawer = SpritebatchDrawer.FromTextureAsset(_glowTextureAsset, NPC.Center);
        glowLineDrawer.color = Color.Lerp(Color.Black, Color.White, ExtraMath.Osc(0f, 1f));
        glowLineDrawer.color *= 0.5f;
        glowLineDrawer.color.A = 0;
        glowLineDrawer.rotation = NPC.rotation;
        glowLineDrawer.scale = headDrawer.scale;
        glowLineDrawer.spriteEffects = SpriteEffects.None;
        spriteBatch.Draw(glowLineDrawer);


        headDrawer.color = Color.Black * _electricAlpha;
        spriteBatch.Draw(headDrawer);

        glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, NPC.Center);
        glowDrawer.scale *= 0.5f;
        glowDrawer.color = Color.LightSkyBlue * _electricAlpha * ExtraMath.Osc(0f, 1f, speed: 32);
     //   glowDrawer.color *= 0.3f;
        glowDrawer.color.A = 0;
        spriteBatch.Draw(glowDrawer);

        glowLineDrawer.color = Color.LightSkyBlue * _electricAlpha * ExtraMath.Osc(0f, 1f, speed: 32);
        //   glowDrawer.color *= 0.3f;
        glowLineDrawer.color.A = 0;
        spriteBatch.Draw(glowLineDrawer);


       // Lighting.AddLight(NPC.position, Color.White.ToVector3() * 0.6f);
        return false;
    }

    private void DrawWhite(SpriteBatch spriteBatch)
    {
        SpritebatchDrawer headDrawer = SpritebatchDrawer.FromNPC(NPC);
        headDrawer.color = _outliner.outlineColor;
        headDrawer.scale = Vector2.Lerp(Vector2.One, new Vector2(0.9f, 1.2f), ExtraMath.Osc(0f, 1f, 3));
        headDrawer.spriteEffects = SpriteEffects.None;
        spriteBatch.Draw(headDrawer);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if(NPC.life <= 0f)
        {
            if (State != AIState.Death)
                SwitchState(AIState.Death);
            NPC.life = 1;
        }
    }

    public override void OnKill()
    {
        base.OnKill();
        DownedBossTracker.ClearFlag(DownedBossFlag.KingJellyfish);
    }
}

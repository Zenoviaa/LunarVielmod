using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Content.Areas.WaterSide.KingJellyfishBoss;
using Stellamod.Core.Effects;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.Common.Shaders.PrimitiveTrailManager;

namespace Stellamod.Core.Utilities;

public static class PixelPrimitiveCircleFactory
{
    public static void CreateFlamingCircle(Vector2 position)
    {
        Color StripColors(TrailCircleStep progressOnStrip)
        {
            //  return Color.Lerp(Color.LightGoldenrodYellow, Color.White, Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            return Color.Lerp(Color.White, Color.Transparent, EasingFunction.InOutSine(progressOnStrip.progressInCircle));
        }

        float StripWidth(TrailCircleStep progressOnStrip)
        {
            float baseWidth = 64;
            return MathHelper.SmoothStep(baseWidth, baseWidth, progressOnStrip.progressInTrail) * MathHelper.Lerp(1f, 0.5f, EasingFunction.InOutSine(progressOnStrip.progressInCircle));
        }
        float StripWidth2(TrailCircleStep progressOnStrip)
        {
            return StripWidth(progressOnStrip) * 2f;
        }

        void RenderBloom(TrailCircleDraw draw)
        {
            BloomTrailShader bloomTrailShader = BloomTrailShader.Instance;
            bloomTrailShader.InnerColor = Color.OrangeRed;
            bloomTrailShader.OuterColor = Color.Red;
            DrawUtilities.DrawUserIndexedPrimitivesWithEffect(draw.vertices, draw.indices, bloomTrailShader);
        }
        void RenderFire(TrailCircleDraw draw)
        {

            BlackFireShader blackFireShader = BlackFireShader.Instance;
            blackFireShader.SetDefaults();
            blackFireShader.InnerEmitColor = Color.Yellow * 0.2f;
            blackFireShader.OuterEmiteColor = Color.Red;
            DrawUtilities.DrawUserIndexedPrimitivesWithEffect(draw.vertices, draw.indices, blackFireShader);
        }


        PixelCircleUpdater.Create(RenderBloom, position, StripWidth2, StripColors, minRadius: 8, maxRadius: 64, 24);
        PixelCircleUpdater.Create(RenderFire, position, StripWidth, StripColors, minRadius: 8, maxRadius: 64, 24);
    }
    public static void CreateClosingGustCircle(Vector2 position)
    {
        Color StripColors(TrailCircleStep progressOnStrip)
        {
            //  return Color.Lerp(Color.LightGoldenrodYellow, Color.White, Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            return Color.Lerp(Color.LightGray, Color.Transparent, progressOnStrip.progressInCircle) * 0.5f;
        }

        float StripWidth(TrailCircleStep progressOnStrip)
        {
            return 4;
        }

        void RenderTrail(TrailCircleDraw draw)
        {

            var shader = MagicRadianceShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
            shader.PrimaryColor = Color.Lerp(Color.White, Color.LightGray, 0.5f);
            shader.NoiseColor = Color.LightGray;
            shader.OutlineColor = Color.Transparent;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.15f;
            shader.Power = 0.25f;
            DrawUtilities.DrawUserIndexedPrimitivesWithEffect(draw.vertices, draw.indices, shader);
        }

        PixelCircleUpdater.Create(RenderTrail, position, StripWidth, StripColors, minRadius: 100, maxRadius: 0, 45);
    }

    public record struct FireBloomColors(Color fireInnerColor, Color fireOuterColor, Color fireBackColor, Color bloomInnerColor, Color bloomOuterColor);
    public static void CreateDefaultFireBloomCircle(Vector2 position, Func<TrailCircleStep, float> getTrailWidthFunction, Func<TrailCircleStep, Color> getTrailColorFunction,
         FireBloomColors colors, float minRadius, float maxRadius, float time)
    {
        void RenderBloom(TrailCircleDraw draw)
        {
            var bloomTrailShader = BloomTrailShader.Instance;
            bloomTrailShader.InnerColor = colors.bloomInnerColor;
            bloomTrailShader.OuterColor = colors.bloomOuterColor;
            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.BlendState = BlendState.Additive;
            DrawUtilities.DrawUserIndexedPrimitivesWithEffect(draw.vertices, draw.indices, bloomTrailShader);
        }
        void RenderFire(TrailCircleDraw draw)
        {
            var blackFireShader = BlackFireShader.Instance;
            blackFireShader.InnerColor = colors.fireInnerColor;
            blackFireShader.OuterColor = colors.fireOuterColor;
            blackFireShader.BackColor = colors.fireBackColor;
            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.BlendState = BlendState.Additive;
            DrawUtilities.DrawUserIndexedPrimitivesWithEffect(draw.vertices, draw.indices, blackFireShader);
        }
     
        PixelCircleUpdater.Create(RenderFire, position, getTrailWidthFunction, getTrailColorFunction, minRadius, maxRadius, time);
        PixelCircleUpdater.Create(RenderBloom, position, getTrailWidthFunction, getTrailColorFunction, minRadius, maxRadius, time);
    }
    public static void CreateDefaultFireBloomCircle(Entity parent, Func<TrailCircleStep, float> getTrailWidthFunction, Func<TrailCircleStep, Color> getTrailColorFunction,
         FireBloomColors colors, float minRadius, float maxRadius, float time)
    {
        void RenderBloom(TrailCircleDraw draw)
        {
            var bloomTrailShader = BloomTrailShader.Instance;
            bloomTrailShader.InnerColor = colors.bloomInnerColor;
            bloomTrailShader.OuterColor = colors.bloomOuterColor;
            DrawUtilities.DrawUserIndexedPrimitivesWithEffect(draw.vertices, draw.indices, bloomTrailShader);
        }
        void RenderFire(TrailCircleDraw draw)
        {
            var blackFireShader = BlackFireShader.Instance;
            blackFireShader.InnerColor = colors.fireInnerColor;
            blackFireShader.OuterColor = colors.fireOuterColor;
            blackFireShader.BackColor = colors.fireBackColor;
            DrawUtilities.DrawUserIndexedPrimitivesWithEffect(draw.vertices, draw.indices, blackFireShader);
        }
        var circle = PixelCircleUpdater.Create(RenderBloom, parent.Center, getTrailWidthFunction, getTrailColorFunction, minRadius, maxRadius, time);
        circle.parent = parent;
        circle = PixelCircleUpdater.Create(RenderFire, parent.Center, getTrailWidthFunction, getTrailColorFunction, minRadius, maxRadius, time);
        circle.parent = parent;
    }

    public static void CreateOrganBoom(Vector2 position)
    {
        float GetTrailWidthFunction(TrailCircleStep interpolant)
        {
            return MathHelper.SmoothStep(64, 0, interpolant.progressInCircle);
        }
        Color GetTrailColorFunction(TrailCircleStep interpolant)
        {
            Color lerp1 = Color.Lerp(Color.White, Color.LightGoldenrodYellow, ExtraMath.Osc(0.5f, 1f, speed: 8));
            lerp1 = Color.Lerp(lerp1, Color.DarkGoldenrod, interpolant.progressInCircle);
            return lerp1;
        }

        CreateDefaultFireBloomCircle(position, GetTrailWidthFunction, GetTrailColorFunction, new(Color.White, Color.LightGoldenrodYellow, Color.DarkGoldenrod, Color.Goldenrod, Color.DarkGoldenrod), 0, 500, 60);
    }
    public static void CreateVerliaMoonBoom(Vector2 position)
    {
        float GetTrailWidthFunction(TrailCircleStep interpolant)
        {
            return MathHelper.SmoothStep(64, 0, interpolant.progressInCircle);
        }
        Color GetTrailColorFunction(TrailCircleStep interpolant)
        {
            Color lerp1 = Color.Lerp(Color.White, Color.SkyBlue, ExtraMath.Osc(0.5f, 1f, speed: 8));
            lerp1 = Color.Lerp(Color.Blue, lerp1, interpolant.progressInCircle);
            return lerp1;
        }

        CreateDefaultFireBloomCircle(position, GetTrailWidthFunction, GetTrailColorFunction, new(Color.White, Color.DarkGray, Color.Black, Color.White, Color.Blue), 888, 0, 45);
    }
    public static void CreateVerliaMoonBoom2(Vector2 position)
    {
        float GetTrailWidthFunction(TrailCircleStep interpolant)
        {
            return MathHelper.SmoothStep(32, 0, interpolant.progressInCircle);
        }
        Color GetTrailColorFunction(TrailCircleStep interpolant)
        {
            Color lerp1 = Color.Lerp(Color.White, Color.SkyBlue, ExtraMath.Osc(0.5f, 1f, speed: 8));
            lerp1 = Color.Lerp(Color.Blue, lerp1, interpolant.progressInCircle);
            return lerp1;
        }
        CreateDefaultFireBloomCircle(position, GetTrailWidthFunction, GetTrailColorFunction, new(Color.White, Color.DarkGray, Color.Black, Color.White, Color.Blue), 444, 0, 45);
    }

    public static void CreateCariyaInMoon(Vector2 position)
    {
        float GetTrailWidthFunction(TrailCircleStep interpolant)
        {
            return MathHelper.SmoothStep(32, 0, interpolant.progressInCircle);
        }
        Color GetTrailColorFunction(TrailCircleStep interpolant)
        {
            Color lerp1 = Color.Lerp(Color.White, Color.SkyBlue, ExtraMath.Osc(0.5f, 1f, speed: 8));
            lerp1 = Color.Lerp(Color.Blue, lerp1, interpolant.progressInCircle);
            return lerp1;
        }
        CreateDefaultFireBloomCircle(position, GetTrailWidthFunction, GetTrailColorFunction, new(Color.White, Color.DarkGray, Color.Black, Color.White, Color.Blue), 100, 0, 45);
    }
    public static void CreateInGoldBoom(Vector2 position)
    {
        float GetTrailWidthFunction(TrailCircleStep interpolant)
        {
            return MathHelper.SmoothStep(8, 0, interpolant.progressInCircle);
        }
        Color GetTrailColorFunction(TrailCircleStep interpolant)
        {
            Color lerp1 = Color.Lerp(Color.White, Color.Gold, ExtraMath.Osc(0.5f, 1f, speed: 8));
            lerp1 = Color.Lerp(Color.Gold, lerp1, interpolant.progressInCircle);
            lerp1 = Color.Lerp(Color.Transparent, lerp1, interpolant.progressInCircle);
            return lerp1;
        }

        CreateDefaultFireBloomCircle(position, GetTrailWidthFunction, GetTrailColorFunction, new(Color.White, Color.Goldenrod, Color.Black, Color.White, Color.DarkGoldenrod), 100, 0, 15);
    }
    public static void CreateEelInSuck(Vector2 position)
    {
        float GetTrailWidthFunction(TrailCircleStep interpolant)
        {
            return MathHelper.SmoothStep(32, 0, interpolant.progressInCircle);
        }
        Color GetTrailColorFunction(TrailCircleStep interpolant)
        {
            Color lerp1 = Color.Lerp(Color.White, Color.SkyBlue, ExtraMath.Osc(0.5f, 1f, speed: 8));
            lerp1 = Color.Lerp(Color.Blue, lerp1, interpolant.progressInCircle);
            return lerp1;
        }
        CreateDefaultFireBloomCircle(position, GetTrailWidthFunction, GetTrailColorFunction, new(Color.White, Color.DarkGray, Color.Black, Color.White, Color.Blue), 666, 0, 45);
    }

    public static void CreateInWhiteSuck(Vector2 position)
    {
        float GetTrailWidthFunction(TrailCircleStep interpolant)
        {
            return MathHelper.SmoothStep(32, 0, interpolant.progressInCircle);
        }
        Color GetTrailColorFunction(TrailCircleStep interpolant)
        {
            Color lerp1 = Color.Lerp(Color.White, Color.LightGray, ExtraMath.Osc(0.5f, 1f, speed: 8));
            lerp1 = Color.Lerp(Color.Gray, lerp1, interpolant.progressInCircle);
            return lerp1;
        }

        CreateDefaultFireBloomCircle(position, GetTrailWidthFunction, GetTrailColorFunction, new(Color.White, Color.DarkGray, Color.Black, Color.White, Color.DarkGray), 333, 0, 25);
    }
    public static void CreateEelInSuckQuick(Vector2 position)
    {
        float GetTrailWidthFunction(TrailCircleStep interpolant)
        {
            return MathHelper.SmoothStep(32, 0, interpolant.progressInCircle);
        }
        Color GetTrailColorFunction(TrailCircleStep interpolant)
        {
            Color lerp1 = Color.Lerp(Color.White, Color.SkyBlue, ExtraMath.Osc(0.5f, 1f, speed: 8));
            lerp1 = Color.Lerp(Color.Blue, lerp1, interpolant.progressInCircle);
            return lerp1;
        }
        CreateDefaultFireBloomCircle(position, GetTrailWidthFunction, GetTrailColorFunction, new(Color.White, Color.DarkGray, Color.Black, Color.White, Color.Blue), 666, 0, 25);
    }
    public static void CreateEreshkigalSuck(Entity parent)
    {
        float GetTrailWidthFunction(TrailCircleStep interpolant)
        {
            return MathHelper.SmoothStep(0, 32, interpolant.progressInCircle);
        }
        Color GetTrailColorFunction(TrailCircleStep interpolant)
        {
            Color lerp1 = Color.Lerp(Color.White, Color.Goldenrod, ExtraMath.Osc(0.5f, 1f, speed: 8));
            lerp1 = Color.Lerp(Color.Blue, lerp1, interpolant.progressInCircle);
            lerp1 = Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant.progressInCircle));
            return lerp1;
        }

        CreateDefaultFireBloomCircle(parent, GetTrailWidthFunction, GetTrailColorFunction, new(Color.White, Color.DarkGray, Color.Black, Color.White, Color.Goldenrod), 333, 0, 30);
    }
    public static void CreateEelSiningSuck(Entity parent)
    {
        float GetTrailWidthFunction(TrailCircleStep interpolant)
        {
            return MathHelper.SmoothStep(0, 64, interpolant.progressInCircle);
        }
        Color GetTrailColorFunction(TrailCircleStep interpolant)
        {
            Color lerp1 = Color.Lerp(Color.White, Color.Goldenrod, ExtraMath.Osc(0.5f, 1f, speed: 8));
            lerp1 = Color.Lerp(Color.Blue, lerp1, interpolant.progressInCircle);
            return lerp1;
        }

        CreateDefaultFireBloomCircle(parent, GetTrailWidthFunction, GetTrailColorFunction, new(Color.White, Color.DarkGray, Color.Black, Color.White, Color.Goldenrod), 666, 0, 25);
    }
    public static void CreateHeavenlyBoom(Vector2 position)
    {
        float GetTrailWidthFunction(TrailCircleStep interpolant)
        {
            return MathHelper.SmoothStep(64, 0, interpolant.progressInCircle);
        }
        Color GetTrailColorFunction(TrailCircleStep interpolant)
        {
            Color lerp1 = Color.Lerp(Color.White, Color.LightGoldenrodYellow, ExtraMath.Osc(0.5f, 1f, speed: 8));
            lerp1 = Color.Lerp(lerp1, Color.DarkGoldenrod, interpolant.progressInCircle);
            return lerp1;
        }

        CreateDefaultFireBloomCircle(position, GetTrailWidthFunction, GetTrailColorFunction, new(Color.White, Color.LightGoldenrodYellow, Color.DarkGoldenrod, Color.Goldenrod, Color.DarkGoldenrod), 0, 100, 25);
    }
    public static void CreateMoonBoom(Vector2 position)
    {
        float GetTrailWidthFunction(TrailCircleStep interpolant)
        {
            return MathHelper.SmoothStep(64, 0, interpolant.progressInCircle);
        }
        Color GetTrailColorFunction(TrailCircleStep interpolant)
        {
            Color lerp1 = Color.Lerp(Color.White, Color.Aquamarine, ExtraMath.Osc(0.5f, 1f, speed: 8));
            lerp1 = Color.Lerp(lerp1, Color.DarkBlue, interpolant.progressInCircle);
            return lerp1;
        }
        CreateDefaultFireBloomCircle(position, GetTrailWidthFunction, GetTrailColorFunction, new(Color.White, Color.Aquamarine, Color.DarkBlue, Color.Black, Color.Black), 0, 100, 45);
    }
    public static void CreateGenericInBoom(Vector2 position, Color startColor, Color endColor, float time, float endRadius)
    {
        float GetTrailWidthFunction(TrailCircleStep interpolant)
        {
            return MathHelper.SmoothStep(32, 0, interpolant.progressInCircle);
        }
        Color GetTrailColorFunction(TrailCircleStep interpolant)
        {
            Color lerp1 = Color.Lerp(startColor, endColor, ExtraMath.Osc(0.5f, 1f, speed: 8));
            lerp1 = Color.Lerp(lerp1, Color.Lerp(endColor, Color.Black, 0.75f), interpolant.progressInCircle);
            return lerp1;
        }

        CreateDefaultFireBloomCircle(position, GetTrailWidthFunction, GetTrailColorFunction, new(startColor, endColor, Color.Lerp(endColor, Color.Black, 0.5f), startColor, endColor), endRadius, 0, time);
    }
    public static void CreateGenericBoom(Vector2 position, Color startColor, Color endColor, float time, float endRadius)
    {
        float GetTrailWidthFunction(TrailCircleStep interpolant)
        {
            return MathHelper.SmoothStep(64, 0, interpolant.progressInCircle);
        }
        Color GetTrailColorFunction(TrailCircleStep interpolant)
        {
            Color lerp1 = Color.Lerp(startColor, endColor, ExtraMath.Osc(0.5f, 1f, speed: 8));
            lerp1 = Color.Lerp(lerp1, Color.Lerp(endColor, Color.Black, 0.75f), interpolant.progressInCircle);
            return lerp1;
        }

        CreateDefaultFireBloomCircle(position, GetTrailWidthFunction, GetTrailColorFunction, new(startColor, endColor, Color.Lerp(endColor, Color.Black, 0.5f), startColor, endColor), 0, endRadius, time);
    }
    public static void CreateElectricBoom(Vector2 position)
    {
        float GetTrailWidthFunction(TrailCircleStep interpolant)
        {
            return MathHelper.SmoothStep(256, 0, interpolant.progressInCircle);
        }
        Color GetTrailColorFunction(TrailCircleStep interpolant)
        {
            return Color.White;
        }
        void Render(TrailCircleDraw draw)
        {
            ZapLightningShader lightingShader = ZapLightningShader.Instance;
            lightingShader.Amplitude = 0.2f;
            float time = Main.GlobalTimeWrappedHourly * 16;
            float levels = 4;
            time = MathF.Floor(time * levels) / levels;
            lightingShader.Time = time;
            Asset<Texture2D> laserTexture = AssetManager.LaserTextures.TexturedLaser2;
            lightingShader.LaserTexture = laserTexture;
            lightingShader.Noise = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BlurryPerlinNoise").Value;
            lightingShader.Gradient = ModContent.Request<Texture2D>(ModContent.GetInstance<ZapShockwave>().Texture + "_Gradient").Value;
            lightingShader.TransformMatrix = TrailDrawer.WorldViewPoint2;
            lightingShader.Levels = 64;
            lightingShader.Tiling = new Vector2(2f);
            DrawUtilities.DrawUserIndexedPrimitivesWithEffect(draw.vertices, draw.indices, lightingShader);
        }

        PixelCircleUpdater.Create(Render, position, GetTrailWidthFunction, GetTrailColorFunction, 0, 384, 45);
    }
    public static void CreateElectricInwardBoom(Vector2 position)
    {
        float GetTrailWidthFunction(TrailCircleStep interpolant)
        {
            return MathHelper.SmoothStep(8, 0, interpolant.progressInCircle);
        }
        Color GetTrailColorFunction(TrailCircleStep interpolant)
        {
            return Color.White * 0.5f;
        }
        void Render(TrailCircleDraw draw)
        {
            ZapLightningShader lightingShader = ZapLightningShader.Instance;
            lightingShader.Amplitude = 0.2f;

            float time = Main.GlobalTimeWrappedHourly * 16;
            float levels = 4;
            time = MathF.Floor(time * levels) / levels;
            lightingShader.Time = time;
            Asset<Texture2D> laserTexture = AssetManager.LaserTextures.TexturedLaser2;
            lightingShader.LaserTexture = laserTexture;
            lightingShader.Noise = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BlurryPerlinNoise").Value;
            lightingShader.Gradient = ModContent.Request<Texture2D>(ModContent.GetInstance<ZapShockwave>().Texture + "_Gradient").Value;
            lightingShader.TransformMatrix = TrailDrawer.WorldViewPoint2;
            lightingShader.Levels = 64;
            lightingShader.Tiling = new Vector2(2f);
            DrawUtilities.DrawUserIndexedPrimitivesWithEffect(draw.vertices, draw.indices, lightingShader);
        }

        PixelCircleUpdater.Create(Render, position, GetTrailWidthFunction, GetTrailColorFunction, 252, 0, 45);
    }
    public static void CreatePunkerBoom(Vector2 position)
    {
        float GetTrailWidthFunction(TrailCircleStep interpolant)
        {
            return MathHelper.SmoothStep(128, 0, interpolant.progressInCircle);
        }
        Color GetTrailColorFunction(TrailCircleStep interpolant)
        {
            Color lerp1 = Color.Lerp(Color.White, Color.Red, ExtraMath.Osc(0.5f, 1f, speed: 8));
            lerp1 = Color.Lerp(lerp1, Color.DarkRed, interpolant.progressInCircle);
            return lerp1;
        }
        void Render(TrailCircleDraw draw)
        {
            BlackFireShader blackFireShader = BlackFireShader.Instance;
            blackFireShader.InnerColor = Color.White;
            blackFireShader.OuterColor = Color.Red;
            blackFireShader.BackColor = Color.DarkRed;
            blackFireShader.PrimaryTexture2 = AssetManager.LaserTextures.Lightning2;
            blackFireShader.InnerEmitColor = Color.White;
            blackFireShader.OuterEmiteColor = Color.White;
            DrawUtilities.DrawUserIndexedPrimitivesWithEffect(draw.vertices, draw.indices, blackFireShader);
        }


        PixelCircleUpdater.Create(Render, position, GetTrailWidthFunction, GetTrailColorFunction, 0, 450, 38);
    }

    public static void CreateSTARRBoom(Vector2 position)
    {
        float GetTrailWidthFunction(TrailCircleStep interpolant)
        {
            return MathHelper.SmoothStep(90, 0, EasingFunction.InSine(interpolant.progressInCircle));
        }
        Color GetTrailColorFunction(TrailCircleStep interpolant)
        {
            Color lerp1 = Color.Lerp(Color.White, Color.LightBlue, ExtraMath.Osc(0.5f, 1f, speed: 16));
            lerp1 = Color.Lerp(lerp1, Color.Black, EasingFunction.InExpo(interpolant.progressInCircle));
            return lerp1;
        }

        CreateDefaultFireBloomCircle(position, GetTrailWidthFunction, GetTrailColorFunction, new(Color.White, Color.Gray, Color.DarkTurquoise, Color.White, Color.DarkGray), 0, 400, 25);
    }

    public static void CreateCelestiaBoom(Vector2 position)
    {
        float GetTrailWidthFunction(TrailCircleStep interpolant)
        {
            return MathHelper.SmoothStep(180, 0, EasingFunction.InSine(interpolant.progressInCircle));
        }
        Color GetTrailColorFunction(TrailCircleStep interpolant)
        {
            Color lerp1 = Color.Lerp(Color.White, Color.Turquoise, ExtraMath.Osc(0.5f, 1f, speed: 16));
            lerp1 = Color.Lerp(lerp1, Color.Black, EasingFunction.InExpo(interpolant.progressInCircle));
            return lerp1;
        }

        CreateDefaultFireBloomCircle(position, GetTrailWidthFunction, GetTrailColorFunction, new(Color.White, Color.Turquoise, Color.DarkTurquoise, Color.White, Color.DarkTurquoise), 0, 400, 45);
    }

    public static void CreateRekInwardBoom(Vector2 position)
    {
        float GetTrailWidthFunction(TrailCircleStep interpolant)
        {
            return MathHelper.SmoothStep(32, 0, EasingFunction.InSine(interpolant.progressInCircle));
        }
        Color GetTrailColorFunction(TrailCircleStep interpolant)
        {
            Color lerp1 = Color.Lerp(Color.White, Color.Gold, ExtraMath.Osc(0.5f, 1f, speed: 16));
            lerp1 = Color.Lerp(lerp1, Color.Black, EasingFunction.InExpo(interpolant.progressInCircle));
            return lerp1;
        }
        void RenderBloom(TrailCircleDraw draw)
        {
            var bloomTrail = BloomTrailShader.Instance;
            bloomTrail.InnerColor = Color.White;
            bloomTrail.OuterColor = Color.DarkRed;
            DrawUtilities.DrawUserIndexedPrimitivesWithEffect(draw.vertices, draw.indices, bloomTrail);
        }
        void RenderFire(TrailCircleDraw draw)
        {
            var shader = BlackFireShader.Instance;
            shader.InnerColor = Color.White;
            shader.OuterColor = Color.Gold;
            shader.BackColor = Color.DarkRed;
            shader.PrimaryTexture2 = AssetManager.LaserTextures.Lightning2;
            DrawUtilities.DrawUserIndexedPrimitivesWithEffect(draw.vertices, draw.indices, shader);
        }

        PixelCircleUpdater.Create(RenderBloom, position, GetTrailWidthFunction, GetTrailColorFunction, minRadius: 400, maxRadius: 0, 24);
        PixelCircleUpdater.Create(RenderFire, position, GetTrailWidthFunction, GetTrailColorFunction, minRadius: 400, maxRadius: 0, 24);
    }

    public static void CreateGothInwardBoom(Vector2 position)
    {
        float GetTrailWidthFunction(TrailCircleStep step)
        {
            return MathHelper.SmoothStep(16, 0, EasingFunction.InSine(step.progressInCircle));
        }
        Color GetTrailColorFunction(TrailCircleStep step)
        {
            Color lerp1 = Color.Lerp(Color.White, Color.Turquoise, ExtraMath.Osc(0.5f, 1f, speed: 16));
            lerp1 = Color.Lerp(lerp1, Color.Black, EasingFunction.InExpo(step.progressInCircle));
            return lerp1;
        }
        void RenderBloom(TrailCircleDraw draw)
        {
            var shader = BloomTrailShader.Instance;
            shader.InnerColor = Color.White;
            shader.OuterColor = Color.DarkTurquoise;
            DrawUtilities.DrawUserIndexedPrimitivesWithEffect(draw.vertices, draw.indices, shader);
        }
        void RenderFire(TrailCircleDraw draw)
        {
            var shader = BlackFireShader.Instance;
            shader.InnerColor = Color.White;
            shader.OuterColor = Color.Turquoise;
            shader.BackColor = Color.DarkTurquoise;
            shader.PrimaryTexture2 = AssetManager.LaserTextures.Lightning2;
            DrawUtilities.DrawUserIndexedPrimitivesWithEffect(draw.vertices, draw.indices, shader);
        }

        PixelCircleUpdater.Create(RenderBloom, position, GetTrailWidthFunction, GetTrailColorFunction, minRadius: 212, maxRadius: 0, 25);
        PixelCircleUpdater.Create(RenderFire, position, GetTrailWidthFunction, GetTrailColorFunction, minRadius: 212, maxRadius: 0, 25);
    }
    public static void CreateCelestiaInwardBoom(Vector2 position)
    {
        float GetTrailWidthFunction(TrailCircleStep step)
        {
            return MathHelper.SmoothStep(32, 0, EasingFunction.InSine(step.progressInCircle));
        }

        Color GetTrailColorFunction(TrailCircleStep step)
        {
            Color lerp1 = Color.Lerp(Color.White, Color.Turquoise, ExtraMath.Osc(0.5f, 1f, speed: 16));
            lerp1 = Color.Lerp(lerp1, Color.Black, EasingFunction.InExpo(step.progressInCircle));
            return lerp1;
        }

        void RenderPrimitivesBloom(TrailCircleDraw draw)
        {
            var shader = BloomTrailShader.Instance;
            shader.InnerColor = Color.White;
            shader.OuterColor = Color.DarkTurquoise;
            DrawUtilities.DrawUserIndexedPrimitivesWithEffect(draw.vertices, draw.indices, shader);
        }

        void RenderPrimitivesFire(TrailCircleDraw draw)
        {
            var shader = BlackFireShader.Instance;
            shader.InnerColor = Color.White;
            shader.OuterColor = Color.Turquoise;
            shader.BackColor = Color.DarkTurquoise;
            shader.PrimaryTexture2 = AssetManager.LaserTextures.Lightning2;
            DrawUtilities.DrawUserIndexedPrimitivesWithEffect(draw.vertices, draw.indices, shader);
        }

        PixelCircleUpdater.Create(RenderPrimitivesBloom, position, GetTrailWidthFunction, GetTrailColorFunction, minRadius: 400, maxRadius: 0, 45);
        PixelCircleUpdater.Create(RenderPrimitivesFire, position, GetTrailWidthFunction, GetTrailColorFunction, minRadius: 400, maxRadius: 0, 45);
    }
}

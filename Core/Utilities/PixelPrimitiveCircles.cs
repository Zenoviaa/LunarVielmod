using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Buffers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities;

public struct PixelPrimitiveCircleParams
{
    public float time;
    public float minRadius;
    public float maxRadius;
}

public static class PixelPrimitiveCircleFactory
{
    public static void CreateFlamingCircle(Vector2 position)
    {
        void RenderPrimitives(Vector2[] points, float completionRatio, in PixelPrimitiveCircleParams circleParams)
        {
            Color StripColors(float progressOnStrip)
            {
                //  return Color.Lerp(Color.LightGoldenrodYellow, Color.White, Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
                return Color.Lerp(Color.White, Color.Transparent, EasingFunction.InOutSine(completionRatio));
            }

            float StripWidth(float progressOnStrip)
            {
                float baseWidth = 64;
                return MathHelper.SmoothStep(baseWidth, baseWidth, progressOnStrip) * MathHelper.Lerp(1f, 0.5f, EasingFunction.InOutSine(completionRatio));
            }
            float StripWidth2(float progressOnStrip)
            {
                return StripWidth(completionRatio) * 2f;
            }

            BlackFireShader blackFireShader = BlackFireShader.Instance;
            blackFireShader.SetDefaults();
            blackFireShader.InnerEmitColor = Color.Yellow * 0.2f;
            blackFireShader.OuterEmiteColor = Color.Red;
            TrailDrawer.Draw(Main.spriteBatch, points, StripColors, StripWidth, blackFireShader);

            BloomTrailShader bloomTrailShader = BloomTrailShader.Instance;
            bloomTrailShader.InnerColor = Color.OrangeRed;
            bloomTrailShader.OuterColor = Color.Red;
            TrailDrawer.Draw(Main.spriteBatch, points, StripColors, StripWidth2, bloomTrailShader);
        }
        PixelPrimitiveCircle circle = new PixelPrimitiveCircle();
        circle.circleParams.minRadius = 8;
        circle.circleParams.maxRadius = 64;
        circle.circleParams.time = 25;
        circle.renderPixelPrimitivesFunction = RenderPrimitives;
        circle.position = position;
    
        ModContent.GetInstance<PixelPrimitiveCircleSystem>().Add(circle);
    }
    public static void CreateClosingGustCircle(Vector2 position)
    {
        void RenderPrimitives(Vector2[] points, float completionRatio, in PixelPrimitiveCircleParams circleParams)
        {
            Color StripColors(float progressOnStrip)
            {
                //  return Color.Lerp(Color.LightGoldenrodYellow, Color.White, Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
                return Color.Lerp(Color.LightGray, Color.Transparent, completionRatio) * 0.5f;
            }

            float StripWidth(float progressOnStrip)
            {
                float baseWidth = 4;
                return MathHelper.SmoothStep(baseWidth, baseWidth, progressOnStrip);
            }

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
            TrailDrawer.Draw(Main.spriteBatch, points, StripColors, StripWidth, shader);
        }
        PixelPrimitiveCircle circle = new PixelPrimitiveCircle();
        circle.circleParams.minRadius = 100;
        circle.circleParams.maxRadius = 0;
        circle.circleParams.time = 45;
        circle.renderPixelPrimitivesFunction = RenderPrimitives;
        circle.position = position;
        ModContent.GetInstance<PixelPrimitiveCircleSystem>().Add(circle);
    }
    public static void CreateOrganBoom(Vector2 position)
    {
        void RenderPrimitives(Vector2[] points, float completionRatio, in PixelPrimitiveCircleParams circleParams)
        {
            float GetTrailWidthFunction(float interpolant)
            {
                return MathHelper.SmoothStep(64, 0, completionRatio);
            }
            ;
            Color GetTrailColorFunction(float interpolant)
            {
                Color lerp1 = Color.Lerp(Color.White, Color.LightGoldenrodYellow, ExtraMath.Osc(0.5f, 1f, speed: 8));
                lerp1 = Color.Lerp(lerp1, Color.DarkGoldenrod, completionRatio);
                return lerp1;
            }
            ;
            BlackFireShader blackFireShader = BlackFireShader.Instance;
            blackFireShader.InnerColor = Color.White;
            blackFireShader.OuterColor = Color.LightGoldenrodYellow;
            blackFireShader.BackColor = Color.DarkGoldenrod;
            TrailDrawer.Draw(Main.spriteBatch, points, GetTrailColorFunction, GetTrailWidthFunction, blackFireShader);
            BloomTrailShader bloomTrail = BloomTrailShader.Instance;
            bloomTrail.InnerColor = Color.Goldenrod;
            bloomTrail.OuterColor = Color.DarkGoldenrod;
            TrailDrawer.Draw(Main.spriteBatch, points, GetTrailColorFunction, GetTrailWidthFunction, bloomTrail);
        }
        PixelPrimitiveCircle circle = new PixelPrimitiveCircle();
        circle.circleParams.minRadius = 0;
        circle.circleParams.maxRadius = 500;
        circle.circleParams.time = 60;
        circle.renderPixelPrimitivesFunction = RenderPrimitives;
        circle.position = position;
        ModContent.GetInstance<PixelPrimitiveCircleSystem>().Add(circle);
    }
    public static void CreateVerliaMoonBoom(Vector2 position)
    {
        void RenderPrimitives(Vector2[] points, float completionRatio, in PixelPrimitiveCircleParams circleParams)
        {
            float GetTrailWidthFunction(float interpolant)
            {
                return MathHelper.SmoothStep(64, 0, completionRatio);
            }
            ;
            Color GetTrailColorFunction(float interpolant)
            {
                Color lerp1 = Color.Lerp(Color.White, Color.SkyBlue, ExtraMath.Osc(0.5f, 1f, speed: 8));
                lerp1 = Color.Lerp(Color.Blue, lerp1, completionRatio);
                return lerp1;
            }
            ;
            BlackFireShader blackFireShader = BlackFireShader.Instance;
            blackFireShader.InnerColor = Color.White;
            blackFireShader.OuterColor = Color.DarkGray;
            blackFireShader.BackColor = Color.Black;
            TrailDrawer.Draw(Main.spriteBatch, points, GetTrailColorFunction, GetTrailWidthFunction, blackFireShader);
            BloomTrailShader bloomTrail = BloomTrailShader.Instance;
            bloomTrail.InnerColor = Color.White;
            bloomTrail.OuterColor = Color.Blue;
            TrailDrawer.Draw(Main.spriteBatch, points, GetTrailColorFunction, GetTrailWidthFunction, bloomTrail);
        }
        PixelPrimitiveCircle circle = new PixelPrimitiveCircle();
        circle.circleParams.minRadius = 0;
        circle.circleParams.maxRadius = 100;
        circle.circleParams.time = 25;
        circle.renderPixelPrimitivesFunction = RenderPrimitives;
        circle.position = position;
        ModContent.GetInstance<PixelPrimitiveCircleSystem>().Add(circle);
    }
    public static void CreateHeavenlyBoom(Vector2 position)
    {
        void RenderPrimitives(Vector2[] points, float completionRatio, in PixelPrimitiveCircleParams circleParams)
        {
            float GetTrailWidthFunction(float interpolant)
            {
                return MathHelper.SmoothStep(64, 0, completionRatio);
            }
            ;
            Color GetTrailColorFunction(float interpolant)
            {
                Color lerp1 = Color.Lerp(Color.White, Color.LightGoldenrodYellow, ExtraMath.Osc(0.5f, 1f, speed: 8));
                lerp1 = Color.Lerp(lerp1, Color.DarkGoldenrod, completionRatio);
                return lerp1;
            }
            ;
            BlackFireShader blackFireShader = BlackFireShader.Instance;
            blackFireShader.InnerColor = Color.White;
            blackFireShader.OuterColor = Color.LightGoldenrodYellow;
            blackFireShader.BackColor = Color.DarkGoldenrod;
            TrailDrawer.Draw(Main.spriteBatch, points, GetTrailColorFunction, GetTrailWidthFunction, blackFireShader);
            BloomTrailShader bloomTrail = BloomTrailShader.Instance;
            bloomTrail.InnerColor = Color.Goldenrod;
            bloomTrail.OuterColor = Color.DarkGoldenrod;
            TrailDrawer.Draw(Main.spriteBatch, points, GetTrailColorFunction, GetTrailWidthFunction, bloomTrail);
        }
        PixelPrimitiveCircle circle = new PixelPrimitiveCircle();
        circle.circleParams.minRadius = 0;
        circle.circleParams.maxRadius = 100;
        circle.circleParams.time = 25;
        circle.renderPixelPrimitivesFunction = RenderPrimitives;
        circle.position = position;
        ModContent.GetInstance<PixelPrimitiveCircleSystem>().Add(circle);
    }
    public static void CreateMoonBoom(Vector2 position)
    {
        void RenderPrimitives(Vector2[] points, float completionRatio, in PixelPrimitiveCircleParams circleParams)
        {
            float GetTrailWidthFunction(float interpolant)
            {
                return MathHelper.SmoothStep(64, 0, completionRatio);
            }
            ;
            Color GetTrailColorFunction(float interpolant)
            {
                Color lerp1 = Color.Lerp(Color.White, Color.Aquamarine, ExtraMath.Osc(0.5f, 1f, speed: 8));
                lerp1 = Color.Lerp(lerp1, Color.DarkBlue, completionRatio);
                return lerp1;
            }
            ;
            BlackFireShader blackFireShader = BlackFireShader.Instance;
            blackFireShader.InnerColor = Color.White;
            blackFireShader.OuterColor = Color.Aquamarine;
            blackFireShader.BackColor = Color.DarkBlue;
            TrailDrawer.Draw(Main.spriteBatch, points, GetTrailColorFunction, GetTrailWidthFunction, blackFireShader);
        }
        PixelPrimitiveCircle circle = new PixelPrimitiveCircle();
        circle.circleParams.minRadius = 888;
        circle.circleParams.maxRadius = 0;
        circle.circleParams.time = 45;
        circle.renderPixelPrimitivesFunction = RenderPrimitives;
        circle.position = position;
        ModContent.GetInstance<PixelPrimitiveCircleSystem>().Add(circle);
    }
    public static void CreatePunkerBoom(Vector2 position)
    {
        void RenderPrimitives(Vector2[] points, float completionRatio, in PixelPrimitiveCircleParams circleParams)
        {
            float GetTrailWidthFunction(float interpolant)
            {
                return MathHelper.SmoothStep(64, 0, completionRatio);
            }
            ;
            Color GetTrailColorFunction(float interpolant)
            {
                Color lerp1 = Color.Lerp(Color.White, Color.Red, ExtraMath.Osc(0.5f, 1f, speed: 8));
                lerp1 = Color.Lerp(lerp1, Color.DarkRed, completionRatio);
                return lerp1;
            }
            ;
            BlackFireShader blackFireShader = BlackFireShader.Instance;
            blackFireShader.InnerColor = Color.White;
            blackFireShader.OuterColor = Color.Red;
            blackFireShader.BackColor = Color.DarkRed;
            blackFireShader.PrimaryTexture2 = AssetManager.LaserTextures.Lightning2;
            TrailDrawer.Draw(Main.spriteBatch, points, GetTrailColorFunction, GetTrailWidthFunction, blackFireShader);
        }
        PixelPrimitiveCircle circle = new PixelPrimitiveCircle();
        circle.circleParams.minRadius = 0;
        circle.circleParams.maxRadius = 450;
        circle.circleParams.time = 19;
        circle.renderPixelPrimitivesFunction = RenderPrimitives;
        circle.position = position;
        ModContent.GetInstance<PixelPrimitiveCircleSystem>().Add(circle);
    }
    public static void CreateClockworkBoom(Vector2 position)
    {
        void RenderPrimitives(Vector2[] points, float completionRatio, in PixelPrimitiveCircleParams circleParams)
        {
            float GetTrailWidthFunction(float interpolant)
            {
                return MathHelper.SmoothStep(64, 0, completionRatio);
            }
            ;
            Color GetTrailColorFunction(float interpolant)
            {
                Color lerp1 = Color.Lerp(Color.White, Color.Turquoise, ExtraMath.Osc(0.5f, 1f, speed: 8));
                lerp1 = Color.Lerp(lerp1, Color.DarkTurquoise, completionRatio);
                return lerp1;
            }
            ;
            BlackFireShader blackFireShader = BlackFireShader.Instance;
            blackFireShader.InnerColor = Color.White;
            blackFireShader.OuterColor = Color.Turquoise;
            blackFireShader.BackColor = Color.DarkTurquoise;
            blackFireShader.PrimaryTexture2 = AssetManager.LaserTextures.Lightning2;
            TrailDrawer.Draw(Main.spriteBatch, points, GetTrailColorFunction, GetTrailWidthFunction, blackFireShader);
        }
        PixelPrimitiveCircle circle = new PixelPrimitiveCircle();
        circle.circleParams.minRadius = 0;
        circle.circleParams.maxRadius = 100;
        circle.circleParams.time = 45;
        circle.renderPixelPrimitivesFunction = RenderPrimitives;
        circle.position = position;
        ModContent.GetInstance<PixelPrimitiveCircleSystem>().Add(circle);
    }
}

public class PixelPrimitiveCircleSystem : ModSystem
{
    private List<PixelPrimitiveCircle> _pixelPrimitiveCircles;
    public override void Load()
    {
        base.Load();
        _pixelPrimitiveCircles = new List<PixelPrimitiveCircle>(16);
        On_Main.DrawProjectiles += QueueCircleDraws;
    }
    public override void Unload()
    {
        base.Unload();
        _pixelPrimitiveCircles.Clear();
        _pixelPrimitiveCircles = null;
        On_Main.DrawProjectiles -= QueueCircleDraws;
    }
    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();
        foreach (var item in _pixelPrimitiveCircles)
        {
            item.Update();
        }
        _pixelPrimitiveCircles.RemoveAll(x => !x.active);
    }
    private void QueueCircleDraws(On_Main.orig_DrawProjectiles orig, Main self)
    {
        orig(self);
        foreach (var item in _pixelPrimitiveCircles)
        {
            item.QueueDraw();
        }
    }



    public void Add(PixelPrimitiveCircle circle)
    {
        circle.active = true;
        _pixelPrimitiveCircles.Add(circle);
    }
}
public class PixelPrimitiveCircle
{
    public delegate void CalculateCirclePoints(ref Vector2[] points, in PixelPrimitiveCircleParams circleParams);
    public delegate void RenderPixelPrimitivesInner(Vector2[] points, float completionRatio, in PixelPrimitiveCircleParams circleParams);
    private Vector2[] _points;

    public float timer;
    public PixelPrimitiveCircleParams circleParams;
    public Vector2 position;
    public bool active;
    public CalculateCirclePoints circlePointsFunction;
    public RenderPixelPrimitivesInner renderPixelPrimitivesFunction;

    public const int TRAIL_POINTS = 128;

    public PixelPrimitiveCircle()
    {
        circlePointsFunction = DefaultCirclePointsFunction;
    }

    public void DefaultCirclePointsFunction(ref Vector2[] points, in PixelPrimitiveCircleParams circleParams)
    {
        float completionRatio = timer / circleParams.time;
        float radius = MathHelper.Lerp(circleParams.minRadius, circleParams.maxRadius, EasingFunction.InOutSine(completionRatio));
        float maxRadians = MathHelper.ToRadians(362);
        for (int f = 0; f < points.Length; f++)
        {
            float ratio = (float)(f) / (float)(points.Length - 1);
            ref Vector2 point = ref points[f];

            float radians = ratio * maxRadians;
            float x = MathF.Sin(radians) * radius;
            float y = MathF.Cos(radians) * radius;
            point = position + new Vector2(x, y);
        }
    }

    public void Update()
    {
        timer++;
        if (timer == 1)
        {
            _points = ArrayPool<Vector2>.Shared.Rent(TRAIL_POINTS);
        }

        if (circlePointsFunction == null)
            return;

        circlePointsFunction(ref _points, in circleParams);
        if (timer >= circleParams.time)
        {
            active = false;
        }
    }

    public void QueueDraw()
    {
        PixelationManager.QueuePrimitivesDrawAction(RenderPixelPrimitives);
    }

    public void RenderPixelPrimitives(GraphicsDevice graphicsDevice)
    {
        if (renderPixelPrimitivesFunction == null)
            return;
        renderPixelPrimitivesFunction(_points, timer / circleParams.time, in circleParams);
    }


}

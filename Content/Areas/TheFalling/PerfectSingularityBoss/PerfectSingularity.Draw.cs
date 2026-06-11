using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.TheFalling.PerfectSingularityBoss;

public partial class PerfectSingularity :
    IDrawToRenderTarget
{
    private Asset<Texture2D> _chainTextureAsset;
    private Asset<Texture2D> _eyesTextureAsset;
    private Asset<Texture2D> _eyes2TextureAsset;
    private float IntensityInterpolant => _intensityTimeLeft / ChainWhip_StartupTime;
    private Vector2 GetIntensityScale()
    {
        float lerp = EasingFunction.QuadraticBump(IntensityInterpolant);
        float ease = EasingFunction.OutExpo(IntensityInterpolant);
        float lerp3 = MathHelper.SmoothStep(lerp, ease, IntensityInterpolant);
        return Vector2.Lerp(Vector2.One, Vector2.One * 0.75f, lerp3 );
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        LazyLoadTextureAssets();

        Vector2 shake = _intensityShake * IntensityInterpolant;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.rotation += Main.GlobalTimeWrappedHourly * 0.03f;
        drawer.color = Color.White;
        drawer.rotation = 0;
        drawer.scale = new Vector2(1.5f, 1f) * GetIntensityScale();
        drawer.worldPosition += shake;



        //Draw a dark shadow behind the singularity
        //Using a shader for this so it isn't pixelated, also no need to save a whole ass new texture for this
        PerfectDarknessShader darknessShader = ShaderContent.GetInstance<PerfectDarknessShader>();
        spriteBatch.Restart(SpriteSortMode.Immediate, effect: darknessShader.Effect);

        drawer.scale = Vector2.One * 2;

        spriteBatch.Draw(drawer);

        PerfectSingularityShader perfectSingularityShader = ShaderContent.GetInstance<PerfectSingularityShader>();
        perfectSingularityShader.Time = Main.GlobalTimeWrappedHourly * 4;
        perfectSingularityShader.NoiseTexture = AssetManager.Noise.Whirly.Value;
        spriteBatch.Restart(SpriteSortMode.Immediate, effect: perfectSingularityShader.Effect);


        perfectSingularityShader.Eyes = null;
        drawer.color = Color.White;

        drawer.scale = Vector2.One * GetIntensityScale();
        perfectSingularityShader.Eyes = _eyes2TextureAsset.Value;
        SpritebatchDrawer singDrawer = drawer;
        singDrawer.scale *= 1.25f;
        //spriteBatch.Draw(singDrawer);


        spriteBatch.Draw(drawer);

        spriteBatch.RestartDefaults();

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, NPC.Center);
        glowDrawer.color = Color.White * ExtraMath.Osc(0.9f, 1f, speed: 6) * EasingFunction.InExpo(IntensityInterpolant);
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 3;
        glowDrawer.scale *= new Vector2(2f, 0.7f);
        glowDrawer.scale.Y *= MathHelper.Lerp(1f, 0f, EasingFunction.QuadraticBump(IntensityInterpolant));
        spriteBatch.Draw(glowDrawer);
        return false;
    }
    private void DrawOutlines(SpriteBatch spriteBatch)
    {
        LazyLoadTextureAssets();

        Vector2 shake = _intensityShake * IntensityInterpolant;
        PerfectSingularityShader perfectSingularityShader = ShaderContent.GetInstance<PerfectSingularityShader>();
        perfectSingularityShader.Time = Main.GlobalTimeWrappedHourly * 4;
        perfectSingularityShader.NoiseTexture = AssetManager.Noise.Whirly.Value;

        spriteBatch.Restart(effect: perfectSingularityShader.Effect);

        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.rotation += Main.GlobalTimeWrappedHourly * 0.03f;
        // drawer.scale *= 2;
        drawer.color = Color.White * 0.125f;
        drawer.rotation = 0;
        drawer.scale = new Vector2(1.5f, 1f) * GetIntensityScale();
        drawer.worldPosition += shake;

        drawer.rotation += Main.GlobalTimeWrappedHourly * 0.3f;
        // drawer.scale *= 2;
        drawer.color = _outliner.outlineColor;

        drawer.scale = Vector2.One * GetIntensityScale();
        spriteBatch.Draw(drawer);



        spriteBatch.RestartDefaults();

        /*
        drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.rotation += Main.GlobalTimeWrappedHourly;

        PerfectRingShader perfectRingShader = ShaderContent.GetInstance<PerfectRingShader>();
        perfectRingShader.Time = Main.GlobalTimeWrappedHourly * 1;
        spriteBatch.Restart(effect: perfectRingShader.Effect);

        drawer.color = Color.White * 0.75f;

        drawer.rotation += Main.GlobalTimeWrappedHourly;
       // spriteBatch.Draw(drawer);

        drawer.color = Color.White * 1f;
        drawer.rotation = ExtraMath.Osc(0.2f, 0.1f);
        drawer.scale = new Vector2(3f, 0.6f) ;
        spriteBatch.Draw(drawer);

        drawer.color = Color.White * 0.05f;
        drawer.rotation = ExtraMath.Osc(-0.7f, -0.5f);
        drawer.scale = new Vector2(3f, 0.6f) ;
        spriteBatch.Draw(drawer);
        spriteBatch.RestartDefaults();*/
    }
    private void LazyLoadTextureAssets()
    {
        _chainTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Chain");
        _eyesTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Eyes");
        _eyes2TextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Eyes2");
    }
    private void DrawSingularity(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        LazyLoadTextureAssets();



    }

    private Color GetTrailColorFunc(float ratio)
    {
        return Color.White * 0.55f;
        //return Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuadraticBump(ratio));
    }
    private Color GetTrailColorFunc2(float ratio)
    {
        return Color.Lerp(Color.White, Color.Black, 0.46f);
        //return Color.White;
        //return Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuadraticBump(ratio));
    }
    private float GetTrailWidthFunc(float ratio)
    {
        return MathHelper.SmoothStep(0, 98, EasingFunction.QuadraticBump(ratio));
    }
    private void DrawRings(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        LazyLoadTextureAssets();


        var drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.rotation += Main.GlobalTimeWrappedHourly;

        PerfectRingShader perfectRingShader = ShaderContent.GetInstance<PerfectRingShader>();
        perfectRingShader.Time = Main.GlobalTimeWrappedHourly * 1;
        spriteBatch.Restart(effect: perfectRingShader.Effect);

        drawer.color = Color.White * 0.75f;

        drawer.rotation += Main.GlobalTimeWrappedHourly;
        // spriteBatch.Draw(drawer);

        drawer.color = Color.White * 1f;
        drawer.rotation = ExtraMath.Osc(0.2f, 0.1f);
        drawer.scale = new Vector2(3f, 0.6f);
        spriteBatch.Draw(drawer);

        drawer.color = Color.White * 0.05f;
        drawer.rotation = ExtraMath.Osc(-0.7f, -0.5f);
        drawer.scale = new Vector2(3f, 0.6f);
        spriteBatch.Draw(drawer);

    }
    private void DrawEyes(GraphicsDevice gDevice)
    {
        LazyLoadTextureAssets();
        Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        float numPoints = 64;
        List<Vector2> eyePoints = new List<Vector2>();

        float scaleMult = MathHelper.Lerp(1f, 0.46f, EasingFunction.QuadraticBump(IntensityInterpolant));
        Vector2 start = NPC.Center - Vector2.UnitX * 455 * scaleMult;
        Vector2 end = NPC.Center + Vector2.UnitX * 455 * scaleMult;
        for (float f = 0; f < numPoints; f++)
        {
            float ratio = f / numPoints;
            Vector2 p = Vector2.Lerp(start, end, EasingFunction.InOutQuad(ratio));
            p += Vector2.UnitY * MathHelper.Lerp(0f, 256, EasingFunction.QuadraticBump(ratio));
            eyePoints.Add(p);

        }

        PerfectEyesShader eyesShader = ShaderContent.GetInstance<PerfectEyesShader>();
        eyesShader.Texture = _eyesTextureAsset.Value;
        eyesShader.Time = Main.GlobalTimeWrappedHourly * 2;
        eyesShader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        TrailDrawer.Draw(eyePoints.ToArray(), GetTrailColorFunc, GetTrailWidthFunc, eyesShader);



        start += new Vector2(0, -32);
        end += new Vector2(0, -32);
        eyePoints.Clear();
        for (float f = 0; f < numPoints; f++)
        {
            float ratio = f / numPoints;
            Vector2 p = Vector2.Lerp(start, end, EasingFunction.InOutQuad(ratio));
            Vector2 v = -Vector2.UnitY;
            p += v * MathHelper.Lerp(0f, 256, EasingFunction.QuadraticBump(ratio));
            eyePoints.Add(p);

        }

        TrailDrawer.Draw(eyePoints.ToArray(), GetTrailColorFunc, GetTrailWidthFunc, eyesShader);


        start = NPC.Center + Vector2.UnitY * 450 * scaleMult;
        end = NPC.Center - Vector2.UnitY * 450 * scaleMult;
        start = start.RotatedBy(0.5f, NPC.Center);
        end = end.RotatedBy(0.5f, NPC.Center);
        eyePoints.Clear();
        Vector2 v2 = -Vector2.UnitX;
        for (float f = 0; f < numPoints; f++)
        {
            float ratio = f / numPoints;
            Vector2 p = Vector2.Lerp(start, end, EasingFunction.InOutQuad(ratio));

            p += v2 * MathHelper.Lerp(0f, 256, EasingFunction.QuadraticBump(ratio));
            eyePoints.Add(p);

        }
        TrailDrawer.Draw(eyePoints.ToArray(), GetTrailColorFunc, GetTrailWidthFunc, eyesShader);


    }
    private void DrawChains(GraphicsDevice gDevice)
    {
        LazyLoadTextureAssets();
        Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        float numPoints = 64;
        List<Vector2> eyePoints = new List<Vector2>();

        float scaleMult = MathHelper.Lerp(1f, 0.46f, EasingFunction.QuadraticBump(IntensityInterpolant));
        Vector2 start = NPC.Center - Vector2.UnitX * 455 * scaleMult;
        Vector2 end = NPC.Center + Vector2.UnitX * 455 * scaleMult;
        start = start.RotatedBy(0.5f, NPC.Center);
        end = end.RotatedBy(0.5f, NPC.Center);
        for (float f = 0; f < numPoints; f++)
        {
            float ratio = f / numPoints;
            Vector2 p = Vector2.Lerp(start, end, EasingFunction.InOutQuad(ratio));
            p += Vector2.UnitY * MathHelper.Lerp(0f, 256, EasingFunction.QuadraticBump(ratio));
            eyePoints.Add(p);

        }

        PerfectEyesShader eyesShader = ShaderContent.GetInstance<PerfectEyesShader>();
        eyesShader.Texture = _chainTextureAsset.Value;
        eyesShader.Time = Main.GlobalTimeWrappedHourly * 6;
        eyesShader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        eyesShader.DistortionStrength = MathHelper.Lerp(0.025f, 0.2f, EasingFunction.QuadraticBump(IntensityInterpolant));
        TrailDrawer.Draw(eyePoints.ToArray(), GetTrailColorFunc2, GetTrailWidthFunc, eyesShader);



        start += new Vector2(0, -32);
        end += new Vector2(0, -32);
        eyePoints.Clear();
        for (float f = 0; f < numPoints; f++)
        {
            float ratio = f / numPoints;
            Vector2 p = Vector2.Lerp(start, end, EasingFunction.InOutQuad(ratio));
            Vector2 v = -Vector2.UnitY;
            p += v * MathHelper.Lerp(0f, 256, EasingFunction.QuadraticBump(ratio));
            eyePoints.Add(p);

        }

        TrailDrawer.Draw(eyePoints.ToArray(), GetTrailColorFunc2, GetTrailWidthFunc, eyesShader);


        start = NPC.Center + Vector2.UnitY * 450 * scaleMult;
        end = NPC.Center - Vector2.UnitY * 450 * scaleMult;
        start = start.RotatedBy(1.5f, NPC.Center);
        end = end.RotatedBy(1.5f, NPC.Center);
        eyePoints.Clear();
        Vector2 v2 = -Vector2.UnitX.RotatedBy(1.0f);
        for (float f = 0; f < numPoints; f++)
        {
            float ratio = f / numPoints;
            Vector2 p = Vector2.Lerp(start, end, EasingFunction.InOutQuad(ratio));

            p += v2 * MathHelper.Lerp(0f, 256, EasingFunction.QuadraticBump(ratio));
            eyePoints.Add(p);

        }
        TrailDrawer.Draw(eyePoints.ToArray(), GetTrailColorFunc2, GetTrailWidthFunc, eyesShader);


    }

    public void DrawToRenderTargets()
    {
        //  OutlineRenderer.Queue(DrawOutlines);
        PixelationManager.QueueSpritebatchDrawAction(DrawRings, DrawLayer.OverNPCs);

        PixelationManager.QueuePrimitivesDrawAction(DrawEyes, DrawLayer.OverNPCs);

        PixelationManager.QueuePrimitivesDrawAction(DrawChains, DrawLayer.OverNPCsAdditive);
    }
}

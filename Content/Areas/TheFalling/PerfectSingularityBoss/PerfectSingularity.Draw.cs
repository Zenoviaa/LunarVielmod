using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Stellamod.Content.Areas.TheFalling.PerfectSingularityBoss;

public partial class PerfectSingularity :
    IDrawToRenderTarget
{
    private Asset<Texture2D> _chainTextureAsset;
    private Asset<Texture2D> _eyesTextureAsset;
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {    
        LazyLoadTextureAssets();
        PerfectSingularityShader perfectSingularityShader = ShaderContent.GetInstance<PerfectSingularityShader>();
        perfectSingularityShader.Time = Main.GlobalTimeWrappedHourly * 4;
        perfectSingularityShader.NoiseTexture = AssetManager.Noise.Whirly.Value;
        spriteBatch.Restart(effect: perfectSingularityShader.Effect);

        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.rotation += Main.GlobalTimeWrappedHourly * 0.03f;
        // drawer.scale *= 2;
        drawer.color = Color.White * 0.125f;
        drawer.rotation = 0;
        drawer.scale = new Vector2(1.5f, 1f);
        spriteBatch.Draw(drawer);

        drawer.color = Color.White;
        drawer.rotation += Main.GlobalTimeWrappedHourly * 0.3f;
        // drawer.scale *= 2;
        drawer.color = Color.White;
      
        drawer.scale = Vector2.One;
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
        return false;
    }
    private void LazyLoadTextureAssets()
    {
        _chainTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Chain");
        _eyesTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Eyes");
    }
    private void DrawSingularity(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        LazyLoadTextureAssets();



    }

    private Color GetTrailColorFunc(float ratio)
    {
        return Color.White;
        //return Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuadraticBump(ratio));
    }
    private Color GetTrailColorFunc2(float ratio)
    {
        return Color.Lerp(Color.White, Color.Black, 0.76f);
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
        Vector2 start = NPC.Center - Vector2.UnitX * 455;
        Vector2 end = NPC.Center + Vector2.UnitX * 455;
        for(float f = 0; f < numPoints; f++)
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


        start = NPC.Center + Vector2.UnitY * 450;
        end = NPC.Center - Vector2.UnitY * 450;
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
        Vector2 start = NPC.Center - Vector2.UnitX * 455;
        Vector2 end = NPC.Center + Vector2.UnitX * 455;
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


        start = NPC.Center + Vector2.UnitY * 450;
        end = NPC.Center - Vector2.UnitY * 450;
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
        PixelationManager.QueueSpritebatchDrawAction(DrawRings, DrawLayer.OverNPCs);
       
        PixelationManager.QueuePrimitivesDrawAction(DrawEyes, DrawLayer.OverNPCs);

        PixelationManager.QueuePrimitivesDrawAction(DrawChains, DrawLayer.OverNPCsAdditive);
    }
}

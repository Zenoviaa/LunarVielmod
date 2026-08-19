using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Common.Shaders;
using Stellamod.Core.Rendering;
using Stellamod.Effects.RekFlames;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;


public delegate void SilhouetteDraw(SpriteBatch sb);

[Autoload(Side = ModSide.Client)]
public class RekSilhouetteSystem : ModSystem
{
    private RenderTargetProvider _maskedTarget = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
    private RenderTargetProvider _waterMaskRT = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
    public readonly List<SilhouetteDraw> SilhouettesToDraw = new();
    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += RenderWaterMask;
        On_Main.DrawInfernoRings += RenderSilhouettes;
    }



    private void RenderWaterMask(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (SilhouettesToDraw.Count <= 0)
            return;

        //We need the water target as a mask.
        //I really hope this isn't glitchy
        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
        graphicsDevice.SetRenderTarget(_waterMaskRT);
        graphicsDevice.Clear(Color.Transparent);
        spriteBatch.Begin();
        spriteBatch.Draw(Main.waterTarget, Main.sceneWaterPos - Main.screenPosition, Color.White);
        spriteBatch.End();


        graphicsDevice.SetRenderTarget(_maskedTarget);
        graphicsDevice.Clear(Color.Transparent);
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        foreach (var draw in SilhouettesToDraw)
            draw(spriteBatch);
        spriteBatch.End();
    }


    private void RenderSilhouettes(On_Main.orig_DrawInfernoRings orig, Main self)
    {
        orig(self);

        if (SilhouettesToDraw.Count <= 0)
            return;


        LavaSilShader silShader = ShaderContent.GetInstance<LavaSilShader>();
        silShader.MaskTexture = _waterMaskRT;
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, silShader.Effect);
        spriteBatch.Draw(_maskedTarget, Vector2.Zero, Color.DarkRed);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

    }

    public override void PreUpdateNPCs()
    {
        base.PreUpdateNPCs();
        SilhouettesToDraw.Clear();
    }
}

public class SilhouetteGlobalNPC : GlobalNPC
{
    public override void PostAI(NPC npc)
    {
        base.PostAI(npc);
        if (npc.ModNPC is IWaterSilhouette silhouette)
        {
            silhouette.PrepareSilhouetteDrawing(ModContent.GetInstance<RekSilhouetteSystem>());
        }
    }
}

/// <summary>
/// Implement this to draw a silhouette of the NPC over liquids
/// Example implementation in Rek's boss code.
/// </summary>
public interface IWaterSilhouette
{
    void PrepareSilhouetteDrawing(RekSilhouetteSystem system);
}

public partial class RekBoss : IWaterSilhouette
{
    private void DrawSegmentWetWhite(int index)
    {
        ref RekSegment segment = ref Segments[index];
        Asset<Texture2D> textureAsset = BodySegmentsTextures[segment.bodyFrame];
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(textureAsset, segment.position);
        drawer.rotation = segment.rotation;
        switch (segment.bodyFrame)
        {
            default:
                drawer.VerticalFrame(0, 3);
                drawer.CenterOrigin();
                break;
            case 5:
            case 6:
                drawer.CenterOrigin();
                break;
        }
        Color color = Color.White;
        drawer.color = color;
        Main.spriteBatch.Draw(drawer);
    }
    private void DrawWetWhite(SpriteBatch spriteBatch)
    {
        NPC.DrawAnimator(spriteBatch, Color.Red);
        for (int i = 1; i < Segments.Length; i++)
        {
            DrawSegmentWetWhite(i);
        }
    }

    public void PrepareSilhouetteDrawing(RekSilhouetteSystem system)
    {
        system.SilhouettesToDraw.Add(DrawWetWhite);
    }

    private void DrawSegment(int index)
    {
        ref RekSegment segment = ref Segments[index];
        Asset<Texture2D> textureAsset = BodySegmentsTextures[segment.bodyFrame];
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(textureAsset, segment.position);
        drawer.rotation = segment.rotation;
        switch (segment.bodyFrame)
        {
            default:
                drawer.VerticalFrame(0, 3);
                drawer.CenterOrigin();
                break;
            case 5:
            case 6:
                drawer.CenterOrigin();
                break;
        }

        Main.spriteBatch.Draw(drawer);
        switch (segment.bodyFrame)
        {
            default:
                drawer.VerticalFrame(1, 3);
                drawer.CenterOrigin();
                drawer.color = Color.White * segment.burnAlpha * ExtraMath.Osc(0.5f, 1f, speed: 3) * 0.5f;
                drawer.color.A = 0;
                Main.spriteBatch.Draw(drawer);

                Vector2 pos = drawer.worldPosition;
                for (float f = 0; f < MathHelper.TwoPi; f += MathHelper.PiOver2)
                {
                    Main.spriteBatch.Draw(drawer with { worldPosition = pos + (f + Main.GlobalTimeWrappedHourly * 2).ToRotationVector2() * ExtraMath.Osc(4f, 8f, speed: 2) * segment.burnAlpha });
                }
                break;
            case 5:
            case 6:

                break;
        }
    }
    private void DrawSegmentWhite(int index)
    {
        ref RekSegment segment = ref Segments[index];
        if (segment.isBurningNoWarning)
            return;
        Asset<Texture2D> textureAsset = BodySegmentsTextures[segment.bodyFrame];
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(textureAsset, segment.position);
        drawer.rotation = segment.rotation;
        switch (segment.bodyFrame)
        {
            default:
                drawer.VerticalFrame(0, 3);
                drawer.CenterOrigin();
                break;
            case 5:
            case 6:
                drawer.CenterOrigin();
                break;
        }
        Color color = Color.Yellow * segment.burnAlpha;
        if (segment.deadly)
            color = Color.Red * segment.burnAlpha;
        drawer.color = color;
        Main.spriteBatch.Draw(drawer);
    }
    private void DrawSegmentHeat(int index)
    {
        ref RekSegment segment = ref Segments[index];
        if (!segment.isBurningNoWarning)
            return;
        var glowCircle = AssetManager.GlowMask.SimpleGlowCircle;
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(glowCircle, segment.position);
        glowDrawer.scale *= 0.48f;
        glowDrawer.color = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 12)) * ExtraMath.Osc(0.5f, 0.75f, speed: 8) * segment.burnAlpha * 0.2f;
        glowDrawer.color.A = 0;
        Main.spriteBatch.Draw(glowDrawer);
    }

    private void DrawAfterImages(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        for (int i = 0; i < NPC.oldPos.Length; i++)
        {
            float ratio = i / (float)NPC.oldPos.Length;
            Vector2 pos = NPC.oldPos[i] + NPC.Size * 0.5f;
            drawColor = Color.Lerp(Color.Red, Color.Transparent, ratio) * _afterImageAlpha;
            drawColor.A = 0;
            NPC.DrawAnimator(spriteBatch, drawColor, pos);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        //Ok, so we draw everything here yah?
        for (int i = 1; i < Segments.Length; i++)
        {
            DrawSegment(i);
        }
        for (int i = 1; i < Segments.Length; i++)
        {
            DrawSegmentHeat(i);
        }
        DrawAfterImages(spriteBatch, screenPos, drawColor);
        NPC.DrawAnimator(spriteBatch, drawColor);
        OutlineRenderer.Queue(DrawWhite);
        return false;
    }

    private void DrawWhite(SpriteBatch spriteBatch)
    {
        NPC.DrawAnimator(spriteBatch, _outliner.outlineColor);
        for (int i = 1; i < Segments.Length; i++)
        {
            DrawSegmentWhite(i);
        }
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        base.PostDraw(spriteBatch, screenPos, drawColor);
    }


}

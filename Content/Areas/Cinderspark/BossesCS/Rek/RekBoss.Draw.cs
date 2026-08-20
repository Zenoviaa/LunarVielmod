using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering;
using Stellamod.Effects.RekFlames;
using Stellamod.Effects.RoyalMagic;
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
    private Vector2[] _oldOuroborosPos;
    private float GetTrailWidth(float ratio)
    {
        return MathHelper.Lerp(284, 89, ratio);
    }
    private float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 1.5f;
    }
    private float GetTrailWidth3(float ratio)
    {
        return GetTrailWidth(ratio) * 2f;
    }

    private Color GetTrailColor(float ratio)
    {
        Color c = Color.Lerp(Color.Orange, Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 16)), ratio) * EasingFunction.QuadraticBump(ratio) * _ouroborosAlpha;// * EasingFunction.QuadraticBump(_swingTrailAlpha);
                                                                                                                                                             // c.A = 0;
        return c;
    }
    private Color GetTrailColor2(float ratio)
    {
        Color c = Color.Lerp(Color.Orange, Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 16)), ratio) * 0.24f * EasingFunction.QuadraticBump(ratio) * _ouroborosAlpha;// * EasingFunction.QuadraticBump(_swingTrailAlpha);                                                                                                                     // c.A = 0;
        return c;
    }
    private void DrawFlameTrail(GraphicsDevice gDevice)
    {
        if (_oldOuroborosPos == null)
            return;

        //FixedRichLaserShader shader = ShaderContent.GetInstance<FixedRichLaserShader>();
        AlcadSlashShader shader = ShaderContent.GetInstance<AlcadSlashShader>();
        shader.ScrollingLaser = TrailRegistry.Beamlight.Value;
        shader.Noise = AssetManager.Noise.Whirly.Value;
        shader.Slash = AssetManager.LaserTextures.CometTrail.Value;
        shader.BloomColor = Color.DarkRed;
        shader.Time = Main.GlobalTimeWrappedHourly * 24;
        shader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        shader.Distortion = 0.15f;
        TrailDrawer.Draw(_oldOuroborosPos, GetTrailColor, GetTrailWidth, shader);
        //TrailDrawer.Draw(_oldOuroborosPos, GetTrailColor, GetTrailWidth2, shader);

        FixedRichLaserShader shader2 = ShaderContent.GetInstance<FixedRichLaserShader>();
        shader2.LaserTexture = TrailRegistry.BeamTrail;
        TrailDrawer.Draw(_oldOuroborosPos, GetTrailColor2, GetTrailWidth3, shader2);
    }

    private float GetSlashTrailWidth(float ratio)
    {
        return MathHelper.Lerp(150, 89, ratio) * 0.8f;
    }
    private float GetSlashTrailWidth2(float ratio)
    {
        return GetSlashTrailWidth(ratio) * 1.5f;
    }
    private float GetSlashTrailWidth3(float ratio)
    {
        return GetSlashTrailWidth(ratio) * 2f;
    }
    private float GetSlashTrailWidth4(float ratio)
    {
        return GetSlashTrailWidth(ratio) * 1.45f * MathHelper.SmoothStep(1f, 0, ratio);
    }


    private Color GetSlashTrailColor(float ratio)
    {

        Color inbetweenColor = Color.OrangeRed;
        inbetweenColor = Color.Lerp(inbetweenColor, Color.DarkRed, ExtraMath.Osc(0f, 1f, speed: 9));
        Color c1 = Color.Lerp(Color.Orange, inbetweenColor, ratio);
        Color c2 = Color.Lerp(inbetweenColor, Color.DarkRed, ratio);
        c2 = Color.Lerp(c2, Color.OrangeRed, ExtraMath.Osc(0f, 1f, speed: 16));
        Color c3 = Color.Lerp(c1, c2, ratio);
        c3 *= 0.5f;
        c3 *= _ouroborosAlpha;
        c3 *= EasingFunction.QuadraticBump(ratio);
        // c3.A = 0;
        return c3;
    }

    private Color GetSlashTrailColor2(float ratio)
    {
        Color c = GetSlashTrailColor(ratio) * 0.24f * EasingFunction.QuadraticBump(ratio);// * EasingFunction.QuadraticBump(_swingTrailAlpha);     
        c *= EasingFunction.QuadraticBump(ratio);
        // c.A = 0;
        c *= _ouroborosAlpha;
        return c;
    }
    private Color GetSlashTrailColor3(float ratio)
    {
        Color c = Color.Orange;
        c = Color.Lerp(Color.OrangeRed, c, ratio);
        c = Color.Lerp(c, Color.DarkRed, ExtraMath.Osc(0f, 1f, speed: 9));
        c *= EasingFunction.QuadraticBump(ratio);
        c *= _ouroborosAlpha;// * EasingFunction.QuadraticBump(_swingTrailAlpha);                                                                                                                     // c.A = 0;
        return c;
    }

    private Vector2 CalculateSwingOffset(float interpolant)
    {
        Vector2 v = Vector2.UnitY;
        v = v.RotatedBy(MathHelper.TwoPi * interpolant * MathHelper.Lerp(1f, 1.5f, EasingFunction.InOutSine(Timer / 90)));
        v = v.RotatedBy(_spinRot);
        v *= 144;
        return v;
    }
    private Vector2 CalculateSwingOffset(float interpolant, float dist)
    {
        Vector2 v = Vector2.UnitY;
        v = v.RotatedBy(MathHelper.TwoPi * interpolant * MathHelper.Lerp(1f, 1.5f, EasingFunction.InOutSine(Timer / 90)));
        v = v.RotatedBy(_spinRot);
        v *= dist;
        return v;
    }


    private void DrawSlashEffect(GraphicsDevice gDevice)
    {
        Vector2[] position = new Vector2[64];
        for (int i = 0; i < position.Length; i++)
        {
            //Here we use parent.center cause projectil.center might be the wrong spottt
            float ratio = i / (float)position.Length;
            Vector2 v = CalculateSwingOffset(ratio);
            v = v.RotatedBy(MathHelper.ToRadians(180));
            Vector2 point = _ouroborosOrigin + v;
            position[i] = point;
        }

        //FixedRichLaserShader shader = ShaderContent.GetInstance<FixedRichLaserShader>();
        AlcadSlashShader shader = ShaderContent.GetInstance<AlcadSlashShader>();
        shader.ScrollingLaser = TrailRegistry.Beamlight.Value;
        shader.Noise = AssetManager.Noise.Whirly.Value;
        shader.Slash = AssetManager.GlowMask.SwordSlash.Value;
        shader.BloomColor = Color.Red;
        shader.Time = Main.GlobalTimeWrappedHourly * 24;
        shader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        shader.Distortion = 0.15f;
        TrailDrawer.Draw(position, GetSlashTrailColor, GetSlashTrailWidth, shader);
        TrailDrawer.Draw(position, GetSlashTrailColor, GetSlashTrailWidth, shader);

        FixedRichLaserShader shader2 = ShaderContent.GetInstance<FixedRichLaserShader>();
        shader2.LaserTexture = TrailRegistry.BeamTrail;
        shader2.LaserColor = Color.Lerp(Color.OrangeRed, Color.DarkRed, ExtraMath.Osc(0f, 1f, speed: 8));
        TrailDrawer.Draw(position, GetSlashTrailColor2, GetSlashTrailWidth3, shader2);

        for (int i = 0; i < position.Length; i++)
        {
            //Here we use parent.center cause projectil.center might be the wrong spottt
            float ratio = i / (float)position.Length;
            Vector2 v = CalculateSwingOffset(MathHelper.Lerp(0.75f, 1f, ratio), 244);
            v = v.RotatedBy(MathHelper.ToRadians(180 - 45));
            Vector2 point = _ouroborosOrigin + v;
            position[i] = point;
        }

    }
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
        if(_ouroborosAlpha > 0)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawSlashEffect, DrawLayer.OverNPCsAdditive);
            PixelationManager.QueuePrimitivesDrawAction(DrawFlameTrail, DrawLayer.OverNPCsAdditive);
        }
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

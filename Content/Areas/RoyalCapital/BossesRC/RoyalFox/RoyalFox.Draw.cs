using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering;
using Stellamod.Core.Utilities;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.Actions.Sprites;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox;

[Autoload(Side = ModSide.Client)]
public class RoyalFoxCloneRenderer : ModSystem
{
    private RenderTargetProvider _cloneRT = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
    private readonly Queue<Action> _cloneDrawActions = new();
    public override void Load()
    {
        base.Load();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady += RenderClones;
        On_Main.DoDraw_DrawNPCsOverTiles += DrawClones;
    }

    private void DrawClones(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
    {
        orig(self);
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);
        spriteBatch.Draw(_cloneRT, Vector2.Zero, null, Main.DiscoColor * 0.4f * ExtraMath.Osc(0.5f, 1f, speed: 32), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        spriteBatch.End(); 
    }

    public override void Unload()
    {
        base.Unload();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady -= RenderClones;
    }
    private void RenderClones()
    {
        GraphicsDevice gDevice = Main.graphics.GraphicsDevice;
        gDevice.SetRenderTarget(_cloneRT);
        gDevice.Clear(Color.Transparent);

        SpriteBatch sb = Main.spriteBatch;
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null);
        while (_cloneDrawActions.Count > 0)
        {
            _cloneDrawActions.Dequeue()();
        }
        sb.End();
    }
    public static void Queue(Action drawAction)
    {
        RoyalFoxCloneRenderer clone = ModContent.GetInstance<RoyalFoxCloneRenderer>();
        clone._cloneDrawActions.Enqueue(drawAction);
    }
}
public partial class RoyalFox
{

    private float GetTrailWidth(float ratio)
    {
        return MathHelper.Lerp(150, 89, ratio);
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
        Color c = Color.Lerp(Color.White, Color.Lerp(Color.Pink, Color.Blue, ExtraMath.Osc(0f, 1f, speed: 16)), ratio) * _swingTrailAlpha * EasingFunction.QuadraticBump(ratio);// * EasingFunction.QuadraticBump(_swingTrailAlpha);
                                                                                                                                                                                // c.A = 0;
        return c;
    }
    private Color GetTrailColor2(float ratio)
    {
        Color c = Color.Lerp(Color.White, Color.Lerp(Color.Pink, Color.Blue, ExtraMath.Osc(0f, 1f, speed: 16)), ratio) * _swingTrailAlpha * 0.24f * EasingFunction.QuadraticBump(ratio);// * EasingFunction.QuadraticBump(_swingTrailAlpha);                                                                                                                     // c.A = 0;
        return c;
    }

    private void DrawSwordSlash(SpriteBatch sb, Vector2 sp)
    {
        float endPoint = _swingTrailEndRatio;
        Vector2 point = _startDashPoint + CalculateSwingOffset(_swingVelocity, endPoint);
        point += _swingVelocity.SafeNormalize(Vector2.Zero) * 200;
        SpritebatchDrawer glowSword = SpritebatchDrawer.FromTextureAsset(
            ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/GlowSword_Sword"), point);
        glowSword.rotation = (point - _startDashPoint).ToRotation() + MathHelper.PiOver4 / 2f;
        Color startColor = Color.White;
        Color glowColor = Color.Lerp(Color.Blue, Color.Pink, ExtraMath.Osc(0f, 1f, speed: 3));
        Color mixColor = Color.Lerp(startColor, glowColor, ExtraMath.Osc(0f, 1f, speed: 6));
        glowSword.color = mixColor * ExtraMath.Osc(0.8f, 1f, speed: 3) * _swingTrailAlpha;
        glowSword.color.A = 0;
        glowSword.scale *= 1.6f;
        sb.Draw(glowSword);
        sb.Draw(glowSword);
    }
    private void DrawPixelatedMoon(SpriteBatch sb, Vector2 screenPos)
    {
        if (_darkMoonTimer <= 0)
            return;

        float alpha = _darkMoonTimer / 60f;
        Vector2 scale = Vector2.One * 0.2f;
        var scrollingMoonTextureAsset = ModContent.Request<Texture2D>(Texture + "_ScrollingMoon");
        var maskTextureAsset = ModContent.Request<Texture2D>(Texture + "_Moon");
        SpritebatchDrawer moonSprite = SpritebatchDrawer.FromTextureAsset(maskTextureAsset, _moonPosition);

        ScrollingMoonShader scrollingMoonShader = ScrollingMoonShader.Instance;
        scrollingMoonShader.ScrollingTexture = scrollingMoonTextureAsset.Value;
        scrollingMoonShader.MaskSize = maskTextureAsset.Value.Size();

        float time = Main.GlobalTimeWrappedHourly * 0.6f * 1;
        scrollingMoonShader.ScrollOffset = new Vector2(time, 0f);
        scrollingMoonShader.BendStrength = 1.8f;
        scrollingMoonShader.Tiling = new Vector2(0.13f, 0.45f);


        //Draw the moon itself
        sb.Restart(effect: scrollingMoonShader.Effect);
        moonSprite.rotation = MathHelper.ToRadians(-12);
        moonSprite.color = Color.White * alpha; // Color.Lerp(Color.White, Color.DarkBlue, 0.5f);
        moonSprite.scale *= scale;
        Main.spriteBatch.Draw(moonSprite);
        sb.RestartDefaults();

        var shadowMoonTextureAsset = ModContent.Request<Texture2D>(Texture + "_Shadow");
        SpritebatchDrawer shadowDrawer = SpritebatchDrawer.FromTextureAsset(shadowMoonTextureAsset, _moonPosition);

        shadowDrawer.color *= alpha;
        shadowDrawer.scale *= scale * 1.05f;
        Main.spriteBatch.Draw(shadowDrawer);
    }
    private void DrawSlashEffect(GraphicsDevice gDevice)
    {
        Vector2[] position = new Vector2[128];
        float endPoint = _swingTrailEndRatio;
        float startPoint = endPoint - 0.35f;
        for (int i = 0; i < position.Length; i++)
        {
            float ratio = i / (float)position.Length;
            float interp = MathHelper.Lerp(endPoint, startPoint, ratio);
            Vector2 point = _startDashPoint + CalculateSwingOffset(_swingVelocity, interp);
            point += _swingVelocity.SafeNormalize(Vector2.Zero) * 200;
            position[i] = point;
        }

        //FixedRichLaserShader shader = ShaderContent.GetInstance<FixedRichLaserShader>();
        AlcadSlashShader shader = ShaderContent.GetInstance<AlcadSlashShader>();
        shader.ScrollingLaser = TrailRegistry.Beamlight.Value;
        shader.Noise = AssetManager.Noise.Whirly.Value;
        shader.Slash = AssetManager.GlowMask.SwordSlash.Value;
        shader.BloomColor = Color.Purple;
        shader.Time = Main.GlobalTimeWrappedHourly * 24;
        shader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        shader.Distortion = 0.15f;
        TrailDrawer.Draw(position, GetTrailColor, GetTrailWidth, shader);
        TrailDrawer.Draw(position, GetTrailColor, GetTrailWidth2, shader);

        FixedRichLaserShader shader2 = ShaderContent.GetInstance<FixedRichLaserShader>();
        shader2.LaserTexture = TrailRegistry.BeamTrail;
        TrailDrawer.Draw(position, GetTrailColor2, GetTrailWidth3, shader2);
    }

    private void DrawTelegraphLine(SpriteBatch spriteBatch)
    {
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromTextureAsset(
            ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/RayLight4"), _startDashPoint);
        sbDrawer.LeftCenterOrigin();
        sbDrawer.color = Color.White * _telegraphLineAlpha * ExtraMath.Osc(0.6f, 1f, speed: 14);
        sbDrawer.color.A = 0;
        sbDrawer.rotation = _dashLineVelocity.ToRotation();
        sbDrawer.scale.X *= 7;
        sbDrawer.scale.Y *= 0.5f;
        spriteBatch.Draw(sbDrawer);
    }

    private void DrawEyeFlash(SpriteBatch spriteBatch)
    {
        SpritebatchDrawer eyeFlashDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare2, _eyeFlashPosition);
        eyeFlashDrawer.scale = Vector2.Lerp(Vector2.Zero, Vector2.One, EasingFunction.OutSine(_eyeFlashAlpha)) * 1.3f;
        eyeFlashDrawer.color = Color.White;
        eyeFlashDrawer.color.A = 0;
        eyeFlashDrawer.worldPosition += _eyeFlashOffset;
        eyeFlashDrawer.rotation = Main.GlobalTimeWrappedHourly * 4;
        Main.spriteBatch.Draw(eyeFlashDrawer);
    }

    private void DrawTeleportTelegraph(SpriteBatch sb)
    {
        if (_teleportAlpha < 0.01f)
            return;
        SpritebatchDrawer flare = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare2, _teleportTelegraphPosition);
        Color startColor = Color.White;
        Color glowColor = Color.Lerp(Color.Blue, Color.Pink, ExtraMath.Osc(0f, 1f, speed: 3));
        Color mixColor = Color.Lerp(startColor, glowColor, ExtraMath.Osc(0f, 1f, speed: 6));
        flare.color = mixColor * ExtraMath.Osc(0.8f, 1f, speed: 3);
        flare.color.A = 0;
        flare.scale *= 0.5f * MathHelper.Lerp(0.7f, 1f, _teleportAlpha) * _teleportAlpha;
        flare.rotation = Main.GlobalTimeWrappedHourly * 4;
        sb.Draw(flare);

        SpritebatchDrawer circleDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.WhiteCircle, _teleportTelegraphPosition);
        circleDrawer.color = glowColor * _teleportAlpha * 0.2f;
        circleDrawer.color.A = 0;
        circleDrawer.scale = Vector2.One * MathHelper.Lerp(0f, 1f, _teleportAlpha) * 3;
        sb.Draw(circleDrawer);
    }

    private void DrawGravityField(SpriteBatch sb)
    {
        if (_gravityFieldAlpha < 0.01f)
            return;
        GravityFieldShader gravityFieldShader = ShaderContent.GetInstance<GravityFieldShader>();
        gravityFieldShader.Time = Main.GlobalTimeWrappedHourly * 4;
        gravityFieldShader.NoiseTexture = AssetManager.Noise.PerlinBlurred.Value;
        sb.Restart(effect: gravityFieldShader.Effect);

        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(_gravityFieldTextureAsset, Rig.bodyParts[2].worldPosition);
        drawer.color = Color.Lerp(Color.Blue, Color.DarkViolet, ExtraMath.Osc(0f, 1f, speed: 3)) * _gravityFieldAlpha;
        drawer.scale *= 2;
        sb.Draw(drawer);

        sb.RestartDefaults();
    }
    private void DrawBackWing(bool darkened) => DrawWings(true, darkened);
    private void DrawFrontWing(bool darkened) => DrawWings(false, darkened);

    private void DrawWings(bool backWings, bool darkened)
    {
        if (!_canDrawWings)
            return;

        Main.spriteBatch.End();
        Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>($"{Texture}_Wing").Value;
        Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        PerfectWingShader perfectWingShader = ShaderContent.GetInstance<PerfectWingShader>();
        perfectWingShader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        perfectWingShader.StarTexture = AssetManager.Noise.FlamethrowerNoise.Value;
        perfectWingShader.NoiseTexture = AssetManager.Noise.Whirly.Value;
        perfectWingShader.Distortion = 0.1f;
        perfectWingShader.Time = Main.GlobalTimeWrappedHourly * 3;

        //I believe we just get the perspective matrix, transform it, and then offset it to the correct spot???
        var segmentToDrawOn = Rig.bodyParts[2];
        Matrix perspectiveMatrix = segmentToDrawOn.GetFullMatrix();
        Vector2 worldPos = _wingPos;
        for (int i = 0; i < 3; i++)
        {
            Vector3 yAxis = new Vector3(0, 1, 0);
            float zRotation = MathHelper.Lerp(MathHelper.ToRadians(-35), MathHelper.ToRadians(35), ExtraMath.Osc(0f, 1f, speed: 3, offset: i));
            if (backWings)
                zRotation *= -1;
            Quaternion zQuaternion = Quaternion.CreateFromAxisAngle(yAxis, zRotation);
            Matrix flapMatrix = Matrix.CreateFromQuaternion(zQuaternion);

            Vector3 xAxis = new Vector3(1, 0, 0);
            float xRot = MathHelper.Lerp(MathHelper.ToRadians(30), 0, i / 3f);
            if (backWings)
                xRot *= -1;
            Quaternion offsetWingQuaternion = Quaternion.CreateFromAxisAngle(xAxis, xRot);
            Matrix m = Matrix.CreateFromQuaternion(offsetWingQuaternion);


            Vector3 zAxis = new Vector3(0, 0, 1);
            float zRot = MathHelper.Lerp(MathHelper.ToRadians(25), MathHelper.ToRadians(-25), ExtraMath.Osc(0f, 1f, speed: 3, offset: i));
            Quaternion zWingQuat = Quaternion.CreateFromAxisAngle(zAxis, zRot);
            Matrix z = Matrix.CreateFromQuaternion(zWingQuat);


            Vector3 offset = -Rig.bodyParts[3].forwardVectors[0] * 0.8f * MathHelper.Lerp(1f, 0f, i / 3f);
            Matrix translationMatrix = Matrix.CreateTranslation(offset);
            Matrix fullMatrix = z * flapMatrix * m * perspectiveMatrix * translationMatrix;
            WingQuad.CalculateBottomCenterVertices(worldPos, 256, 128, fullMatrix);

            Color glowColor = Color.Lerp(Color.Blue, Color.Pink, ExtraMath.Osc(0f, 1f, speed: 3));
            Color wingColor = Color.Lerp(Color.White, glowColor, ExtraMath.Osc(0f, 0.8f, speed: 1.5f));
            wingColor = Color.Lerp(Color.Lerp(Color.White, Color.Black, 0.8f), wingColor, i / 3f);
            if (darkened)
            {
                //        wingColor = Color.Lerp(wingColor, Color.Black, 0.5f);
            }

            wingColor *= 0.5f;
            wingColor.A = 0;
            WingQuad.SetColor(wingColor * _invisibleAlpha);
            //  WingQuad.vertices[0].Color = Color.Transparent;
            WingQuad.DrawWithShader(perfectWingShader);
        }
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
    }



    private void DrawFox(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        _gravityFieldTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_GravityField");
        _sigilTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Sigil");
        if (_renderMotionBlur)
        {
            DashBlurShader dashBlurShader = ShaderContent.GetInstance<DashBlurShader>();
            spriteBatch.Restart(effect: dashBlurShader.Effect);
        }

        for (int i = 0; i < NPC.oldPos.Length; i++)
        {
            Vector2 offset = ((NPC.oldPos[i] + NPC.Size * 0.5f) - NPC.Center);
            float alpha = MathHelper.Lerp(0.04f, 0f, i / (float)NPC.oldPos.Length) * _invisibleAlpha;
            for (int j = 0; j < Rig.segmentsByZLayer.Length; j++)
            {
                Rig.segmentsByZLayer[j].alpha = alpha;
            }
            Rig.Draw(spriteBatch, screenPos - offset, drawColor);
        }
        for (int i = 0; i < Rig.segmentsByZLayer.Length; i++)
        {
            Rig.segmentsByZLayer[i].alpha = _invisibleAlpha;
        }




        _canDrawWings = true;
        Rig.Draw(spriteBatch, screenPos, drawColor);
        _canDrawWings = false;


        DrawTelegraphLine(spriteBatch);
        DrawEyeFlash(spriteBatch);

        Vector2 drawPos = Rig.headPart.worldPosition;

        float rot = RegularRotation + MathHelper.PiOver4;
        drawPos += (rot - MathHelper.PiOver2).ToRotationVector2() * 45;
        SpritebatchDrawer headDrawer = SpritebatchDrawer.FromTextureAsset(_sigilTextureAsset, drawPos);
        headDrawer.rotation = rot - MathHelper.PiOver4; ;
        headDrawer.color *= _invisibleAlpha;
        // headDrawer.scale *= 5;
        spriteBatch.Draw(headDrawer);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, drawPos);
        glowDrawer.color = Color.White * 0.3f * ExtraMath.Osc(0.5f, 1f, speed: 3) * _invisibleAlpha;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.25f;
        spriteBatch.Draw(glowDrawer);
        if (_renderMotionBlur)
        {
            spriteBatch.RestartDefaults();
        }

        if (_roaringCircleAlpha > 0)
        {
            SpritebatchDrawer circleDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.WhiteCircle, HeadPosition);
            circleDrawer.color = _roaringCircleColor * _roaringCircleAlpha * 0.3f;
            circleDrawer.color.A = 0;
            circleDrawer.scale = Vector2.One * _roaringCircleScale;
            spriteBatch.Draw(circleDrawer);
        }
        DrawGravityField(spriteBatch);
   
    }
    private void DrawMoonTeleport(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        DrawTeleportTelegraph(spriteBatch);
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (_dontRender)
            return false;
        if (IsAClone)
            return false;
        DrawFox(spriteBatch, screenPos, drawColor);

        //  DrawLaserTelegraph(Main.spriteBatch);
        return false;
    }

    private void DrawLaserTelegraph(SpriteBatch sb, Vector2 sp)
    {
        if (_laserTelegraphAlpha <= 0)
            return;

        Vector2 endPoint = CalculateLaserSpawnPoint(1);
        Vector2 vel = CalculateLaserSpawnVelocity();
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, endPoint + vel * 92);
        glowDrawer.color = Color.White * ExtraMath.Osc(0.6f, 1f, speed: 12);
        glowDrawer.color.A = 0;
        glowDrawer.scale.X *= 0.5f;
        glowDrawer.scale *= 0.5f;
        glowDrawer.rotation = CalculateLaserSpawnVelocity().ToRotation();
        sb.Draw(glowDrawer);
    }
    private void DrawOutlines(SpriteBatch sb)
    {
        Rig.Draw(sb, Main.screenPosition, _outliner.outlineColor);
    }


    private void DrawFull()
    {

        RenderPixelatedDashTrail(Main.graphics.GraphicsDevice);
        DrawHair(Main.graphics.GraphicsDevice);
        DrawFox(Main.spriteBatch, Main.screenPosition, Color.White);
        DrawSlashEffect(Main.graphics.GraphicsDevice);
        DrawSwordSlash(Main.spriteBatch, Main.screenPosition);
    }
    public void DrawToRenderTargets()
    {
        ModContent.GetInstance<FenixDomain>().drawFenix = true;
        if (IsAClone)
        {
            OutlineRenderer.Queue(DrawOutlines);
            RoyalFoxCloneRenderer.Queue(DrawFull);
            PixelationManager.QueueSpritebatchDrawAction(DrawLaserTelegraph, DrawLayer.OverPlayers);
            return;
        }
        PixelationManager.QueueSpritebatchDrawAction(DrawLaserTelegraph, DrawLayer.OverPlayers);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedMoon);
        OutlineRenderer.Queue(DrawOutlines);
        if (_dashTrailAlpha > 0)
        {
            PixelationManager.QueuePrimitivesDrawAction(RenderPixelatedDashTrail);
        }
   
        if (!_tailInFront)
            PixelationManager.QueuePrimitivesDrawAction(DrawHair, DrawLayer.BehindNPCsWithOutline);
        else
            PixelationManager.QueuePrimitivesDrawAction(DrawHair, DrawLayer.OverNPCs);
        PixelationManager.QueuePrimitivesDrawAction(DrawSlashEffect, DrawLayer.OverNPCs);
        PixelationManager.QueueSpritebatchDrawAction(DrawSwordSlash, DrawLayer.OverNPCs);
        PixelationManager.QueueSpritebatchDrawAction(DrawMoonTeleport, DrawLayer.OverPlayers);
    }
}

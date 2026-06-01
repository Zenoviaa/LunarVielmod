using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox;

public partial class RoyalFox
{
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

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (_dontRender)
            return false;

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



        Rig.Draw(spriteBatch, screenPos, drawColor);
        DrawTelegraphLine(spriteBatch);
        DrawEyeFlash(spriteBatch);

        Vector2 drawPos = Rig.headPart.worldPosition;

        float rot = RegularRotation + MathHelper.PiOver4;
        drawPos += (rot-MathHelper.PiOver2).ToRotationVector2() * 45;
        SpritebatchDrawer headDrawer = SpritebatchDrawer.FromTextureAsset(_sigilTextureAsset, drawPos);
        headDrawer.rotation =rot - MathHelper.PiOver4; ;
       // headDrawer.scale *= 5;
        Main.spriteBatch.Draw(headDrawer);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, drawPos);
        glowDrawer.color = Color.White * 0.3f * ExtraMath.Osc(0.5f, 1f, speed: 3);
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.25f;
        Main.spriteBatch.Draw(glowDrawer);
        if (_renderMotionBlur)
        {
            spriteBatch.RestartDefaults();
        }

        if(_roaringCircleAlpha > 0)
        {
            SpritebatchDrawer circleDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.WhiteCircle, HeadPosition);
            circleDrawer.color = _roaringCircleColor * _roaringCircleAlpha * 0.3f;
            circleDrawer.color.A = 0;
            circleDrawer.scale = Vector2.One * _roaringCircleScale;
            Main.spriteBatch.Draw(circleDrawer);
        }
      //  DrawLaserTelegraph(Main.spriteBatch);
        return false;
    }

    private void DrawLaserTelegraph(SpriteBatch sb)
    {
        if (_laserTelegraphAlpha <= 0)
            return;

        Vector2 endPoint = CalculateLaserSpawnPoint(1);
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, endPoint);
        glowDrawer.color = Color.White * ExtraMath.Osc(0.6f, 1f, speed: 12);
        glowDrawer.color.A = 0;
      //  glowDrawer.scale.Y *= 0.5f;
     //   glowDrawer.scale *= 0.5f * 4;
        glowDrawer.rotation = CalculateLaserSpawnVelocity().ToRotation();
        sb.Draw(glowDrawer);
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

    public void DrawToRenderTargets()
    {
        OutlineRenderer.Queue(DrawOutlines);
        if (_dashTrailAlpha > 0)
        {
            PixelationManager.QueuePrimitivesDrawAction(RenderPixelatedDashTrail);
        }
        PixelationManager.QueueSpritebatchDrawAction(DrawLaserTelegraph, DrawLayer.OverPlayers);
        if(!_tailInFront)
            PixelationManager.QueuePrimitivesDrawAction(DrawHair, DrawLayer.BehindNPCsWithOutline);
        else
            PixelationManager.QueuePrimitivesDrawAction(DrawHair, DrawLayer.OverNPCs);
    }
}

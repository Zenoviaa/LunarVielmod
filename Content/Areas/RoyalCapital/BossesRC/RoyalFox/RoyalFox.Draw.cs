using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Core.ZTileSystem;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Stellamod.Trails;
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
        for(int i = 0; i < 3; i++)
        {
            Vector3 yAxis = new Vector3(0, 1, 0);
            float zRotation = MathHelper.Lerp(MathHelper.ToRadians(-35), MathHelper.ToRadians(35), ExtraMath.Osc(0f, 1f, speed: 3, offset: i));
            if (backWings)
                zRotation *= -1;
            Quaternion zQuaternion = Quaternion.CreateFromAxisAngle(yAxis, zRotation);
            Matrix flapMatrix = Matrix.CreateFromQuaternion(zQuaternion);

            Vector3 xAxis = new Vector3(1, 0, 0);
            float xRot = MathHelper.Lerp(MathHelper.ToRadians(30), 0, (float)i / 3f);
            if (backWings)
                xRot *= -1;
            Quaternion offsetWingQuaternion = Quaternion.CreateFromAxisAngle(xAxis, xRot);
            Matrix m = Matrix.CreateFromQuaternion(offsetWingQuaternion);


            Vector3 zAxis = new Vector3(0, 0, 1);
            float zRot = MathHelper.Lerp(MathHelper.ToRadians(25), MathHelper.ToRadians(-25), ExtraMath.Osc(0f, 1f, speed: 3, offset: i));
            Quaternion zWingQuat = Quaternion.CreateFromAxisAngle(zAxis, zRot);
            Matrix z = Matrix.CreateFromQuaternion(zWingQuat);


            Vector3 offset = -Rig.bodyParts[3].forwardVectors[0] * 0.8f * MathHelper.Lerp(1f, 0f, (float)i / 3f);
            Matrix translationMatrix = Matrix.CreateTranslation(offset);
            Matrix fullMatrix = z * flapMatrix * m  * perspectiveMatrix * translationMatrix;
            WingQuad.CalculateBottomCenterVertices(worldPos, 256, 128, fullMatrix);

            Color glowColor = Color.Lerp(Color.Blue, Color.Pink, ExtraMath.Osc(0f, 1f, speed: 3));
            Color wingColor = Color.Lerp(Color.White, glowColor, ExtraMath.Osc(0f, 0.8f, speed: 1.5f));
            wingColor = Color.Lerp(Color.Lerp(Color.White, Color.Black, 0.8f), wingColor, (float)i / 3f);
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




        _canDrawWings = true;
        Rig.Draw(spriteBatch, screenPos, drawColor);
        _canDrawWings = false;
    

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

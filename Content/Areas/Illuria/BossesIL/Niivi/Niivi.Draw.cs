using Stellamod.Core.Pixelation;
using Terraria;
using Terraria.GameContent;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;

public partial class Niivi
{
    public static int CrystalFrameCount => 9;
    public static int IceCrystalFrame => 0;
    public static int StarCrystalFrame => 1;
    public static int LightningCrystalFrame => 2;
    public static int CrystalWhiteFrameOffset => 3;
    public static int CrystalOutlineFrameOffset => 6;

    void FK_ArcBody(float arcRadians = -0.2f, float interp = 0.1f)
    {
        for (var i = 0; i < ForwardKinematicsRig.segments.Length; i++)
        {
            ref var segment = ref ForwardKinematicsRig.segments[i];
            var targetAngle = i * arcRadians;
            segment.angleBehind = Utils.AngleLerp(segment.angleBehind, targetAngle, interp);
        }
    }

    void DrawCrystals(SpriteBatch sb, Vector2 screenPos, Color drawColor)
    {
        var iceDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.Illuria.NPCs.NiiviCrystal.Asset, _iceCrystalPosition);
        iceDrawer.color = drawColor;
        iceDrawer.VerticalFrame(IceCrystalFrame, CrystalFrameCount);
        iceDrawer.CenterOrigin();
        sb.Draw(iceDrawer);

        var lightningDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.Illuria.NPCs.NiiviCrystal.Asset, _lightningCrystalPosition);
        lightningDrawer.color = drawColor;
        lightningDrawer.VerticalFrame(LightningCrystalFrame, CrystalFrameCount);
        lightningDrawer.CenterOrigin();
        sb.Draw(lightningDrawer);

        var starDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.Illuria.NPCs.NiiviCrystal.Asset, _starCrystalPosition);
        starDrawer.color = drawColor;
        starDrawer.VerticalFrame(StarCrystalFrame, CrystalFrameCount);
        starDrawer.CenterOrigin();
        sb.Draw(starDrawer);

        if (_crystalCharge > 0.05f)
        {
            iceDrawer.color = lightningDrawer.color = starDrawer.color = Main.DiscoColor* _crystalCharge * ExtraMath.Osc(0.5f, 1f, speed: 16);
            iceDrawer.VerticalFrame(IceCrystalFrame + CrystalWhiteFrameOffset, CrystalFrameCount);
            lightningDrawer.VerticalFrame(LightningCrystalFrame + CrystalWhiteFrameOffset, CrystalFrameCount);
            starDrawer.VerticalFrame(StarCrystalFrame + CrystalWhiteFrameOffset, CrystalFrameCount);

            sb.Draw(iceDrawer);
            sb.Draw(lightningDrawer);
            sb.Draw(starDrawer);
        }



    }
    void DrawBody(SpriteBatch sb, Vector2 screenPos, Color drawColor)
    {
        NiiviIceArenaRenderer.effectTimer = 30;
        //Draw tail first, then all the way up to head
        ref var neckSegment = ref SegmentPositions[ArmSegmentIndex];
        ref var buttSegment = ref SegmentPositions[LegSegmentIndex];
        ref var wingSegment = ref SegmentPositions[WingSegmentIndex];

        var backBackArmDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Npc[Type], buttSegment);
        backBackArmDrawer.drawOrigin = SegmentOrigins[9];
        backBackArmDrawer.VerticalFrame(9, 10);
        backBackArmDrawer.rotation = SegmentRotations[LegSegmentIndex];
        backBackArmDrawer.scale = new Vector2(SegmentBaseScales[LegSegmentIndex]);
        backBackArmDrawer.color = drawColor;
        sb.Draw(backBackArmDrawer);

        var bacWingDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.Illuria.NPCs.NiiviWingBack.Asset, wingSegment);
        bacWingDrawer.drawOrigin = BackWingDrawOrigin;
        bacWingDrawer.VerticalFrame(_wingFrame, WingFrameCount);
        bacWingDrawer.rotation = SegmentRotations[WingSegmentIndex];
        bacWingDrawer.scale = new Vector2(SegmentBaseScales[WingSegmentIndex]);
        bacWingDrawer.color = drawColor;
        sb.Draw(bacWingDrawer);

        var frontBackArmDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Npc[Type], neckSegment);
        frontBackArmDrawer.drawOrigin = SegmentOrigins[7];
        frontBackArmDrawer.VerticalFrame(7, 10);
        frontBackArmDrawer.rotation = SegmentRotations[ArmSegmentIndex];
        frontBackArmDrawer.scale = new Vector2(SegmentBaseScales[ArmSegmentIndex]);
        frontBackArmDrawer.color = drawColor;
        sb.Draw(frontBackArmDrawer);

        for (var i = SegmentPositions.Length - 1; i >= 0; i--)
        {
            ref var segment = ref SegmentPositions[i];
            var rotation = NPC.rotation;
            var ratio = i / (float)SegmentPositions.Length;
            if (i > 0)
            {
                ref var nextSegment = ref SegmentPositions[i - 1];
                rotation = (nextSegment - segment).ToRotation();
            }

            var drawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Npc[Type], segment);
            var frameIndex = SegmentFrameIndices[i];
            drawer.drawOrigin = SegmentOrigins[frameIndex];
            drawer.VerticalFrame(frameIndex, 10);
            drawer.rotation = rotation;
            drawer.scale = new Vector2(SegmentBaseScales[i]);
            drawer.color = drawColor;
            sb.Draw(drawer);
            SegmentRotations[i] = rotation;
        }

        //Draw front wing
        var frontWingDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.Illuria.NPCs.NiiviWingFront.Asset, wingSegment);
        frontWingDrawer.drawOrigin = FrontWingDrawOrigin;
        frontWingDrawer.VerticalFrame(_wingFrame, WingFrameCount);
        frontWingDrawer.rotation = SegmentRotations[WingSegmentIndex];
        frontWingDrawer.scale = new Vector2(SegmentBaseScales[WingSegmentIndex]);
        frontWingDrawer.color = drawColor;
        sb.Draw(frontWingDrawer);

        //Draw frong arms

        var backFrontArmDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Npc[Type], buttSegment);
        backFrontArmDrawer.drawOrigin = SegmentOrigins[8];
        backFrontArmDrawer.VerticalFrame(8, 10);
        backFrontArmDrawer.rotation = SegmentRotations[LegSegmentIndex];
        backFrontArmDrawer.scale = new Vector2(SegmentBaseScales[LegSegmentIndex]);
        backFrontArmDrawer.color = drawColor;
        sb.Draw(backFrontArmDrawer);

        var frontArmDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Npc[Type], neckSegment);
        frontArmDrawer.drawOrigin = SegmentOrigins[6];
        frontArmDrawer.VerticalFrame(6, 10);
        frontArmDrawer.rotation = SegmentRotations[ArmSegmentIndex];
        frontArmDrawer.scale = new Vector2(SegmentBaseScales[ArmSegmentIndex]);
        frontArmDrawer.color = drawColor;
        sb.Draw(frontArmDrawer);
    }
    public Vector2[] GetSegmentPoints()
    {
        if (_simpleChainRendering)
            return Chain.points;
        else
        {
            Vector2[] points = new Vector2[Segments.Length];
            for(var i = 0; i < points.Length; i++)
            {
                ref var point = ref points[i];
                point = Segments[i].position;
            }
            return points;
        }
    }
  
    void DrawPixelatedStar(SpriteBatch sb, Vector2 sp)
    {

        var starColor = Color.Lerp(Color.Transparent, Main.DiscoColor * ExtraMath.Osc(0.5f, 1f, speed: 12), _blackAlpha);
        starColor.A = 0;
        var starDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.Star3.Asset, NPC.Center);
        starDrawer.color = starColor;
        starDrawer.scale *= 0.5f;

        sb.Draw(starDrawer);
        starColor = Color.Lerp(Color.Transparent, Color.White * ExtraMath.Osc(0.5f, 1f, speed: 12), _blackAlpha);
        starColor.A = 0;
        starDrawer.color = starColor;
        starDrawer.scale *= 0.8f;

        sb.Draw(starDrawer);

        var glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, NPC.Center);

        starColor = Color.Lerp(Color.Transparent, Main.DiscoColor * ExtraMath.Osc(0.5f, 1f, speed: 12), _blackAlpha);
        starColor.A = 0;
        glowDrawer.color = starColor;
        glowDrawer.scale *= 0.5f;
        sb.Draw(glowDrawer);
    }
    public void DrawForMask(SpriteBatch spriteBatch)
    {
        DrawBody(spriteBatch, Main.screenPosition, Color.White);
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        drawColor = Color.Lerp(drawColor, Color.Black, _blackAlpha);
        drawColor *= _invisAlpha;
        DrawCrystals(spriteBatch, screenPos, drawColor);
        DrawBody(spriteBatch, screenPos, drawColor);

        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedStar, DrawLayer.OverPlayers);
        return false;
    }
    public void DrawToRenderTargets()
    {
        if (!_outliner.VisibleEnough)
            return;
        void DrawOutlines(SpriteBatch sb)
        {
            DrawBody(sb, Main.screenPosition, _outliner.outlineColor);
            DrawCrystals(sb, Main.screenPosition, _outliner.outlineColor);
        }
        OutlineRenderer.Queue(DrawOutlines);
    }
}

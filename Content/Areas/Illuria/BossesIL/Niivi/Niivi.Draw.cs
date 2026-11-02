using Stellamod.Core.Pixelation;
using Terraria;
using Terraria.GameContent;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;

public partial class Niivi
{
    void DrawBody(SpriteBatch sb, Vector2 screenPos, Color drawColor)
    {
        NiiviIceArenaRenderer.effectTimer = 30;
        //Draw tail first, then all the way up to head
        ref var neckSegment = ref Segments[ArmSegmentIndex];
        ref var buttSegment = ref Segments[LegSegmentIndex];
        ref var wingSegment = ref Segments[WingSegmentIndex];

        var backBackArmDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Npc[Type], buttSegment.position);
        backBackArmDrawer.drawOrigin = SegmentOrigins[9];
        backBackArmDrawer.VerticalFrame(9, 10);
        backBackArmDrawer.rotation = SegmentRotations[LegSegmentIndex];
        backBackArmDrawer.scale = new Vector2(SegmentBaseScales[LegSegmentIndex]);
        backBackArmDrawer.color = drawColor;
        sb.Draw(backBackArmDrawer);

        var bacWingDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.Illuria.NPCs.NiiviWingBack.Asset, wingSegment.position);
        bacWingDrawer.drawOrigin = BackWingDrawOrigin;
        bacWingDrawer.VerticalFrame(_wingFrame, WingFrameCount);
        bacWingDrawer.rotation = SegmentRotations[WingSegmentIndex];
        bacWingDrawer.scale = new Vector2(SegmentBaseScales[WingSegmentIndex]);
        bacWingDrawer.color = drawColor;
        sb.Draw(bacWingDrawer);

        var frontBackArmDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Npc[Type], neckSegment.position);
        frontBackArmDrawer.drawOrigin = SegmentOrigins[7];
        frontBackArmDrawer.VerticalFrame(7, 10);
        frontBackArmDrawer.rotation = SegmentRotations[ArmSegmentIndex];
        frontBackArmDrawer.scale = new Vector2(SegmentBaseScales[ArmSegmentIndex]);
        frontBackArmDrawer.color = drawColor;
        sb.Draw(frontBackArmDrawer);

        for (var i = Segments.Length - 1; i >= 0; i--)
        {
            ref var segment = ref Segments[i];
            var rotation = NPC.rotation;
            var ratio = i / (float)Segments.Length;
            if (i > 0)
            {
                ref var nextSegment = ref Segments[i - 1];
                rotation = (nextSegment.position - segment.position).ToRotation();
            }

            var drawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Npc[Type], segment.position);
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
        var frontWingDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.Illuria.NPCs.NiiviWingFront.Asset, wingSegment.position);
        frontWingDrawer.drawOrigin = FrontWingDrawOrigin;
        frontWingDrawer.VerticalFrame(_wingFrame, WingFrameCount);
        frontWingDrawer.rotation = SegmentRotations[WingSegmentIndex];
        frontWingDrawer.scale = new Vector2(SegmentBaseScales[WingSegmentIndex]);
        frontWingDrawer.color = drawColor;
        sb.Draw(frontWingDrawer);

        //Draw frong arms

        var backFrontArmDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Npc[Type], buttSegment.position);
        backFrontArmDrawer.drawOrigin = SegmentOrigins[8];
        backFrontArmDrawer.VerticalFrame(8, 10);
        backFrontArmDrawer.rotation = SegmentRotations[LegSegmentIndex];
        backFrontArmDrawer.scale = new Vector2(SegmentBaseScales[LegSegmentIndex]);
        backFrontArmDrawer.color = drawColor;
        sb.Draw(backFrontArmDrawer);

        var frontArmDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Npc[Type], neckSegment.position);
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
    void DrawBody2(SpriteBatch sb, Vector2 screenPos, Color drawColor)
    {
        NiiviIceArenaRenderer.effectTimer = 30;
        //Draw tail first, then all the way up to head
        ref var neckSegment = ref Chain.points[ArmSegmentIndex];
        ref var buttSegment = ref Chain.points[LegSegmentIndex];
        ref var wingSegment = ref Chain.points[WingSegmentIndex];

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

        for (var i = Chain.points.Length - 1; i >= 0; i--)
        {
            ref var segment = ref Chain.points[i];
            var rotation = NPC.rotation;
            var ratio = i / (float)Chain.points.Length;
            if (i > 0)
            {
                ref var nextSegment = ref Chain.points[i - 1];
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
        if (_simpleChainRendering)
        {
            DrawBody2(spriteBatch, Main.screenPosition, Color.White);
        }
        else
            DrawBody(spriteBatch, Main.screenPosition, Color.White);
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        drawColor = Color.Lerp(drawColor, Color.Black, _blackAlpha);
        if (_simpleChainRendering)
        {
            DrawBody2(spriteBatch, screenPos, drawColor);
        } 
        else
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
            if (_simpleChainRendering)
            {
                DrawBody2(sb, Main.screenPosition, _outliner.outlineColor);
            }
            else
                DrawBody(sb, Main.screenPosition, _outliner.outlineColor);
  
        }
        OutlineRenderer.Queue(DrawOutlines);
    }
}

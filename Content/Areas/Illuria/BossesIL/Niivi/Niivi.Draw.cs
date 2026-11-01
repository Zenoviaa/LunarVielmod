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
        sb.Draw(backBackArmDrawer);

        var bacWingDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.Illuria.NPCs.NiiviWingBack.Asset, wingSegment.position);
        bacWingDrawer.drawOrigin = BackWingDrawOrigin;
        bacWingDrawer.VerticalFrame(_wingFrame, WingFrameCount);
        bacWingDrawer.rotation = SegmentRotations[WingSegmentIndex];
        bacWingDrawer.scale = new Vector2(SegmentBaseScales[WingSegmentIndex]);
        sb.Draw(bacWingDrawer);

        var frontBackArmDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Npc[Type], neckSegment.position);
        frontBackArmDrawer.drawOrigin = SegmentOrigins[7];
        frontBackArmDrawer.VerticalFrame(7, 10);
        frontBackArmDrawer.rotation = SegmentRotations[ArmSegmentIndex];
        frontBackArmDrawer.scale = new Vector2(SegmentBaseScales[ArmSegmentIndex]);
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
            sb.Draw(drawer);
            SegmentRotations[i] = rotation;
        }

        //Draw front wing
        var frontWingDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.Illuria.NPCs.NiiviWingFront.Asset, wingSegment.position);
        frontWingDrawer.drawOrigin = FrontWingDrawOrigin;
        frontWingDrawer.VerticalFrame(_wingFrame, WingFrameCount);
        frontWingDrawer.rotation = SegmentRotations[WingSegmentIndex];
        frontWingDrawer.scale = new Vector2(SegmentBaseScales[WingSegmentIndex]);
        sb.Draw(frontWingDrawer);

        //Draw frong arms

        var backFrontArmDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Npc[Type], buttSegment.position);
        backFrontArmDrawer.drawOrigin = SegmentOrigins[8];
        backFrontArmDrawer.VerticalFrame(8, 10);
        backFrontArmDrawer.rotation = SegmentRotations[LegSegmentIndex];
        backFrontArmDrawer.scale = new Vector2(SegmentBaseScales[LegSegmentIndex]);
        sb.Draw(backFrontArmDrawer);

        var frontArmDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Npc[Type], neckSegment.position);
        frontArmDrawer.drawOrigin = SegmentOrigins[6];
        frontArmDrawer.VerticalFrame(6, 10);
        frontArmDrawer.rotation = SegmentRotations[ArmSegmentIndex];
        frontArmDrawer.scale = new Vector2(SegmentBaseScales[ArmSegmentIndex]);
        sb.Draw(frontArmDrawer);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        DrawBody(spriteBatch, screenPos, drawColor);
        return false;
    }
    public void DrawToRenderTargets()
    {
        if (!_outliner.VisibleEnough)
            return;
        void DrawOutlines(SpriteBatch sb)
        {
            DrawBody(sb, Main.screenPosition, _outliner.outlineColor); 
        }
        OutlineRenderer.Queue(DrawOutlines);
    }
}

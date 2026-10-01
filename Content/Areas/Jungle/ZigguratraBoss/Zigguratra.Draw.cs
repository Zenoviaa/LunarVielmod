using Stellamod.Common.Animations;
using Stellamod.Core;
using Terraria;

namespace Stellamod.Content.Areas.Jungle.ZigguratraBoss;

public partial class Zigguratra
{
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if(_afterImageAlpha > 0.05f)
        {
            var drawInfo = NPC.GetAnimatorDrawInfo(drawColor);
            for(var i =0; i < NPC.oldPos.Length; i++)
            {

                var ratio = (float)i / (float)NPC.oldPos.Length;
                var afDrawInfo = drawInfo;
                var op = NPC.oldPos[i];
                var posToDraw = op + NPC.Size * 0.5f;
                afDrawInfo.rotation = NPC.oldRot[i];
                afDrawInfo.worldPosition = posToDraw;
                afDrawInfo.color = Color.Lerp(Color.Gold, Color.Transparent, ratio) * 0.15f * _afterImageAlpha;
                afDrawInfo.color.A = 0;

                var offset = afDrawInfo.drawOrigin - this.AseAnimator.centerDrawOrigin;
                afDrawInfo.worldPosition += offset;
                Main.spriteBatch.Draw(afDrawInfo);
            }
        }

        NPC.DrawAnimator(spriteBatch, drawColor * _invisibleAlpha);
        var axeGlowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, AxeBackPosition);
        axeGlowDrawer.color = Color.Gold * ExtraMath.Osc(0.6f, 1f, speed: 16) * 0.6f * _axeLightningAlpha * AxeCrash_ChargeLevel;
        axeGlowDrawer.color.A = 0;
        axeGlowDrawer.scale *= 0.5f;
        spriteBatch.Draw(axeGlowDrawer);

        axeGlowDrawer.color *= 0.5f;
        axeGlowDrawer.scale *= 2f;
        spriteBatch.Draw(axeGlowDrawer);


        axeGlowDrawer.color *= 0.5f;
        axeGlowDrawer.scale *= 2f;
        spriteBatch.Draw(axeGlowDrawer);
        OutlineRenderer.Queue(DrawOutline);
        return false;
    }

    void DrawOutline(SpriteBatch spriteBatch)
    {
        NPC.DrawAnimator(spriteBatch, _outliner.outlineColor * _invisibleAlpha);
    }
}

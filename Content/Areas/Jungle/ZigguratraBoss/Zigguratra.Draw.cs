using Stellamod.Core;

namespace Stellamod.Content.Areas.Jungle.ZigguratraBoss;

public partial class Zigguratra
{
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor);
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
        NPC.DrawAnimator(spriteBatch, _outliner.outlineColor);
    }
}

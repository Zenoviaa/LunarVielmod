using ReLogic.Content;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Special.DeadRomancesExcalibur;

public class HeavenlyMarkGlobalNPC : GlobalNPC
{
    public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        base.PostDraw(npc, spriteBatch, screenPos, drawColor);
        if (npc.HasBuff<HeavenlyMark>())
        {
            Asset<Texture2D> sigilTextureAsset = ModContent.GetInstance<HeavenlyMark>().SigilTextureAsset;
            Vector2 drawOffset = new Vector2(0, -32);
            drawOffset.Y += ExtraMath.Osc(0f, -4f);
            SpritebatchDrawer sigilDrawer = sigilTextureAsset.GetDrawer(npc.Center + drawOffset);
            sigilDrawer.color = Color.LightGoldenrodYellow;
            sigilDrawer.color *= ExtraMath.Osc(0f, 1f, speed: 3);
            sigilDrawer.color.A = 0;
            spriteBatch.Draw(sigilDrawer);
        }
    }
}
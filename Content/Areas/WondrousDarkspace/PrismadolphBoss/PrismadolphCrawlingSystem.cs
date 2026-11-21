using Stellamod.Core.Backgrounds;
using Stellamod.Core.Rendering;
using Stellamod.Core.Rendering.RTs;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.PrismadolphBoss;

[Autoload(Side = ModSide.Client)]
public class PrismadolphCrawlingSystem : ModSystem
{
    enum Segment : byte
    {
        Head,
        Body,
        Tail
    }
    float RotOffset => -MathHelper.PiOver2;
    int PrismadolphSegmentType;
    public Vector2[] Chain => field ??= new Vector2[48];
    public float alpha;
    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += PrepareCrawlingDrawing;
    }
    public override void PostAddRecipes()
    {
        base.PostAddRecipes();
        PrismadolphSegmentType = ModContent.NPCType<PrismadolphSegment>();
    }
    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        var p = Main.LocalPlayer.GetModPlayer<PrismadolphCrawlPlayer>();
        if (!p.isCrawling)
            return;
        p.crawlTimer++;
        if(p.crawlTimer == 1)
        {
            for(var i = 0; i < Chain.Length; i++)
            {
                Chain[i] = p.crawlStartScreenPosition - new Vector2(0, i * 0.04f);
            }
        }
        if(p.crawlTimer >= p.crawlTime)
        {
            p.isCrawling = false;
        }


        var sp = p.closestToPlayer - Main.screenPosition;
        var rect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        if (rect.Contains(sp.ToPoint()))
        {
            alpha = MathHelper.Lerp(alpha, 0f, 0.1f);
        }
        else
        {
            alpha = MathHelper.Lerp(alpha, 1f, 0.1f);
        }

        var diff = Main.screenPosition - Main.screenLastPosition;
        diff *= -0.1f;
        p.crawlStartScreenPosition += new Vector2(diff.X, 0);
        p.crawlTargetScreenPosition += new Vector2(diff.X, 0);

        var startRatio = p.crawlTimer / p.crawlTime;
        for(var i = 0; i < Chain.Length; i++)
        {
            var ratio = startRatio - i * 0.01f;
            Chain[i] = CalculatePoint(
                p.crawlStartScreenPosition, 
                p.crawlTargetScreenPosition, ratio);

        }
        
    }

    Vector2 CalculatePoint(Vector2 start, Vector2 end, float progress)
    {
        var linear = Vector2.Lerp(start, end, progress);
        var perp = (end - start).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);
        linear += perp * MathF.Sin(progress * 64) * 80;
        return linear;
    }

    private void PrepareCrawlingDrawing(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (Main.gameMenu)
            return;
        var p = Main.LocalPlayer.GetModPlayer<PrismadolphCrawlPlayer>();
        if(p.crawlTimer > 0)
        {
            DrawLayerHooks.OverWaterDrawActions.Enqueue(DrawPrismadolphCrawling);
        }
    }

    Segment GetSegment(int i)
    {
        var frame = 0;

        if (i == 0)
            frame = 0;
        else if (i == Chain.Length - 1)
            frame = 2;
        else
            frame = 1;
        return (Segment)frame;
    }
    void DrawUnderbody(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        //loop over all the segments and everything that's underneath
        for (var i = 0; i < Chain.Length; i++)
        {
            var pos = Chain[i];
            float rot = 0F;
            if (i < Chain.Length - 1)
            {
                var nextPos = Chain[i + 1];
                rot = (nextPos - pos).ToRotation();
            }
            else if (i > 0)
            {
                var nextPos = Chain[i - 1];
                rot = (nextPos - pos).ToRotation();
            }
            rot += RotOffset;
            DrawJigglyCore(spriteBatch,pos + Main.screenPosition, rot, GetSegment(i));
        }
    }
    void DrawJigglyCore(SpriteBatch spriteBatch, Vector2 pos, float rot, Segment segment )
    {
        if (segment == Segment.Tail)
            return;
        if (segment == Segment.Head)
            return;

        var tex = TextureAssets.Npc[PrismadolphSegmentType].Value;
        var drawer = SpritebatchDrawer.FromTextureGetLightingColor(tex, pos);
        drawer.VerticalFrame(Prismadolph.FRAME_CORE, Main.npcFrameCount[PrismadolphSegmentType]);
        drawer.CenterOrigin();
        drawer.rotation = rot ;
        spriteBatch.Draw(drawer);
    }

    void DrawArmor(SpriteBatch spriteBatch, Vector2 pos, float rot, Segment segment)
    {
        if (segment == Segment.Head)
        {
            DrawArmorHead(spriteBatch, pos, rot, segment);
        }
        else if (segment == Segment.Tail)
        {
            DrawArmorTail(spriteBatch, pos, rot, segment);
        }
        else
        {
            var tex = TextureAssets.Npc[PrismadolphSegmentType].Value;
            var drawer = SpritebatchDrawer.FromTextureGetLightingColor(tex, pos);
            drawer.VerticalFrame(Prismadolph.FRAME_ARMOR, Main.npcFrameCount[PrismadolphSegmentType]);
            drawer.CenterOrigin();
            drawer.rotation = rot;
            spriteBatch.Draw(drawer);
        }
    }

    void DrawArmorTail(SpriteBatch spriteBatch, Vector2 pos, float rot, Segment segment)
    {
        var tex = TextureAssets.Npc[PrismadolphSegmentType].Value;
        var drawer = SpritebatchDrawer.FromTextureGetLightingColor(tex, pos);
        drawer.VerticalFrame(Prismadolph.FRAME_TAIL, Main.npcFrameCount[PrismadolphSegmentType]);
        drawer.CenterOrigin();
        drawer.rotation = rot ;
        spriteBatch.Draw(drawer);
    }

    void DrawArmorHead(SpriteBatch spriteBatch, Vector2 pos, float rot, Segment segment)
    {
        var offY = 12;

        var tex = TextureAssets.Npc[PrismadolphSegmentType].Value;
        var drawer = SpritebatchDrawer.FromTextureGetLightingColor(tex, pos);
        drawer.VerticalFrame(Prismadolph.FRAME_HEAD, Main.npcFrameCount[PrismadolphSegmentType]);
        drawer.CenterOrigin();
        drawer.drawOrigin.Y += offY;
        drawer.rotation = rot;
        spriteBatch.Draw(drawer);

    }

    void DrawArmor(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        for (var i = 0; i < Chain.Length; i++)
        {
            var pos = Chain[i];
            float rot = 0F;
            if (i < Chain.Length - 1)
            {
                var nextPos = Chain[i + 1];
                rot = (pos - nextPos).ToRotation();
            }
            else if (i > 0)
            {
                var nextPos = Chain[i - 1];
                rot = (nextPos - pos).ToRotation();
            }
            rot += RotOffset;
           DrawArmor(spriteBatch, pos + Main.screenPosition, rot, GetSegment(i));
        }
    }

    void DrawEntireSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        DrawUnderbody(spriteBatch, screenPos, drawColor);
        DrawArmor(spriteBatch, screenPos, drawColor);

    }
    void DrawPrismadolphCrawling(SpriteBatch sb)
    {
        //Should render exactly the same as the other thing
        sb.EndOut(out var oldParameters);
        using var src = RT.Context(RenderTargets.ScreenTarget);
        using var dst = RT.Context(RenderTargets.ScreenTarget);

        using (RT.Clear(src, Color.Transparent))
        {
            sb.Begin(oldParameters with { matrix = Matrix.Identity });
            DrawEntireSprite(sb, Main.screenPosition, Color.White);
            sb.End();
        }

        GaussianBlurHelper.ScreenBlur(src, dst, 8);


        sb.Begin(oldParameters);
        sb.Draw(dst, Vector2.Zero - dst.Size() * 0.5F, null, Color.Black * alpha, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
        sb.End();
        sb.Begin(oldParameters);
    }
}

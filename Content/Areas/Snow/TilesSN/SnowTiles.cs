using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering;
using Stellamod.WorldG;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Content.Areas.Snow.TilesSN;

public class ThickSnow : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<ThickSnowTile>());
    }
}

public class ThickSnowGlobalTile : GlobalTile
{
    public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        base.DrawEffects(i, j, type, spriteBatch, ref drawData);
        if (TileSets.ThickSnow[type])
        {
            bool left = Framing.GetTileSafely(i - 1, j).HasTile;
            bool right = Framing.GetTileSafely(i + 1, j).HasTile;
            bool up = Framing.GetTileSafely(i, j - 1).HasTile;
            if (left && right && !up)
                ThickSnowRenderer.SpecialPoints.Add(new Point(i, j));
        }
    }
}

public class ThickSnowTile : ModTile
{
    public override void SetStaticDefaults()
    {

        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;

        //Main.tileFrameImportant[Type] = true;
        Main.tileLargeFrames[Type] = 2;
        TileID.Sets.ChecksForMerge[Type] = true;
        TileSets.ThickSnow[Type] = true;
        HitSound = SoundID.Item48;
        RegisterItemDrop(ModContent.ItemType<ThickSnow>());
        AddMapEntry(new Color(40, 40, 140));
    }
}



[Autoload(Side = ModSide.Client)]
public class ThickSnowRenderer : ModSystem
{
    private UnifiedRandom _snowRandom = new UnifiedRandom();
    private Asset<Texture2D> _snowPileAsset = null!;
    public static readonly HashSet<Point> SpecialPoints = new();
    public override void Load()
    {
        base.Load();
        On_Main.RenderTiles += ResetSpecialPoints;
        On_Main.DrawPlayers_AfterProjectiles += RenderSnow;
    }


    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _snowPileAsset = ModContent.Request<Texture2D>(ModContent.GetInstance<ThickSnowTile>().Texture + "_Pile");
    }
    private void ResetSpecialPoints(On_Main.orig_RenderTiles orig, Main self)
    {
        if (!Main.drawToScreen)
        {
            SpecialPoints.Clear();
        }
        orig(self);
    }

    private void RenderSnow(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
    {
        orig(self);
        if (SpecialPoints.Count <= 0)
            return;
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Begin(
            SpriteSortMode.Deferred, 
            BlendState.AlphaBlend, 
            SamplerState.PointClamp, 
            DepthStencilState.None,
            Main.Rasterizer, 
            null, 
            Main.GameViewMatrix.TransformationMatrix);
        foreach (Point point in SpecialPoints)
        {
            SpecialDraw(point.X, point.Y, Main.spriteBatch);
        }
        spriteBatch.End();
    }

    private Rectangle GetFrame(int index)
    {
        int frameCount = 4;
        int frameWidth = _snowPileAsset.Width();
        int frameHeight = _snowPileAsset.Height() / frameCount;
        Rectangle sourceRect = new Rectangle(0, frameHeight * index, frameWidth, frameHeight);
        return sourceRect;
    }

    public void SpecialDraw(in int i, in int j, SpriteBatch spriteBatch)
    {
        _snowRandom.SetSeed(i + j);
        if (!_snowRandom.NextBool(3))
            return;
        Vector2 snowPosition = (new Vector2(i, j) + VeilGen.TileAdj) * 16;
        snowPosition.Y += 4;
        snowPosition -= new Vector2(Main.offScreenRange);

        Rectangle frame;
        int index = _snowRandom.Next(0, 4);
        frame = GetFrame(index);
        Color lightColor = Lighting.GetColor(i, j);

        float scale = _snowRandom.NextFloat(0.5f, 1f);
        spriteBatch.Draw(_snowPileAsset.Value, snowPosition - Main.screenPosition, frame, lightColor, 0, frame.Size() * new Vector2(0.5f, 1f), scale, SpriteEffects.None, 0);
        snowPosition.Y += 12;
        spriteBatch.Draw(_snowPileAsset.Value, snowPosition - Main.screenPosition, frame, lightColor, 0, frame.Size() * new Vector2(0.5f, 1f), scale, SpriteEffects.FlipVertically, 0);
    }
}
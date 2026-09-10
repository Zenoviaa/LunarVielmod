using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Content.Areas.Tundra.Abyss.TilesAB;

public class AbyssalKelp : ModTile,
    IWaterTileSilhouette
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        LocalizedText name = CreateMapEntryName();
        Main.tileLighted[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;
        AddMapEntry(new Color(169, 200, 93), name);
       // RegisterItemDrop(ItemID.Wood);
    }

    private float GetLeafSway(float offset, float magnitude, float speed)
    {
        return (float)Math.Sin(Main.GameUpdateCount * speed + offset) * magnitude;
    }

    private void DrawKelpGlow(int i, int j, SpriteBatch spriteBatch)
    {
        Tile tile = Main.tile[i, j];
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Tile[Type],
            TileUtilities.ToWorldCoordinatesFromTileRendering(i, j));
        drawer.color = Color.Aqua * 0.5f;
        drawer.color.A = 0;
        drawer.rotation = GetLeafSway(i + j, 0.07f, 0.05f);
        drawer.VerticalFrame(tile.TileFrameY + 6, 12);
        drawer.BottomCenterOrigin();
        drawer.worldPosition.Y += drawer.sourceRect.Value.Height;
        spriteBatch.Draw(drawer);
    }
    private void DrawKelp(int i, int j, SpriteBatch spriteBatch)
    {
        Tile tile = Main.tile[i, j];
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Tile[Type],
            TileUtilities.ToWorldCoordinatesFromTileRendering(i, j));
        drawer.color = Color.White.MultiplyRGB(Lighting.GetColor(i, j));
        drawer.rotation = GetLeafSway(i + j, 0.07f, 0.05f);
        drawer.VerticalFrame(tile.TileFrameY, 12);
        drawer.BottomCenterOrigin();
        drawer.worldPosition.Y += drawer.sourceRect.Value.Height;
        spriteBatch.Draw(drawer);

    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        Main.tileSolid[Type] = false;
       
        DrawKelpGlow(i, j, spriteBatch);
        Main.instance.TilesRenderer.AddSpecialLegacyPoint(new Point(i, j));

        return false;
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        base.DrawEffects(i, j, spriteBatch, ref drawData);

    }
 
    
    public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
    {
        base.SpecialDraw(i, j, spriteBatch);
        DrawKelp(i, j, spriteBatch);
    }
    public void PrepareSilhouetteDrawing(int i, int j, RekSilhouetteSystem system)
    {
        system.SilhouettesToDraw.Add((SpriteBatch sb) =>
        {
            DrawWaterSilhouette(i, j, sb);
        });
    }

    public void DrawWaterSilhouette(int i, int j, SpriteBatch sb)
    {
        Tile tile = Main.tile[i, j];
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Tile[Type],
            new Point(i, j).ToWorldCoordinates());
        drawer.color = Color.DarkBlue;
        drawer.rotation = GetLeafSway(i + j, 0.07f, 0.05f);
        drawer.VerticalFrame(tile.TileFrameY, 12);
        drawer.BottomCenterOrigin();
        drawer.worldPosition.Y += drawer.sourceRect.Value.Height;
        drawer.worldPosition.X -= 8;
        sb.Draw(drawer);
      
    }
    
    public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
        if (fail || effectOnly)
            return;

        Framing.GetTileSafely(i, j).HasTile = false;

        bool up = Framing.GetTileSafely(i, j - 1).TileType == Type || Framing.GetTileSafely(i, j - 1).TileType == Type;
        bool down = Framing.GetTileSafely(i, j + 1).TileType == Type;

        if (up)
            WorldGen.KillTile(i, j - 1);
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
    {
        short x = 0;
        short y = 0;

        bool up = Framing.GetTileSafely(i, j - 1).TileType == Type|| Framing.GetTileSafely(i, j - 1).TileType == Type;
        bool down = Framing.GetTileSafely(i, j + 1).TileType == Type;

        if (up && down)
        {
            y = (short)Main.rand.Next(1, 3);
        }
        else if (up)
        {
            y = 0;
        }
        else if (down)
        {
            y = (short)Main.rand.Next(4, 6);
        }

        Tile tile = Framing.GetTileSafely(i, j);
        tile.TileFrameX = 255;
        tile.TileFrameY = y;
        return false;
    }


}

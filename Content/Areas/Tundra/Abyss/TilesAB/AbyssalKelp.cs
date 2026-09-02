using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.TilesAB;

public class AbyssalKelp : ModTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        LocalizedText name = CreateMapEntryName();
        TileID.Sets.IsATreeTrunk[Type] = true;
        Main.tileAxe[Type] = true;
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
        DrawKelpGlow(i, j, spriteBatch);
        DrawKelp(i, j, spriteBatch);
        return false;
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
        tile.TileFrameX = x;
        tile.TileFrameY = y;
        return false;
    }
}

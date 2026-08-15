using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.TilesTR;

internal class BridewellTileItem : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<BridewellTile>());
    }
}
public class BridewellTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;

        //Main.tileFrameImportant[Type] = true;
        Main.tileLargeFrames[Type] = 2;
        TileID.Sets.ChecksForMerge[Type] = true;
        MineResist = 2f;
        MinPick = 225;
        RegisterItemDrop(ModContent.ItemType<BridewellTileItem>());
        AddMapEntry(new Color(200, 40, 40));
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
 
        base.PostDraw(i, j, spriteBatch);
        Vector2 pos = (new Vector2(i, j)) * 16;
        pos += new Vector2(Main.offScreenRange);
        Color color = Lighting.GetColor(i, j);

        Tile tile = Framing.GetTileSafely(i, j);

        //Glow mask is 288 pixels over
        Rectangle frame = new Rectangle(tile.TileFrameX + 234, tile.TileFrameY, 16, 16);
        Color glowColor = Color.Lerp(Color.White, new Color(125, 125, 125), ExtraMath.Osc(0f, 1f));
        glowColor.A = 0;
        spriteBatch.Draw(TextureAssets.Tile[Type].Value, pos - Main.screenPosition, frame, glowColor, 0, Vector2.Zero, 1, 0, 1);
    }
}
internal class ScarletBrick : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<ScarletBrickTile>());
    }
}
internal class ScarletBrickTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;

        //Main.tileFrameImportant[Type] = true;
        Main.tileLargeFrames[Type] = 2;
        TileID.Sets.ChecksForMerge[Type] = true;
        MineResist = 2f;
        MinPick = 225;
        RegisterItemDrop(ModContent.ItemType<ScarletBrick>());
        AddMapEntry(new Color(200, 40, 40));
    }
}

internal class ScarletDiamondBrick : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<ScarletDiamondBrickTile>());
    }
}
internal class ScarletDiamondBrickTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;

        //Main.tileFrameImportant[Type] = true;
        Main.tileLargeFrames[Type] = 2;
        TileID.Sets.ChecksForMerge[Type] = true;
        MineResist = 2f;
        MinPick = 225;
        RegisterItemDrop(ModContent.ItemType<ScarletBrick>());
        AddMapEntry(new Color(200, 40, 40));
    }
}
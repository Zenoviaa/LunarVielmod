using ReLogic.Content;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Tiles;

public class CathediteTile : ModTile
{
    private Asset<Texture2D> _glowTextureAsset;
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMergeDirt[Type] = true;
        Main.tileBlockLight[Type] = true;
        RegisterItemDrop(ModContent.ItemType<Items.Placeable.Cathedral.CathediteBlock>());
        DustType = Main.rand.Next(110, 113);

        MineResist = 2f;
        MinPick = 225;

        AddMapEntry(new Color(2, 14, 26));

        // TODO: implement
        // SetModTree(new Trees.ExampleTree());
    }
    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 1 : 3;
    }
    public override bool CanExplode(int i, int j) => false;
    // TODO: implement
    // public override void ChangeWaterfallStyle(ref int style) {
    // 	style = mod.GetWaterfallStyleSlot("ExampleWaterfallStyle");
    // }
    //
    // public override int SaplingGrowthType(ref int style) {
    // 	style = 0;
    // 	return TileType<ExampleSapling>();
    // }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        base.PostDraw(i, j, spriteBatch);
        _glowTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Outline");
        Vector2 pos = (new Vector2(i, j)) * 16;
        pos += new Vector2(Main.offScreenRange);
        Color color = Lighting.GetColor(i, j);

        Tile tile = Framing.GetTileSafely(i, j);

        Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
        Color glowColor = Color.Lerp(Color.Pink, Color.Blue, ExtraMath.Osc(0f, 1f));
        glowColor.A = 0;
        spriteBatch.Draw(_glowTextureAsset.Value, pos - Main.screenPosition, frame, glowColor, 0, Vector2.Zero, 1, 0, 1);
    }
}
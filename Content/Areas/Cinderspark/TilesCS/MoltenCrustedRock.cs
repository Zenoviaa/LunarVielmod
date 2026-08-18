using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.TilesCS;

public class MoltenCrustedRock : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<MoltenCrustedRockTile>());
    }
}

public class MoltenCrustedRockTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMerge[Type][Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileBlendAll[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileShine[Type] = 45000;
        Main.tileLighted[Type] = true;
        AddMapEntry(new Color(100, 25, 40));
        RegisterItemDrop(ModContent.ItemType<MoltenCrustedRock>());
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        base.ModifyLight(i, j, ref r, ref g, ref b);
        //r = 0.12f;
    }

    public override bool CanExplode(int i, int j) => true;
    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 1 : 3;
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        base.DrawEffects(i, j, spriteBatch, ref drawData);
        Tile tile = Framing.GetTileSafely(i, j);
        if (tile.Slope != Terraria.ID.SlopeType.Solid)
            return;
        Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
    }

    public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
    {
        base.SpecialDraw(i, j, spriteBatch);
        Tile tile = Framing.GetTileSafely(i, j);
        Vector2 pos = (new Vector2(i, j)) * 16;
        pos += new Vector2(Main.offScreenRange);

        //Glow mask is 288 pixels over
        Rectangle frame = new Rectangle(tile.TileFrameX + 288, tile.TileFrameY, 16, 16);
        Color glowColor = Color.Lerp(Color.Yellow, Color.Red, ExtraMath.Osc(0f, 1f, offset: i * j));
        glowColor *= ExtraMath.Osc(0f, 1f, offset: i * j);
        glowColor.A = 0;
        spriteBatch.Draw(TextureAssets.Tile[Type].Value, pos - Main.screenPosition, frame, glowColor, 0, Vector2.Zero, 1, 0, 1);
    }
    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        base.PostDraw(i, j, spriteBatch);

    }
}


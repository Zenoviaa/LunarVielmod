using ReLogic.Content;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace Stellamod.Content.Areas.Underground.TilesUG;

public class CharredStoneBlock : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<CharredStone>());
    }
}

public class CharredStone : ModTile
{
    private Asset<Texture2D> _glowTextureAsset;
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMergeDirt[Type] = true;
        Main.tileBlockLight[Type] = true;

        TileID.Sets.Stone[Type] = true;
        Main.tileLighted[Type] = true;
        HitSound = new SoundStyle("Stellamod/Assets/Sounds/HardRockHit") with { PitchVariance = 0.8f };
        DustType = DustID.Torch;
        MineResist = 1f;

        RegisterItemDrop(ModContent.ItemType<CharredStoneBlock>());
        AddMapEntry(new Color(25, 25, 25));
    }
    public override void Unload()
    {
        base.Unload();
        _glowTextureAsset = null;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        base.ModifyLight(i, j, ref r, ref g, ref b);
       r = MathF.Max(r, ExtraMath.Osc(0.25f, 0.75f, speed: 2, offset: i + j));
        
    }

    public override bool CanExplode(int i, int j) => true;
    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 1 : 3;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        return base.PreDraw(i, j, spriteBatch);
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        base.PostDraw(i, j, spriteBatch);
        Main.tileShine[Type] = 45000;
        Main.tileLighted[Type] = true;
        _glowTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Outline");
        Vector2 pos = (new Vector2(i, j)) * 16;
        pos += new Vector2(Main.offScreenRange);
        Color color = Lighting.GetColor(i, j);

        Tile tile = Framing.GetTileSafely(i, j);

        Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
        Color glowColor = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f)) * Lighting.Brightness(i, j) * ExtraMath.Osc(0f, 1f, speed: 2, offset: i + j);
        glowColor.A = 0;
        spriteBatch.Draw(_glowTextureAsset.Value, pos - Main.screenPosition, frame, glowColor, 0, Vector2.Zero, 1, 0, 1);
    }
}

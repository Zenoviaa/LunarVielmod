using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.TilesFB;

public class FableGrass : ModTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableGrass.KEY;
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;

        //Main.tileFrameImportant[Type] = true;
        Main.tileLargeFrames[Type] = 2;
        TileID.Sets.ChecksForMerge[Type] = true;
        MineResist = 2f;
        MinPick = 225;
        RegisterItemDrop(ModContent.ItemType<FableGrassItem>());
        AddMapEntry(new Color(101, 114, 60));
    }
}
public class FableGrassItem : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableGrassItem.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<FableGrass>());
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.TilesFB;


internal class WitchCarvedStoneItem : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.WitchCarvedStoneItem.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<WitchCarvedStone>());
    }
}
public class WitchCarvedStone : ModTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.WitchCarvedStone.KEY;
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;

        //Main.tileFrameImportant[Type] = true;
        Main.tileLargeFrames[Type] = 2;
        TileID.Sets.ChecksForMerge[Type] = true;
        MineResist = 2f;
        MinPick = 225;
        RegisterItemDrop(ModContent.ItemType<WitchCarvedStoneItem>());
        AddMapEntry(new Color(139, 108, 74));
    }
}

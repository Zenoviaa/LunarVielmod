using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Ishtar.TilesIS;

public class IshtarTempleBrick : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<IshtarTempleBlock>());
    }
}
public class IshtarTempleBlock : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMerge[Type][Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileBlendAll[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileBlockLight[Type] = true;
        RegisterItemDrop(ModContent.ItemType<IshtarTempleBrick>());
        AddMapEntry(new Color(6, 5, 7));
        MineResist = 8f;
        MinPick = 200;
        

    }

    public override bool CanExplode(int i, int j) => false;
    public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
    }
}
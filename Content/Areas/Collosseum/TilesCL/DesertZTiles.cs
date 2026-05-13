using Stellamod.Core.ZTileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Stellamod.Tiles.SpecialDecorativeWall;

namespace Stellamod.Content.Areas.Collosseum.TilesCL;

public class Tomb : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class DesertRedBanner : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 3;
        drawOrigin = TileDrawOrigin.TopDown;
    }
}
public class DesertBloodPot : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class HangingDesertBloodPot : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.TopDown;
    }
}
public class DesertBackground : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 3;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class DesertRunicWall1 : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 6;
        drawOrigin = TileDrawOrigin.Center;
    }
}
public class DesertRunicWall2 : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 6;
        drawOrigin = TileDrawOrigin.Center;
    }
}
public class DesertRunicWall3 : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 6;
        drawOrigin = TileDrawOrigin.Center;
    }
}
public class DesertBellPile : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class DesertBookshelves : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 3;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
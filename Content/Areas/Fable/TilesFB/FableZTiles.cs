using Stellamod.Core.ZTileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace Stellamod.Content.Areas.Fable.TilesFB;

public class FableBackBrick : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 2;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class FableBackBrickBig : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class FableBox : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class FableWall : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class FableSkinnyWall : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class SkullSpike : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 2;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class FableLanternSmall : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
    public override void Update(int i, int j)
    {
        base.Update(i, j);
        Lighting.AddLight(i, j, TorchID.Torch, 0.5f);
    }
    public override void PostDraw(SpriteBatch spriteBatch, in ZTileDrawData drawData, in ZTileDrawParams drawParams)
    {
        base.PostDraw(spriteBatch, drawData, drawParams);
    }
}
public class FableLanternBig : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
    public override void Update(int i, int j)
    {
        base.Update(i, j);
        Lighting.AddLight(i, j, TorchID.Torch, 0.5f);
    }
    public override void PostDraw(SpriteBatch spriteBatch, in ZTileDrawData drawData, in ZTileDrawParams drawParams)
    {
        base.PostDraw(spriteBatch, drawData, drawParams);
    }
}

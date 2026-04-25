using Stellamod.Content.Areas.MoonspiralTower.TilesMT;
using Stellamod.Core.ZTileSystem;

namespace Stellamod.Content.Areas.Terror.TilesTR;

public class AegislavFlower : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 4;
        drawOrigin = TileDrawOrigin.BottomUp;
        
        windSwayOffset = 0f;
        windSwayMagnitude = 0.2f;
        windSwaySpeed = 0.02f;
    }
}

public class AegislavHangingCage : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 3;
        drawOrigin = TileDrawOrigin.TopDown;
        
        windSwayOffset = 0f;
        windSwayMagnitude = 0.2f;
        windSwaySpeed = 0.02f;
    }
}

public class BloodyCauldron : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class HangingBloodyCauldron : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.TopDown;

        windSwayOffset = 0f;
        windSwayMagnitude = 0.2f;
        windSwaySpeed = 0.02f;
    }
}
public class DeadShrub : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 3;
        drawOrigin = TileDrawOrigin.BottomUp;

        windSwayOffset = 0f;
        windSwayMagnitude = 0.2f;
        windSwaySpeed = 0.02f;
    }
}
public class BloodCathedralWindow : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class TheDreadmire : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.Center;
    }
}
public class GrimmingPainting : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.Center;
    }
}
public class AegislavBlade : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class AegislavBookcase : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 2;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class AegislavSmallChain : AbstractZTileChain { }
public class AegislavLargeChain : AbstractZTileChain { }



public class HangingSmallBottle : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 4;
        drawOrigin = TileDrawOrigin.TopDown;

        windSwayOffset = 0f;
        windSwayMagnitude = 0.2f;
        windSwaySpeed = 0.02f;
    }
}

public class HangingMediumBottle : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 2;
        drawOrigin = TileDrawOrigin.TopDown;

        windSwayOffset = 0f;
        windSwayMagnitude = 0.2f;
        windSwaySpeed = 0.02f;
    }
}

public class HangingBigBottle : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 3;
        drawOrigin = TileDrawOrigin.TopDown;

        windSwayOffset = 0f;
        windSwayMagnitude = 0.2f;
        windSwaySpeed = 0.02f;
    }
}


public class HangingBloodyPot : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.TopDown;

        windSwayOffset = 0f;
        windSwayMagnitude = 0.2f;
        windSwaySpeed = 0.02f;
    }
}



public class BloodAltar : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class BloodBath : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
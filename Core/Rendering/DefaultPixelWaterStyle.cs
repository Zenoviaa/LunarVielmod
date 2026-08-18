namespace Stellamod.Core.Rendering;

/// <summary>
/// Default pixel water that looks like the ocean
/// </summary>
public class DefaultPixelWaterStyle : PixelWaterStyle
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        //Set a priority to negative one so it goes dead last
        priority = -1;
    }
}

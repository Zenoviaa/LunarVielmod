using Stellamod.Common.Platforms;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;


public class BigMoltenPlatform : AbstractPlatformNPC
{
    public override Point GetPlatformSize()
    {
        return new Point(856, 64);
    }
    public override Point GetPlatformOffset()
    {
        return new Point(0, -176);
    }
}

public class SmallMoltenPlatform : AbstractPlatformNPC
{
    public override Point GetPlatformSize()
    {
        return new Point(146, 130);
    }
}
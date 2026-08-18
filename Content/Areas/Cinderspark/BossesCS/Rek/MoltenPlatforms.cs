using Stellamod.Common.Platforms;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;


public class BigMoltenPlatform : AbstractPlatformNPC
{
    public override Point GetPlatformSize()
    {
        return new Point(416, 64);
    }
}

public class SmallMoltenPlatform : AbstractPlatformNPC
{
    public override Point GetPlatformSize()
    {
        return new Point(146, 50);
    }
}
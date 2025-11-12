namespace Stellamod.Core.LunarLightingSystem
{
    public enum PointLightState : byte
    {
        INACTIVE = 0,
        ACTIVE = 1,
        NEEDS_UPDATING=2,
        NEEDS_BAKING=3,
        CUSTOM=4
    }
}

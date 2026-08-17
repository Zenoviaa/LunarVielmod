using System;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.ProjectileHelpers;

[Flags]
public enum DebuffFlags
{
    None = 0,
    Burning_Serpent = 1
}

public class ProjectileSets : ModSystem
{
    public static bool[] ResistedByFlamecrestShield;
    public static bool[] ResetBossMultihitDamageFalloff;
    public static bool[] BossMultihitDamageFalloff;
    public static DebuffFlags[] CommonDebuffs;
    public override void ResizeArrays()
    {
        base.ResizeArrays();
        ResistedByFlamecrestShield = ProjectileID.Sets.Factory.CreateBoolSet(false);
        ResetBossMultihitDamageFalloff = ProjectileID.Sets.Factory.CreateBoolSet(false);
        BossMultihitDamageFalloff = ProjectileID.Sets.Factory.CreateBoolSet(false);
        CommonDebuffs = ProjectileID.Sets.Factory.CreateCustomSet<DebuffFlags>(DebuffFlags.None);
    }
}


public static class ProjectileSetExtensions
{
    public static void AddCommonDebuff(this ModProjectile proj, DebuffFlags flags)
    {
        ProjectileSets.CommonDebuffs[proj.Type] |= flags;
    }
}
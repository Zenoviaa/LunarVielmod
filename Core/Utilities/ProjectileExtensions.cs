using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Core.Utilities;

public static class ProjectileExtensions
{
    /// <summary>
    /// Sets the trail cache length and trailing mode for a projectile, this function just exists to remove some boiler-plate
    /// </summary>
    /// <param name="proj"></param>
    /// <param name="cacheLength"></param>
    /// <param name="trailingMode"></param>
    public static void SetTrailCacheLength(this Projectile proj, int cacheLength, int trailingMode = 2)
    {
        ProjectileID.Sets.TrailCacheLength[proj.type] = cacheLength;
        ProjectileID.Sets.TrailingMode[proj.type] = trailingMode;
    }

    public static bool TryGetNPCParent(this Projectile proj, out NPC npc)
    {
        IEntitySource sourc = proj.GetSource_FromThis();
        if (sourc is EntitySource_Parent entityParent)
        {
            if (entityParent.Entity is NPC n)
            {
                
                npc = n;
                return true;
            }
        }
        npc = null;
        return false;
    }
}

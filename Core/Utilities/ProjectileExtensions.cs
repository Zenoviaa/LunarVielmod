using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Core.Utilities;

public static class ProjectileExtensions
{
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

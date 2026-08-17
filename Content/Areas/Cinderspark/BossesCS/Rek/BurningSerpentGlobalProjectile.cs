using Stellamod.Core.ProjectileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;


public class BurningSerpentGlobalProjectile : GlobalProjectile
{
    public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(projectile, target, info);
        if (ProjectileSets.CommonDebuffs[projectile.type].HasFlag(DebuffFlags.Burning_Serpent))
        {
            //TODO: DO SOMETHINGGG
        }
    }
}

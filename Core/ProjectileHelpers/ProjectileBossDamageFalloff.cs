using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.ProjectileHelpers
{
    public class ProjectileBossDamageFalloff : GlobalNPC
    {
        private float _hitCount;
        public override bool InstancePerEntity => true;
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitByProjectile(npc, projectile, ref modifiers);
            if (!npc.boss)
                return;
            bool shouldResetFallOff = ProjectileSets.ResetBossMultihitDamageFalloff[projectile.type];
            if (shouldResetFallOff)
            {
                _hitCount = 0;
            }
            bool shouldFallOff = ProjectileSets.BossMultihitDamageFalloff[projectile.type];
            if (shouldFallOff)
            {
                _hitCount++;
                float modifier = 1 / _hitCount;
                modifiers.FinalDamage *= modifier;
            }
        }
    }
}

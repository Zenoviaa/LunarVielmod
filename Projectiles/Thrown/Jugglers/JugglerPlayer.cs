using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown.Jugglers
{
    internal class JugglerPlayer : ModPlayer
    {
        private int[] _legalProjectiles = new int[]
        {
            ModContent.ProjectileType<BasicBaseballProj>(),
            ModContent.ProjectileType<SpikedLobberProj>(),
            ModContent.ProjectileType<StickyCardsProj>(),
            ModContent.ProjectileType<DaggerDaggerProj>(),
            ModContent.ProjectileType<DaggerDaggerProj2>(),
            ModContent.ProjectileType<CinderBomberProj>(),
            ModContent.ProjectileType<LilStingerProj>(),
            ModContent.ProjectileType<FlinchMachineProj>()
        };

        public float DamageBonus;
        public int CatchCount;
        public bool SpecialAttack;
        public void ResetJuggle()
        {
            DamageBonus = 0f;
            CatchCount = 0;
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            bool isLegal = false;
            for(int i = 0; i < _legalProjectiles.Length; i++)
            {
                if(proj.type == _legalProjectiles[i])
                {
                    isLegal = true;
                    break;
                }
            }

            if (!isLegal)
                return;

            modifiers.ScalingBonusDamage += DamageBonus;
        }
    }
}

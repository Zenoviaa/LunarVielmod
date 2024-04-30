using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown.Jugglers
{
    internal class JugglerPlayer : ModPlayer
    {
        private int[] _legalProjectiles = new int[]
        {
            ModContent.ProjectileType<BasicBaseballProj>()
        };

        public float DamageBonus;
        public int HitCount;
        public void ResetJuggle()
        {
            DamageBonus = 0f;
            HitCount = 0;
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

            HitCount++;
            modifiers.ScalingBonusDamage += DamageBonus;
        }
    }
}

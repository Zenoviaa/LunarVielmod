using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Colosseum.Common
{
    public abstract class BaseColosseumNPC : ModNPC
    {
        public override bool CheckActive()
        {
            return !ColosseumWaveManager.IsActive();
        }

        public override void OnKill()
        {
            base.OnKill();
            ColosseumWaveManager.ColosseumEnemyKilled();
        }

        protected bool IsColosseumActive()
        {
            return ColosseumWaveManager.IsActive();
        }

        protected void DespawnExplosion()
        {
            for (int i = 0; i < 24; i++)
            {
                float f = i;
                float num = 24;
                float progress = f / num;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 vel = rot.ToRotationVector2() * 6;
                Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, vel);
            }
            NPC.active = false;
        }
    }
}

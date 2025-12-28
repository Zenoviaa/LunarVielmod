using Microsoft.Xna.Framework;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
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
        public sealed override void AI()
        {
            base.AI();
            if (!IsColosseumActive())
            {
                Despawning_AI();
            }
            Colosseum_AI();
        }
        public virtual void Despawning_AI()
        {
            DespawnExplosion();
        }
        public virtual void Colosseum_AI()
        {

        }

        public void GintzeHitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 2; k++)
            {
                Vector2 pos = NPC.position;
                pos.X += Main.rand.Next(0, NPC.width);
                pos.Y += Main.rand.Next(0, NPC.height);
                DustParticle dp = Particle<DustParticle>.Spawn(pos, Vector2.UnitX * hit.HitDirection * Main.rand.NextFloat(1f, 4f), Scale: 0.5f);
                dp.outerColor = Color.DarkGray;
                dp.gravity = 0.01f;
                dp.fast = true;
            }
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

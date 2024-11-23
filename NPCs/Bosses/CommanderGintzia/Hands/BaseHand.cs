using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.CommanderGintzia.Hands
{
    internal abstract class BaseHand : ModNPC
    {
        protected enum AIState
        {
            Orbit,
            Attack,
            DoAttack,
            Despawn,
            DoTransition,
            Transition,
            DoDeath,
        }

        private float OrbitProgress;
        private float DespawnProgress;
        protected ref float Timer => ref NPC.ai[0];
        protected AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }
        protected int ParentIndex
        {
            get => (int)NPC.ai[2];
            set => NPC.ai[2] = (int)value;
        }
        protected ref float RotationTimer => ref NPC.ai[3];

        protected NPC Parent => Main.npc[ParentIndex];
        protected Player Target => Main.player[NPC.target];
        protected Vector2 DirectionToTarget
        {
            get
            {
                return (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            }
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            NPCID.Sets.TrailCacheLength[Type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 16;
            NPC.height = 16;
            NPC.damage = 40;
            NPC.defense = 1;
            NPC.lifeMax = 100;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return State == AIState.Attack;
        }

        public override void AI()
        {
            base.AI();
            RotationTimer++;
            if (State == AIState.DoAttack)
            {
                SwitchState(AIState.Attack);
            }
            if (State == AIState.DoTransition)
            {
                SwitchState(AIState.Transition);
            }
            if (State == AIState.DoDeath)
            {
                SwitchState(AIState.Despawn);
            }
            bool shouldKill = !Parent.active || Parent.type != ModContent.NPCType<CommanderGintzia>();
            if (shouldKill && State != AIState.Despawn)
            {
                SwitchState(AIState.Despawn);
            }
  
            switch (State)
            {
                case AIState.Orbit:
                    AI_Orbit();
                    break;
                case AIState.Attack:
                    AI_Attack();
                    break;
                case AIState.Despawn:
                    AI_Despawn();
                    break;
                case AIState.Transition:
                    AI_Transition();
                    break;
            }
        }

        private void AI_Transition()
        {
            Timer++;
            float progress = Timer / 180f;
            float easedProgress = Easing.SpikeOutCirc(progress);
            RotationTimer += (easedProgress * 3f);
            AI_Orbit();
            if (Timer >= 180)
            {
                SwitchState(AIState.Orbit);
            }
        }
        private void AI_Despawn()
        {
            Timer++;
            DespawnProgress = Timer / 60f;
            NPC.velocity *= 0.92f;
            if(Timer >= 60)
            {
                NPC.Kill();
            }
        }

        protected virtual void AI_Orbit()
        {

            float swingRange = MathHelper.TwoPi;
            float swingXRadius = 128;
            float swingYRadius = 48;
            float swingProgress = RotationTimer / 120f;
            float xOffset = swingXRadius * MathF.Sin(swingProgress * swingRange + swingRange);
            float yOffset = swingYRadius * MathF.Cos(swingProgress * swingRange + swingRange);
            Vector2 offset = new Vector2(xOffset, yOffset);
            Vector2 targetCenter = Parent.Center + offset + new Vector2(0, -16);
            Vector2 targetVelocity = (targetCenter - NPC.Center) * 0.25f;
            OrbitProgress += 0.01f;
            if (OrbitProgress >= 1f)
                OrbitProgress = 1f;
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, OrbitProgress);

            float targetRotation = (targetCenter - NPC.Center).ToRotation();
            NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, 0.1f);
        }

        protected virtual void AI_Attack()
        {
            Timer++;
            OrbitProgress = 0f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = texture.Size() / 2f;
            float drawRotation = NPC.rotation;
            float drawScale = NPC.scale;
            float dp = 1f - DespawnProgress;

            if(State == AIState.Orbit)
            {
                drawColor = drawColor.MultiplyRGB(Color.LightGray);
            }


            spriteBatch.Restart(blendState: BlendState.Additive);
            for (float f = 0f; f < 1f; f += 0.25f)
            {
                float rot = f * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * VectorHelper.Osc(2f, 4f);
                Vector2 glowDrawPos = drawPos + offset;
                Color glowColor = drawColor * 0.8f;
                spriteBatch.Draw(texture, glowDrawPos, null, glowColor * dp, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
                if (State == AIState.Transition)
                {
                    spriteBatch.Draw(texture, glowDrawPos, null, glowColor * dp, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
                }
            }
            spriteBatch.RestartDefaults();
            spriteBatch.Draw(texture, drawPos, null, drawColor * dp, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
            return false;
        }

        protected void SwitchState(AIState state)
        {
            if (StellaMultiplayer.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        public override bool CheckActive()
        {
            if (!Parent.active || Parent.type != ModContent.NPCType<CommanderGintzia>())
                return true;
            return false;
        }

        public override void OnKill()
        {
            base.OnKill();
            for (int i = 0; i < 12; i++)
            {
                float progress = (float)i / 12f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 vel = rot.ToRotationVector2() * 4;
                Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, vel);
            }
        }
    }
}

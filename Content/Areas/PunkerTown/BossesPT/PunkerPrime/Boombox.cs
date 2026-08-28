using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.PunkerPrime
{
    public class Boombox : ModNPC
    {
        private enum AIState
        {
            IdleFollow,
            Warn
        }
        private ref float Timer => ref NPC.ai[0];
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private Metronome _metronome;
        private Metronome Metronome
        {
            get
            {
                _metronome ??= new Metronome(150);
                return _metronome;
            }
        }
        private NPC Parent => Main.npc[(int)NPC.ai[2]];
        private bool ShouldWarn => NPC.ai[3] == 1;
        private float _upDown;
        private Vector2 _upDownOffset;
        private Vector2 _bounceOffset;
        private float _rotOffset;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 100;
            NPC.defense = 14;
            NPC.lifeMax = 6000;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;

            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            NPC.dontTakeDamageFromHostiles = true;
        }

        public override void AI()
        {
            base.AI();
            if (!Parent.active)
            {
                NPC.active = false;
            }

            Metronome.Update();
            if (_upDown == 0)
                _upDown = 1;
            if (Metronome.beatHit)
            {
                _upDown *= -1;
            }
            _rotOffset = MathHelper.Lerp(_rotOffset, 0.5f * _upDown, 0.2f);
            _upDownOffset = Vector2.Lerp(_upDownOffset, Vector2.UnitY * _upDown * 8, 0.2f);
            switch (State)
            {
                case AIState.IdleFollow:
                    AI_IdleFollow();
                    break;
                case AIState.Warn:
                    AI_Warn();
                    break;
            }
        }



        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }
        private void AI_IdleFollow()
        {
            Timer++;
            Chase();
            if (ShouldWarn)
            {
                SwitchState(AIState.Warn);
            }
        }

        private void Chase()
        {
            //Crazy movement code
            Vector2 targetPosition = Parent.Center;
            targetPosition.Y -= 8;
            Vector2 velocityToPlayer = (targetPosition - NPC.Center);
            velocityToPlayer = velocityToPlayer.SafeNormalize(Vector2.Zero);
            float dist = Vector2.Distance(NPC.Center, targetPosition);
            if (dist <= 0)
                dist = 1;

            float interp = dist / 384;
            interp = EasingFunction.InOutSine(interp);
            float speed = MathHelper.Lerp(6, 20, interp);

            float xDist = MathF.Abs(targetPosition.X - NPC.Center.X);
            if (xDist < 256)
                velocityToPlayer.Y -= 0.5f;

            if (dist < speed)
                speed = dist;
            velocityToPlayer *= speed;
            velocityToPlayer *= ExtraMath.Osc(0.5f, 1f, speed: 2);
            velocityToPlayer.Y += ExtraMath.Osc(-5, 5, speed: 2);
            NPC.velocity = Vector2.Lerp(NPC.velocity, velocityToPlayer, 0.02f);
            NPC.rotation = NPC.velocity.X * 0.02f + ExtraMath.Osc(-0.05f, 0.05f, speed: 2);
        }

        private void AI_Warn()
        {
            Timer++;
            NPC.velocity *= 0.9f;
            if(Timer == 2)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Kabloowie>(), 1, 1,
                        Main.myPlayer, ai1: 0);
                }
            }
            _bounceOffset = Vector2.Lerp(Vector2.UnitY * -64, Vector2.UnitY * 64, ExtraMath.Osc(0f, 1f, speed: 4)) * MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(Timer / 60f));
            NPC.ai[3] = 0;
            if(Timer >= 60)
            {
                SwitchState(AIState.IdleFollow);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
            Vector2 offset = _upDownOffset;
            offset.Y += ExtraMath.Osc(-8f, 8f, 8f);
            offset += _bounceOffset;
            drawer.worldPosition += offset;
            drawer.rotation += _rotOffset;
            spriteBatch.Draw(drawer);
            return false;
        }
    }
}

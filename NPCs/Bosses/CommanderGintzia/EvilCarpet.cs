using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.CommanderGintzia
{
    internal class EvilCarpet : ModNPC
    {
        private int _frame;
        protected enum AIState
        {
            Hover,
            Idle,
            Despawn
        }

        protected ref float Timer => ref NPC.ai[0];
        protected float DespawnProgress;
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
        protected NPC Parent => Main.npc[ParentIndex];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 60;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 16;
            NPC.height = 16;
            NPC.damage = 24;
            NPC.defense = 10;
            NPC.lifeMax = 1000;
            NPC.noTileCollide = false;
            NPC.dontCountMe = true;
            NPC.dontTakeDamage = true;
            NPC.dontTakeDamageFromHostiles = true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += 1f;
            if (NPC.frameCounter >= 1f)
            {
                NPC.frameCounter = 0f;
                _frame++;
            }

            if (_frame >= 60)
            {
                _frame = 0;
            }
            NPC.frame.Y = _frame * frameHeight;
        }

        public override void AI()
        {
            base.AI();
            switch (State)
            {
                case AIState.Hover:
                    AI_Hover();
                    break;
                case AIState.Despawn:
                    AI_Despawn();
                    break;
            }

            bool shouldKill = !Parent.active || (Parent.type != ModContent.NPCType<CommanderGintzia>() && Parent.type != ModContent.NPCType<CommanderGintziaTaunting>());
            if (shouldKill && State != AIState.Despawn)
            {
                SwitchState(AIState.Despawn);
            }

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

        private void AI_Hover()
        {
            NPC.Center = Vector2.Lerp(NPC.Center, Parent.Center + Vector2.UnitY * 48, 0.5f);
            NPC.rotation = Parent.rotation;
        }

        private void AI_Despawn()
        {
            Timer++;
            DespawnProgress = Timer / 60f;
            NPC.velocity *= 0.92f;
            if (Timer >= 60)
            {
                NPC.Kill();
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            float drawRotation = NPC.rotation;
            float drawScale = NPC.scale;
            float dp = 1f - DespawnProgress;
            spriteBatch.Restart(blendState: BlendState.Additive);
            for (float f = 0f; f < 1f; f += 0.25f)
            {
                float rot = f * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * VectorHelper.Osc(2f, 4f);
                Vector2 glowDrawPos = drawPos + offset;
                Color glowColor = drawColor * 0.8f;
                spriteBatch.Draw(texture, glowDrawPos, NPC.frame, glowColor * dp, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
            }
            spriteBatch.RestartDefaults();
            spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor * dp, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool CheckActive()
        {
            NPC parent = Main.npc[ParentIndex];
            if (!parent.active || parent.type != ModContent.NPCType<CommanderGintzia>())
                return true;
            return false;
        }

        public override void OnKill()
        {
            base.OnKill();
        }
    }
}

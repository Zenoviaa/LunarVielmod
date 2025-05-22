using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.NPCs.Colosseum.Common;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.CommanderGintzia
{
    internal class CommanderGintziaTaunting : ModNPC
    {
        private Vector2 FollowCenter;

        private enum AIState
        {
            Spawn,
            FlyingAround,
            Despawn
        }

        private int _frame;
        private ref float Timer => ref NPC.ai[0];
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private float FadeProgress;
        private Player Target => Main.player[NPC.target];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 30;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 128;
            NPC.height = 128;
            NPC.damage = 14;
            NPC.defense = 10;
            NPC.lifeMax = 2500;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 1);
       
            NPC.dontCountMe = true;
            NPC.dontTakeDamage = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.aiStyle = -1;
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += 0.5f;
            if (NPC.frameCounter >= 1f)
            {
                NPC.frameCounter = 0f;
                _frame++;
            }

            if (_frame >= 30)
            {
                _frame = 0;
            }
            NPC.frame.Y = frameHeight * _frame;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            FollowCenter = ModContent.GetInstance<ColosseumSystem>().GongSpawnWorld + new Vector2(MathF.Sin(Timer * 0.01f) * 800, -168);
            NPC.TargetClosest();
            NPC.spriteDirection = NPC.direction;
            switch (State)
            {
                case AIState.Spawn:
                    AI_Spawn();
                    break;
                case AIState.FlyingAround:
                    AI_FlyingAround();
                    break;
                case AIState.Despawn:
                    AI_Despawn();
                    break;
            }
        }

        private void SwitchState(AIState state)
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
            ColosseumSystem colosseumSystem = ModContent.GetInstance<ColosseumSystem>();
            return !colosseumSystem.IsActive();
        }

        private void AI_Spawn()
        {
            Timer++;
            if (Timer == 1)
            {
                int xSpawn = (int)NPC.position.X;
                int ySpawn = (int)NPC.position.Y;
                if (StellaMultiplayer.IsHost)
                {
                    NPC.NewNPC(NPC.GetSource_FromThis(), xSpawn, ySpawn, ModContent.NPCType<EvilCarpet>(),
                        ai2: NPC.whoAmI);
                }
            }

            FadeProgress = Timer / 90f;
            NPC.velocity.Y = MathHelper.Lerp(10, 0f, Timer / 90f);
            if(Timer >= 90f)
            {
                SwitchState(AIState.FlyingAround);
            }
        }

        private void AI_FlyingAround()
        {
            Timer++;
            if(Timer % 900 == 0)
            {
                BattleTaunt();
            }

            Vector2 targetCenter = FollowCenter + new Vector2(0, -212);
            Vector2 velToPlayer = targetCenter - NPC.Center;
            velToPlayer = velToPlayer.SafeNormalize(Vector2.Zero);

            //Home to this point
            float maxSpeed = 6f;
            Vector2 targetVelocity = velToPlayer;
            float distance = Vector2.Distance(NPC.Center, targetCenter);
            if (distance < maxSpeed)
            {
                targetVelocity *= distance;
            }
            else
            {
                targetVelocity *= maxSpeed;
            }
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.01f);
            NPC.velocity.Y += MathF.Sin(Timer * 0.1f) * 0.02f;

            float targetRotation = NPC.velocity.X * 0.025f;
            NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, 0.1f);
            ColosseumSystem colosseumSystem = ModContent.GetInstance<ColosseumSystem>();
            if (!colosseumSystem.IsActive() || 
                (colosseumSystem.waveIndex == 6 && colosseumSystem.colosseumIndex == 2))
            {
                SwitchState(AIState.Despawn);
            }
        }

        private void AI_Despawn()
        {
            Timer++;
            if(Timer == 1)
            {
                DeathTaunt();
            }
            FadeProgress = Timer / 90f;
            NPC.velocity.Y = MathHelper.Lerp(0f, -10f, Timer / 90f);
            NPC.velocity.X *= 0.98f;
            NPC.rotation *= 0.98f;
            FadeProgress = 1f - (Timer / 90f);
            NPC.EncourageDespawn(60);
        }

        private void BattleTaunt()
        {
            string localString = "Taunt" + Main.rand.Next(1, 12);
            string taunt = LangText.Chat(this, localString);
            int combatText = CombatText.NewText(NPC.getRect(), Color.White, taunt, true);
            CombatText numText = Main.combatText[combatText];
            numText.lifeTime = 250;
        }

        private void DeathTaunt()
        {
            string localString = "Death" + Main.rand.Next(1, 7);
            string taunt = LangText.Chat(this, localString);
            int combatText = CombatText.NewText(NPC.getRect(), Color.White, taunt, true);
            CombatText numText = Main.combatText[combatText];
            numText.lifeTime = 250;
        }

        private void DrawGintzia(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            float drawRotation = NPC.rotation;
            float drawScale = NPC.scale;

            Color gintziaColor = Color.White.MultiplyRGB(drawColor);
            gintziaColor = gintziaColor.MultiplyRGB(Color.Gray);
            gintziaColor *= FadeProgress;

            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, drawPos, NPC.frame, gintziaColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Draw Carpet
            DrawGintzia(spriteBatch, screenPos, drawColor);
            return false;
        }
    }
}

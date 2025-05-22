using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.NPCs.Bosses.StarrVeriplant;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Catacombs.Water.WaterCogwork
{
    [AutoloadBossHead]
    internal class WaterCogwork : ModNPC
    {
        private enum AttackState
        {
            Idle = 0,
            Spin_Slow = 1,
            Spin_Fast = 2,
            Bolt = 3,
            Rifle = 4,
            Launcher = 5,
            Ram = 6
        }


        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 46;
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[NPC.type] = 3;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        }

        public override void SetDefaults()
        {
            NPC.damage = 40;
            NPC.defense = 26;
            NPC.width = 166;
            NPC.height = 138;
            NPC.lifeMax = 6000;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(gold: 10);
            NPC.aiStyle = NPCAIStyleID.BlazingWheel;
            NPC.BossBar = ModContent.GetInstance<MiniBossBar>();
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/CatacombsBoss");
            }
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }


        private float ai_State;
        private float ai_Counter;
        private float ai_last_State;
        private bool _resetState;

        private void FinishResetState()
        {
            if (_resetState)
            {
                ai_Counter = 0;
                _frameCounter = 0;
                _frameTick = 0;
                _resetState = false;
            }
        }

        private void SwitchState(AttackState attackState)
        {
            if (StellaMultiplayer.IsHost)
            {
                ai_last_State = ai_State;
                ai_State = (float)attackState;
                _resetState = true;
                NPC.netUpdate = true;
            }
        }

        private void WheelSparks(Vector2 sparksOffset)
        {
            float count = 8;
            float degreesPer = 360 / count;
            for (int k = 0; k < count; k++)
            {
                float degrees = k * degreesPer;
                Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                Vector2 vel = direction * 4;
                Dust.NewDust(NPC.Center + sparksOffset, 0, 0, DustID.Iron, vel.X, vel.Y);
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ai_State);
            writer.Write(ai_last_State);
            writer.Write(_resetState);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ai_State = reader.ReadSingle();
            ai_last_State = reader.ReadSingle();
            _resetState = reader.ReadBoolean();
        }

        private int _frameCounter;
        private int _frameTick;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int height = 138;
            int width = 166;
            SpriteEffects effects = SpriteEffects.None;
            Vector2 drawPosition = NPC.Center - screenPos;  
            Vector2 origin = new Vector2(width/2, height/2);

            //Trail
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            int npcFrames = Main.npcFrameCount[NPC.type];
            int frameHeight = texture.Height / npcFrames;

            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, frameHeight);
            Vector2 drawOrigin = sourceRectangle.Size() / 2f;
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + drawOrigin;
                Color color = NPC.GetAlpha(Color.Lerp(Color.Aqua, Color.Transparent, 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                Main.spriteBatch.Draw(texture, drawPos, sourceRectangle, color, NPC.oldRot[k], drawOrigin, NPC.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            //Animated
            Texture2D cogworkTexture = ModContent.Request<Texture2D>(Texture).Value;
            AttackState attackState = (AttackState)ai_State;
            int speed = 1;
       
            if (attackState == AttackState.Spin_Slow || attackState == AttackState.Spin_Fast)
            {
                int frameCount = 26;
                int startFrame = 0;
                Rectangle rect = new Rectangle(0, startFrame * height, width, frameCount * height);
                spriteBatch.Draw(cogworkTexture, drawPosition,
                    cogworkTexture.AnimationFrame(ref _frameCounter, ref _frameTick, speed, frameCount, rect, true),
                    drawColor, 0f, origin, 1f, effects, 0f);
            }
            else
            {
                int frameCount = 20;
                int startFrame = 26;

                Rectangle rect = new Rectangle(0, startFrame * height, width, frameCount * height);
                spriteBatch.Draw(cogworkTexture, drawPosition,
                    cogworkTexture.AnimationFrame(ref _frameCounter, ref _frameTick, speed, frameCount, rect, true),
                    drawColor, 0f, origin, 1f, effects, 0f);
            }

            return false;
        }

        public override void AI()
        {
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
                if (!NPC.HasValidTarget)
                {
                    NPC.noTileCollide = true;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, 8), 0.025f);
                    NPC.EncourageDespawn(120);
                    return;
                }
            }
            else
            {
                NPC.noTileCollide = false;
            }

            AttackState attackState = (AttackState)ai_State;
            AttackState attackLastState = (AttackState)ai_last_State;
            FinishResetState();
            switch (attackState)
            {
                case AttackState.Idle:
                    ai_Counter++;
                    if(ai_Counter > 240 && _frameCounter == 0 && StellaMultiplayer.IsHost)
                    {
                        //Determine the Attack
                        if(attackLastState == AttackState.Spin_Slow || attackLastState == AttackState.Spin_Fast)
                        {
                            switch (Main.rand.Next(0, 3))
                            {
                                case 0:
                                    SwitchState(AttackState.Bolt);
                                    break;
                                case 1:
                                    SwitchState(AttackState.Launcher);
                                    break;
                                case 2:
                                    SwitchState(AttackState.Rifle);
                                    break;
                            }
                        }
                        else
                        {
                            SwitchState(AttackState.Spin_Slow);
                        }      
                    }

                    break;

                case AttackState.Spin_Slow:

                    ai_Counter++;
                    NPC.velocity *= (ai_Counter % 26) / 11;
                    if (ai_Counter % 26 == 11)
                    {
                        WheelSparks(Vector2.Zero);
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SkyrageShasher"));
                    }
                    if (ai_Counter > 52)
                    {
                        SwitchState(AttackState.Idle);
                    }
                    break;
                case AttackState.Bolt:
                    if (ai_Counter == 0)
                    {
                        if (StellaMultiplayer.IsHost)
                        {
                           NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
                                ModContent.NPCType<WaterGun>(), ai2: NPC.whoAmI);
                        }
         
                    }
                    ai_Counter++;
                    SwitchState(AttackState.Idle);
                    break;
                
                case AttackState.Rifle:
                    if (ai_Counter == 0)
                    {
                        if (StellaMultiplayer.IsHost)
                        {
                            NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
                            ModContent.NPCType<WaterRifle>(), ai2: NPC.whoAmI);
                        }
                    }
                    ai_Counter++;
                    SwitchState(AttackState.Idle);
                    break;
                
                case AttackState.Launcher:
                    if (ai_Counter == 0)
                    {
                        if (StellaMultiplayer.IsHost)
                        {
                            NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
                            ModContent.NPCType<WaterLauncher>(), ai2: NPC.whoAmI);
                        }
                    }
                    ai_Counter++;
                    SwitchState(AttackState.Idle);
                    break;

                case AttackState.Ram:
                    SwitchState(AttackState.Idle);
                    break;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TreasureBoxWater>(), chanceDenominator: 1, minimumDropped: 1, maximumDropped: 1));
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedCogwork, -1);
        }
    }
}

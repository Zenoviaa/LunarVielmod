using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Catacombs.Water.WaterCogwork
{
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

        private enum MoveDirection
        {
            Left = 0,
            Right = 1,
            Up = 2,
            Down = 3
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 46;
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[NPC.type] = 3;
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
        }

        //AI Stuffs
        private NPC _gun;
 
        private ref float ai_State => ref NPC.ai[0];
        private ref float ai_Counter => ref NPC.ai[1];
        private ref float ai_last_State => ref NPC.ai[2];
        private ref float ai_move_Direction => ref NPC.ai[3];

        private void SwitchState(AttackState attackState)
        {
            ai_Counter = 0;
            ai_last_State = ai_State;
            ai_State = (float)attackState;
        }

        private void SwitchMoveDirection(MoveDirection moveDirection)
        {
            ai_move_Direction = (float)moveDirection;
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
                Dust.NewDust(NPC.Center + sparksOffset, 0, 0, DustID.Water, vel.X, vel.Y);
            }
        }

        private void WheelMovement(float speed = 8)
        {
            MoveDirection moveDirection = (MoveDirection)ai_move_Direction;
            AttackState attackState = (AttackState)ai_State;
            switch (attackState)
            {
                case AttackState.Spin_Slow:
                case AttackState.Spin_Fast:
                    if (_frameCounter < 11 || _frameCounter > 19)
                    {
                        speed = 0;
                    }
                    else if (_frameCounter == 11)
                    {
                        WheelSparks(Vector2.Zero);
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SkyrageShasher"));
                    }
                    break;
            }

            Vector2 sparksOffset = Vector2.Zero;
            float sparksWidth = 132/2;
            float sparksHeight = 134/2;
            switch (moveDirection)
            {
                case MoveDirection.Left:
                    NPC.velocity = new Vector2(-speed, 0);
                    if (NPC.collideX)
                    {
                        SwitchMoveDirection(MoveDirection.Down);
                    }
                    sparksOffset = new Vector2(0, -sparksHeight);
                    break;
                case MoveDirection.Down:
                    NPC.velocity = new Vector2(0, speed);
                    if (NPC.collideY)
                    {
                        SwitchMoveDirection(MoveDirection.Right);
                    }
                    sparksOffset = new Vector2(-sparksWidth, 0);
                    break;
                case MoveDirection.Right:
                    NPC.velocity = new Vector2(speed, 0);
                    if (NPC.collideX)
                    {
                        SwitchMoveDirection(MoveDirection.Up);
                    }
                    sparksOffset = new Vector2(0, sparksHeight);
                    break;
                case MoveDirection.Up:
                    NPC.velocity = new Vector2(0, -speed);
                    if (NPC.collideY)
                    {
                        SwitchMoveDirection(MoveDirection.Left);
                    }
                    sparksOffset = new Vector2(sparksWidth, 0);
                    break;
            }

            if(ai_Counter % 8 == 0)
            {
                WheelSparks(sparksOffset);
            } 
        }

        private void GunMovement()
        {
            if (_gun == null)
                return;
            if (!_gun.active)
                return;

            MoveDirection moveDirection = (MoveDirection)ai_move_Direction;
            Vector2 sparksOffset = Vector2.Zero;
            float sparksWidth = 132 / 2;
            float sparksHeight = 134 / 2;
            switch (moveDirection)
            {
                case MoveDirection.Left:
   
                    sparksOffset = new Vector2(0, sparksHeight);
                    break;
                case MoveDirection.Down:
                    sparksOffset = new Vector2(sparksWidth, 0);
                    break;
                case MoveDirection.Right:
                    sparksOffset = new Vector2(0, -sparksHeight);
                    break;
                case MoveDirection.Up:
                    sparksOffset = new Vector2(-sparksWidth, 0);
                    break;
            }

            _gun.Center = Vector2.Lerp(_gun.Center, NPC.Center + sparksOffset, 0.2f);
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
            //OK so 
            //Cogwork will move around the arena kinda like a blazing wheel
            //He has contact damage obviously
            //Rotates around the arena and shoots projectiles
            //He'll make gear noises as he moves and have sparke particles coming out from where he touches the ground
            //The cogwork will roll around the arena and every once in a while stop and pull out a different gun to shoot you with
            //He sticks to walls like blazing wheels
            //Also has a ram attack where he revs up and goes around fast, you have to jump over em
            //So 4 attacks

            NPC.TargetClosest();
            if (!NPC.HasValidTarget)
            {
                WheelMovement(2);
                NPC.EncourageDespawn(120);
                return;
            }

            AttackState attackState = (AttackState)ai_State;
            AttackState attackLastState = (AttackState)ai_last_State;

            GunMovement();
            switch (attackState)
            {
                case AttackState.Idle:
                    WheelMovement(6);
                    ai_Counter++;
                    if(ai_Counter > 100 && _frameCounter == 0)
                    {
                        //Determine the Attack
                        if(attackLastState == AttackState.Spin_Slow || attackLastState == AttackState.Spin_Fast)
                        {
                            switch(Main.rand.Next(0, 4))
                            {
                                case 0:
                                    SwitchState(AttackState.Bolt);
                                    break;
                                case 1:
                                    SwitchState(AttackState.Ram);
                                    break;
                                case 2:
                                    SwitchState(AttackState.Launcher);
                                    break;
                                case 3:
                                    SwitchState(AttackState.Rifle);
                                    break;
                            }
                        }
                        else
                        {
                            if (Main.rand.NextBool(5))
                            {
                                SwitchState(AttackState.Spin_Fast);   
                            }
                            else
                            {
                                SwitchState(AttackState.Spin_Slow);
                            }                      
                        }      
                    }

                    break;

                case AttackState.Spin_Slow:

                    //Slowly moving around with movement similar to blazing wheels
                    //Bouncing movements
                    WheelMovement(15);
                    ai_Counter++;
                    if(ai_Counter > 120 && _frameCounter == 0)
                    {
                        SwitchState(AttackState.Idle);
                    }

                    break;
                
                case AttackState.Spin_Fast:

                    //Fastly
                    WheelMovement(15);
                    ai_Counter++;
                    if (ai_Counter > 120 && _frameCounter == 0)
                    {
                        SwitchState(AttackState.Idle);
                    }

                    break;
                
                case AttackState.Bolt:
                    if (ai_Counter == 0)
                    {
                        _gun = NPC.NewNPCDirect(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, 
                            ModContent.NPCType<WaterGun>());
                    }
                    ai_Counter++;
                    SwitchState(AttackState.Idle);
                    break;
                
                case AttackState.Rifle:
                    if (ai_Counter == 0)
                    {
                        _gun = NPC.NewNPCDirect(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
                            ModContent.NPCType<WaterRifle>());
                    }
                    ai_Counter++;
                    SwitchState(AttackState.Idle);
                    break;
                
                case AttackState.Launcher:
                    if (ai_Counter == 0)
                    {
                        _gun = NPC.NewNPCDirect(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, 
                            ModContent.NPCType<WaterLauncher>());
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
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Catacombs.Water.WaterCogwork
{
    internal class WaterCogwork : ModNPC
    {
        private bool _reverseMovement;
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

        public override void SetDefaults()
        {
            NPC.damage = 1;
            NPC.defense = 26;
            NPC.width = 132;
            NPC.height = 134;
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

        private void WheelMovement(float speed = 8)
        {
            MoveDirection moveDirection = (MoveDirection)ai_move_Direction;
            Vector2 sparksOffset = new Vector2(-NPC.width / 2, 0);
       
            switch (moveDirection)
            {
                case MoveDirection.Left:
                    NPC.velocity = new Vector2(-speed, 0);
                    if (NPC.collideX)
                    {
                        SwitchMoveDirection(MoveDirection.Down);
                    }

                    //Spark Visuals
                    sparksOffset = new Vector2(0, -NPC.height/2);
                    break;
                case MoveDirection.Down:
                    NPC.velocity = new Vector2(0, speed);
                    if (NPC.collideY)
                    {
                        SwitchMoveDirection(MoveDirection.Right);
                    }

                    //Spark Visuals
                    sparksOffset = new Vector2(-NPC.width / 2, 0);
                    break;
                case MoveDirection.Right:
                    NPC.velocity = new Vector2(speed, 0);
                    if (NPC.collideX)
                    {
                        SwitchMoveDirection(MoveDirection.Up);
                    }

                    //Spark Visuals
                    sparksOffset = new Vector2(0, NPC.height / 2);
                    break;
                case MoveDirection.Up:
                    NPC.velocity = new Vector2(0, -speed);
                    if (NPC.collideY)
                    {
                        SwitchMoveDirection(MoveDirection.Left);
                    }

                    //Spark Visuals
                    sparksOffset = new Vector2(NPC.width / 2, 0);
                    break;
            }

            //Gear Visuals & Sounds
            //Every 8 frames do some effects
            if(ai_Counter % 8 == 0)
            {
                Vector2 sparksPosition = NPC.Center + sparksOffset;
                for (int i = 0; i < 4; i++)
                {
                    Dust.NewDustPerfect(sparksPosition, DustID.Lava, Scale: Main.rand.NextFloat(0.5f, 0.75f), Alpha: 255);
                }

                //Play Clanky Sound
                SoundEngine.PlaySound(SoundID.NPCHit3);
            }
        }

        private void GunMovement()
        {
            if (_gun == null)
                return;
            if (!_gun.active)
                return;


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
            AttackState attackState = (AttackState)ai_State;
            AttackState attackLastState = (AttackState)ai_last_State;

            GunMovement();
            switch (attackState)
            {
                case AttackState.Idle:
                    ai_Counter++;
                    if(ai_Counter > 60)
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
                    WheelMovement();
                    ai_Counter++;
                    if(ai_Counter > 120)
                    {
                        NPC.velocity = Vector2.Zero;
                        SwitchState(AttackState.Idle);
                    }

                    break;
                
                case AttackState.Spin_Fast:
                    //Fastly
                    WheelMovement(13);
                    ai_Counter++;
                    if (ai_Counter > 120)
                    {
                        NPC.velocity = Vector2.Zero;
                        SwitchState(AttackState.Idle);
                    }

                    break;
                
                case AttackState.Bolt:
                    if (ai_Counter == 0)
                    {
                        _gun = NPC.NewNPCDirect(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WaterGun>());
                    }
                    ai_Counter++;
                    SwitchState(AttackState.Idle);
                    break;
                
                case AttackState.Rifle:
                    if (ai_Counter == 0)
                    {
                        _gun = NPC.NewNPCDirect(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WaterGun>());
                    }
                    ai_Counter++;
                    SwitchState(AttackState.Idle);
                    break;
                
                case AttackState.Launcher:
                    if (ai_Counter == 0)
                    {
                        _gun = NPC.NewNPCDirect(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<WaterGun>());
                    }
                    ai_Counter++;
                    SwitchState(AttackState.Idle);
                    break;

                case AttackState.Ram:
                    SwitchState(AttackState.Idle);
                    break;
            }
        }
    }
}

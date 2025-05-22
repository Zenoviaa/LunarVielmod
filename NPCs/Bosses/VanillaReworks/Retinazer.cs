using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.VanillaReworks
{
    public class Retinazer : GlobalNPC
    {
        private float _counter;
        private float _fireCounter;
        private float _passiveFireCounter;
        private Vector2 _dashDirection;
        private SpinDashState _spinDashState = SpinDashState.Preparing;
        public enum SpinDashState
        {
            Preparing = 0,
            Spinning = 1,
            Charging = 2,
            Charging2 = 3,
            Charging3 = 4,
            Charging4 = 5,
            Charging5 = 6,
        }

        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            //This should make it only apply to eye of cthulu
            return entity.type == NPCID.Retinazer && lateInstantiation;
        }

        public override bool PreAI(NPC npc)
        {
            //Vanilla eye variables
            //ai_Phase == 0 is phase 1
            //ai_Phase == 3 is phase 2
            ref float ai_Phase = ref npc.ai[0];
            ref float ai_AttackState = ref npc.ai[1];
            ref float ai_Timer = ref npc.ai[2];

            //Passive fire once low on HP
            if (npc.life < npc.lifeMax / 2 && npc.HasValidTarget)
            {
                _passiveFireCounter++;
                Vector2 targetCenter = Main.player[npc.target].Center;
                bool lineOfSight = Collision.CanHitLine(npc.Center, 1, 1, targetCenter, 1, 1);
                if (_passiveFireCounter > 60 && lineOfSight)
                {
                    SoundEngine.PlaySound(SoundID.Item72, npc.position);

                    float speedVariance = 3;
                    float speedVariance2 = 1;
                    for (int i = 0; i < 3; i++)
                    {
                        float speedX = Main.rand.NextFloat(speedVariance2, speedVariance);
                        float speedY = Main.rand.NextFloat(speedVariance2, speedVariance);
                        Vector2 speed = new Vector2(speedX, speedY);
                        Vector2 direction = (Main.player[npc.target].Center - npc.Center).SafeNormalize(Vector2.UnitX);
                        direction = direction.RotatedByRandom(MathHelper.ToRadians(10));

                        int projectile = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, direction * 3 * speed,
                        ProjectileID.EyeLaser, 32, 0, Main.myPlayer);
                        Main.projectile[projectile].timeLeft = 300;
                        Projectile ichor = Main.projectile[projectile];
                        ichor.hostile = true;
                        ichor.friendly = false;
                    }

                    _passiveFireCounter = 0;
                }
            }


            //Remember 3f, is phase 2 for this boss, might not be the same for other bosses
            if (ai_Phase == 3f)
            {
                switch (_spinDashState)
                {
                    case SpinDashState.Preparing:
                        _counter++;

                        if (npc.HasValidTarget)
                        {
                            //Line of sight check prevents starting this attack while within tiles
                            Vector2 targetCenter = Main.player[npc.target].Center;
                            bool lineOfSight = Collision.CanHitLine(npc.Center, 1, 1, targetCenter, 1, 1);
                            if (_counter >= 700 && npc.HasValidTarget && ai_Timer == 0f && lineOfSight)
                            {
                                _spinDashState = SpinDashState.Spinning;
                                _dashDirection = npc.DirectionTo(Main.player[npc.target].Center);
                                //Reset the counter
                                _counter = 0;
                            }
                        }
                        //Checking for ai_Timer == 0f will make it only start spinning once other attacks have finished

                        //Attack is on cooldown so just gonna do normal AI
                        return true;
                    case SpinDashState.Spinning:
                        {
                            //Need to actually charge at the player now.
                            float dashSpeed = 12;

                            const float PI = (float)Math.PI;

                            //This makes him smoothly face towards the dash direction, it looks nice I think
                            float targetRotation = MathHelper.WrapAngle(_dashDirection.ToRotation() - PI / 2);
                            npc.rotation = MathHelper.WrapAngle(MathHelper.Lerp(npc.rotation, targetRotation, 0.07f));
                            npc.velocity = _dashDirection * dashSpeed;

                            _counter++;
                            _fireCounter++;
                            if (_fireCounter >= 20)
                            {
                                int spawnCount = Main.rand.Next(2, 6);
                              

                                for (int i = 0; i < 32; i++)
                                {
                                    Vector2 speed = Main.rand.NextVector2CircularEdge(2f, 2f);
                                    var d = Dust.NewDustPerfect(npc.Center, DustID.BoneTorch, speed * 11, Scale: 3f);
                                    d.noGravity = true;
                                }

                                SoundEngine.PlaySound(SoundID.Item12, npc.position);
                                _fireCounter = 0;
                            }

                            if (_counter >= 40)
                            {
                                _dashDirection = npc.DirectionTo(Main.player[npc.target].Center);
                                _spinDashState = SpinDashState.Charging2;

                                //Reset the counter
                                _counter = 0;
                            }
                            return false;









                        }
                        
                       
                
                    case SpinDashState.Charging:
                    {


                            //Need to actually charge at the player now.
                            float dashSpeed = 12;

                            const float PI = (float)Math.PI;

                            //This makes him smoothly face towards the dash direction, it looks nice I think
                            float targetRotation = MathHelper.WrapAngle(_dashDirection.ToRotation() - PI / 2);
                            npc.rotation = MathHelper.WrapAngle(MathHelper.Lerp(npc.rotation, targetRotation, 0.07f));
                            npc.velocity = _dashDirection * dashSpeed;

                            _counter++;
                            _fireCounter++;
                            if (_fireCounter >= 20)
                            {
                                int spawnCount = Main.rand.Next(2, 6);
                               

                                for (int i = 0; i < 32; i++)
                                {
                                    Vector2 speed = Main.rand.NextVector2CircularEdge(2f, 2f);
                                    var d = Dust.NewDustPerfect(npc.Center, DustID.BoneTorch, speed * 11, Scale: 3f);
                                    d.noGravity = true;
                                }

                                SoundEngine.PlaySound(SoundID.Item12, npc.position);
                                _fireCounter = 0;
                            }

                            if (_counter >= 40)
                            {
                                _dashDirection = npc.DirectionTo(Main.player[npc.target].Center);
                                _spinDashState = SpinDashState.Charging2;

                                //Reset the counter
                                _counter = 0;
                            }
                            return false;

                        }
                       




                    case SpinDashState.Charging2:
                        {


                            //Need to actually charge at the player now.
                            float dashSpeeda = 12;
                            const float PI = (float)Math.PI;
                            //This makes him smoothly face towards the dash direction, it looks nice I think
                            float targetRotationa = MathHelper.WrapAngle(_dashDirection.ToRotation() - PI / 2);
                            npc.rotation = MathHelper.WrapAngle(MathHelper.Lerp(npc.rotation, targetRotationa, 0.07f));
                            npc.velocity = _dashDirection * dashSpeeda;

                            _counter++;
                            _fireCounter++;
                            if (_fireCounter >= 20)
                            {
                                int spawnCount = Main.rand.Next(2, 6);


                                for (int i = 0; i < 32; i++)
                                {
                                    Vector2 speed = Main.rand.NextVector2CircularEdge(2f, 2f);
                                    var d = Dust.NewDustPerfect(npc.Center, DustID.BoneTorch, speed * 11, Scale: 3f);
                                    d.noGravity = true;
                                }

                                SoundEngine.PlaySound(SoundID.Item12, npc.position);
                                _fireCounter = 0;
                            }

                            if (_counter >= 40)
                            {
                                _dashDirection = npc.DirectionTo(Main.player[npc.target].Center);
                                _spinDashState = SpinDashState.Charging3;

                                //Reset the counter
                                _counter = 0;
                            }
                            return false;
                        }


                    case SpinDashState.Charging3:
                        {

                            float dashSpeedab = 22;
                            const float PI = (float)Math.PI;
                            //This makes him smoothly face towards the dash direction, it looks nice I think
                            float targetRotationab = MathHelper.WrapAngle(_dashDirection.ToRotation() - PI / 2);
                            npc.rotation = MathHelper.WrapAngle(MathHelper.Lerp(npc.rotation, targetRotationab, 0.07f));
                            npc.velocity = _dashDirection * dashSpeedab;

                            _counter++;
                            _fireCounter++;
                            if (_fireCounter >= 20)
                            {
                                int spawnCount = Main.rand.Next(2, 6);
                                

                                for (int i = 0; i < 32; i++)
                                {
                                    Vector2 speed = Main.rand.NextVector2CircularEdge(2f, 2f);
                                    var d = Dust.NewDustPerfect(npc.Center, DustID.BoneTorch, speed * 11, Scale: 3f);
                                    d.noGravity = true;
                                }

                                SoundEngine.PlaySound(SoundID.Item12, npc.position);
                                _fireCounter = 0;
                            }

                            if (_counter >= 40)
                            {
                                _dashDirection = npc.DirectionTo(Main.player[npc.target].Center);
                                _spinDashState = SpinDashState.Charging4;

                                //Reset the counter
                                _counter = 0;
                            }
                            return false;






                        }

                    case SpinDashState.Charging4:
                        {

                            float dashSpeeda = 12;
                            const float PI = (float)Math.PI;
                            //This makes him smoothly face towards the dash direction, it looks nice I think
                            float targetRotationa = MathHelper.WrapAngle(_dashDirection.ToRotation() - PI / 2);
                            npc.rotation = MathHelper.WrapAngle(MathHelper.Lerp(npc.rotation, targetRotationa, 0.07f));
                            npc.velocity = _dashDirection * dashSpeeda;

                            _counter++;
                            _fireCounter++;
                            if (_fireCounter >= 20)
                            {
                                int spawnCount = Main.rand.Next(2, 6);



                                for (int i = 0; i < 32; i++)
                                {
                                    Vector2 speed = Main.rand.NextVector2CircularEdge(2f, 2f);
                                    var d = Dust.NewDustPerfect(npc.Center, DustID.BoneTorch, speed * 11, Scale: 3f);
                                    d.noGravity = true;
                                }

                                SoundEngine.PlaySound(SoundID.Item12, npc.position);
                                _fireCounter = 0;
                            }

                            if (_counter >= 40)
                            {
                                _dashDirection = npc.DirectionTo(Main.player[npc.target].Center);
                                _spinDashState = SpinDashState.Charging5;

                                //Reset the counter
                                _counter = 0;
                            }
                            return false;






                        }

                    case SpinDashState.Charging5:
                        {

                            float dashSpeeda = 12;
                            const float PI = (float)Math.PI;
                            //This makes him smoothly face towards the dash direction, it looks nice I think
                            float targetRotationa = MathHelper.WrapAngle(_dashDirection.ToRotation() - PI / 2);
                            npc.rotation = MathHelper.WrapAngle(MathHelper.Lerp(npc.rotation, targetRotationa, 0.07f));
                            npc.velocity = _dashDirection * dashSpeeda;

                            _counter++;
                            _fireCounter++;
                            if (_fireCounter >= 20)
                            {
                                int spawnCount = Main.rand.Next(2, 6);



                                for (int i = 0; i < 32; i++)
                                {
                                    Vector2 speed = Main.rand.NextVector2CircularEdge(2f, 2f);
                                    var d = Dust.NewDustPerfect(npc.Center, DustID.BoneTorch, speed * 11, Scale: 3f);
                                    d.noGravity = true;
                                }

                                SoundEngine.PlaySound(SoundID.Item12, npc.position);
                                _fireCounter = 0;
                            }

                            if (_counter >= 40)
                            {
                                _spinDashState = SpinDashState.Preparing;

                                //Reset the counter
                                _counter = 0;
                            }
                            return false;






                        }
                        //Need to actually charge at the player now.

                }





            }

            //Return true to run the vanilla AI
            //Return false to completely override the vanilla AI
            return true;
        }
    }
}

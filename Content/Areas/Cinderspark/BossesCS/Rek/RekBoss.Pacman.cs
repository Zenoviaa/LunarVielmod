using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;
using Stellamod.Core.Camera;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{
    private bool _blowtorched;
    private int Pac_Boom_Damage => 50;
    private float Pac_Delay_Time => 190;
    private float Pac_Man_Delay_Startup_Time => 80;
    private void AI_Pacman()
    {
        void PlacePacPoint(Vector2 position)
        {
            if (AttackCount >= Segments.Length)
                return;

            if (MultiplayerHelper.IsHost)
            {
                var seg = Segments[(int)AttackCount];
                ProjFirer firer = ProjFirer.From<PacmanSegment>(NPC);
                firer.damage = Pac_Boom_Damage;
                firer.position = seg.position;
                firer.velocity = position - seg.position;
                firer.ai0 = NPC.whoAmI;
                firer.ai1 = AttackCount;
                firer.New();
            }
            AttackCount++;
        }
   
        bool IsFull()
        {
          
            foreach(var seg in Segments)
            {
                if (seg.noWorm)
                    return false;
            }
            return true;
        }

        RekSegment GetNextSegmentToEat2()
        {
         for(int i = 0; i < Segments.Length; i++)
            {
                var seg = Segments[i];
                if (seg.noWorm)
                    return seg;
            }
            return null;
        }

        float surfaceY = LavaSurface();
        Vector2 GetPointOnPath(float ratio)
        {

            Vector2 left = EruptionLeft;
            Vector2 right = EruptionRight;
            float y = surfaceY;
            y = _arenaCenter.Y;
            left.Y = y;
            right.Y = y;
            return Vector2.Lerp(left, right, ratio);
        }

        Timer++;


        switch (AttackCycle)
        {
            case 0:
                {

    
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                    }

 
                    NPC.velocity *= 0.98f;
                    NPC.velocity = NPC.velocity.RotatedBy(0.05f);
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.03f);
                    //Prepare the points
                    SegmentsMeteorFloatAlways();
                    if(Timer >= Pac_Man_Delay_Startup_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if(Timer == 1)
                    {
                        _initialVelocity = NPC.velocity;
                        _centerPoint = NPC.Center;
                        foreach(var segment in Segments)
                        {
                            segment.initialPosition = segment.position;
                        }
                    }

                    if (Timer < 18)
                    {
                        Animator.PlayAnimation(ANIM_MOUTHOPEN, AnimationParams.Default with { IsLooping = false });
                    }
                    else if (Timer < 36)
                    {
                        Animator.PlayAnimation(ANIM_MOUTH_BIG_OPEN, AnimationParams.Default with { IsLooping = false });
                    }
      
                    SegmentsMeteorFloatAlways();

                    CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.1f));
                    Vector2 targetPoint = GetPointOnPath(-0.1f);
                    float headRatio = Timer / Pac_Delay_Time;
                    float headEase = EasingFunction.InOutExpo(headRatio);
                    Vector2 pos = Vector2.Lerp(_centerPoint, targetPoint, headEase);
                    Vector2 vel = pos - NPC.Center;
                    Vector2 easedVel = Vector2.Lerp(_initialVelocity, vel, headEase);
                
                    NPC.velocity = easedVel;
                    var seg = GetNextSegmentToEat2();
                    if(seg != null)
                    {
                        float targetAngle = (seg.position - NPC.Center).ToRotation();
                        NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.03f);

                    }

                    int i = 0;
                    float segIndex = 0;
                    foreach(var segment in Segments)
                    {
                        float ratio = (Timer - i) / 120f;
                        float ease = EasingFunction.InOutExpo(ratio);
                        Vector2 t = GetPointOnPath(segIndex / (float)Segments.Length);
                        Vector2 easedPoint = Vector2.Lerp(segment.initialPosition, t, ease);
                        segment.position = easedPoint;
                        segment.position += segment.velocity;
                        segment.velocity *= 0.96f;
                        i++;
                        segIndex++;
                    }
                    if(Timer <= Segments.Length)
                    {

                    }

                    if(Timer >= Pac_Delay_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    if (Timer == 1)
                    {
                        _blowtorched = false;
                    }


                    _outliner.attacking = true;
                    _showAfterImages = true;
                    Animator.PlayAnimation(ANIM_MOUTH_BIG_OPEN, AnimationParams.Default with { IsLooping = false });
                    SegmentsMeteorFloatAlways();
                    Vector2 lastPoint = GetPointOnPath(1f);
                    Vector2 dir = (lastPoint - NPC.Center).SafeNormalize(Vector2.Zero);
                    CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.1f));
                    var seg = GetNextSegmentToEat2();
                    if(seg != null)
                    {
                        float travelSpeed = MathHelper.Lerp(0f, 45f, EasingFunction.InOutExpo(Timer / 120f));
                        Vector2 velToTarget = (seg.position - NPC.Center);
                        velToTarget = velToTarget.SafeNormalize(Vector2.Zero);
                        Vector2 travelVelocity = velToTarget * travelSpeed;
                        float distToTarget = Vector2.Distance(seg.position, NPC.Center);
                        if(distToTarget < travelSpeed)
                        {
                            if (!_blowtorched && MultiplayerHelper.IsHost)
                            {
                                int i = 0;
                                var segment = Segments[i];
                                var firer = ProjFirer.From<PacMeteorBoom>(NPC);
                                firer.velocity = (segment.rotation - MathHelper.PiOver2).ToRotationVector2() * 1000;
                                firer.position = segment.position;
                                firer.damage = Pac_Boom_Damage;
                                firer.ai1 = NPC.whoAmI;
                                firer.ai2 = i;
                                firer.New();
                                _blowtorched = true;
                            }

                            CreateSegmentEatEffect(seg);
                            seg.noWorm = false;
                        }

                        NPC.velocity = travelVelocity;
                        NPC.rotation = Utils.AngleLerp(NPC.rotation, travelVelocity.ToRotation(), 0.15f);
                    }

                    if (IsFull())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:

                {

                    NPC.velocity *= 1.02f;
                    NPC.velocity = NPC.velocity.RotatedBy(-0.05f);
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.03f);
                    if(Timer >= 30)
                    {
                        AttackCycle++;
                        Timer = 0;
                    }
                }
                break;
            case 4:
                {
                    NPC.velocity.Y += 0.5f;
                    NPC.velocity.X -= 0.1f;
         
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.03f);
                    if (Timer >= 120)
                    {
                        SwitchState(AIState.FireBreath);
                    }
             
                }
                break;
        }
    }
}

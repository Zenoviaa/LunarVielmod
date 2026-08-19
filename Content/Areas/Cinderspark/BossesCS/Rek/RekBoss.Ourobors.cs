using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Common.Animations;
using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;
using Stellamod.Content.Areas.MoonspiralTower.VerliaBoss.Projectiles;
using Stellamod.Core.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Liquid;
using Terraria.GameContent.Shaders;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{
    private float Ouroboros_Coil_Time => 190;
    private void AI_Ouroboros()
    {

        float rotationSpeed = -0.05f;
        var animator = this.GetAnimator();
        Timer++;
     
        switch (AttackCycle)
        {
            case 0:
                {
                    Vector2 eruptionLeft = FindEruptionLeft();
                    Vector2 eruptionRight = FindEruptionRight();

                    ResetLavaSegments();
                    eruptionRight.Y -= 64;
                    eruptionLeft.Y -= 64;
                    //Vector2 moveToPoint = Vector2.Lerp(eruptionLeft, eruptionRight, 0.7f);

                    Vector2 midPoint = Vector2.Lerp(eruptionLeft, eruptionRight, 0.5f);

                    float moveTime = 180;
                    float xRadius = 512;
                    float yRadius = 384;
                    float ratio = Timer / moveTime;
                    float ease = EasingFunction.InOutSine(ratio);
                    float x = MathF.Sin(ease * MathHelper.Pi) * xRadius;
                    float y = MathF.Cos(ease * MathHelper.Pi) * yRadius;

                    Vector2 moveToPoint = midPoint + new Vector2(-x, y);
                    Vector2 targetVel = moveToPoint - NPC.Center;
                    NPC.velocity = targetVel;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);


                    if (Timer == 1)
                    {
                        Vector2 ePos = Vector2.Lerp(eruptionLeft, eruptionRight, 0.5f);
                        ePos.Y += 666;
                        Teleport(ePos);
                    }

                    if (Timer >= moveTime)
                    {
                        Timer = 0;
                        AttackCycle++;

                    }
                }
                break;
            case 1:
                {
                    Vector2 eruptionRight = FindEruptionRight();
                    Vector2 eruptionLeft = FindEruptionLeft();
                    //SLowly spin in circles and then flash bang to create the circle lmao
                    if (Timer == 1)
                    {
                        var sound = new SoundStyle("Stellamod/Assets/Sounds/RekClappbackStart") with { PitchVariance = 0.3f };
                        SoundEngine.PlaySound(sound, NPC.position);
                        NPC.TargetClosest();
                        _centerPoint = Vector2.Lerp(eruptionLeft, eruptionRight, 0.5f);
                        _centerPoint.Y = _arenaCenter.Y;
                        _initialVelocity = NPC.velocity;
                    }

                    //Glowing white
                    if (Timer < Ouroboros_Coil_Time)
                    {
                        //Lets just make a circle and have it shrink over time, that should look a lot cleaner
                        //Than what we had before

                        if (Timer < 37)
                        {
                            animator.PlayAnimation(ANIM_MOUTHOPEN, AnimationParams.Default with { IsLooping = false });
                        }
                        else if (Timer < 64)
                        {
                            animator.PlayAnimation(ANIM_MOUTH_BIG_OPEN, AnimationParams.Default with { IsLooping = false });
                        }
                        else if (Timer < 100)
                        {
                            animator.PlayAnimation(ANIM_MOUTH_BIG_OPEN_READY, AnimationParams.Default with { IsLooping = false });
                        }
                        else
                        {
                            ShakeScreenPosition.Shake = 4;
                            CreateFlameSuckParticles(_centerPoint);
                            if (Main.rand.NextBool(2))
                            {
                                var fx = FXUtil.GlowCircleBoom(_centerPoint, Color.White, Color.OrangeRed, Color.Red);
                                fx.VectorScale *= Timer / 50f;
                            }
                            animator.PlayAnimation(ANIM_MOUTH_BIG_OPEN_HOLD, AnimationParams.Default with { IsLooping = true });
                        }

                        

                        float ratio = Timer / Ouroboros_Coil_Time;
                        float ease = EasingFunction.InOutSine(ratio);
                        float radius = MathHelper.SmoothStep(392, 192, ease);
                        float x = MathF.Sin(ratio * MathHelper.TwoPi * 4) * radius;
                        float y = MathF.Cos(ratio * MathHelper.TwoPi * 4) * radius;
                        Vector2 pos = _centerPoint + new Vector2(x, y);
                        Vector2 vel = pos - NPC.Center;

                        float ease2 = EasingFunction.InOutExpo(ratio / 0.5f);
                        Vector2 easedVeclotiy = Vector2.Lerp(_initialVelocity, vel, ease2);
                        NPC.velocity = easedVeclotiy;
                        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.8f);

              

                        int max = (int)(Segments.Length * ease);
                        max = Segments.Length - max;
                        for (int i = Segments.Length - 1; i >= max; i--)
                        {
                            Segments[i].isBurning = true;
                        }

                    }
                    if(Timer >= Ouroboros_Coil_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    Vector2 eruptionRight = FindEruptionRight();
                    Vector2 eruptionLeft = FindEruptionLeft();
                    if (Timer == 1)
                    {
                        var iminShock = new SoundStyle("Stellamod/Assets/Sounds/RekShockwave");
                        SoundEngine.PlaySound(iminShock, NPC.position);
                        var fx = FXUtil.GlowCircleBoom(_centerPoint, Color.White, Color.OrangeRed, Color.Red);
                        fx.VectorScale *= 6f;
                        foreach(PositionVelocity posVel in new RandomCircleOrientation(_centerPoint, 64, 64))
                        {
                            Particles.BitDust.Spawn(BitDustFactory.Default with { position = posVel.position, velocity = posVel.velocity * Main.rand.NextFloat(5, 15), timeLeft = 120 });
                        }
                        foreach (PositionVelocity posVel in new RandomCircleOrientation(_centerPoint, 450, 44))
                        {
                            Particles.FaintSmokeDust.Spawn(FaintSmokeDustData.Default with { position = posVel.position, color = Color.White * 0.2f });
                        }
                        FXUtil.ShakeCamera(NPC.Center, 1024, 16);
                        for(int k = 0; k < 4; k++)
                        {
                            FXUtil.CreateRipple(_centerPoint);
                        }
                       
           
                        NPC.TargetClosest();
                        AllNoWorm();
                        _initialVelocity = Vector2.Lerp(eruptionLeft, eruptionRight, 0.5f);
                        _initialVelocity.Y = _arenaCenter.Y;
                    }

                    animator.PlayAnimation(ANIM_MOUTH_BITE, AnimationParams.Default with { IsLooping = false });
                    //Create a circle
                    int i = 0;
                    var orientation = new CircleOrientation(_initialVelocity, spawnEdgeRadius: 200, Segments.Length);
                    foreach (PositionVelocity posVel in orientation)
                    {
                        PositionVelocity next = orientation.Get(i + 1);
                        next.position = next.position.RotatedBy(Timer * rotationSpeed, _initialVelocity);
   
                        var segment = Segments[i];
                        segment.position = posVel.position.RotatedBy(Timer * rotationSpeed, _initialVelocity);


                        float rot = (next.position - segment.position).ToRotation();
                        segment.rotation = rot;
                        i++;
                    }

                    PositionVelocity headPos = orientation.Get(0);
                    Vector2 pos = headPos.position.RotatedBy(Timer * rotationSpeed, _initialVelocity);
                    PositionVelocity nextPos = orientation.Get(-1);
                    Vector2 nPos = nextPos.position.RotatedBy(Timer * rotationSpeed, _initialVelocity);
                    NPC.Center = Segments[0].position;
                    NPC.rotation = (nPos - pos).ToRotation();
                    NPC.velocity *= 0f;
                }
                break;
        }
    }
}

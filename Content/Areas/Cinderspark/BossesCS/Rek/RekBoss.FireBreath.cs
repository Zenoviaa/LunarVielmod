using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;
using Stellamod.Visual.Particles;
using System;
using Terraria;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{

    private int Fire_Breath_Damage => 40;
    private void AI_FireBreath()
    {
        Timer++;
        float direction = AttackCount % 2 == 0 ? -1 : 1;
        switch (AttackCycle)
        {
            case 0:
                {
   
                    Vector2 eruptionLeft = FindEruptionLeft();
                    Vector2 eruptionRight = FindEruptionRight();

                    //Vector2 moveToPoint = Vector2.Lerp(eruptionLeft, eruptionRight, 0.7f);

                    Vector2 midPoint = Vector2.Lerp(eruptionLeft, eruptionRight, 0.5f);
                    if (Timer == 1)
                    {
                        _initialVelocity = midPoint + new Vector2(0, 256);
                        Teleport(midPoint + new Vector2(0, 256));
                    }
                    float moveTime = 180;

                    float ratio = Timer / moveTime;
                    float ease = EasingFunction.InOutSine(ratio);
                    Vector2 moveToPoint = midPoint + new Vector2(0, -300);
                   
                    moveToPoint.Y -= 340;
                    Vector2 targetPoint = Vector2.Lerp(_initialVelocity, moveToPoint, ease);
                    targetPoint.X += MathF.Sin(ratio * 12.0f) * 32;
                    Vector2 targetVel = targetPoint - NPC.Center;
                    NPC.velocity = targetVel;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);



                    if (Timer >= moveTime)
                    {
                        Timer = 0;
                        AttackCycle++;

                    }

                    Animator.PlayAnimation(ANIM_MOUTHOPEN, AnimationParams.Default with { IsLooping = false });
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                    }
                    //Look at player, charge for a bit, do 3 fireball shots
                    //repeat 3 times for a total of 9 shots
                    Animator.PlayAnimation(ANIM_MOUTHOPEN_HOLD, AnimationParams.Default with { IsLooping = false });



                    Vector2 lookOne = new Vector2(-1, -0.5f);
                    Vector2 vec = lookOne.SafeNormalize(Vector2.Zero);
  
                    float rot = vec.ToRotation();
                    float a = Utils.AngleLerp(NPC.rotation, rot, 0.03f);
                    NPC.rotation = a;
                    NPC.velocity *= 0.94f;

                    if (Timer % 2 == 0)
                    {
                        var pos = NPC.Center + Main.rand.NextVector2CircularEdge(144, 144);
                        pos += vec * 128;
                        var suck = NPC.Center - pos;
                        suck *= 0.02f;
                        var d = DustParticle.Spawn(pos + vec * 64, suck, DustParticleSpawnParams.Default);
                        d.Scale *= 0.8f;
                        d.gravity = 0;
                    
                    }

                    if (Timer % 30 == 0)
                    {
                        PixelPrimitiveCircleFactory.CreateGenericInBoom(NPC.Center + vec * 44, Color.Wheat, Color.White, 30, 384);
                    }

                    _outliner.warning = true;

                    if (Timer >= 90)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    Animator.PlayAnimation(ANIM_MOUTHOPEN_HOLD, AnimationParams.Default with { IsLooping = false });

                    _outliner.attacking = true;
                    foreach(var seg in Segments)
                    {
                        seg.isBurning = true;
                    }

                    Vector2 lookOne = new Vector2(-1 * direction, -0.5f);
                    Vector2 lookTwo = new Vector2(1 * direction, -0.5f);

                    float ratio = Timer / 80;
                    float ease = EasingFunction.InOutSine(ratio);
                    Vector2 look = Vector2.Lerp(lookOne, lookTwo, ease);
                    Vector2 vec = look.SafeNormalize(Vector2.Zero);
                    float rot = vec.ToRotation();
                    float a = Utils.AngleLerp(NPC.rotation, rot, 0.1f);
                    NPC.rotation = a;
                    NPC.velocity *= 0.8f;

                    if(Timer % 24 == 0)
                    {
                        NPC.velocity -= vec * 8;
               
                    }
     
                    if (MultiplayerHelper.IsHost && Timer % 24 == 0)
                    {
                        ProjFirer firer = ProjFirer.From<VulcanFireball>(NPC);
                        firer.velocity = vec * 18;
                        firer.velocity += Main.rand.NextVector2Circular(4, 4);
                        firer.damage = Fire_Breath_Damage;
                        firer.New();

                    }

                    if (Timer >= 80)
                    {
                        Timer = 0;
                        AttackCycle++;
                        AttackCount++;
                    }

                }
                break;
            case 3:
                {
                    NPC.velocity.X += direction * 0.05f;
                    NPC.velocity.Y += 0.5f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    if(Timer >= 30)
                    {
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {
                    NPC.velocity.X += direction * 0.05f;
                    NPC.velocity.Y -= 0.35f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    if (Timer >= 120)
                    {
                        Timer = 0;
                        if (AttackCount >= 3)
                        {
                            AttackCycle = 5;
                        }
                        else
                        {
                            AttackCycle = 1;
                        }
                    }
                }
                break;
            case 5:
                {
                    var animator = this.GetAnimator();
                    animator.PlayAnimation(ANIM_MOUTH_BITE, AnimationParams.Default with { IsLooping = false });
                    NPC.velocity.X += -0.2f;
                    NPC.velocity.Y += 0.5f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    if (Timer >= 90)
                    {
                        SwitchState(AIState.Ouroboros);
                    }
                }
                break;
        }
    }
}

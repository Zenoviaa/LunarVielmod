using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;
using Stellamod.Core.Camera;
using System;
using Terraria;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{
    private int Fire_Breath_V2_Damage => 50;
    private float Fire_Breath_V2_Blast_Time => 150;
    private float Fire_Breath_V2_Come_In_Time => 180;
    private float Fire_Breath_V2_X_Radius => 512;
    private float Fire_Breath_V2_Y_Radius => 384;
    private void AI_FireBreathV2()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    Vector2 eruptionLeft = EruptionLeft;
                    Vector2 eruptionRight = EruptionRight;

                 //   CameraTargetSystem.AddTarget(NPC.Center);

                    eruptionRight.Y -= 384;
                    eruptionLeft.Y -= 384;

                    Vector2 midPoint = Vector2.Lerp(eruptionLeft, eruptionRight, 0.75f);

                    float moveTime = Fire_Breath_V2_Come_In_Time;
                    float xRadius = Fire_Breath_V2_X_Radius;
                    float yRadius = Fire_Breath_V2_Y_Radius;
                    float ratio = Timer / moveTime;
                    float ease = EasingFunction.InOutSine(ratio);
                    float x = MathF.Sin(ease * MathHelper.Pi) * xRadius;
                    float y = MathF.Cos(ease * MathHelper.Pi) * yRadius;

                    Vector2 moveToPoint = midPoint + new Vector2(x, y);
                    Vector2 targetVel = moveToPoint - NPC.Center;
                    NPC.velocity = targetVel;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);

                    Animator.PlayAnimation(ANIM_IDLE, AnimationParams.NoLooping);
                    if (Timer == 1)
                    {
                        Teleport(eruptionLeft);
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
                    Vector2 diff = Vector2.UnitY;
                    float rot = diff.ToRotation();
                    NPC.velocity *= 0.96f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, rot, 0.01f);
                    foreach (var segment in Segments)
                    {
                        segment.isBurning = true;
                    }
                    _outliner.warning = true;
                    if (Timer < 18)
                    {
                        Animator.PlayAnimation(ANIM_MOUTHOPEN, AnimationParams.NoLooping);
                    }
                    else if (Timer < 36)
                    {
                        Animator.PlayAnimation(ANIM_MOUTH_BIG_OPEN, AnimationParams.NoLooping);
                    }

                    if (Timer >= 90)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    foreach (var segment in Segments)
                    {
                        segment.isBurning = true;
                    }
                    _outliner.warning = true;
                    Vector2 diff = Vector2.UnitY;
                    float rot = diff.ToRotation();
                    NPC.velocity *= 0.96f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, rot, 0.01f);
                    if (Timer >= 60)
                    {
                        CreateFirebreathChargeEffect(NPC.Center);
                        Timer = 0;
                        AttackCount++;
                        if (AttackCount >= 3)
                        {
                            Timer = 0;
                            AttackCycle++;
                            AttackCount = 0;
                        }
                    }
                }
                break;
            case 3:
                {
                    if(Timer % 8 == 0)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            
                            ProjFirer breathFirer = ProjFirer.From<RekFireBreathV2>(NPC);
                            breathFirer.damage = Fire_Breath_V2_Damage;
                            breathFirer.velocity = NPC.rotation.ToRotationVector2() * 17;
                            breathFirer.New();
                        }
                    }
                    NPC.velocity *= 0.96f;
                    foreach (var segment in Segments)
                    {
                        segment.isBurning = true;
                        segment.deadly = true;
                    }
                    _outliner.attacking = true;
                    NPC.rotation += EasingFunction.InOutExpo(Timer / 60f) * 0.025f * MathHelper.Lerp(1f, 0f, EasingFunction.InSine(Timer / Fire_Breath_V2_Blast_Time));
                    if (Timer >= Fire_Breath_V2_Blast_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {
                    //Swim out
                    Animator.PlayAnimation(ANIM_MOUTH_BITE, AnimationParams.NoLooping);
                    NPC.velocity.X += -0.2f;
                    NPC.velocity.Y += 0.5f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    if (Timer >= 90)
                    {
                        NextState();
                    }
                }
                break;
        }
    }

}

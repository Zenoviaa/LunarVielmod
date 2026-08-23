using Stellamod.Assets;
using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;
using Stellamod.Core.Camera;
using Stellamod.Core.InverseKinematics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Animations;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{
    private float _soundTimer;
    private float _startRotation;
    private float _firebreathSide;
    private int Fire_Breath_V2_Damage => 50;
    private float Fire_Breath_V2_Blast_Time => 90;
    private float Fire_Breath_V2_Come_In_Time => 180;
    private float Fire_Breath_V2_X_Radius => 512;
    private float Fire_Breath_V2_Y_Radius => 384;
    private float Fire_Breath_V2_Charge_Time => 240;
    private void AI_FireBreathV2()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    Vector2 eruptionLeft = Vector2.Lerp(EruptionLeft, EruptionRight, 0.2f);
                    Vector2 eruptionRight = Vector2.Lerp(EruptionLeft, EruptionRight, 0.8f);
                    eruptionRight.Y -= 192;
                    eruptionLeft.Y -= 192;
                    if (Timer == 1)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            _firebreathSide = Main.rand.NextBool(2) ? -1 : 1;
                            Vector2 teleportPoint = _firebreathSide < 0 ? eruptionLeft : eruptionRight;
                            Teleport(teleportPoint);
                        }
              
                    }

                    Timer = 0;
                    AttackCycle++;
                    NPC.netUpdate = true;
                }
                break;
            case 1:
                {
                    _soundTimer = 0;
                    Vector2 eruptionLeft = EruptionLeft;
                    Vector2 eruptionRight = EruptionRight;

                    eruptionRight.Y -= 384;
                    eruptionLeft.Y -= 384;

                    float sideAlpha = _firebreathSide < 0 ? 0.25f : 0.75f;
                    Vector2 midPoint = Vector2.Lerp(eruptionLeft, eruptionRight, sideAlpha);
      
                    float moveTime = Fire_Breath_V2_Come_In_Time;
                    float xRadius = Fire_Breath_V2_X_Radius;
                    float yRadius = Fire_Breath_V2_Y_Radius;
                    float ratio = Timer / moveTime;
                    float ease = EasingFunction.InOutSine(ratio);
                    float x = MathF.Sin(ease * MathHelper.Pi) * xRadius;
                    float y = MathF.Cos(ease * MathHelper.Pi) * yRadius;

                    Vector2 moveToPoint = midPoint + new Vector2(x * _firebreathSide, y);
                    Vector2 targetVel = moveToPoint - NPC.Center;
                    NPC.velocity = targetVel;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);

                    Animator.PlayAnimation(ANIM_IDLE, AnimationParams.NoLooping);
            
                    if (Timer >= moveTime)
                    {
                        Timer = 0;
                        AttackCycle++;

                    }
                }
                break;
            case 2:
                {
                    if(Timer == 1)
                    {
                        _startRotation = NPC.rotation;
                        _soundTimer = -11f;
       
                    }

                    if(Timer == 60)
                    {
                        var chargeSound = AssetRegistry.Sounds.Rek.BigLaserChargeRek;
                        SoundEngine.PlaySound(chargeSound, MyTarget.position);
                    }
                    Vector2 diff = Vector2.UnitY;
                    float rot = diff.ToRotation();
                    NPC.velocity *= 0.96f;

                    float ratio = Timer / Fire_Breath_V2_Charge_Time;
                    float ease = ratio;
                    float easedRotation = Utils.AngleLerp(_startRotation, diff.ToRotation(), ease * 0.68f);
                    NPC.rotation = easedRotation;

                    _outliner.warning = true;
                    if (Timer < 18)
                    {
                        Animator.PlayAnimation(ANIM_MOUTHOPEN, AnimationParams.NoLooping);
                    }
                    else if (Timer < 36)
                    {
                        Animator.PlayAnimation(ANIM_MOUTH_BIG_OPEN, AnimationParams.NoLooping);
                    } else if (Timer < 64)
                    {
                        Animator.PlayAnimation(ANIM_MOUTH_BIG_OPEN_READY, AnimationParams.NoLooping);
                    }
                    else
                    {
                        Animator.PlayAnimation(ANIM_MOUTH_BIG_OPEN_HOLD);
                    }

                    if(Timer >= 64)
                    {
                        if(Timer % 4 == 0)
                        {
                       //     FXUtil.GlowCircleBoom(NPC.Center + NPC.rotation.ToRotationVector2() * 64, Color.Yellow, Color.Red, Color.DarkRed, 45);
                        }
                        _showMouthAura = true;


                        if (Main.rand.NextBool(3))
                        {
                            Vector2 pos = NPC.Center;
                            pos += Main.rand.NextVector2Circular(64, 64);
                            pos += NPC.rotation.ToRotationVector2() * 165;
                            Vector2 vel = NPC.Center - pos;
                            vel *= 0.1f;
                            Color color = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0f, 1f));
                            Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
                            {
                                position = pos,
                                velocity = vel,
                                timeLeft = 45,
                                innerColor = color.ToVector4(),
                                outerColor = Color.Red.ToVector4()
                            });
                        }
                        _rekfireballAlpha = MathHelper.Lerp(0f, 1f, (Timer - 64) / (Fire_Breath_V2_Charge_Time - 64));
                    }
               
                    int j = 0;
                    //All parts should glow and charge up, with sawblades coming out and whatnot
                    for(int i = Segments.Length - 1; i >= 0 ; i--)
                    {
                        var segment = Segments[i];
                        float time = Timer - j * 11f;
                        if (time > 0)
                        {
                            segment.isBurning = true;
                            segment.sawBladeAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutExpo(time / 11f));
                        }
                        if(i % 2 == 0)
                            j++;
                    }
                    _soundTimer++;
                    if(_soundTimer >= 11f)
                    {
                        var sound = AssetRegistry.Sounds.Rek.RekSpikeOut;
                        SoundEngine.PlaySound(sound, NPC.position);
                        _soundTimer = 0;
                    }
    
                    if (Timer >= Fire_Breath_V2_Charge_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    if(Timer == 1)
                    {
                        NPC.velocity -= NPC.rotation.ToRotationVector2() * 5;
                    }
                    if(Timer == 1)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            
                            ProjFirer breathFirer = ProjFirer.From<ReksGreatFireBreath>(NPC);
                            breathFirer.ai0 = NPC.whoAmI;
                            breathFirer.damage = Fire_Breath_V2_Damage;
                            breathFirer.velocity = NPC.rotation.ToRotationVector2() * 1024;
                            breathFirer.New();
                        }
                    }

                    NPC.velocity *= 0.96f;
                    NPC.velocity -= NPC.rotation.ToRotationVector2() * 0.1f;
                    foreach (var segment in Segments)
                    {
                        segment.isBurning = true;
                        segment.deadly = true;
                        segment.sawBladeAlpha = 1f;
                    }

                    _outliner.attacking = true;
                    NPC.rotation += EasingFunction.InOutExpo(Timer / 20) * 0.035f * MathHelper.Lerp(1f, 0f, EasingFunction.InSine(Timer / Fire_Breath_V2_Blast_Time)) * _firebreathSide;
                    if (Timer >= Fire_Breath_V2_Blast_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {
                    int j = 0;
                    //All parts should glow and charge up, with sawblades coming out and whatnot
                    for (int i = Segments.Length - 1; i >= 0; i--)
                    {
                        var segment = Segments[i];
                        float time = Timer - j * 2f;
                        segment.sawBladeAlpha = MathHelper.Lerp(1f, 0f, EasingFunction.InOutExpo(time / 10f));
                        j++;
                    }

                    //Swim out
                    Animator.PlayAnimation(ANIM_MOUTH_BITE, AnimationParams.NoLooping);
                    if(Timer >= 45)
                    {
                        NPC.velocity.X += -0.2f * _firebreathSide;
                        NPC.velocity.Y += 0.5f;
                        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                        if (Timer >= 135)
                        {
                            NextState();
                        }
                    }
         
                }
                break;
        }
    }

}

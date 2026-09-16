using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR
{
    private ref Vector2 JumpStartPosition => ref _vector1;
    private ref Vector2 JumpTargetPosition => ref _vector2;
    private float Num_Falling_Rocks => 7;
    private float Jump_Rock_Prep_Time => 15;
    private float Jump_Ready_Time => 45;
    private int Jump_Rock_Crash_Damage => 45;
    private int Jump_Rock_Rock_Damage => 32;
    private float Jump_Rock_Punch_Out_Time => 60;
    private float Jump_Rock_Falling_Time => 88;
    private void AI_JumpRockSlam()
    {
        CameraTargetSystem.AddTarget(Vector2.Lerp(MyTarget.Center, NPC.Center, 0.25f));
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        GruntSound();
                    }

                    StayGrounded();
                    FaceTarget();
                    float time = Jump_Rock_Prep_Time;
                    float ratio = Timer / time;
                    Vector2 lerp1 = Vector2.Lerp(Vector2.One, new Vector2(1.3f, 0.9f), EasingFunction.OutExpo(ratio));
                    Vector2 lerp2 = Vector2.Lerp(new Vector2(1.3f, 0.9f), new Vector2(0.9f, 1.2f), EasingFunction.InOutSine(ratio));
                    Vector2 lerp3 = Vector2.Lerp(lerp1, lerp2, ratio);
                    _jumpScale = lerp3;
                    this.AseAnimator.PlayAnimation(ANIM_CROUCH, AnimationParams.NoLooping);
                    if (Timer >= Jump_Rock_Prep_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        var sound = AssetReferences.Assets.Sounds.STARR.STARRInAir.Asset with { PitchVariance = 0.5f };
                        SoundEngine.PlaySound(sound, NPC.position);
                        BigGruntSound();
                        MakeJumpVFX(NPC.Bottom, -Vector2.UnitY * 15);
                        SoundStyle bellHit = AssetRegistry.Sounds.Magic.AutomationHit1;
                        bellHit.PitchVariance = 0.2f;
                        SoundEngine.PlaySound(bellHit, NPC.position);
                        NPC.velocity.Y = -11;

                        float xDirection = NPC.Center.X < MyTarget.Center.X ? 1 : -1;
                        NPC.velocity.X += xDirection * 4f;
                        var p = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Bottom, Vector2.UnitY);
                        var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Bottom, Vector2.UnitY * 4);
                        p2.Scale *= 0.5f;

                    }

                    _outliner.warning = true;
                    MakeJumpingParticles();
                    if(Main.rand.NextBool(6))
                        MakeSparkleAroundVFX(NPC.Center);
                    _afterImages = true;
                    _jumpingTrail = true;

                    if (Timer >= 15)
                    {
                        if (Timer <= 45)
                        {
                            NPC.velocity.Y *= 0.95f;
                            NPC.rotation = NPC.velocity.X * 0.05f;
                        }
                        else
                        {
                            if (Timer == 44)
                            {
                                SoundStyle fallSound = AssetRegistry.Sounds.Bishinine.BishinineFastfall;
                                fallSound.PitchVariance = 0.1f;
                                SoundEngine.PlaySound(fallSound, NPC.position);
                            }
                            NPC.rotation = -NPC.velocity.X * 0.05f;
                            NPC.velocity.X += NPC.direction * 0.1f;
                            NPC.velocity.Y *= 1.07f;
                            NPC.noGravity = true;
                            if (Timer % 5 == 0)
                            {
                                var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Bottom, -NPC.velocity);
                                p2.Scale *= 0.5f;
                            }
                        }
                    }

                    NPC.noGravity = true;
                    this.AseAnimator.PlayAnimation(ANIM_JUMPFRAME, AnimationParams.NoLooping);
                    if (Timer >= Jump_Ready_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    _outliner.attacking = true;
               
                    if(Timer == 1)
                    {
        
                        _initialVelocity = NPC.velocity;
                        JumpStartPosition = NPC.Center;
                        JumpTargetPosition = TileUtilities.FallToSolidTile(JumpStartPosition.ToTileCoordinates()).ToWorldCoordinates();
                        JumpTargetPosition -= new Vector2(0, 64);
                    }

                    _afterImages = true;

                    float time = Jump_Rock_Falling_Time;
                    float ratio = Timer / time;
                    if(ratio > 0.2f)
                    {
                        MakeCometParticles(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero));
                    }


 
                    JumpStartPosition.X = MathHelper.Lerp(JumpStartPosition.X, MyTarget.Center.X, 0.2f);
                    JumpTargetPosition.X = MathHelper.Lerp(JumpTargetPosition.X, MyTarget.Center.X, 0.2f);

                    _bigStarAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutExpo(Timer / 64));
                    Vector2 up = JumpStartPosition + new Vector2(0, -256);
                    Vector2 lerp1 = Vector2.Lerp(JumpStartPosition, up, EasingFunction.OutExpo(ratio));
                    Vector2 lerp2 = Vector2.Lerp(up, JumpTargetPosition, EasingFunction.InExpo(ratio));
                    Vector2 lerp3 = Vector2.Lerp(lerp1, lerp2, EasingFunction.InSine(ratio));
                    Vector2 vel = lerp3 - NPC.Center;
                    NPC.velocity = Vector2.Lerp(_initialVelocity, vel, EasingFunction.InOutSine(ratio));
                    NPC.rotation *= 0.92f;
                    _jumpingTrail = true;
                    NPC.noGravity = true;
                    MakeFallingCrashParticles();

                    _starRot = MathHelper.Lerp(0, MathHelper.TwoPi * 2, Timer / time);
               
                    if(Timer < 25)
                    {
                        this.AseAnimator.PlayAnimation(ANIM_JUMPTOHOVER, AnimationParams.NoLooping);
                    }
                    else if(Timer < 50)
                    {
                        this.AseAnimator.PlayAnimation(ANIM_PUNCH_DOWN_READY, AnimationParams.NoLooping); 
                    }
                    else
                    {
                        this.AseAnimator.PlayAnimation(ANIM_PUNCH_DOWN, AnimationParams.Default);

                    }

                    if (Timer >= time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    if (Timer == 1)
                    {
                        KickImpactVFX(NPC.Bottom, -Vector2.UnitY * 15);
                        EarthQuakeVFX(NPC.Bottom, -Vector2.UnitY * 15);
                        StarBitVFX(NPC.Bottom, -Vector2.UnitY * 15);
                        CrackVFX(NPC.Bottom);
                        StarBoomVFX(NPC.Bottom);
                        IFartedVFX(NPC.Bottom, -Vector2.UnitY * 7);
                        var sound = AssetReferences.Assets.Sounds.STARR.StarrSlam.Asset with { PitchVariance = 0.5f };
                        SoundEngine.PlaySound(sound, NPC.position);
                        if (MultiplayerHelper.IsHost)
                        {
                            ProjFirer crashFirer = ProjFirer.From<STARROCKCRASHSLAM>(NPC);
                            crashFirer.position = NPC.Bottom;
                            crashFirer.velocity = -Vector2.UnitY;
                            crashFirer.damage = Jump_Rock_Crash_Damage;
                            crashFirer.knockback = 1;
                            crashFirer.New();
                            for (float i = 0; i < Num_Falling_Rocks; i++)
                            {
                                ProjFirer fallingRockFirer = ProjFirer.From<STARROCKCRASH>(NPC);
                                fallingRockFirer.position = NPC.Bottom + new Vector2(0, -38);
                                fallingRockFirer.velocity = -Vector2.UnitY * 20;
                                fallingRockFirer.velocity = fallingRockFirer.velocity.RotatedBy(MathHelper.Lerp(-0.8f, 0.8f, i / Num_Falling_Rocks));
                                fallingRockFirer.damage = Jump_Rock_Rock_Damage;
                                fallingRockFirer.New();
                            }
                        }
                    }

                    float ratio = Timer / Jump_Rock_Punch_Out_Time;
                    _medalAlpha = MathHelper.Lerp(1f, 0f, EasingFunction.OutQuad(ratio));
                    _medalScale = MathHelper.Lerp(0f, 2f, EasingFunction.OutSine(ratio));
                    MakeSparkleAroundVFX(NPC.Center);
                    OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -44);
                    ShakeScreenPosition.Shake = MathHelper.Lerp(8, 2, EasingFunction.InOutSine(Timer / Jump_Rock_Punch_Out_Time));
                    StayGrounded();
                    this.AseAnimator.PlayAnimation(ANIM_PUNCH_DOWN_OUT, AnimationParams.NoLooping);
                    if(Timer >= Jump_Rock_Punch_Out_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {
                    ChooseAttack();
                }
                break;
        }
    }

}

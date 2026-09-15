using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Camera;
using System;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR
{
    private ref Vector2 JumpStartPosition => ref _vector1;
    private ref Vector2 JumpTargetPosition => ref _vector2;
    private float Num_Falling_Rocks => 7;
    private float Jump_Rock_Prep_Time => 40;
    private float Jump_Ready_Time => 60;
    private int Jump_Rock_Crash_Damage => 45;
    private int Jump_Rock_Rock_Damage => 32;
    private float Jump_Rock_Punch_Out_Time => 60;
    private float Jump_Rock_Falling_Time => 46;
    private void AI_JumpRockSlam()
    {
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
                        BigGruntSound();
                        JumpStartPosition = NPC.Center;
                        JumpTargetPosition = MyTarget.Center - Vector2.UnitY * 384;
                    }

                    _outliner.warning = true;
                    MakeJumpingParticles();

                    _afterImages = true;
                    _jumpingTrail = true;
                    float xDirection = MathF.Sign(JumpTargetPosition.X - JumpStartPosition.X);
                    Vector2 upward = (JumpTargetPosition - JumpStartPosition).RotatedBy(MathHelper.PiOver2 * -xDirection).SafeNormalize(Vector2.Zero);
                    Vector2 pullPoint = JumpStartPosition + JumpTargetPosition;
                    pullPoint *= 0.5f;
                    pullPoint += upward * 128;

                    float ratio = Timer / Jump_Ready_Time;
                    float easeOut = EasingFunction.OutExpo(ratio);
                    Vector2 lerp1 = Vector2.Lerp(JumpStartPosition, pullPoint, easeOut);
                    Vector2 lerp2 = Vector2.Lerp(pullPoint, JumpTargetPosition, easeOut);
                    Vector2 lerp3 = Vector2.Lerp(lerp1, lerp2, ratio);
                    Vector2 targetVelocity = lerp3 - NPC.Center;
                    NPC.velocity = targetVelocity;
                    this.AseAnimator.PlayAnimation(ANIM_PUNCH_DOWN_READY, AnimationParams.NoLooping);
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
                    NPC.velocity.X *= 0.5f;
                    if(Timer == 1)
                    {
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

                    _bigStarAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutExpo(Timer / 64));
                    Vector2 up = JumpStartPosition + new Vector2(0, -100);
                    Vector2 lerp1 = Vector2.Lerp(JumpStartPosition, up, EasingFunction.OutExpo(ratio));
                    Vector2 lerp2 = Vector2.Lerp(up, JumpTargetPosition, EasingFunction.InExpo(ratio));
                    Vector2 lerp3 = Vector2.Lerp(lerp1, lerp2, ratio);
                    Vector2 vel = lerp3 - NPC.Center;
                    NPC.velocity = vel;

                    _jumpingTrail = true;
                    NPC.noGravity = true;
                    MakeFallingCrashParticles();
 
                    this.AseAnimator.PlayAnimation(ANIM_PUNCH_DOWN, AnimationParams.Default);
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
                        var sound = AssetReferences.Assets.Sounds.RocketExplosion.Asset with { PitchVariance = 0.5f };
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
                    OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -44);
                    ShakeScreenPosition.Shake = 8;
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
                    SwitchState(AIState.CapeOut);
                }
                break;
        }
    }

}

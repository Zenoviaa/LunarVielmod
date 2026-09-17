using Stellamod.Common.Particles;
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
    private ref Vector2 StartDashPosition => ref _vector1;
    private ref Vector2 EndDashPosition => ref _vector2;
    private float Grab_Start_Time => 60;
    private int Got_You_Damage => 70;
    private float Got_You_Back_Time => 45;
    private float Fake_Out_Count => 4;
    private float Grab_Time => 38;
    private void AI_CommandGrab()
    {
        void HoldPlayer()
        {
            if(_grabbedPlayer != -1)
            {
                Vector2 dir = NPC.spriteDirection == 1 ? Vector2.UnitX : -Vector2.UnitX;
                Player player = Main.player[_grabbedPlayer];
                GotYouPlayer gotYouPlayer = player.GetModPlayer<GotYouPlayer>();
                gotYouPlayer.grabPosition = NPC.Top + dir * 32 + new Vector2(0, 18);
            }
        }
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        FaceTarget();
                        GruntSound();

                        SoundStyle bellHit = AssetRegistry.Sounds.Magic.AutomationHit1;
                        bellHit.PitchVariance = 0.2f;
                        SoundEngine.PlaySound(bellHit, NPC.position);

                        var p = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Bottom, Vector2.UnitY);
                        p.outerColor = Color.DarkOrange;
                        p.innerColor = Color.Gold;
                        StartDashPosition = NPC.Center;
                        EndDashPosition = MyTarget.Center;
                        EndDashPosition = TileUtilities.FallToSolidTile(EndDashPosition);
                        Vector2 dir = EndDashPosition - StartDashPosition;
                        dir = dir.SafeNormalize(Vector2.Zero);
                        EndDashPosition.X -= dir.X * 64;
                    }

                    _outliner.warning = true;
                    _afterImages = true;
                    _jumpingTrail = true;

                    float time = Tornado_Jump_Time;
                    float ratio = Timer / time;
                    Vector2 positionToMoveTo = Vector2.Lerp(StartDashPosition, EndDashPosition, ratio);
                    float yOut = MathHelper.Lerp(0, -128, EasingFunction.OutExpo(ratio));
                    float yIn = MathHelper.Lerp(-128, 0, EasingFunction.InExpo(ratio));
                    float yOffset = MathHelper.Lerp(yOut, yIn, ratio);
                    positionToMoveTo.Y += yOffset;
                    Vector2 vel = positionToMoveTo - NPC.Center;
                    NPC.velocity = vel;
                    NPC.rotation = vel.X * 0.01f;
                    this.AseAnimator.PlayAnimation(ANIM_JUMPFRAME, AnimationParams.NoLooping);
                    if (Timer >= time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    //This gonna repeat three times
                    if(Timer == 1)
                    {
                        GruntSound();
                        if(AttackCounter > 0)
                        {
                            NPC.velocity.X -= NPC.XDirectionToTarget * 8;
                            SoundStyle bellHit = AssetRegistry.Sounds.Magic.AutomationHit1;
                            bellHit.PitchVariance = 0.2f;
                            bellHit.Volume = 0.6f;
                            SoundEngine.PlaySound(bellHit, NPC.position);


                            var p = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Bottom, Vector2.UnitY);
                            p.outerColor = Color.DarkOrange;
                            p.innerColor = Color.Gold;
                        }
        
                    }
                    CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.5f));
                    StayGrounded();
                    FaceTarget();
                    NPC.rotation *= 0.92f;
                    _outliner.warning = true;
                    _afterImages = true;
                    if (Timer < 30 && AttackCounter == 0)
                    {
                        this.AseAnimator.PlayAnimation(ANIM_GRAB_READY, AnimationParams.NoLooping);
                    }
                    else
                    {
                        this.AseAnimator.PlayAnimation(ANIM_GRAB_IDLE, AnimationParams.Default);
                    }
          
                    if (Timer >= Got_You_Back_Time)
                    {
                        Timer = 0;
                        AttackCounter++;
                        if(AttackCounter >= Fake_Out_Count)
                        {
                            AttackCycle++;
                        }
                    }
                }
                break;
            case 2:
                {
                    _grabbing = true;
                    if(Timer == 1)
                    {
                        _grabbedPlayer = -1;
                        
                    }
     
                    HoldPlayer();
                    FaceTarget();
                    StartDashPosition = NPC.Bottom;
                    EndDashPosition = MyTarget.Bottom;
                    Vector2 pos = Vector2.Lerp(StartDashPosition, EndDashPosition, EasingFunction.InSine(Timer / Grab_Time));
                    Vector2 vel = pos - NPC.Bottom;
                    NPC.velocity = vel;
                    _outliner.attacking = true;
                    _afterImages = true;
                    _jumpingTrail = true;
                    MakeCometParticles(NPC.Bottom, -Vector2.UnitY * 8);
                    MakeJumpingParticles();
                    if (Timer % 9 == 0)
                    {
                        var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero) * 3);
                        p2.Scale *= 0.5f;
                        p2.innerColor = Color.Gold;
                        p2.outerColor = Color.DarkGoldenrod;
                        p2.fadeToColor = Color.DarkOrange;
                    }
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    this.AseAnimator.PlayAnimation(ANIM_GRAB_TRY, AnimationParams.NoLooping);
                    if(Timer >= Grab_Time || _grabbedPlayer != -1)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    HoldPlayer();
                    if(Timer == 1 && _grabbedPlayer != -1)
                    {
                        var sound = AssetReferences.Assets.Sounds.STARR.STARRMyMy.Asset;
                        SoundEngine.PlaySound(sound, MyTarget.Center);
                    }
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    NPC.velocity.X *= 0.88f;
                    NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, MathF.Sin(Timer * 0.08f + 3.28f) * 5f, 0.13f);
                    _afterImages = true;
                    if (Main.rand.NextBool(8))
                    {
                        Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(154, 154);
                        var sp = SparkleParticle.Spawn(pos, Vector2.Zero);
                        sp.gravity = 0;
                        sp.innerColor = Color.LightGoldenrodYellow;
                        sp.outerColor = Color.DarkOrange;
                        sp.dampening = 0.05f;
                        sp.Scale *= 0.34f;
                        sp.fast = true;
                    }

                    if (Timer % 2 == 0)
                    {
                        Vector2 pos = NPC.Center;
                        pos.Y += Main.rand.NextFloat(-128, 128);
                        Particles.GoldenLeaf.Spawn(GoldenLeaf.Data.Default with { rootPosition = pos, timeLeft = 150 });
                    }

                    this.AseAnimator.PlayAnimation(ANIM_GRAB_TRY, AnimationParams.NoLooping);
                    if(Timer >= 90)
                    {
                        if(_grabbedPlayer != -1)
                        {
                            Timer = 0;
                            AttackCycle = 9;
                        }
                        else
                        {
                            Timer = 0;
                            AttackCycle++;
                        }
                    }
                }
                break;
            case 4:
                {
                    if(Timer == 1)
                    {
                        var sound = AssetReferences.Assets.Sounds.STARR.STARRDescendant.Asset;
                        sound.Volume = 0.25f;
                        SoundEngine.PlaySound(sound, MyTarget.Center);
                    }
                    StayGrounded();
                    this.AseAnimator.PlayAnimation(ANIM_GRAB_NO, AnimationParams.NoLooping);
                    if (Timer >= 60)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;


            //yippeee, you suck
            case 9:
                {
                    if (Timer == 1)
                    {
                        var sound = AssetReferences.Assets.Sounds.STARR.STARRPathetic.Asset;
                        SoundEngine.PlaySound(sound, MyTarget.Center);
                    }
           
                    StayGrounded();
                    if(Timer >= 18)
                    {
                        this.AseAnimator.PlayAnimation(ANIM_PUNCH, AnimationParams.NoLooping);
                    }
                    else
                    {
                        HoldPlayer();
                        this.AseAnimator.PlayAnimation(ANIM_PUNCH_READY, AnimationParams.NoLooping);
                    }
                    if (Timer == 24 && MultiplayerHelper.IsHost)
                    {
                        ProjFirer gotYou = ProjFirer.From<GOTYOU>(NPC);
                        gotYou.damage = Got_You_Damage;
                        gotYou.knockback = 1;
                        Vector2 direction = NPC.spriteDirection == 1 ? Vector2.UnitX : -Vector2.UnitX;
                        gotYou.velocity = direction * 24;
                        gotYou.velocity.Y -= 15;
                        gotYou.ai0 = _grabbedPlayer;
                        gotYou.New();
                    }

                    if (Timer >= 30)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 10:
                {
                    if (Timer == 1)
                    {
                        BigGruntSound();
                    }
                    _outliner.attacking = true;
                    this.AseAnimator.PlayAnimation(ANIM_PUNCH_BACKAIR, AnimationParams.NoLooping);
  
                    if(Timer >= 60)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }
}

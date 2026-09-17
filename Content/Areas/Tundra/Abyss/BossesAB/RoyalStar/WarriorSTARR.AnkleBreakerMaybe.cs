using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR 
{
    private float Tornado_Jump_Time => 45;
    private float Tornado_Warn_Time => 45;
    private float Tornadoing_Time => 540;
    private int Tornado_Damage => 40;
    private float Tornado_End_Time => 90;

    /*
     * Jumps backwards and then runs towards you after charging for a bit, 
     * he’ll then either do a flying spinning kick (like that one street fighter move)
     * or a low ankle breaker type kick, each telegraphed differently you have to pay attention, 
     * the flying kick will only hit you if you’re airborne and the ankle breaker only hits you if you’re grinded, 
     * it also kicks rocks so it’ll still hit you if you’re far
     */
    private void AI_AnkleBreakerMaybe()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    //He'll jump to you
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        FaceTarget();
                        GruntSound();

                        SoundStyle bellHit = AssetRegistry.Sounds.Magic.AutomationHit1;
                        bellHit.PitchVariance = 0.2f;
                        SoundEngine.PlaySound(bellHit, NPC.position);

                        MakeGoldenDonut(NPC.Bottom, Vector2.UnitY);

                        StartDashPosition = NPC.Center;
                        EndDashPosition = MyTarget.Center;
                        EndDashPosition = TileUtilities.FallToSolidTile(EndDashPosition);
       
                        if (MultiplayerHelper.IsHost && _canFakeOut)
                        {
                            _fakeOut = Main.rand.NextBool(3);
                            NPC.netUpdate = true;
                        }
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
        

                    NPC.rotation *= 0.8f;
                    NPC.velocity *= 0.6F;
                    StayGrounded();
                    FaceTarget();
                    _outliner.warning = true;
                    this.AseAnimator.PlayAnimation(ANIM_SPIN_KICK_READY, AnimationParams.NoLooping);
                    if (Timer == (int)(Tornado_Warn_Time - 30))
                    {
              
                        if (_fakeOut)
                        {
                            Timer = 0;
                            AttackCycle = 10;
                            var sound = AssetReferences.Assets.Sounds.STARR.STARRKidding.Asset;
                            SoundEngine.PlaySound(sound, MyTarget.position);
                        }
                        else
                        {
                            var sound = AssetReferences.Assets.Sounds.STARR.STARRGoldenWind.Asset;
                            sound.Volume = 0.76f;
                            SoundEngine.PlaySound(sound, MyTarget.position);
                        }

                    }

                    if (Timer >= Tornado_Warn_Time)
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
                 
                        if (MultiplayerHelper.IsHost)
                        {
                            ProjFirer starNadoFirer = ProjFirer.From<STARNADO>(NPC);
                            starNadoFirer.ai0 = NPC.whoAmI;
                            starNadoFirer.damage = Tornado_Damage;
                            starNadoFirer.New();
                        }
                    }

                    if(Timer % 16 == 0)
                    {
                        SoundStyle spin = AssetReferences.Assets.Sounds.Jiitas.JiitasLightSpin.Asset;
                        SoundEngine.PlaySound(spin, MyTarget.position);
                    }
  
                    _outliner.attacking = true;
                    _afterImages = true;
                    _jumpingTrail = true;
                    MakeJumpingParticles();
                    this.AseAnimator.PlayAnimation(ANIM_SPIN_KICK, AnimationParams.Default);
                    float xMovement = NPC.XDirectionToTarget * 6f;
                    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, xMovement, 0.06f);

                    float yOsc = MathF.Sin(Timer * 0.03f + 3f) * 5f;
                    NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, yOsc, 0.13f);
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.015f, 0.03f);
                    if (Timer >= Tornadoing_Time)
                    {
                        SwitchState(AIState.JumpRockSlam);
                        Timer = 0;
                        AttackCycle = 2;
                        NPC.netUpdate = true;
                        //AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    _afterImages = true;
                    _jumpingTrail = true;
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    NPC.velocity *= 0.92f;
                    NPC.velocity.Y -= 0.75f;
                    NPC.rotation *= 0.94f;
                    if(Timer >= Tornado_End_Time)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;


            case 10:
                {
                    SwitchState(AIState.DiscThrow);
                }
                break;
        }
    }
}

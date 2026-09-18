using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR
{
    private int Rock_Spike_Count => 4;
    private int Rock_Spike_Damage => 32;
    private float Rock_Spike_Prep_Time => 15;
    private float Rock_Spike_Run_Time => 90;
    private void AI_RockSpikeRun()
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
                    float time = Jump_Rock_Prep_Time;
                    float ratio = Timer / time;
                    Vector2 lerp1 = Vector2.Lerp(Vector2.One, new Vector2(1.3f, 0.9f), EasingFunction.OutExpo(ratio));
                    Vector2 lerp2 = Vector2.Lerp(new Vector2(1.3f, 0.9f), new Vector2(0.9f, 1.2f), EasingFunction.InOutSine(ratio));
                    Vector2 lerp3 = Vector2.Lerp(lerp1, lerp2, ratio);
                    _jumpScale = lerp3;
                    this.AseAnimator.PlayAnimation(ANIM_CROUCH, AnimationParams.NoLooping);
                    if (Timer >= Rock_Spike_Prep_Time)
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

                        SoundStyle bellHit = AssetRegistry.Sounds.Magic.AutomationHit1;
                        bellHit.PitchVariance = 0.2f;
                        SoundEngine.PlaySound(bellHit, NPC.position);

                        float xDirection = NPC.Center.X < MyTarget.Center.X ? 1 : -1;

                        MakeGoldenDonut(NPC.Bottom, Vector2.UnitY);
                        StartDashPosition = NPC.Center;
                        EndDashPosition = StartDashPosition;
                        EndDashPosition.X += xDirection * 100;
                    }

                    _outliner.warning = true;
                    MakeJumpingParticles();
                    _afterImages = true;
                    _jumpingTrail = true;

                    float ratio = Timer / Jump_Ready_Time;
                    Vector2 pos = Vector2.Lerp(StartDashPosition, EndDashPosition, ratio);
                    float yOut = MathHelper.Lerp(0, -128, EasingFunction.OutExpo(ratio));
                    float yIn = MathHelper.Lerp(-128, 0, EasingFunction.InExpo(ratio));
                    float y = MathHelper.Lerp(yOut, yIn, ratio);
                    pos.Y += y;
                    Vector2 vel = pos - NPC.Center;
                    NPC.velocity = vel;
                    NPC.rotation = NPC.velocity.X * 0.05f;
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
                    StayGrounded();
                    if(Timer == 1)
                    {
                    //    MakeJumpVFX(NPC.Bottom, -Vector2.UnitY * 15);
                        AttackCounter++;
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 pos = NPC.Bottom;
                            NPC.NewNPC(SourceFromThis, (int)pos.X, (int)pos.Y, ModContent.NPCType<ROCKSPIKE>());
                        }
                    }
                    this.AseAnimator.PlayAnimation(ANIM_CROUCH, AnimationParams.NoLooping);
                    NPC.velocity.X *= 0.95f;
                    NPC.rotation = NPC.velocity.X * 0.05f;
                    if (Timer >= 30)
                    {
                        if(AttackCounter >= Rock_Spike_Count)
                        {
                            Timer = 0;
                            AttackCycle++;
                        }
                        else
                        {
                            Timer = 0;
                            AttackCycle = 0;
                        }
                    }
                }
                break;

            case 3:
                {
                    SwitchState(AIState.Idle);
                }
                break;
        }
    }
}

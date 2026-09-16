using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;
using Terraria;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR
{
    private int Rock_Spike_Damage => 32;
    private float Rock_Spike_Prep_Time => 50;
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
                        GruntSound();
                        NPC.TargetClosest();                 
                    }

                    _outliner.warning = true;
                    FaceTarget();
                    StayGrounded();

                    Vector2 direction = MyTarget.Center.X > NPC.Center.X ? Vector2.UnitX : -Vector2.UnitX;
                    float ratio = Timer / Flying_Or_Ankle_Prep_Time;
                    float ease = EasingFunction.InSine(ratio);
                    NPC.velocity = Vector2.Lerp(-direction * 32, Vector2.Zero, ease);
                    this.AseAnimator.PlayAnimation(ANIM_IDLE, AnimationParams.Default);
                    
                    StartDashPosition = NPC.Center;
                    EndDashPosition = NPC.Center + direction * 384;
                    if (Timer >= Rock_Spike_Prep_Time)
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
                    }
                    _afterImages = true;
                    _jumpingTrail = true;
                    _outliner.attacking = true;
                    if(Timer % 8 == 0 && Timer >= 15)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            var rok = ProjFirer.From<ROCKSPIKE>(NPC);
                            rok.position = NPC.Bottom;
                            rok.velocity = -Vector2.UnitY.RotatedByRandom(0.5f);
                            rok.damage = Rock_Spike_Damage;
                            rok.New();
                        }
                    }
                    this.AseAnimator.PlayAnimation(ANIM_RUN);

                    float time = Rock_Spike_Run_Time;
                    float ratio = Timer / time;
                    Vector2 pos = Vector2.Lerp(StartDashPosition, EndDashPosition, ratio);
                    Vector2 vel = pos - NPC.Center;

                    NPC.spriteDirection = NPC.velocity.X > 0 ? 1 : -1;
                    NPC.velocity = vel;
             
                    if (Timer >= time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;

            case 2:
                {
                    SwitchState(AIState.Idle);
                }
                break;
        }
    }
}

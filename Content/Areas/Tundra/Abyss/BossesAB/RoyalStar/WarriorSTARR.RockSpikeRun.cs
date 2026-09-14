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
                        RunDirection = MyTarget.Center.X > NPC.Center.X ? Vector2.UnitX : -Vector2.UnitX;
                    }

                    _outliner.warning = true;
                    FaceTarget();

                    float ratio = Timer / Flying_Or_Ankle_Prep_Time;
                    float ease = EasingFunction.OutExpo(ratio);
                    NPC.velocity = Vector2.Lerp(-RunDirection * 8, Vector2.Zero, ease);
                    this.AseAnimator.PlayAnimation(ANIM_IDLE, AnimationParams.Default);
                    if (Timer >= Rock_Spike_Prep_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    _outliner.attacking = true;
                    if(Timer % 8 == 0 && NPC.velocity.Length() > 4)
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
                    NPC.velocity = Vector2.Lerp(NPC.velocity, RunDirection * 8, 0.2f);
                    if (Timer >= Rock_Spike_Run_Time)
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

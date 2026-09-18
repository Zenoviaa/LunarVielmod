using Terraria;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR
{
    private float Cape_Out_Time => 27;
    private float Cape_In_Time => 27;
    private void AI_CapeOut()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                    }
                    if (Timer == 5)
                    {
                        MakeCapeOutEffect();
                    }
                    StayGrounded();
                    this.AseAnimator.PlayAnimation(ANIM_CAPE_OUT, AnimationParams.NoLooping);
                    if (Timer >= Cape_Out_Time)
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
                        TeleportToAnArenaSide();
                    }
                    if(Timer == 5)
                    {
                        MakeCapeInEffect();
                    }
                    FaceTarget();
                    StayGrounded();
                    this.AseAnimator.PlayAnimation(ANIM_CAPE_IN, AnimationParams.NoLooping);
                    if (Timer >= Cape_In_Time)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            SwitchState(AIState.CapeOut);
                        }
                        else
                        {
                            SwitchState(AIState.Idle);
                        }
                
                    }
                }
                break;
        }
    }
}

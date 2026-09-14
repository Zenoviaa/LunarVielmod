using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;
using Terraria;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;


public partial class WarriorSTARR
{
    private ref Vector2 StartDashPosition => ref _vector1;
    private ref Vector2 EndDashPosition => ref _vector2;
    private float Grab_Start_Time => 60;
    private int Got_You_Damage => 40;
    private void AI_CommandGrab()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        FaceTarget();
                        StartDashPosition = NPC.Center;
                        EndDashPosition = MyTarget.Center;
                    }
                    float steps = Vector2.Distance(EndDashPosition, StartDashPosition) / 16f;
                    float ratio = Timer / steps;
                    float ease = EasingFunction.InExpo(ratio);
                    Vector2 posToMoveTo = Vector2.Lerp(StartDashPosition, EndDashPosition, ease);
                    Vector2 vel = posToMoveTo - NPC.velocity;
                    NPC.velocity = vel;
                    this.AseAnimator.PlayAnimation(ANIM_GRAB_READY, AnimationParams.NoLooping);
                    _outliner.warning = true;
                    if (Timer >= steps)
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
                        NPC.TargetClosest();
                    }

                    NPC.velocity *= 0.94f;
                    FaceTarget();
                    _outliner.warning = true;
                    this.AseAnimator.PlayAnimation(ANIM_GRAB_START_SLOW, AnimationParams.NoLooping);
                    if (Timer >= Grab_Start_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    _grabbing = true;
                    _contactDamage = true;
                    _outliner.attacking = true;
                    if (Timer == 1)
                    {
                        NPC.velocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 24;
                        GruntSound();
                    }
                    NPC.velocity *= 0.94f;
                    NPC.noGravity = true;
                    if (AttackCounter == 1)
                    {
                        Timer = 0;
                        AttackCycle = 10;
                        NPC.netUpdate = true;
                    }
                    this.AseAnimator.PlayAnimation(ANIM_GRAB_TRY, AnimationParams.NoLooping);
                    if (NPC.velocity.Length() <= 1)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    this.AseAnimator.PlayAnimation(ANIM_GRAB_NO, AnimationParams.NoLooping);
                    StayGrounded();
                    if (Timer >= 60)
                    {
                        SwitchState(AIState.CapeOut);
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
                    this.AseAnimator.PlayAnimation(ANIM_GRAB_THROW, AnimationParams.NoLooping);
                    if (Timer == 10 && MultiplayerHelper.IsHost)
                    {
                        ProjFirer gotYou = ProjFirer.From<GOTYOU>(NPC);
                        gotYou.damage = Got_You_Damage;
                        gotYou.knockback = 1;
                        gotYou.velocity = NPC.velocity;
                        gotYou.velocity.X *= -1;
                        gotYou.velocity.Y -= 15;
                        gotYou.ai0 = _grabbedPlayer;
                        gotYou.New();
                    }
                }
                break;
        }
    }
}

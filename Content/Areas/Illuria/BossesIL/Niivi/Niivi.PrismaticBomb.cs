using Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;
using Terraria;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;

public partial class Niivi
{
    float PrismaticBomb_PrepTime => 100;
    int PrismaticBomb_Damage => 55;
    float PrismaticBomb_AttackTime => 1280;
    void AI_PrismaticBomb()
    {
        ref Vector2 StartPosition = ref _vec1;
        ref Vector2 EndPosition = ref _vec2;
        ref Vector2 InitialVelocity = ref _vec3;

        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();
                        StartPosition = NPC.Center;
                        EndPosition = MyTarget.Center + new Vector2(0, -128);
                        InitialVelocity = NPC.velocity;
                    }

                    var ratio = Timer / PrismaticBomb_PrepTime;
                    var posToMoveTo = Vector2.Lerp(StartPosition, EndPosition, ratio);
                    var velToGoTo = Vector2.Lerp(InitialVelocity, Vector2.Zero, EasingFunction.InOutExpo7(ratio));
                    posToMoveTo += velToGoTo;
                    NPC.velocity = Vector2.Zero;
                    NPC.Center = posToMoveTo;
                    TryLookAtTarget();
                    if(Timer >= PrismaticBomb_PrepTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if (Timer == 1 && MultiplayerHelper.IsHost)
                    {
                        var firer = ProjFirer.From<NiiviPrismaticBomb>(NPC);
                        firer.ai2 = MyTarget.whoAmI;
                        firer.damage = PrismaticBomb_Damage;
                        firer.knockback = 1;
                        firer.position += new Vector2(0, -252);
                        firer.New();
                    }
                    NPC.velocity = Vector2.Zero;
                    TryLookAtBomb();
                    if(Timer >= PrismaticBomb_AttackTime)
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

using Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;

public partial class Niivi
{
    float StarBomb_ChargeTime => 300;
    int StarBomb_Damage => 55;
    float StarBombPrepTime => 60;
    void AI_StarBomb()
    {
        ref Vector2 StartPosition = ref _vec1;
        ref Vector2 EndPosition = ref _vec2;
        ref Vector2 InitialVelocity = ref _vec3;

        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        StartPosition = NPC.Center;
                        EndPosition = MyTarget.Center + new Vector2(0, -128);
                        InitialVelocity = NPC.velocity;
                    }

                    var ratio = Timer / StarBombPrepTime;
                    var posToMoveTo = Vector2.Lerp(StartPosition, EndPosition, ratio);
                    NPC.Center = posToMoveTo;
                    NPC.velocity = Vector2.Lerp(InitialVelocity, Vector2.Zero, ratio);
                    if (Timer >= StarBombPrepTime)
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
                        if (MultiplayerHelper.IsHost)
                        {
                            var firer = ProjFirer.From<NiiviStarBomb>(NPC);
                            firer.damage = StarBomb_Damage;
                            firer.position = NPC.Center + new Vector2(0, -128);
                            firer.ai2 = 30;
                            firer.New();
                        }
                    }
                    if (Timer >= StarBomb_ChargeTime)
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

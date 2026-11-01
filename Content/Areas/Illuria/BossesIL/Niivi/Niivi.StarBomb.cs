using Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;
using Terraria;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;

public partial class Niivi
{
    float StarBomb_FlyDistance => 353 * ExtraMath.Osc(0.7f, 1f, speed: 6);
    float StarBomb_ChargeTime => 500;
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
                        _ikTarget = Segments[Segments.Length - 1].position;
                        StartPosition = NPC.Center;
                        EndPosition = NPC.Center + new Vector2(0, -128);
                        if (MultiplayerHelper.IsHost)
                        {
                            var firer = ProjFirer.From<NiiviStarBomb>(NPC);
                            firer.damage = StarBomb_Damage;
                            firer.position = EndPosition;
                            firer.ai2 = 24;
                            firer.New();
                        }
                    }

                    Vector2 GetFlyPosition(Vector2 endPosition, float radians)
                    {
                        var posToFlyTo = endPosition + (-Vector2.UnitY * StarBomb_FlyDistance).RotatedBy(radians);
                        return posToFlyTo;
                    }

                    float t = Timer * 0.04F;
                    NPC.Center = Vector2.Lerp(StartPosition, GetFlyPosition(EndPosition,t), EasingFunction.InOutSine(Timer / 120F));
             

                    NPC.velocity = Vector2.Zero;
                    NPC.rotation = (NPC.Center - EndPosition).ToRotation() + MathHelper.PiOver2;

                    var newTarget = Vector2.Lerp(GetFlyPosition(EndPosition, t - 1.45F), EndPosition, 0.1F);
         
                    _ikTarget = Vector2.Lerp(_ikTarget, newTarget, 0.4f * EasingFunction.InOutSine(Timer / 120F));
                    //_ikInitialDirection = -NPC.rotation.ToRotationVector2();
                    if (Timer >= StarBomb_ChargeTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    SwitchState(AIState.Idle);
                }
                break;
        }
    }
}

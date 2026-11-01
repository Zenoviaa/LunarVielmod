using Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;
using System;
using Terraria;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;

public partial class Niivi
{
    float StarBomb_BigFlyDistance => 512 * ExtraMath.Osc(0.7f, 1f, speed: 3);
    float StarBomb_FlyDistance => 353 * ExtraMath.Osc(0.7f, 1f, speed: 6);
    float StarBomb_ChargeTime => 500;
    float StarBomb_WaitTime => 750;
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
                        InitialVelocity = NPC.velocity;
                    }

                    Vector2 GetFlyPosition(Vector2 endPosition, float radians)
                    {
                        var posToFlyTo = endPosition + (-Vector2.UnitY * StarBomb_FlyDistance).RotatedBy(radians);
                        return posToFlyTo;
                    }

                    float t = Timer * 0.052F;
                    var targetPos = Vector2.Lerp(StartPosition, GetFlyPosition(EndPosition, t), EasingFunction.InOutSine(Timer / 170));
                    var targetVel = (targetPos - NPC.Center);

                    NPC.velocity = Vector2.Lerp(InitialVelocity, targetVel, EasingFunction.InOutSine(Timer / 170));
                    NPC.rotation = (NPC.Center - EndPosition).ToRotation() + MathHelper.PiOver2;

                    var newTarget = Vector2.Lerp(GetFlyPosition(EndPosition, t - 1.45F), EndPosition, 0.1F);
         
                    _ikTarget = Vector2.Lerp(_ikTarget, newTarget, 0.4f * EasingFunction.InOutSine(Timer / 170));
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
                    if(Timer == 1)
                    {
                        StartPosition = NPC.Center;
                        EndPosition = new Vector2(StartPosition.X, _arenaY + 160);
                        InitialVelocity = NPC.velocity;
                    }

                    //Ok
                    //so the way we're gonna do this boss is by making TONS OF HECKING CURVES
                    //Every motion will be preset just with calcualted points
                    //all the ik points will be calculated too except for a few
                    //guh
                    Vector2 ArcRightFallDownLeft(float t, Vector2 origin)
                    {
                        var width = 474;
                        var height = 1222;
                        var leftToRight = Vector2.Lerp(origin, origin + new Vector2(width, 0), t / 0.4f);
                        var bottomToTop = Vector2.Lerp(origin + new Vector2(width, 0), origin + new Vector2(0, -height), t / 0.4f);
                        var topToBottomLeft = Vector2.Lerp(origin + new Vector2(0, -height), origin + new Vector2(-512, 2000), t / 1f);

                        var lerp1 = Vector2.Lerp(leftToRight, bottomToTop, t / 0.4f);
                        var lerp2 = Vector2.Lerp(bottomToTop, topToBottomLeft, t);
                        var lerp3 = Vector2.Lerp(lerp1, lerp2, t);
                        return lerp3;
                    }


                    var t = Timer / 240F;
                    t = EasingFunction.OutSine(t);
                    var posToMoveTo = ArcRightFallDownLeft(t, StartPosition);
                    var easedVelocity = Vector2.Lerp(InitialVelocity, (posToMoveTo - NPC.Center), 1f);
                    NPC.velocity = easedVelocity;
                    var targetRotation = easedVelocity.ToRotation();
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, targetRotation, 0.04F);

                    var dist = Vector2.Distance(_ikTarget, NPC.Center);

                    var rigDist = FabrikSolver.RigLength(Segments);
                    var iterations = 0;
                    while(dist > rigDist)
                    {
                        iterations++;
                        if (iterations >= 128)
                            break;
                        var dir = (NPC.Center - _ikTarget).SafeNormalize(Vector2.Zero);
                        _ikTarget += dir;
                        dist = Vector2.Distance(_ikTarget, NPC.Center);
                    }
                    
                    if (Timer >= StarBomb_WaitTime)
                    {
                        Timer = 0;
                        AttackCycle ++;
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

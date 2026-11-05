using Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;
using System;
using Terraria;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;

public partial class Niivi
{
    bool _didBreath;
    int FrostBreath_Damage => 90;
    float FrostBreath_LookTime => 70;
    float FrostBreath_ChargeTime => 120;
    float FrostBreath_SwingTime => 70;
    float FrostBreath_OutTime => 60;
    void AI_FrostBreath()
    {
        ref var StartPosition = ref _vec1;
        ref var Angles = ref _vec2;
        ref var Forward = ref _vec3;
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        var xDir = (MyTarget.Center.X < NPC.Center.X) ? 1 : -1;
                        NPC.velocity.X = xDir * 3;
                    }

                    //Slo wing movement
                    _wingFrameCounter -= 1;

                    //Look at you and then look up
                    var rotationToPlayer = (MyTarget.Center - NPC.Center).ToRotation();
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, rotationToPlayer, 0.1f);
                    NPC.velocity.X *= 0.96F;
                    NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, MathF.Sin(Timer * 0.04F) * 0.3F, 0.1F);
                    _outliner.warning = true;
                    _useFK = true;
                    FK_ArcBody(-0.2f, 0.1f);

                    if (Timer >= FrostBreath_LookTime)
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
                        Forward = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    }

                    var ratio = Timer / FrostBreath_SwingTime;

                    if (ratio >= 0.1f && !_didBreath)
                    {
                        _didBreath = true;
                        if (MultiplayerHelper.IsHost)
                        {
                            var firer = ProjFirer.From<NiiviFrostBreath>(NPC);
                            firer.damage = FrostBreath_Damage;
                            firer.ai1 = NPC.whoAmI;
                            firer.ai2 = FrostBreath_SwingTime - 30;
                            firer.New();
                        }

                    }
                    _outliner.attacking = true;

                    NPC.velocity += Forward * 0.3f;
                    NPC.velocity = NPC.velocity.RotatedBy(0.05F);
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1F);
                    _simpleChainRendering = true;
                    LimpIKTarget();
                    if (Timer >= FrostBreath_SwingTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    NPC.velocity *= 0.96F;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1F);
                    _simpleChainRendering = true;
                    LimpIKTarget();
                    if (Timer >= FrostBreath_OutTime)
                    {
                        Timer = 0;
                        AttackCycle++;
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

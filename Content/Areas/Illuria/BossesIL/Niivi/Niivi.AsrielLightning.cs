using Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;
using System;
using Terraria;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi
{
    public partial class Niivi
    {
        float AsrielLightning_OutTime => 60;
        float AsrielLightning_StrikeTime => MathHelper.Lerp(130, 100, AttackCount / AsrielLightning_StrikeCount);
        float AsrielLightning_StrikeCount => 7;
        float AsrielLightning_SwoopOutTime => 60;
        int AsrielLightning_Damage => 67;
        void AI_AsrielLightning()
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
                        _outliner.warning = true;
                        _isBLACK = true;
                        _useFK = true;
                        _crystalRotationRadians += MathHelper.Lerp(0F, 0.05F, Timer / AsrielLightning_OutTime);
                        _crystalCharge = MathHelper.Lerp(0F, 1F, Timer / AsrielLightning_OutTime);
                        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0.2f, 0.1f);
                        NPC.velocity.X -= 0.03F;
                        NPC.velocity.Y -= 0.06F;
                        FK_ArcBody(-0.2f * ExtraMath.Osc(0.5f, 1.2f), 0.1f);
                        if (Timer >= AsrielLightning_OutTime)
                        {
                            Timer = 0;
                            AttackCycle++;
                        }
                    }
                    break;
                case 1:
                    {
                        _outliner.attacking = true;
                        _isBLACK = true;
                        _useFK = true;
                        _crystalRotationRadians += 0.05f;
                        _crystalCharge = 1F;
                        NPC.velocity *= 0.96F;
                        NPC.velocity.Y += MathF.Sin(Timer * 0.04f) * 0.1f;
                        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0.2f, 0.1f);
                        FK_ArcBody(-0.2f * ExtraMath.Osc(0.5f, 1.2f), 0.1f);
                        if (Timer == 1)
                        {
                            if (MultiplayerHelper.IsHost)
                            {
                                var firer = ProjFirer.From<NiiviLightningStrike>(NPC);
                                firer.damage = AsrielLightning_Damage;
                                firer.position = MyTarget.Center;
                                firer.ai2 = AsrielLightning_StrikeTime;
                                firer.New();
                            }
                        }
                        if (Timer >= AsrielLightning_StrikeTime)
                        {
                            Timer = 0;
                            AttackCount++;
                            if (AttackCount >= AsrielLightning_StrikeCount)
                            {
                                AttackCycle++;
                            }
                        }
                    }
                    break;
                case 2:
                    {
                        _isAfterImaging = true;
                        if (Timer < 20)
                        {
                            NPC.velocity *= 0.93f;
                        }
                        var ratio = Timer / AsrielLightning_SwoopOutTime;
                        NPC.velocity.X -= 0.03f;
                        NPC.velocity.Y -= 0.06f;
                        _invisAlpha = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(Timer / AsrielLightning_SwoopOutTime));
                        _useFK = true;
                        FK_ArcBody(-0.2f * ExtraMath.Osc(0.5f, 1.2f), 0.1f);
                        if (Timer >= AsrielLightning_SwoopOutTime)
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
}

using Stellamod.Core.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using static Stellamod.Core.LocalizationReferences.Mods.Stellamod.ArmorReforge.Zemmified;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;

public partial class Niivi
{
    float IdleTime => 240;
    float Idle_SwoopInTime => 140;
    float Idle_SwoopOutTime => 240;
    float Idle_SwoopInStartDistance => 1700;
    float Idle_SwoopDownStartDistance => 70;

    void AI_IdleOut()
    {
        NPC.velocity.X += 0.05f;
        NPC.velocity.Y -= 0.135f;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1F);
        if (Timer >= Idle_SwoopOutTime)
        {
            Timer = 0;
            AttackCycle++;
            //ChooseAttack();
        }
    }
    void AI_Idle()
    {
        ref Vector2 StartPosition = ref _vec1;
        Timer++;

        CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.02f));
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        StartPosition = _arenaCenter;
                        StartPosition.Y = _arenaY;
                        StartPosition.Y -= 256;
                        StartPosition.X -= Idle_SwoopInStartDistance * 1.2f;
                        Teleport(StartPosition);
                    }
                    var ratio = Timer / Idle_SwoopInTime;
                    ratio *= 0.8f;
                    var originPoint = StartPosition;
                    var posToMoveTo = ArcSwoopDown(ratio, originPoint, Idle_SwoopInStartDistance, Idle_SwoopDownStartDistance);
                    var vel = (posToMoveTo - NPC.Center);
                    NPC.velocity = vel;

                    var targetRot = (MyTarget.Center - NPC.Center).ToRotation();
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, targetRot, 0.04f);
              
                    _simpleChainRendering = true;
                    _ikTarget = ArcSwoopDown(ratio - 0.24f, originPoint, Idle_SwoopInStartDistance, Idle_SwoopDownStartDistance);
                    _ikTarget.Y += ExtraMath.Osc(-32, 232);
                    _ikTarget.X += ExtraMath.Osc(0, 187);
                    if (Timer >= Idle_SwoopInTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    _simpleChainRendering = true;
                    if (MathF.Abs(NPC.velocity.X) > 3)
                        NPC.velocity *= 0.99F;
                    NPC.velocity.X *= 0.999f;
                    NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, MathF.Sin(Timer * 0.05f), MathF.Sin(Timer * 0.1f) * 0.5f + 0.5f);
                    LimpIKTarget();
                    var targetRot = (MyTarget.Center - NPC.Center).ToRotation();
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, Utils.AngleLerp(NPC.rotation, targetRot, 0.2f), 0.04f);
                    if(Timer >= IdleTime)
                    {
                        ChooseAttack();
                    }
                }
                break;
        }

    
    }
}

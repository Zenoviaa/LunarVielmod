using Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;
using Stellamod.Core.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;

public partial class Niivi
{
    bool _didBreath;
    float FrostBreath_SwoopTime => 300;
    float FrostBreath_SwoopRadius => 1500;
    float FrostBreath_SwoopDepth => 1500;
    float FrostBreath_SwoopOffY => -1800;
    void AI_FrostBreath()
    {
        ref var StartPosition = ref _vec1;
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();
                        StartPosition = MyTarget.Center;
                      
                        Teleport(StartPosition + new Vector2(-FrostBreath_SwoopRadius, FrostBreath_SwoopOffY));
                        _didBreath = false;
                    }
                   // _simpleChainRendering = true;
                    _outliner.attacking = true;
                    var ratio = Timer / FrostBreath_SwoopTime;
                    if(ratio >= 0.1f && !_didBreath)
                    {
                        _didBreath = true;
                        var firer = ProjFirer.From<NiiviFrostBreath>(NPC);
                        firer.ai1 = NPC.whoAmI;
                        firer.ai2 = FrostBreath_SwoopTime - 30;
                        firer.New();
                    }
                    OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -64);
                    CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.2f));
                    var originPoint = StartPosition + new Vector2(0, FrostBreath_SwoopOffY);
                    originPoint.X -= 400;
                    _simpleChainRendering = true;
                    var posToMoveTo = ArcSwoop(ratio, originPoint, FrostBreath_SwoopDepth, FrostBreath_SwoopRadius);
                    var vel = (posToMoveTo - NPC.Center);
                    NPC.velocity = vel;
                    var targetRot = vel.ToRotation();
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, targetRot, 0.01f);
                    _ikTarget = ArcSwoop(ratio - 0.24f, originPoint, FrostBreath_SwoopDepth, FrostBreath_SwoopRadius);
                    _ikTarget.Y += ExtraMath.Osc(-32, 232);
                    _ikTarget.X += ExtraMath.Osc(0, 187);
                    if (Timer >= FrostBreath_SwoopTime)
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

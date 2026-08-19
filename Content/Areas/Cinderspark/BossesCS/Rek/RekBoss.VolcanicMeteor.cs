using Stellamod.Core.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{

    private float TimeBetweenMeteors => 12;
    private int Volcanic_Meteor_Damage => 60;
    private void AI_VolcanicMeteor()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    _noWorm = true;
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();
                        Teleport(_arenaCenter + new Vector2(0, -999));
                    }
                    OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -252);
                    int i = 0;
                    foreach(var segment in Segments)
                    {
                        if(i < AttackCount)
                        {
                            segment.isBurning = true;
                            segment.deadly = true;
                            segment.position += segment.velocity;
                        }
                        i++;
                    }
                    if (Timer % TimeBetweenMeteors == 0)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            ProjFirer firer = ProjFirer.From<VolcanicMeteor>(NPC);
                            firer.position = _arenaCenter;
                            firer.position.Y -= 1024;
                            firer.position.X += Main.rand.NextFloat(-1400, 1400);
                            firer.damage = Volcanic_Meteor_Damage;
                            firer.velocity = Vector2.UnitY * 18;
                            firer.velocity = firer.velocity.RotatedByRandom(0.3f);
                            firer.ai0 = NPC.whoAmI;
                            firer.ai1 = AttackCount;
                            firer.ai2 = LavaSurface();
                            firer.New();
                        }
  
                        AttackCount++;
                    }

                    _outliner.attacking = true;
                    float totalLength = Segments.Length * TimeBetweenMeteors;
                    if(AttackCount >= Segments.Length)
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

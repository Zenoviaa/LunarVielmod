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

    private void SegmentsMeteorFloat()
    {
        int i = 0;
        float surface = LavaSurface();
        foreach (var segment in Segments)
        {
            if (i < AttackCount)
            {
                segment.isBurning = true;
                segment.deadly = true;
                segment.position += segment.velocity;
                Point segmentTile = segment.position.ToTileCoordinates();
                if (segment.position.Y >= surface + 128)
                {
                    if (segment.velocity.Y > 1)
                        segment.velocity *= 0.88f;
                    segment.velocity.Y -= 0.15f;
                }
                else
                {
                    segment.velocity.Y *= 0.96f;
                    segment.velocity.Y += 0.02f;
                }
            }
            i++;
        }
    }
    private void SegmentsMeteorFloatAlways()
    {
        float surface = LavaSurface();
        foreach (var segment in Segments)
        {
            segment.isBurning = true;
            segment.deadly = true;
            segment.position += segment.velocity;
            Point segmentTile = segment.position.ToTileCoordinates();
            if (segment.position.Y >= surface + 128)
            {
                if (segment.velocity.Y > 1)
                    segment.velocity *= 0.88f;
                segment.velocity.Y -= 0.15f;
            }
            else
            {
                segment.velocity.Y *= 0.96f;
                segment.velocity.Y += 0.02f;
            }
        }
    }
    private void AI_VolcanicMeteor()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    _outliner.warning = true;
                    int i = 0;
                    //All parts should glow and float up
                    foreach (var segment in Segments)
                    {
                        float time = Timer - i * 3;
                        float ratio = EasingFunction.InOutExpo(time / 10f);
                        if (segment.velocity.Y > 1)
                            segment.velocity.Y *= 0.95f;
                       segment.velocity.Y -= ratio * 0.58f;
                        segment.rotation += 0.05f * ratio;
                        if (time > 0)
                            segment.isBurning = true;
                        i++;
                    }
                    if (NPC.velocity.Y > 1)
                        NPC.velocity.Y *= 0.94f;
                    NPC.velocity.Y -= 0.15f;
                    NPC.rotation -= 0.05f;
                    AllNoWorm();
                    foreach (var segment in Segments)
                    {
                        segment.position += segment.velocity;
                    }
                    if (Timer >= 180)
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
                        NPC.TargetClosest();
                        Teleport(_arenaCenter + new Vector2(0, -999));
                    }


                    int i = 0;
                    //All parts should glow and float up
                    foreach (var segment in Segments)
                    {
                        if(AttackCount < i)
                        {
                            float ratio = 1f;
                            if (segment.velocity.Y > 1)
                                segment.velocity.Y *= 0.95f;
                            segment.velocity.Y -= ratio * 0.58f;
                            segment.rotation += 0.05f * ratio;
                            segment.isBurning = true;
                            segment.deadly = true;
                            segment.position += segment.velocity;
                        }
       
                        i++;
                    }

                    OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -252);
                    SegmentsMeteorFloat();
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
            case 2:
                {

                    SwitchState(AIState.Pacman);
                }
                break;
        }
    }
}

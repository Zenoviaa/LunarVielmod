using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{
    private float _segmentTimer;
    private int _segmentToEat;
    private int Pac_Boom_Damage => 50;
    private float Pac_Time_Between_Points_Spawning => 15;
    private void AI_Pacman()
    {
        void PlacePacPoint(Vector2 position)
        {
            if (AttackCount >= Segments.Length)
                return;

            if (MultiplayerHelper.IsHost)
            {
                var seg = Segments[(int)AttackCount];
                ProjFirer firer = ProjFirer.From<PacmanSegment>(NPC);
                firer.damage = Pac_Boom_Damage;
                firer.position = seg.position;
                firer.velocity = position - seg.position;
                firer.ai0 = NPC.whoAmI;
                firer.ai1 = AttackCount;
                firer.New();
            }
            AttackCount++;
        }
        bool ShouldStopEating()
        {
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type == ModContent.ProjectileType<PacmanSegment>())
                    return false;
            }
            return true;
        }

        bool IsFull()
        {
          
            foreach(var seg in Segments)
            {
                if (seg.noWorm)
                    return false;
            }
            return true;
        }
        
        PacmanSegment GetNextSegmentToEat()
        {
            PacmanSegment eat = null;
            float lowestIndex = 999;
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type == ModContent.ProjectileType<PacmanSegment>() && proj.ai[0] == NPC.whoAmI && proj.ai[1] < lowestIndex)
                {
                    eat = proj.ModProjectile as PacmanSegment;
                    lowestIndex = proj.ai[1];
                }
            }
            return eat;
        }


        Timer++;
        _segmentTimer++;
        if(_segmentTimer % 40 == 0)
        {
            PlacePacPoint(MyTarget.Center + Main.rand.NextVector2CircularEdge(16, 16));
        }
        switch (AttackCycle)
        {
            case 0:
                {

                    _segmentToEat = 0;
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        AllNoWorm();
                    }
                    _segmentTimer = 0;
                    //Prepare the points
                    //1st point is somewhere around the head, floating above the lava
                    //2nd point is more or less the same but slightly towards you
                    //3rd point is on top of you
                    //4th point is randomly placed around you
                    Timer = 0;
                    AttackCycle++;
                    Animator.PlayAnimation(ANIM_IDLE);
                }
                break;
            case 1:
                {
                    if(Timer >= 40)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    if (Timer == 1)
                    {
          
                    }

            
                    _outliner.attacking = true;
                    Animator.PlayAnimation(ANIM_MOUTH_BITE, AnimationParams.Default with { IsLooping = true });
                    Animator.Update();
                    float speed = 12;
                    var seg = GetNextSegmentToEat();
                    
                    if(seg != null)
                    {
                        Segments[(int)seg.Projectile.ai[1]].isBurning=true;
                        Vector2 vel = seg.Projectile.Center - NPC.Center;
                        vel = vel.SafeNormalize(Vector2.Zero);
                        vel *= speed;

                        NPC.velocity = Vector2.Lerp(NPC.velocity, vel, 0.05f *  EasingFunction.InOutSine(Timer / 30f));
                        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.15f);
                        float dist = Vector2.Distance(NPC.Center, seg.Projectile.Center);
                        if (dist < 100)
                        {
                            Timer = 0;
                            if (MultiplayerHelper.IsHost)
                            {
                                var firer = ProjFirer.From<MeteorBoom>(NPC);
                                firer.velocity = NPC.velocity * 1024;
                                firer.position = NPC.Center;
                                firer.damage = Pac_Boom_Damage;
                                firer.New();
                            }
 
                            seg.Projectile.ai[2] = 1;
                            seg.Projectile.Kill();     
                        }
                    }
                    else
                    {
                        Timer = 0;
                        if (IsFull())
                        {
                            AttackCycle++;
                        }
                        else if (ShouldStopEating())
                        {
                            AttackCycle = 2;
                        }
                    }
              
                }
                break;
            case 3:
                {
                    SwitchState(AIState.FireBreath);
                }
                break;
        }
    }
}

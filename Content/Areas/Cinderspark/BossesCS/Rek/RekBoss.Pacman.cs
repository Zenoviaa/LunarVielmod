using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;
using Stellamod.Core.Camera;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{
    private float _ouroborosIndex;
    private float _eatProgress;
    private float _hitstopTimer;
    private float _segmentTimer;
    private int _segmentToEat;
    private int Pac_Boom_Damage => 50;
    private float Pac_Time_Between_Points_Spawning => 15;
    private float Pac_Dash_Time => 160f;
    private float Pac_Delay_Time => 90;
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

        Vector2 GetPointOnPath(float ratio)
        {
            Vector2 circlePoint = VectorHelper.PointOnCircle(_arenaCenter,
                xRadius: 800,
                yRadius: 192,
                startRadians: 0,
                endRadians: MathHelper.ToRadians(310), ratio);
            circlePoint = circlePoint.RotatedBy(_ouroborosIndex * MathHelper.ToRadians(60), _arenaCenter);
            return circlePoint;
        }

        Timer++;
        _segmentTimer++;
        int numWaves = 3;
        int segmentsPerWave = Segments.Length / numWaves;
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
                    NPC.velocity *= 0.98f;
                    NPC.velocity = NPC.velocity.RotatedBy(0.05f);
                    //Prepare the points
                    SegmentsMeteorFloat();
                    if(Timer >= 30)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if(Timer < segmentsPerWave)
                    {
                        float ratio = Timer / segmentsPerWave;
                        PlacePacPoint(GetPointOnPath(ratio));
                    }

                    _eatProgress = 0f;
                    if(Timer >= Pac_Delay_Time)
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

                    _hitstopTimer++;
                    _outliner.attacking = true;
                    _showAfterImages = true;
                    Animator.PlayAnimation(ANIM_MOUTH_BITE, AnimationParams.Default with { IsLooping = true });
                    Animator.Update();
                    CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.1f));
                    var seg = GetNextSegmentToEat();
                    if(seg != null)
                    {
                        float travelSpeed = MathHelper.Lerp(0f, 45f, EasingFunction.InOutExpo(Timer / 120f));
                        travelSpeed += MathHelper.Lerp(-21, 3, EasingFunction.InOutExpo(_hitstopTimer / 30f));
                        Vector2 velToTarget = (seg.Projectile.Center - NPC.Center);
                        velToTarget = velToTarget.SafeNormalize(Vector2.Zero);
                        Vector2 travelVelocity = velToTarget * travelSpeed;
                        float distToTarget = Vector2.Distance(seg.Projectile.Center, NPC.Center);
                        if(distToTarget < travelSpeed)
                        {
                            travelVelocity = velToTarget * distToTarget;
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
                            _hitstopTimer = 0;
                            if (IsFull())
                            {
                                AttackCycle++;
                            }
                            else if(ShouldStopEating())
                            {
                                _ouroborosIndex++;
                                Timer = 0;
                                AttackCycle = 1;
                            }
                        }
                        NPC.velocity = travelVelocity;
                        NPC.rotation = Utils.AngleLerp(NPC.rotation, travelVelocity.ToRotation(), 0.15f);
      
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

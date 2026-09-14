using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR
{
    private Vector2 _kickPunchDirection;
    private float KICK_WARNING_TIME => 70;
    private float KICK_DOWN_TIME => 30;
    private float KICK_PUNCH_DELAY_TIME => 40;
    private float KICK_PUNCH_PREP_TIME => 40;
    private float KICK_PUNCH_TIME => 30;
    private float KICK_PUNCH_END_TIME => 30;
    private int KICK_BOULDER_DAMAGE => 35;


    /*
     * 
     * Kicks the floor and makes three boulders fall from the ceiling,
     * he then punches each of them one at a time, 
     * he can either launch it at mach speed, make it spin and bounce, 
     * or shatter it into multiple smaller boulders, each requiring a different method to dodge
     */
    private void AI_BoulderKick()
    {
        NPC.velocity.X *= 0.94f;
        NPC.noGravity = false;
        NPC.noTileCollide = false;

        Timer++;
        //Gotta make sure he's always facing the boulders, so he can't swap the way he's punching when he does this attack
        if(AttackCycle < 2)
        {
            FaceTarget();
            _kickPunchDirection = Vector2.UnitX * NPC.spriteDirection;
        }
        switch (AttackCycle)
        {
            case 0:
                {
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();
                        GruntSound();
                    }
                    this.AseAnimator.PlayAnimation(ANIM_KICK_UP, AnimationParams.NoLooping);
                    _outliner.warning = true;
                    if (Timer >= KICK_WARNING_TIME)
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
                        AirSwooshSound();
                    }
                    _outliner.attacking = true;
                    this.AseAnimator.PlayAnimation(ANIM_KICK_DOWN, AnimationParams.NoLooping);
                    if (Timer >= KICK_DOWN_TIME)
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
                        KickImpactVFX(NPC.Center, -Vector2.UnitY);
                        if (MultiplayerHelper.IsHost)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                ProjFirer firer = ProjFirer.From<STARBOULDER>(NPC);
                                firer.damage = KICK_BOULDER_DAMAGE;
                                firer.position = NPC.Bottom + _kickPunchDirection * 16 + -Vector2.UnitY * i * 24;
                                firer.ai0 = NPC.whoAmI;
                                firer.ai1 = i;
                                firer.ai2 = -10;
                                firer.New();
                            }
                        }
                    }

                    Timer = 0;
                    AttackCycle++;
                }
                break;
            case 3:
                {
                    this.AseAnimator.PlayAnimation(ANIM_IDLE, AnimationParams.Default);
                    if (Timer >= KICK_PUNCH_DELAY_TIME)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
      
                }
                break;
            case 4:
                {
                    _outliner.warning = true;
                    this.AseAnimator.PlayAnimation(ANIM_PUNCH_READY, AnimationParams.Default);
                    if(Timer >= KICK_PUNCH_PREP_TIME && HasAnotherBoulder())
                    {
                        Timer = 0;
                        AttackCycle++;
                    } 
                    else if (Timer >= KICK_PUNCH_PREP_TIME)
                    {
                        Timer = 0;
                        AttackCycle += 2;
                    }
                }
                break;
            case 5:
                {
                    if(Timer == 1)
                    {
                        AirSwooshSound();
                    }
                    _outliner.attacking = true;
                    this.AseAnimator.PlayAnimation(ANIM_PUNCH, AnimationParams.NoLooping);
                    if(Timer == 10)
                    {
                        PunchVFX(NPC.Right, Vector2.UnitX * NPC.spriteDirection * 15);
                    }

                    if(Timer >= KICK_PUNCH_TIME)
                    {
                        Timer = 0;
                        AttackCycle--;
                    }
                }
                break;
            case 6:
                {
                    this.AseAnimator.PlayAnimation(ANIM_IDLE, AnimationParams.Default);
                    if (Timer >= KICK_PUNCH_END_TIME)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }
}

using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR
{
    private Vector2 _kickPunchDirection;
    private float KICK_WARNING_TIME => 55;
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
                    if (Timer == 15)
                    {
                        int rockBuffType = ModContent.BuffType<Grounded>();
                        KickImpactVFX(NPC.Bottom + _kickPunchDirection * 36, -Vector2.UnitY * 15);
                        EarthQuakeVFX(NPC.Bottom, -Vector2.UnitY * 15);
                        foreach (var player in Main.ActivePlayers)
                        {
                            player.AddBuff(rockBuffType, 1200);
                        }

                        if (MultiplayerHelper.IsHost)
                        {
                            for (int i = 0; i < 14; i++)
                            {
                                ProjFirer firer = ProjFirer.From<STARBOULDER>(NPC);
                                firer.damage = KICK_BOULDER_DAMAGE;
                                firer.position = NPC.Bottom + _kickPunchDirection * 48 + -Vector2.UnitY * 1 * 36 + -Vector2.UnitY * 18;
                                firer.velocity = _kickPunchDirection * 15;
                                firer.ai0 = Main.rand.Next(3);
                                if (firer.ai0 == 1)
                                    firer.damage /= 2;
                                firer.ai1 = i;
                                firer.ai2 = -10;
                                firer.New();
                            }
                        }
                    }

                    if (Timer >= KICK_DOWN_TIME)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {


                    Timer = 0;
                    AttackCycle++;
                }
                break;
            case 3:
                {
                    this.AseAnimator.PlayAnimation(ANIM_KICK_DOWN, AnimationParams.NoLooping);
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
                    this.AseAnimator.PlayAnimation(ANIM_PUNCH_READY, AnimationParams.NoLooping);
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
             
                    if(Timer < 15)
                    {
                        this.AseAnimator.PlayAnimation(ANIM_PUNCH, AnimationParams.NoLooping);
                    }
                    else
                    {
                        this.AseAnimator.PlayAnimation(ANIM_PUNCH_BACKGROUND, AnimationParams.NoLooping);
                    }
                    if(Timer == 7)
                    {
                        PunchVFX(NPC.Right, Vector2.UnitX * NPC.spriteDirection * 15);
                        PunchBoulder((int)AttackCounter, Vector2.UnitX * NPC.spriteDirection * 15);
                        AttackCounter++;
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

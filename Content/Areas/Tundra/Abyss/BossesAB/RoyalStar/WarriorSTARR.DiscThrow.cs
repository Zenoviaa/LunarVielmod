using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;
using Stellamod.Content.Dusts;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR
{
    private int Disc_Throw_Damage => 30;
    private float Disc_Throw_Count => 3;
    private float Disc_Spread => MathHelper.ToRadians(70);
    private void AI_DiscThrow()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        NPC.velocity.Y = -12;
                        float xDirection = NPC.XDirectionToTarget;
                        NPC.velocity.X = -xDirection * 16;


                        SoundStyle bellHit = AssetRegistry.Sounds.Magic.AutomationHit1;
                        bellHit.PitchVariance = 0.2f;
                        SoundEngine.PlaySound(bellHit, NPC.position);

                        MakeGoldenDonut(NPC.Bottom, Vector2.UnitY);

                    }
                    _afterImages = true;
                    _outliner.attacking = true;
                    FaceTarget();
                    StarBitDust();
                    NPC.noTileCollide = false;
                    NPC.velocity.X *= 0.98f;
                    NPC.velocity.Y += 0.1f;

                    if(Timer < 30)
                    {
                        this.AseAnimator.PlayAnimation(ANIM_PUNCH_READY, AnimationParams.NoLooping);
                    }
                    else
                    {
                        this.AseAnimator.PlayAnimation(ANIM_DISC_THROW, AnimationParams.NoLooping);
                        if (Timer == 42)
                        {
                            NPC.velocity -= NPC.PlayerTargetDirection * 4;
                            NPC.velocity.Y -= 4;
                            if (MultiplayerHelper.IsHost)
                            {
                                Vector2 throwDirection = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                                for (float f = 0; f < Disc_Throw_Count; f++)
                                {
                                    float radians = MathHelper.Lerp(-Disc_Spread, Disc_Spread, f / Disc_Throw_Count);
                                    Vector2 newDirection = throwDirection.RotatedBy(radians);
                                    ProjFirer discFirer = ProjFirer.From<STARDISC>(NPC);
                                    discFirer.damage = Disc_Throw_Damage;
                                    discFirer.velocity = newDirection * 12;
                                    discFirer.knockback = 1;
                                    discFirer.New();
                                }
                            }
                        }
                    }


 
                    if (IsGrounded() && Timer >= 24)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    FaceTarget();
                    NPC.velocity.X *= 0.96f;
                    SwitchState(AIState.Idle);
                }
                break;
        }
    }
}

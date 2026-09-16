using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;
using Stellamod.Content.Dusts;
using System;
using Terraria;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR
{
    private int Disc_Throw_Damage => 20;
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
                        float xDirection = MathF.Sign(NPC.velocity.X);
                        NPC.velocity.X = -xDirection * 5;
                    }
                    _afterImages = true;
                    _outliner.attacking = true;
                    FaceTarget();
                    StarBitDust();
                    NPC.noTileCollide = false;
                    NPC.velocity.X *= 0.96f;
                    NPC.velocity.Y += 0.2f;
                    this.AseAnimator.PlayAnimation(ANIM_DISC_THROW, AnimationParams.NoLooping);
                    if(Timer == 12)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 throwDirection = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                            for(float f = 0; f < Disc_Throw_Count; f++)
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
                    _bigStarAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(Timer / 72f));
                    if(Timer >= 72)
                    {
                        SwitchState(AIState.WindUpPunch);
                        AttackCounter = 42;
                        Timer = 0;

                    }
                    if (IsGrounded())
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
                    SwitchState(AIState.CapeOut);
                }
                break;
        }
    }
}

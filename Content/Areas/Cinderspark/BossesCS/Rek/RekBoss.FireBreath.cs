using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;
using Terraria;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{
    private float Fire_Breath_Arc_Jump_Count => 3;
    private float Fire_Breath_Arc_Jump_Delay => 24;
    private float Fire_Breath_Arc_Jump_Time => 90;
    private int Fire_Breath_Damage => 40;
    private void AI_FireBreath()
    {
        Timer++;
        float direction = AttackCount % 2 == 0 ? -1 : 1;
        Vector2 eruptionLeft = FindEruptionLeft();
        Vector2 eruptionRight = FindEruptionRight();
        Animator.PlayAnimation(ANIM_IDLE, AnimationParams.Default with { IsLooping = false });

        Vector2 GetArcPoint(float ratio)
        {
            Vector2 start = eruptionLeft;
            Vector2 end = eruptionRight;


            if(AttackCount % 2 == 0)
            {
                start = eruptionRight;
                end = eruptionLeft;
            }
            float y = EasingFunction.QuadraticBump(ratio) * -900;
            Vector2 p = Vector2.Lerp(start, end, ratio);
            p.Y += y;
            return p;
        }
        switch (AttackCycle)
        {
            case 0:
                {
                    //Vector2 moveToPoint = Vector2.Lerp(eruptionLeft, eruptionRight, 0.7f);
                    if (Timer == 1)
                    {
                        Vector2 pointToUse = AttackCount % 2 == 0 ? eruptionLeft : eruptionRight;
                        Teleport(pointToUse);
                    }

                    Vector2 cameraLerp = Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.5f);
                    float ratio = Timer / Fire_Breath_Arc_Jump_Time;
                    Vector2 point = GetArcPoint(ratio);
                    Vector2 nextPoint = GetArcPoint(ratio + 0.1f);

                    _showAfterImages = true;
                    _outliner.attacking = true;
                    foreach (var seg in Segments)
                    {
                        seg.isBurning = true;
                        seg.deadly = true;
                    }

                    if (Timer == 15)
                    {
                        Vector2 vel = (nextPoint - point).SafeNormalize(Vector2.Zero);
                        vel *= 1024;
                        if (MultiplayerHelper.IsHost)
                        {
                            ProjFirer firer = ProjFirer.From<MeteorBoom>(NPC);
                            firer.position = point;
                            firer.velocity = vel;
                            firer.New();

                            for (float f = 0; f < 2; f++)
                            {
                                Vector2 throVelocity = vel.SafeNormalize(Vector2.Zero);
                                throVelocity *= 24;
                                throVelocity = throVelocity.RotatedByRandom(0.05f);

                                ProjFirer ballFirer = ProjFirer.From<BigVulcanFireball>(NPC);
                                ballFirer.velocity = throVelocity;
                                ballFirer.velocity += Main.rand.NextVector2Circular(2, 2);
                                ballFirer.damage = Fire_Breath_Damage;
                                ballFirer.ai1 = Main.rand.NextFloat(0.5f, 1f);
                                ballFirer.New();

                            }

                            for (float f = 0; f < 2; f++)
                            {
                                Vector2 throVelocity = vel.SafeNormalize(Vector2.Zero);
                                throVelocity *= 24;
                                throVelocity = throVelocity.RotatedByRandom(0.2f);

                                float dir = AttackCount % 2 == 0 ? 1 : -1;
                                throVelocity = throVelocity.RotatedBy(0.5f * dir);
                                throVelocity.X *= 0.6f;
                                throVelocity.Y *= 0.8f;
                                ProjFirer ballFirer = ProjFirer.From<BigVulcanFireball>(NPC);
                                ballFirer.velocity = throVelocity;
                                ballFirer.velocity += Main.rand.NextVector2Circular(2, 2);
                                ballFirer.damage = Fire_Breath_Damage;
                                ballFirer.ai1 = Main.rand.NextFloat(0.5f, 1f);
                                ballFirer.New();

                            }
                        }
                    }

                    float rot = (nextPoint - point).ToRotation();
                    float angle = Utils.AngleLerp(NPC.rotation, rot, 0.2f);
                    NPC.velocity = point - NPC.Center;
                    NPC.rotation = angle;
                    if (Timer >= Fire_Breath_Arc_Jump_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                        AttackCount++;
                    }

                }
                break;
            case 1:
                {
                    NPC.velocity.Y += 0.5f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    if (Timer >= Fire_Breath_Arc_Jump_Delay)
                    {
                        Timer = 0;
                        if (AttackCount >= Fire_Breath_Arc_Jump_Count)
                        {
                            AttackCycle++;
                        }
                        else
                        {
                            AttackCycle--;
                        }
                    }
                }
                break;
            case 2:
                {
                    SwitchState(AIState.Ouroboros);
                }
                break;
        }
    }
}

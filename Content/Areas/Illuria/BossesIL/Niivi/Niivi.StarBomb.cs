using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;
using Stellamod.Visual.Particles;
using System;
using Terraria;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;

public partial class Niivi
{
    float StarBomb_BigFlyDistance => 512 * ExtraMath.Osc(0.7f, 1f, speed: 3);
    float StarBomb_FlyDistance => 353 * ExtraMath.Osc(0.7f, 1f, speed: 6);
    float StarBomb_ChargeTime => 120;
    float StarBomb_WaitTime => 750;
    float StarBomb_FlyDownTime => 240;
    int StarBomb_Damage => 55;
    int StarBomb_CometDamage => 85;
    int StarBomb_DashCount => 5;
    float StarBombPrepTime => 60;
    float StarBomb_CometDashTime => 300;

    void AI_StarBomb()
    {
        ref Vector2 StartPosition = ref _vec1;
        ref Vector2 EndPosition = ref _vec2;
        ref Vector2 InitialVelocity = ref _vec3;

        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();
                        StartPosition = MyTarget.Center;
                        InitialVelocity = NPC.velocity;
                        if (MultiplayerHelper.IsHost)
                        {
                            var firer = ProjFirer.From<NiiviStarBomb>(NPC);
                            firer.damage = StarBomb_Damage;
                            firer.position = MyTarget.Center;
                            firer.ai2 = 19;
                            firer.New();
                        }
                    }

                    //create the star bomb and circle around the player
                    _outliner.warning = true;
                    _simpleChainRendering = true;
                    LimpIKTarget();

                    var offset = -Vector2.UnitY * 256;
                    var positionToCircle = StartPosition + offset.RotatedBy(Timer / StarBomb_ChargeTime * 11f);
                    var velToCircle = positionToCircle - NPC.Center;
                    NPC.velocity = Vector2.Lerp(InitialVelocity, velToCircle, EasingFunction.InOutSine(Timer / (StarBomb_ChargeTime * 0.5F)));
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.2f);
                    if (Timer >= StarBomb_ChargeTime)
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
                        StartPosition = NPC.Center;
                        EndPosition = new Vector2(StartPosition.X, _arenaY + 160);
                        InitialVelocity = NPC.velocity;
                    }

                    var t = Timer / StarBomb_FlyDownTime;
                    t = EasingFunction.OutSine(t);
                    var posToMoveTo = ArcRightFallDownLeft(t, StartPosition);
                    var easedVelocity = Vector2.Lerp(InitialVelocity, (posToMoveTo - NPC.Center), 1f);
                    NPC.velocity = easedVelocity;
                    var targetRotation = easedVelocity.ToRotation();
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, targetRotation, 0.04F);

                    _crystalCharge = MathHelper.Lerp(0f, 1f, t);
                    _crystalRotationRadians += MathHelper.Lerp(0, 0.05f, t);
                    LimpIKTarget();
                    if (Timer >= StarBomb_FlyDownTime)
                    {
                        Timer = 3;
                        AttackCycle ++;
                    }
                }
                break;

            case 2:
                {
                    if(Timer == 1)
                    {
                        Teleport(StartPosition);
                        if (MultiplayerHelper.IsHost)
                        {
                            var firer = ProjFirer.From<NiiviCometDash>(NPC);
                            firer.damage = StarBomb_CometDamage;
                            firer.ai0 = NPC.whoAmI;
                            firer.New();
                        }
                
                    }

                    _outliner.attacking = true;
                    _isBLACK = true;
                    _crystalCharge = 1f;
                    _crystalRotationRadians += 0.05f;
                    _simpleChainRendering = true;
                    LimpIKTarget();
                    var s = MyTarget.Center + new Vector2(0, -1444);
                    if (Timer <= StarBomb_CometDashTime * 0.5F && Timer > 4)
                    {
                   
                        StartPosition.Y = s.Y;
                        StartPosition.X = MathHelper.Lerp(StartPosition.X, s.X, 0.4f);
                 
           
                        
                    }
                    StartPosition.X = MathHelper.Lerp(StartPosition.X, s.X, 0.01f);

                    if (Timer >= StarBomb_CometDashTime * 0.5F)
                    {
                        if (Main.rand.NextBool(4))
                        {
                            var pos = StartPosition;
                            pos.X += Main.rand.NextFloat(-24, 24);
                            pos.Y += Main.rand.NextFloat(764, 2048f);
                            var fx = FXUtil.GlowStretch(pos, Vector2.UnitY * 25);
                            fx.VectorScale.X *= 2F;
                            fx.VectorScale.Y *= 0.5F;
                            fx.InnerColor = Color.White;
                            fx.OuterGlowColor = DrawUtilities.Prismatic;
                        }
                        for (var k = 0; k < 6; k++)
                        {
                            var pos = StartPosition;
                            pos.X += Main.rand.NextFloat(-24, 24);
                            pos.Y += Main.rand.NextFloat(764, 2048f);
                            Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
                            {
                                position = pos,
                                velocity = Vector2.UnitY.RotatedByRandom(0.9F) * Main.rand.NextFloat(1, 3),
                                innerColor = Color.White.ToVector4(),
                                outerColor = DrawUtilities.Prismatic.ToVector4(),
                                timeLeft = 15,
                                scale = new Vector2(Main.rand.NextFloat(0.5f, 1f))
                            });
                        }
                    }

                    for (var i = 0; i < 3; i++)
                    {
                        CreateLightningSparks(RandomPositionOnBody);
                    }
                   
                    if (Main.rand.NextBool(8))
                    {
                        var sp = SparkleParticle.Spawn(RandomPositionOnBody, -NPC.velocity.SafeNormalize(Vector2.Zero));
                        sp.innerColor = DrawUtilities.Prismatic;
                        sp.outerColor = Color.Lerp(sp.innerColor, Color.Black, 0.4f);
                        sp.gravity = 0;
                        sp.Scale *= 0.8F;
                    }

                    if (Main.rand.NextBool(2))
                    {
                        Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                        {
                            position = RandomPositionOnBody,
                            velocity = Main.rand.NextVector2Circular(2, 2) + NPC.velocity,
                            innerColor = DrawUtilities.Prismatic.ToVector4(),
                            outerColor = DrawUtilities.Prismatic.ToVector4(),
                            timeLeft = 60,
                            scale = new Vector2(Main.rand.NextFloat(0.6F, 1F))
                        });
                    }

                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 45;
                    NPC.rotation = Angle.Down;
                    if(Timer >= StarBomb_CometDashTime)
                    {
                        Timer = 0;
                        AttackCount++;
                        if (AttackCount >= StarBomb_DashCount)
                        {
                            AttackCycle++;
                        }
          
                    }
                }
                break;

            case 3:
                {
                    if (Timer == 1)
                    {
                        NPC.velocity.X = 0;
                        NPC.velocity.Y = 4;
                        Teleport(MyTarget.Center + new Vector2(0, 128));
                    }

                    _invisAlpha = MathHelper.Lerp(1f, 0f, Timer / AsrielLightning_TeleportInTime);
                    var rotationToPlayer = (MyTarget.Center - NPC.Center).ToRotation();
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, rotationToPlayer, 0.1f);
                    NPC.velocity.Y *= 0.96F;
                    NPC.velocity.X = 0;

                    _useFK = true;
                    FK_ArcBody(-0.2f * ExtraMath.Osc(0.5f, 1.2f), 0.1f);
                    if (Timer >= AsrielLightning_TeleportInTime)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }
}

using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;
using Stellamod.Core.Camera;
using System;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;

public partial class Niivi
{
    float _easeInTimer;
    float _radians;
    Vector2 BlastBreath_SwoopInOffset => new Vector2(-128, -256);
    float BlastBreath_OvalSwoopXRadius => 666;
    float BlastBreath_OvalSwoopYRadius => 128;
    float BlastBreath_OvalMaxRadians => MathHelper.TwoPi * 2;
    float BlastBreath_ChargeTime => 100;
    float BlastBreath_SwoopInTime => 150;
    float BlastBreath_SwoopDepth => 1300;

    float GoInvisibleTime => 120;
    float BlastBreath_SwoopOutRightDistance => 1444;
    float BlastBreath_SwoopOutUpDistance => 256;

    float BlastBreath_ShootTime => 60;
    float BlastBreath_SwoopOutTime => 140;
    float BlastBreath_ShootCount => 3;
    int BlastBreath_Damage => 80;

    void AI_BlastBreath()
    {
        ref var StartPosition = ref _vec1;
        ref var Side = ref _vec2;
        ref var InitialVelocity = ref _vec3;
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    _invisAlpha = MathHelper.Lerp(1f, 0f, EasingFunction.OutSine(Timer / GoInvisibleTime));
                    _simpleChainRendering = true;
                    NPC.velocity.X += 0.05f;
                    NPC.velocity.Y += MathF.Sin(Timer * 0.04f) * 0.032f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    LimpIKTarget();
                    if(Timer >= GoInvisibleTime)
                    {
                        Teleport(MyTarget.Center + new Vector2(-888, -552));
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        StartPosition = NPC.Center;
                        InitialVelocity = NPC.velocity;
                        var sound = AssetReferences.Assets.Sounds.PrimCharge.Asset;
                        SoundEngine.PlaySound(sound);
                    }

                    AmbientSuckingParticles();
                    _outliner.warning = true;
                    _isBLACK = true;
                    var ratio = Timer / (BlastBreath_ShootTime);
                    _crystalCharge = ratio;
                    _easeInTimer++;
                    _radians += 0.0025F;
                    _crystalRotationRadians += MathHelper.Lerp(0f, 0.1f, ratio);
                    OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -64);
                    if(AttackCount == 0)
                    {
                        CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.2f * EasingFunction.InOutSine(ratio)));
                    }
                    else
                    {
                        CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.2f));
                    }
          
                    var origin = StartPosition;
                    FK_ArcBody(-0.2f * ExtraMath.Osc(0.5f, 1.2f), 0.1f);

                    //    ResetSegments();
                    var vel = (NPC.Center - MyTarget.Center).SafeNormalize(Vector2.Zero) * -5;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, vel, 0.03f );
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, (MyTarget.Center - NPC.Center).ToRotation(), 0.42f);
                    _useFK = true;
                    if (Timer >= BlastBreath_ChargeTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    _outliner.attacking = true;
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        StartPosition = NPC.Center;
                        InitialVelocity = NPC.velocity;
                    
                    }
                    _crystalRotationRadians += 0.1f;
                    _isBLACK = true;

                    OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -64);
                    CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.2f));
                    var originPoint = StartPosition;

                    _crystalCharge = 1f;
                    _useFK = true;
                    FK_ArcBody(-0.2f * ExtraMath.Osc(0.5f, 1.2f), 0.1f);
 
                    if(Timer == 1)
                    {
                        var length = (MyTarget.Center - NPC.Center).Length();
                        var cpos = NPC.Center + NPC.rotation.ToRotationVector2() * length;

                        PixelPrimitiveCircleFactory.CreateGenericInBoom(cpos, DrawUtilities.Prismatic, Color.White, 15, 444);
                        var pos = NPC.Center;
                        var dir = NPC.rotation.ToRotationVector2() * 2400;
                        var endPos = CollisionHelper.SolidTileRayCast(pos, dir, 2400);
                        for (var i =0; i < 64; i++)
                        {
                            
                            var randPos = Vector2.Lerp(pos, endPos, Main.rand.NextFloat(0f, 1f));
                            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                            {
                                position = randPos,
                                velocity = NPC.rotation.ToRotationVector2(),
                                innerColor = DrawUtilities.Prismatic.ToVector4(),
                                outerColor = DrawUtilities.Prismatic.ToVector4(),
                                timeLeft = 30,
                                scale = new Vector2(Main.rand.NextFloat(2f)),
                            });
                        }
                    }

                    if (Timer >= BlastBreath_ShootTime)
                    {
                        NPC.velocity -= NPC.rotation.ToRotationVector2() * 17;
                        if (MultiplayerHelper.IsHost)
                        {
                            var firer = ProjFirer.From<NiiviBlastBreath>(NPC);
                            firer.damage = BlastBreath_Damage;
                            var length = (MyTarget.Center - NPC.Center).Length();
                            firer.velocity = NPC.rotation.ToRotationVector2() * length;
                            firer.New();
                        }
                        Timer = 0;
                        AttackCount++;
                        if(AttackCount >= BlastBreath_ShootCount)
                        {
                            AttackCycle++;
                        }
                        else
                        {
                            AttackCycle--;
                        }
                        //AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    _isAfterImaging = true;

                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        StartPosition = NPC.Center;
                    }

                    if(Timer < 20)
                    {
                        NPC.velocity *= 0.93f;
                    }
                    var ratio = Timer / BlastBreath_SwoopOutTime;
                    NPC.velocity.X -= 0.03f;
                    NPC.velocity.Y -= 0.06f;
                    _invisAlpha = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(Timer / BlastBreath_SwoopOutTime));
                    _useFK = true;
                    FK_ArcBody(-0.2f * ExtraMath.Osc(0.5f, 1.2f), 0.1f);
                    if (Timer >= BlastBreath_SwoopOutTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {
                    SwitchState(AIState.Idle);
                }
                break;
        }
    }
}

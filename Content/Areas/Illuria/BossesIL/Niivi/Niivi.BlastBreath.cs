using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;
using Stellamod.Core.Camera;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;

public partial class Niivi
{
    Vector2 BlastBreath_SwoopInOffset => new Vector2(-128, -256);

    float BlastBreath_SwoopInTime => 150;
    float BlastBreath_SwoopDepth => 1300;


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
                    AI_IdleOut();
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        StartPosition = MyTarget.Center;
                        StartPosition.Y = _arenaY;
                        var sound = AssetReferences.Assets.Sounds.PrimCharge.Asset;
                        SoundEngine.PlaySound(sound);
                        if (MultiplayerHelper.IsHost)
                        {
                            Side.X = Main.rand.NextBool(2) ? 1 : 1;
                            var offset = BlastBreath_SwoopInOffset;
                            offset.X *= Side.X;
                            var spotToTeleportTo = StartPosition
                                + new Vector2(-BlastBreath_SwoopOutRightDistance * Side.X, -BlastBreath_SwoopOutUpDistance)
                                + offset;
                          
                            Teleport(spotToTeleportTo);
                            NPC.netUpdate = true;
                        }
                    }

                    AmbientSuckingParticles();
                    _outliner.warning = true;
                    var ratio = Timer / BlastBreath_SwoopInTime;
                    ratio *= 0.62F;
                    OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -64);
                    CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.2f));
                    var originPoint = StartPosition;
                    originPoint += new Vector2(-BlastBreath_SwoopOutRightDistance * Side.X, -BlastBreath_SwoopOutUpDistance);
                    var offset2 = BlastBreath_SwoopInOffset;
                    offset2.X *= Side.X;
                    originPoint += offset2;
                    originPoint.X -= 252;
                    var posToMoveTo = ArcSwoopDown(ratio, originPoint, BlastBreath_SwoopOutRightDistance * Side.X, BlastBreath_SwoopOutUpDistance);
                    var vel = (posToMoveTo - NPC.Center);
                    NPC.velocity = vel;


                    var targetRot = (MyTarget.Center - NPC.Center).ToRotation();
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, targetRot, 0.04f);

                    _isAfterImaging = true;
                    _isBLACK = true;
                    _simpleChainRendering = true;
                    _ikTarget = ArcSwoopDown(ratio - 0.24f, originPoint, BlastBreath_SwoopOutRightDistance * Side.X, BlastBreath_SwoopOutUpDistance);
                    _ikTarget.Y += ExtraMath.Osc(-32, 232);
                    _ikTarget.X += ExtraMath.Osc(0, 187);
                    if (Timer >= BlastBreath_SwoopInTime)
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
                    _isBLACK = true;
                    var ratio = Timer / (BlastBreath_ShootTime);
                    ratio *= 0.18f;

                    OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -64);
                    CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.2f));
                    var originPoint = StartPosition;

                    var posToMoveTo = ArcSwoopUp(ratio, originPoint, BlastBreath_SwoopOutRightDistance * Side.X, -BlastBreath_SwoopOutUpDistance);
                    var vel = (posToMoveTo - NPC.Center);
                    NPC.velocity = Vector2.Lerp(InitialVelocity, vel, EasingFunction.InOutSine(Timer / 15F));

                    _simpleChainRendering = true;
                    _ikTarget = ArcSwoopUp(ratio - 0.24f, originPoint, BlastBreath_SwoopOutRightDistance * Side.X, -BlastBreath_SwoopOutUpDistance);
                    _ikTarget.Y += ExtraMath.Osc(-32, 232);
                    _ikTarget.X += ExtraMath.Osc(0, 187);


                    NPC.velocity *= 0.98F;

                    LimpIKTarget();
                    _simpleChainRendering = true;

                    if(Timer == (int)(BlastBreath_ShootTime - 15))
                    {
                        PixelPrimitiveCircleFactory.CreateGenericInBoom(NPC.Center, DrawUtilities.Prismatic, Color.White, 15, 444);
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

                    if(Timer < BlastBreath_ShootTime)
                    {
                        var targetRot = (MyTarget.Center - NPC.Center).ToRotation();
                        NPC.rotation = Utils.AngleLerp(NPC.rotation, targetRot, 0.04f);
                    }


                    if (Timer >= BlastBreath_ShootTime)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            var firer = ProjFirer.From<NiiviBlastBreath>(NPC);
                            firer.damage = BlastBreath_Damage;
                            firer.velocity = NPC.rotation.ToRotationVector2() * 1800;
                            firer.New();
                        }
                        Timer = 0;
                        AttackCount++;
                        if(AttackCount >= BlastBreath_ShootCount)
                        {
                            AttackCycle++;
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

                    var ratio = Timer / BlastBreath_SwoopOutTime;

                    OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -64);
                    CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.2f));
                    var originPoint = StartPosition;

                    var posToMoveTo = ArcSwoopUp(ratio, originPoint, BlastBreath_SwoopOutRightDistance * Side.X, -BlastBreath_SwoopOutUpDistance);
                    var vel = (posToMoveTo - NPC.Center);
                    NPC.velocity = vel;
                    var targetRot = vel.ToRotation();
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, targetRot, 0.01f);

                    _simpleChainRendering = true;
                    _ikTarget = ArcSwoopUp(ratio - 0.24f, originPoint, BlastBreath_SwoopOutRightDistance * Side.X, -BlastBreath_SwoopOutUpDistance);
                    _ikTarget.Y += ExtraMath.Osc(-32, 232);
                    _ikTarget.X += ExtraMath.Osc(0, 187);
                    if (Timer >= BlastBreath_SwoopOutTime)
                    {
                        Timer = 0;
                        AttackCount++;
                        if (AttackCount >= BlastBreath_ShootCount)
                        {
                            AttackCycle++;
                        }
                        else
                        {
                            AttackCycle = 0;
                        }
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

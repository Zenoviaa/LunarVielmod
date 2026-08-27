using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;
using Stellamod.Core.Camera;
using Stellamod.Core.InverseKinematics;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Animations;
using Terraria.ModLoader;
namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{
    private int Coil_Dash_Damage => 60;
    private float Coil_Coil_Time => MathF.Floor(120 * AttackSpeedMultiplier);
    private float Coil_Dash_Time => 12;
    private void AllNoWorm()
    {
        foreach (var seg in Segments)
            seg.noWorm = true;
    }
    private void ResetWorm()
    {
        foreach (var seg in Segments)
            seg.noWorm = false;
    }
    private float LavaSurface()
    {
        Point t = _arenaCenter.ToTileCoordinates();
        for(int x = 0; x < 100; x++)
        {
            Tile tile = Main.tile[t];
            if (tile.LiquidAmount > 0)
                break;
            t.Y++;
        }

        return t.ToWorldCoordinates().Y;
    }

    private void ResetLavaSegments()
    {
        foreach(var segment in Segments)
        {
            segment.inLava = false;
        }
    }

    private void MakeSegmentsFallIntoLavaAndFloat()
    {
        float surface = LavaSurface();
        foreach (var segment in Segments)
        {
            if (!segment.inLava)
            {
                segment.velocity.X *= 0.98f;
                segment.rotation += MathF.Sign(segment.velocity.X) * 0.05f;
                if (segment.position.Y >= surface)
                {
                    segment.velocity.Y *= 0.92f;
                    if(segment.velocity.Length() < 1)
                    {
                        segment.inLava = true;
                    }
                 
                }
                else
                {
                    segment.velocity.Y += 0.3f;
                }
            }
            else
            {
                segment.velocity.X *= 0.98f;
                Point segmentTile = segment.position.ToTileCoordinates();
                if (segment.position.Y >= surface + 128)
                {
                    segment.velocity.Y -= 0.25f;
                }
                else
                {
                    segment.velocity.Y *= 0.96f;
                    segment.velocity.Y += 0.02f;
                }
                segment.rotation *= 0.98f;
            }
            segment.position += segment.velocity;

        }
    }
    private void AI_CoilDash()
    {
     
        var animator = this.GetAnimator();
        Timer++;

        float GetSide()
        {
            if (_firebreathSide < 0)
                return 0.25f;
            return 0.75f;
        }
        switch (AttackCycle)
        {
            case 0:
                {
                    if(Timer == 1)
                    {
                        _firebreathSide = Main.rand.NextBool(2) ? -1 : 1;
                    }

                    Vector2 eruptionLeft = FindEruptionLeft();
                    Vector2 eruptionRight = FindEruptionRight();

                    ResetLavaSegments();
                    eruptionRight.Y -= 64;
                    eruptionLeft.Y -= 64;
                    //Vector2 moveToPoint = Vector2.Lerp(eruptionLeft, eruptionRight, 0.7f);

                    Vector2 midPoint = Vector2.Lerp(eruptionLeft, eruptionRight, GetSide());

                    float moveTime = 180;
                    float xRadius = 512;
                    float yRadius = 384;
                    float ratio = Timer / moveTime;
                    float ease = EasingFunction.InOutSine(ratio);
                    float x = MathF.Sin(ease * MathHelper.Pi) * xRadius;
                    float y = MathF.Cos(ease * MathHelper.Pi) * yRadius;

                    Vector2 moveToPoint = midPoint + new Vector2(-x, y);
                    Vector2 targetVel = moveToPoint - NPC.Center;
                    NPC.velocity = targetVel;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);


                    if (Timer == 1)
                    {
                        Vector2 ePos = Vector2.Lerp(eruptionLeft, eruptionRight, GetSide());
                        ePos.Y += 666;
                        Teleport(ePos);
                    }

                    if (Timer >= moveTime)
                    {
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
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/HavocCharge"), MyTarget.position);

                        Vector2 directionToPlayer = NPC.Center.DirectionTo(MyTarget.Center);
                        Vector2 targetVelocity = directionToPlayer * 1;
                        NPC.velocity = NPC.rotation.ToRotationVector2() * 2;
                        Vector2 eruptionLeft = FindEruptionLeft();
                        Vector2 eruptionRight = FindEruptionRight();

                        Vector2 pos = Vector2.Lerp(eruptionLeft, eruptionRight, GetSide());
                        pos.Y -= 187;
                        _centerPoint = pos;
                        _initialVelocity = NPC.velocity;
                    }

                    //Glowing white
                    if (Timer < Coil_Coil_Time)
                    {
                        //Lets just make a circle and have it shrink over time, that should look a lot cleaner
                        //Than what we had before

                        if (Timer < 37)
                        {
                            animator.PlayAnimation(ANIM_MOUTHOPEN, AnimationParams.Default with { IsLooping = false });
                        }
                        else if (Timer < 64)
                        {
                            animator.PlayAnimation(ANIM_MOUTH_BIG_OPEN, AnimationParams.Default with { IsLooping = false });
                        }
                        else if (Timer < 100)
                        {
                            animator.PlayAnimation(ANIM_MOUTH_BIG_OPEN_READY, AnimationParams.Default with { IsLooping = false });
                        }
                        else
                        {
                            animator.PlayAnimation(ANIM_MOUTH_BIG_OPEN_HOLD, AnimationParams.Default with { IsLooping = true });
                        }

                        CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, _centerPoint, 0.15f));

                        float ratio = Timer / Coil_Coil_Time;
                        float ease = EasingFunction.InOutSine(ratio);
                        float radius = MathHelper.SmoothStep(392, 192, ease);
                        float x = MathF.Sin(ratio * MathHelper.TwoPi * 2) * radius;
                        float y = MathF.Cos(ratio * MathHelper.TwoPi * 2) * radius;
                        Vector2 pos = _centerPoint + new Vector2(x, y);
                        Vector2 vel = pos - NPC.Center;

                        float ease2 = EasingFunction.InOutQuad(ratio);
                        Vector2 easedVeclotiy = Vector2.Lerp(_initialVelocity, vel, ease2);
                        NPC.velocity = easedVeclotiy;
                        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.8f);


                        int max = (int)(Segments.Length * ease);
                        max = Segments.Length - max;
                        for (int i = Segments.Length - 1; i >= max; i--)
                        {
                            Segments[i].isBurning = true;
                        }

                    }
                    if (Timer > Coil_Coil_Time && Timer < Coil_Coil_Time + 30)
                    {
                        NPC.velocity *= 0.92f;
                        float targetToNRotation = (MyTarget.Center - NPC.Center).ToRotation();
                        NPC.rotation = Utils.AngleLerp(NPC.rotation, targetToNRotation, 0.2f);
                    }

                    if (Timer >= Coil_Coil_Time + 30)
                    {
                        _coilStartPoint = NPC.Center;
                        _targetPoint = MyTarget.Center;
                        _initialVelocity = NPC.velocity;

                        //Turn on the trail and roar!!!
                        //   DrawChargeTrail = true;

                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SNAKEROAR"), MyTarget.position);
                        Timer = 0;
                        AttackCycle++;
                        //  StopSegmentGlow();
                    }
                }
                break;
            case 2:
                {
                    for (int i = Segments.Length - 1; i >= 0; i--)
                    {
                        Segments[i].isBurning = true;
                        Segments[i].deadly = true;
                    }
                    if (Timer == 1)
                    {
                        if (_firebreathSide == -1)
                            _firebreathSide = 1;
                        else
                            _firebreathSide = -1;
                        if (MultiplayerHelper.IsHost)
                        {
                            ProjFirer firer = ProjFirer.From<RekFlameTrail>(NPC);
                            firer.ai0 = NPC.whoAmI;
                            firer.damage = Coil_Dash_Damage;
                            firer.position = NPC.Center;
                            firer.velocity = Vector2.Zero;
                            firer.New();
                        }
                        ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                        screenShaderSystem.TintScreen(Color.Red, 0.3f, 17);
                        FXUtil.ShakeCamera(NPC.Center, 1024, 32);
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(32, 32);
                        Vector2 vel = pos - NPC.Center;
                        DustParticle sp = Particle<DustParticle>.Spawn(pos, vel * Main.rand.NextFloat(0.1f, 0.3f), Scale: Main.rand.NextFloat(0.5f, 3f));
                        sp.gravity = 0f;
                        sp.fast = true;
                        sp.dampening = 0.1f;

                    }

                    _showAfterImages = true;
                    animator.PlayAnimation(ANIM_MOUTH_BITE, AnimationParams.Default with { IsLooping = false });
                    float ratio = Timer / Coil_Dash_Time;
                    float dashOut = EasingFunction.OutExpo(ratio);
                    Vector2 dashVelocity = _targetPoint - _coilStartPoint;
                    dashVelocity = dashVelocity.SafeNormalize(Vector2.Zero);
                    dashVelocity *= 72;
                    NPC.velocity = Vector2.Lerp(_initialVelocity, dashVelocity, dashOut);
                    NPC.rotation = NPC.velocity.ToRotation();
                    _outliner.attacking = true;
                    if (Timer >= Coil_Dash_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    _showAfterImages = true;
                    NPC.velocity.X *= 0.98f;
                    NPC.velocity.Y += MathHelper.Lerp(3f, 1f, Timer / 60f);
                    NPC.rotation = NPC.velocity.ToRotation();
                    float dy = NPC.Center.Y;

                    if(AttackCount >= 2)
                    {
                        if(Timer >= 15)
                        {
                            Timer = 0;
                            AttackCycle = 5;
                        }
                    }
                    else if (dy >= _arenaCenter.Y + 384)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {
                    NPC.velocity.Y *= 0.98f;
                    NPC.rotation = NPC.velocity.ToRotation();
                    if (Timer >= 30)
                    {
                        Timer = 0;
                        AttackCycle = 1;
                        AttackCount++;
                    }
                }
                break;
            case 5:
                {
                    //Explode Magic
                    if(Timer == 1)
                    {
                        AllNoWorm();
                        var sound = new SoundStyle("Stellamod/Assets/Sounds/RekShockwave") with { PitchVariance = 0.3f };
                        SoundEngine.PlaySound(sound, NPC.position);
                        ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                        screenShaderSystem.TintScreen(Color.Red, 0.25f, timer: 30);
                        foreach (var segment in Segments)
                        {
                            //Little boom
                            float boomSize = Main.rand.NextFloat(0.03f, 0.04f);
                            for (float n = 0; n < 2f; n++)
                            {
                                var spawnParams = new DustParticleSpawnParams();
                                spawnParams.innerColor = Color.OrangeRed;
                                spawnParams.outerColor = Color.Red;
                                spawnParams.scaleRange = new Vector2(0.1f, 3f);
                                DustParticle.Spawn(segment.position, Main.rand.NextVector2Circular(4, 4) * Main.rand.NextFloat(0.5f, 1f), spawnParams);
                            }

                            SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(segment.position, -Vector2.UnitY, Color.White, Scale: 1f);
                            sp.initialColor = Color.White * 0.14f;
                            segment.velocity = Main.rand.NextVector2Circular(16, 4);
                            segment.velocity.Y -= 14;
                        }
                    }

                    NPC.velocity.X *= 0.94f;
                    NPC.velocity.Y += 0.4f;
                    NPC.rotation += MathF.Sign(NPC.velocity.X) * 0.05f;
                    MakeSegmentsFallIntoLavaAndFloat();
         
                    if (Timer >= 90)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 6:
                {
                    NextState();
                }
                break;
        }
    }
}

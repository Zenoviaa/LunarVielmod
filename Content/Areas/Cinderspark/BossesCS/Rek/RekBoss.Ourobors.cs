using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;
using Stellamod.Core.Camera;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{
    private float _ouroborosAlpha;
    private bool _ouroborosTrail;
    private float _windUpTimer;
    private Vector2 _ouroborosVelocity;
    private Vector2 _ouroborosOrigin;
    private float _distanceTraveled;
    private float _spinRot;
    private float Ouroboros_Coil_Time => 190;
    private float Ouroboros_Wait_Time => 70;
    private float Ouroboros_Spinup_Time => 120;
    private float Ouroboros_Startup_Time => 150;
    private float Ouroboros_Wall_Slam_Count => 17;
    private void AI_Ouroboros()
    {
        Projectile FindLatchProjectile<T>() where T : ModProjectile
        {
            int t = ModContent.ProjectileType<T>();
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type != t)
                    continue;
                if (proj.ai[0] != NPC.whoAmI)
                    continue;
                return proj;
            }
            return null;
        }

        void SpinAround(Vector2 point, float radians)
        {
            //Create a circle
            int i = 0;
            var orientation = new CircleOrientation(point, spawnEdgeRadius: 200, Segments.Length);
            foreach (PositionVelocity posVel in orientation)
            {
                PositionVelocity next = orientation.Get(i + 1);
                next.position = next.position.RotatedBy(radians, point);

                var segment = Segments[i];
                segment.position = posVel.position.RotatedBy(radians, point);


                float rot = (next.position - segment.position).ToRotation();
                segment.rotation = rot;
                i++;
            }

            PositionVelocity headPos = orientation.Get(0);
            Vector2 pos = headPos.position.RotatedBy(radians, point);
            PositionVelocity nextPos = orientation.Get(-1);
            Vector2 nPos = nextPos.position.RotatedBy(radians, point);
            NPC.Center = Segments[0].position;
            NPC.rotation = (nPos - pos).ToRotation();
            NPC.velocity *= 0f;
        }

        void SlamWall(Vector2 point)
        {
            var iminShock = new SoundStyle("Stellamod/Assets/Sounds/RekShockwave");
            SoundEngine.PlaySound(iminShock, point);
            var fx = FXUtil.GlowCircleBoom(point, Color.White, Color.OrangeRed, Color.Red);
            fx.VectorScale *= 7;
            foreach (PositionVelocity posVel in new RandomCircleOrientation(point, 64, 32))
            {
                Particles.BitDust.Spawn(BitDustFactory.Default with { position = posVel.position, velocity = posVel.velocity * Main.rand.NextFloat(5, 15), timeLeft = 120 });
            }
            foreach (PositionVelocity posVel in new RandomCircleOrientation(point, 450, 24))
            {
                Particles.FaintSmokeDust.Spawn(FaintSmokeDustData.Default with { position = posVel.position, color = Color.White * 0.2f });
            }
            FXUtil.CreateRipple(point);

        }
        _oldOuroborosPos ??= new Vector2[32];
        float rotationSpeed = -0.05f;
        var animator = this.GetAnimator();
        Timer++;

        switch (AttackCycle)
        {
            case 0:
                {
                    Vector2 eruptionLeft = EruptionLeft;
                    Vector2 eruptionRight = EruptionRight;
                    _distanceTraveled = 0;
                    ResetLavaSegments();
                    eruptionRight.Y -= 64;
                    eruptionLeft.Y -= 64;
                    //Vector2 moveToPoint = Vector2.Lerp(eruptionLeft, eruptionRight, 0.7f);

                    Vector2 midPoint = Vector2.Lerp(eruptionLeft, eruptionRight, 0.5f);

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
                        Vector2 ePos = Vector2.Lerp(eruptionLeft, eruptionRight, 0.5f);
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
                    Vector2 eruptionRight = EruptionRight;
                    Vector2 eruptionLeft = EruptionLeft;
                    //SLowly spin in circles and then flash bang to create the circle lmao
                    if (Timer == 1)
                    {
                        var sound = new SoundStyle("Stellamod/Assets/Sounds/RekClappbackStart") with { PitchVariance = 0.3f };
                        SoundEngine.PlaySound(sound, NPC.position);
                        NPC.TargetClosest();
                        _centerPoint = Vector2.Lerp(eruptionLeft, eruptionRight, 0.5f);
                        _centerPoint.Y = _arenaCenter.Y;
                        _initialVelocity = NPC.velocity;
                    }

                    //Glowing white
                    if (Timer < Ouroboros_Coil_Time)
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
                            ShakeScreenPosition.Shake = 4;
                            CreateFlameSuckParticles(_centerPoint);
                            if (Main.rand.NextBool(2))
                            {
                                var fx = FXUtil.GlowCircleBoom(_centerPoint, Color.White, Color.OrangeRed, Color.Red);
                                fx.VectorScale *= Timer / 50f;
                            }
                            animator.PlayAnimation(ANIM_MOUTH_BIG_OPEN_HOLD, AnimationParams.Default with { IsLooping = true });
                        }



                        float ratio = Timer / Ouroboros_Coil_Time;
                        float ease = EasingFunction.InOutSine(ratio);
                        float radius = MathHelper.SmoothStep(392, 192, ease);
                        float x = MathF.Sin(ratio * MathHelper.TwoPi * 4) * radius;
                        float y = MathF.Cos(ratio * MathHelper.TwoPi * 4) * radius;
                        Vector2 pos = _centerPoint + new Vector2(x, y);
                        Vector2 vel = pos - NPC.Center;

                        float ease2 = EasingFunction.InOutExpo(ratio / 0.5f);
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
                    if (Timer >= Ouroboros_Coil_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    Vector2 eruptionRight = FindEruptionRight();
                    Vector2 eruptionLeft = FindEruptionLeft();
                    if (Timer == 1)
                    {
                        var iminShock = new SoundStyle("Stellamod/Assets/Sounds/RekShockwave");
                        SoundEngine.PlaySound(iminShock, NPC.position);
                        var fx = FXUtil.GlowCircleBoom(_centerPoint, Color.White, Color.OrangeRed, Color.Red);
                        fx.VectorScale *= 6f;
                        foreach (PositionVelocity posVel in new RandomCircleOrientation(_centerPoint, 64, 64))
                        {
                            Particles.BitDust.Spawn(BitDustFactory.Default with { position = posVel.position, velocity = posVel.velocity * Main.rand.NextFloat(5, 15), timeLeft = 120 });
                        }
                        foreach (PositionVelocity posVel in new RandomCircleOrientation(_centerPoint, 450, 44))
                        {
                            Particles.FaintSmokeDust.Spawn(FaintSmokeDustData.Default with { position = posVel.position, color = Color.White * 0.2f });
                        }
                        FXUtil.ShakeCamera(NPC.Center, 1024, 16);
                        for (int k = 0; k < 4; k++)
                        {
                            FXUtil.CreateRipple(_centerPoint);
                        }


                        NPC.TargetClosest();
                        AllNoWorm();

                        //Set the new centerp oint
                        _initialVelocity = -Vector2.UnitY * 5;

                    }

                    _centerPoint += _initialVelocity;
                    for (int i = 0; i < Segments.Length; i++)
                    {
                        ref var segment = ref Segments[i];
                        float t = Timer - i * 4;
                        float ratio = t / 25f;
                        float ease = EasingFunction.InOutExpo(ratio);
                        segment.sawBladeAlpha = ease;
                    }

                    animator.PlayAnimation(ANIM_MOUTH_BITE, AnimationParams.Default with { IsLooping = false });
                    _spinRot += rotationSpeed;

                    float surface = LavaSurface();
                    if (_centerPoint.Y + 127 > surface)
                    {
                        if (_initialVelocity.Y > 1)
                            _initialVelocity *= 0.94f;
                        else
                        {
                            _initialVelocity.Y -= 0.03f;
                        }
                    }
                    else
                    {
                        _initialVelocity.Y += 0.5f;
                    }
                    SpinAround(_centerPoint, _spinRot);
                    if (Timer >= Ouroboros_Wait_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    _outliner.warning = true;
                    foreach (var seg in Segments)
                    {
                        seg.isBurning = true;
                    }

                    _spinRot += rotationSpeed + rotationSpeed * 2 * EasingFunction.OutExpo(Timer / Ouroboros_Spinup_Time);
                    _initialVelocity *= 0.98f;
                    _centerPoint += _initialVelocity;
                    SpinAround(_centerPoint, _spinRot);
                    if (Timer >= Ouroboros_Spinup_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {
                    if (Timer == 1)
                    {
                        _initialVelocity = _centerPoint;
                    }
                    var rect = ArenaRectangleUpToLava();
                    rect = rect.CenterPad(-384);
                    rect.Height += 128;
                    Vector2 bottomLeft = rect.BottomLeft();

                    Vector2 currentPoint = VectorHelper.MoveBetweenPointsWrapped(_distanceTraveled, _initialVelocity, bottomLeft);
                    _distanceTraveled += 4 + 28 * EasingFunction.InOutExpo(Timer / 120f);
                    _spinRot += rotationSpeed * 3;
                    SpinAround(currentPoint, _spinRot);
                    if (_distanceTraveled >= Vector2.Distance(_initialVelocity, bottomLeft))
                    {
                        _distanceTraveled = 0;
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 5:
                {

                    var rect = ArenaRectangleUpToLava();
                    rect = rect.CenterPad(-384);
                    rect.Height += 128;
                    _spinRot += rotationSpeed * 3;
                    Vector2 bottomRight = rect.BottomRight();
                    Vector2 bottomLeft = rect.BottomLeft();
                    Vector2 topLeft = rect.TopLeft();
                    Vector2 topRifght = rect.TopRight();

                    Vector2 currentPoint = VectorHelper.MoveBetweenPointsWrapped(_distanceTraveled, bottomLeft, topLeft, topRifght, bottomRight, bottomLeft);
                    _ouroborosOrigin = currentPoint;

                    if (Timer == 1)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            ProjFirer firer = ProjFirer.From<FlameWheel>(NPC);
                            firer.position = currentPoint;
                            firer.ai0 = NPC.whoAmI;
                            firer.New();
                        }

                    }
                    _oldOuroborosPos.PushAndPopOffEnd(currentPoint);
                    Vector2 nextPoint = VectorHelper.MoveBetweenPointsWrapped(_distanceTraveled + 32, bottomLeft, topLeft, topRifght, bottomRight, bottomLeft);


                    _ouroborosTrail = true;
                    Vector2 movementDirection = nextPoint - currentPoint;
                    movementDirection = movementDirection.SafeNormalize(Vector2.Zero);
                    if (movementDirection.X < 0)
                    {
                        if (MultiplayerHelper.IsHost && Timer % 8 == 0)
                        {
                            Vector2 vel = new Vector2(0, -128);
                            vel = vel.RotatedBy(-0.25f);

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
                            ballFirer.position += new Vector2(0, 128);
                            ballFirer.New();
                        }
                    }
                    CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, currentPoint, 0.1f));
                    SpinAround(currentPoint, _spinRot);
                    for (int i = Segments.Length - 1; i >= 0; i--)
                    {
                        Segments[i].isBurning = true;
                        Segments[i].deadly = true;
                    }

                    if (Vector2.Dot(_ouroborosVelocity, movementDirection) < 0.9f && Timer > 15)
                    {

                        SlamWall(currentPoint);
                        AttackCount++;
                        if(AttackCount >= Ouroboros_Wall_Slam_Count)
                        {
                            _centerPoint = currentPoint;
                            _initialVelocity = _arenaCenter - NPC.Center;
                            _initialVelocity = _initialVelocity.SafeNormalize(Vector2.Zero);
                            _initialVelocity *= 12;
                            _initialVelocity.Y -= 24;
                            Timer = 0;
                            AttackCycle++;
                        }
                        if (MultiplayerHelper.IsHost)
                        {
                            ProjFirer firer = ProjFirer.From<RekFlameTrail>(NPC);
                            firer.ai0 = NPC.whoAmI;
                            firer.damage = Coil_Dash_Damage;
                            firer.position = NPC.Center;
                            firer.velocity = Vector2.Zero;
                            firer.New();
                        }
                        _windUpTimer = 0;
                    }

                    var flameWheel = FindLatchProjectile<FlameWheel>();
                    if (flameWheel != null)
                    {
                        flameWheel.Center = currentPoint;
                    }
                    _ouroborosVelocity = movementDirection;
                    _windUpTimer++;

                    float speedUp = MathHelper.SmoothStep(0f, 24, EasingFunction.Clamp(Timer / 300f));
                    _distanceTraveled += 32;
                    _distanceTraveled += ((speedUp) * EasingFunction.InOutBack(_windUpTimer / 35) + 12) * EasingFunction.InOutExpo(Timer / 120f);
                    _spinRot += rotationSpeed * 3f;
                }
                break;
            case 6:
                {
                    SpinAround(_centerPoint, _spinRot);
                    _spinRot += rotationSpeed * 3f;
                    _centerPoint += _initialVelocity;
                    _initialVelocity.X *= 0.97f;
                    _initialVelocity.Y += 0.5f;
                    if(Timer >= 180)
                    {
                        ResetWorm(); 
                        NextState();
                    }
                }
                break;
        }
    }
}

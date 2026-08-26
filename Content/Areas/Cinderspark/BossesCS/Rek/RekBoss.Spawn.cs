using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Common.Particles;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{
    private void AI_Spawn()
    {
        Timer++;

        switch (AttackCycle)
        {
            case 0:
                {

                    Vector2 eruptionLeft = FindEruptionLeft();
                    Vector2 eruptionRight = FindEruptionRight();

                    CameraTargetSystem.AddTarget(NPC.Center);

                    eruptionRight.Y -= 384;
                    eruptionLeft.Y -= 384;
                    //Vector2 moveToPoint = Vector2.Lerp(eruptionLeft, eruptionRight, 0.7f);

                    Vector2 midPoint = Vector2.Lerp(eruptionLeft, eruptionRight, 0.5f);

                    float moveTime = 180;
                    float xRadius = 512;
                    float yRadius = 384;
                    float ratio = Timer / moveTime;
                    float ease = EasingFunction.InOutSine(ratio);
                    float x = MathF.Sin(ease * MathHelper.Pi) * xRadius;
                    float y = MathF.Cos(ease * MathHelper.Pi) * yRadius;

                    Vector2 moveToPoint = midPoint + new Vector2(x, y);
                    Vector2 targetVel = moveToPoint - NPC.Center;
                    NPC.velocity = targetVel;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);

                    if (Timer == 1 && MultiplayerHelper.IsHost && !NPC.AnyNPCs(ModContent.NPCType<BigMoltenPlatform>()))
                    {
                        CreateArena();
                    }
                    if (Timer == 1)
                    {
                        Teleport(eruptionLeft);
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
                    CameraTargetSystem.AddTarget(NPC.Center);

                    float ratio = Timer / 64;
                    float ease = EasingFunction.InOutSine(ratio);
                    int max = (int)(Segments.Length * ease);
                    max = Segments.Length - max;
                    for (int i = Segments.Length - 1; i >= max; i--)
                    {
                        Segments[i].isBurningNoWarning = true;
                    }

                    var animator = this.GetAnimator();
                    if (Timer < 18)
                    {
                        animator.PlayAnimation(ANIM_MOUTHOPEN, AnimationParams.Default with { IsLooping = false });
                    }
                    else if (Timer < 36)
                    {
                        animator.PlayAnimation(ANIM_MOUTH_BIG_OPEN, AnimationParams.Default with { IsLooping = false });
                    }
                    else if (Timer < 64)
                    {
                        animator.PlayAnimation(ANIM_MOUTH_BIG_OPEN_READY, AnimationParams.Default with { IsLooping = false });
                    }
                    else
                    {
                        if(Timer == 65)
                        {
                            ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                            screenShaderSystem.TintScreen(Color.Red, 0.12f, 120);
                        }
                        if (Timer % 10 == 0)
                        {
                            Particles.RoarDust.Spawn(RoarDustData.Default with { position = NPC.Center, timeLeft = 24 });
                        }
                        ShakeScreenPosition.Shake = 8;

                        Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(128, 128);
                        Vector2 vel = pos - NPC.Center;
                        DustParticle sp = Particle<DustParticle>.Spawn(pos, vel * Main.rand.NextFloat(0.1f, 0.3f), Scale: Main.rand.NextFloat(0.5f, 1.5f));
                        sp.gravity = 0f;
                        sp.fast = true;
                        sp.dampening = 0.1f;

                        if (!_roar)
                        {
                            var sound = AssetRegistry.Sounds.Rek.RekBigroar;
                            SoundEngine.PlaySound(sound);

                            var sound2 = new SoundStyle("Stellamod/Assets/Sounds/RekSummon");
                            SoundEngine.PlaySound(sound2);

                            ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                            screenShaderSystem.TintScreen(Color.Red, 0.5f, 50);
                        }
                        _roar = true;
                        animator.PlayAnimation(ANIM_MOUTH_BIG_OPEN_HOLD, AnimationParams.Default with { IsLooping = true });
                    }

                    NPC.velocity *= 0.96f;
                    if (Timer >= 180)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    var animator = this.GetAnimator();
                    animator.PlayAnimation(ANIM_MOUTH_BITE, AnimationParams.Default with { IsLooping = false });
                    NPC.velocity.X += -0.2f;
                    NPC.velocity.Y += 0.5f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    if (Timer >= 90)
                    {
                        NextState();
                    }
                }
                break;
        }


    }
}

using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{
    private void AI_Death()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    _roar = false;

                    _globalAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 60F));
                    Vector2 eruptionLeft = FindEruptionLeft();
                    Vector2 eruptionRight = FindEruptionRight();


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
                    var timeBetween = 7;


                    float ratio = Timer / 64;
                    float ease = EasingFunction.InOutSine(ratio);
                    int max = (int)(Segments.Length * ease);
                    max = Segments.Length - max;
                    for (int i = Segments.Length - 1; i >= max; i--)
                    {
                        Segments[i].isBurningNoWarning = true;
                    }
                //    CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, ease * 0.5F));
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
                        if (Timer % 10 == 0)
                        {
                            PS.RoarDust.Spawn(RoarDustData.Default with { position = NPC.Center });
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

                    //Segments should pop one at a time
                    var localI = 0;
                    for(int i = Segments.Length - 1; i >= 0; i--)
                    {
                        var segment = Segments[i];
                        localI++;
                        if(Timer >= localI * timeBetween)
                        {
                            segment.noWorm = true;
                            if (!segment.killed)
                            {
                                segment.killed = true;
                                segment.velocity = -Vector2.UnitY * 15;
                                segment.velocity.X *= 0.94f;
                                var fx = FXUtil.GlowCircleBoom(segment.position, Color.Yellow, Color.Orange, Color.OrangeRed, 25);
                                fx.Scale *= 2F;
                                for (float f = 0; f < 12; f++)
                                {
                                    Vector2 pos = segment.position;
                                    pos += Main.rand.NextVector2Circular(16, 16);
                                    Color color = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0f, 1f));
                                    PS.SwirlingFlameDust.Spawn(BitDustFactory.Default with
                                    {
                                        position = pos,
                                        velocity = Main.rand.NextVector2Circular(18, 18),
                                        timeLeft = 100,
                                        innerColor = color.ToVector4(),
                                        outerColor = Color.Red.ToVector4(),
                                        scale = new Vector2(Main.rand.NextFloat(1f, 2f))
                                    });
                                }
                                for (int j = 0; j < 7; j++)
                                {
                                    Vector2 pos = segment.position + Main.rand.NextVector2Circular(64, 64);
                                    Vector2 vel = Main.rand.NextVector2Circular(10, 10);
                                    var smokeParitcle = SmokeParticle.SpawnInAlphaLayer(pos, vel);
                                    smokeParitcle.dampening = 0.09f;
                                    smokeParitcle.fadeToColor = Color.Black * 0.5f;
                                    smokeParitcle.initialColor = Color.DarkRed * 0.5f;
                                    smokeParitcle.Scale *= 2f;
                                    smokeParitcle.behindLayer = true;
                                }


                                SoundEngine.PlaySound(SoundID.Item74 with { PitchVariance = 0.6f }, segment.position);
                            }
                        }
  
                    }

                    foreach(var segment in Segments)
                    {
                        segment.position += segment.velocity;
                        segment.velocity.X *= 0.95f;
                        segment.velocity.Y += 0.8f;
                        segment.rotation += 0.1f;
                    }

                    NPC.velocity *= 0.96f;
                    if (Timer >= Segments.Length * timeBetween)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            ProjFirer firer = ProjFirer.From<SpearBoom>(NPC);
                            firer.damage = Volcanic_Spear_Damage;
                            firer.position = NPC.Center;
                            firer.velocity = -Vector2.UnitY * 1024;
                            firer.New();
                        }
                        NPC.Kill();
                    }
                }
                break;
        }
    }
}

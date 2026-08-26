using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;
using Stellamod.Core.Camera;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{
    private int Volcanic_Spear_Damage => 50;
    private float Volcanic_Spear_Come_Up_Time => 70;
    private float Volcanic_Spear_Go_Down_Time => 70;
    private float Volcanic_Spear_Crash_Time => 180;
    private float Volcanic_Spear_End_Time => 90;
    private void AI_VolcanicSpear()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        Vector2 pos = Vector2.Lerp(_eruptionLeft, _eruptionRight, 0.8f);
                        pos.Y -= 100;
                        Teleport(pos);
                        var sound = AssetRegistry.Sounds.Rek.RekIdleroar;
                        SoundEngine.PlaySound(sound, pos);
                    }

                    Vector2 panning = Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.5f);
                    CameraTargetSystem.AddTarget(panning);
                    NPC.velocity.X -= 0.5f;
                    NPC.velocity.Y -= 1f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    if (Timer >= Volcanic_Spear_Come_Up_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    NPC.velocity.X *= 0.96f;
                    if (NPC.velocity.Y < -1)
                        NPC.velocity.Y *= 0.94f;
                    else
                        NPC.velocity.Y += 0.5f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    if (Timer >= Volcanic_Spear_Go_Down_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    if (Timer == 1)
                    {
                        _initialVelocity = NPC.velocity;
                        _coilStartPoint = NPC.Center;
                    }
                    _centerPoint = _destroyArena ? _arenaCenter + new Vector2(0, 128) : MyTarget.Center;
                    _outliner.warning = true;
                    foreach (var segment in Segments)
                    {
                        segment.isBurning = true;
                    }

                    float halfCrashTime = Volcanic_Spear_Crash_Time * 0.5f;
                    float halfHalfCrashTime = halfCrashTime * 0.5f;
                    if (Timer >= halfCrashTime)
                    {
                        _outliner.attacking = true;
                        foreach (var segment in Segments)
                        {
                            segment.isBurning = true;
                            segment.deadly = true;
                        }

                        for (int i = 0; i < Segments.Length; i++)
                        {
                            ref var segment = ref Segments[i];
                            float t = Timer - halfCrashTime;
                            t -= i * 2;
                            float ratio = t / halfHalfCrashTime;
                            float ease = EasingFunction.InOutExpo(ratio);
                            segment.sawBladeAlpha = ease;
                        }
                    }

                    Vector2 panning = Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.5f);
                    CameraTargetSystem.AddTarget(panning);

                    float timePerSpearAlpha = Volcanic_Spear_Crash_Time / 3f;
                    for (int i = 0; i < _spearAlphas.Length; i++)
                    {
                        ref float spearAlpha = ref _spearAlphas[i];
                        spearAlpha = (Timer - i * timePerSpearAlpha) / timePerSpearAlpha;
                        spearAlpha = EasingFunction.InOutExpo7(spearAlpha);
                    }

                    float upEasing = EasingFunction.OutExpo(Timer / Volcanic_Spear_Crash_Time);
                    float downEasing = EasingFunction.InExpo(Timer / Volcanic_Spear_Crash_Time);
                    float midEasing = EasingFunction.InOutSine(Timer / Volcanic_Spear_Crash_Time);
                    float startupEasing = EasingFunction.InOutSine(Timer / 45f);

                    Vector2 upPoint = _coilStartPoint + new Vector2(0, -256);
                    Vector2 upwardLerp = Vector2.Lerp(_coilStartPoint, upPoint, upEasing);
                    Vector2 downwardLerp = Vector2.Lerp(upPoint, _centerPoint, downEasing);
                    Vector2 targetPoint = Vector2.Lerp(upwardLerp, downwardLerp, midEasing);
                    Vector2 movementVelocity = targetPoint - NPC.Center;

                    NPC.velocity = Vector2.Lerp(_initialVelocity, movementVelocity, startupEasing);
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    if (Timer >= Volcanic_Spear_Crash_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    if (Timer == 1)
                    {
                        if (_destroyArena)
                        {
                            //Destroy the main platform
                            foreach(var npc in Main.ActiveNPCs)
                            {
                                if (npc.type == ModContent.NPCType<BigMoltenPlatform>())
                                    npc.Kill();
                            }
                        }
                        //Crash Boom
                        if (MultiplayerHelper.IsHost)
                        {
                            ProjFirer firer = ProjFirer.From<SpearBoom>(NPC);
                            firer.damage = Volcanic_Spear_Damage;
                            firer.position = _targetPoint;
                            firer.New();
                        }
                    }
                    for (int i = 0; i < _spearAlphas.Length; i++)
                    {
                        ref float spearAlpha = ref _spearAlphas[i];
                        spearAlpha = MathHelper.SmoothStep(1f, 0f, Timer / Volcanic_Spear_End_Time);
                    }

                    NPC.velocity.X *= 0.94f;
                    NPC.velocity.Y += 0.5f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    if (Timer >= Volcanic_Spear_End_Time)
                    {
                        NextState();
                    }
                }
                break;
        }
    }

}

using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;
using Stellamod.Core.Camera;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{
    private float _jumpRadians;
    private int Volcanic_Spear_Damage => 50;
    private float Volcanic_Spear_Come_Up_Time => 80;
    private float Volcanic_Spear_Go_Down_Time => 130;
    private float Volcanic_Spear_Crash_Time => 210;
    private float Volcanic_Spear_Stab_Time => 210;
    private float Volcanic_Spear_End_Time => 90;
    private void AI_VolcanicSpear()
    {
        Vector2 eruptionLeft = FindEruptionLeft();
        Vector2 eruptionRight = FindEruptionRight();
        Vector2 GetArcPoint(float ratio)
        {
            Vector2 start = eruptionLeft;
            Vector2 end = eruptionRight;


            if (AttackCount % 2 == 0)
            {
                start = eruptionRight;
                end = eruptionLeft;
            }
            float y = EasingFunction.QuadraticBump(ratio) * -1200;
            Vector2 p = Vector2.Lerp(start, end, ratio);
            p.Y += y;
            return p;
        }
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        Vector2 pos = Vector2.Lerp(FindEruptionLeft(), FindEruptionRight(), 0.8f);
                        pos.Y -= 100;
                        Teleport(pos);
                        var sound = AssetRegistry.Sounds.Rek.RekIdleroar;
                        SoundEngine.PlaySound(sound, pos);
                    }

                    Vector2 panning = Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.25f);
                    CameraTargetSystem.AddTarget(panning);
                    NPC.velocity.X -= 0.11f;
                    NPC.velocity.Y -= 0.15f;
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
                    NPC.velocity.X *= 0.99f;
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
                        Vector2 pos = Vector2.Lerp(FindEruptionLeft(), FindEruptionRight(), 0.8f);
                        pos.Y -= 100;
                        Teleport(pos);
                        _initialVelocity = NPC.velocity;
                        _coilStartPoint = pos;

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

                    float r = Timer / Volcanic_Spear_Crash_Time;
                    float extra = Timer * 0.0005f;
                    Vector2 point = GetArcPoint(EasingFunction.OutExpo(r) * 0.75f + extra);
                    Vector2 nextPoint = GetArcPoint(EasingFunction.OutExpo(r) * 0.75f + 0.1f + extra);
                    float rot = (nextPoint - point).ToRotation();
                    float angle = Utils.AngleLerp(NPC.rotation, rot, 0.2f);
                    NPC.velocity = point - NPC.Center;
                    NPC.rotation = angle;
                    if (Timer >= Volcanic_Spear_Crash_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    if(Timer == 1)
                    {
                        _initialVelocity = NPC.velocity;
                        _coilStartPoint = NPC.Center;

                    }

                    _showAfterImages = true;
                    for (int i = 0; i < Segments.Length; i++)
                    {
                        ref var segment = ref Segments[i];
                        segment.sawBladeAlpha = 1f;
                        segment.deadly = true;
                        segment.isBurning = true;
                    }
                    Vector2 panning = Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.25f);
                    CameraTargetSystem.AddTarget(panning);

                    _outliner.attacking = true;

                    float speed = -0.025f;
                    _coilStartPoint = _coilStartPoint.RotatedBy(speed, _centerPoint);
                    float ratio = Timer / Volcanic_Spear_Stab_Time;
                    float inOut = EasingFunction.OutExpo(ratio);
                    float slowIn = EasingFunction.InExpo(ratio);
                    Vector2 prePoint = _centerPoint + new Vector2(0, 900).RotatedBy(Timer * speed);
                    Vector2 quickOut = Vector2.Lerp(_coilStartPoint, prePoint, inOut);
                    Vector2 backIn = Vector2.Lerp(prePoint, _centerPoint, slowIn);
                    Vector2 targetPos = Vector2.Lerp(quickOut, backIn, slowIn * slowIn * slowIn);
                    Vector2 targetVelocity = targetPos - NPC.Center;
                    NPC.velocity = Vector2.Lerp(_initialVelocity, targetVelocity, EasingFunction.InOutExpo(ratio / 0.75f));
                    
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.4f);
                    if(Timer >= Volcanic_Spear_Stab_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }

                }
                break;
            case 4:
                {
                    if(Timer < 25)
                    {
                        Vector2 panning = Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.5f);
                        CameraTargetSystem.AddTarget(panning);
                    }
    

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
                            firer.position = NPC.Center;
                            firer.velocity = -Vector2.UnitY * 1024;
                            firer.New();
                        }
                    }
                    for (int i = 0; i < _spearAlphas.Length; i++)
                    {
                        ref float spearAlpha = ref _spearAlphas[i];
                        spearAlpha = MathHelper.SmoothStep(1f, 0f, Timer / Volcanic_Spear_End_Time);
                    }

                    NPC.velocity.X -= 2.5f;
                    NPC.velocity.Y += 2.5f;
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

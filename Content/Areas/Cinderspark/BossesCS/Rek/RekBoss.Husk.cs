using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Common.Animations;
using Stellamod.Common.Particles;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{
    private float Husk_End_Time => 170;
    private float Husk_Prep_Time => 90;
    private float Husk_Arch_Time => 120;
    private void AI_Husk()
    {
        bool HasAbaby()
        {
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.type == ModContent.NPCType<RekEye>() && npc.ai[0] == NPC.whoAmI)
                    return true;
            }
            return false;
        }
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        Vector2 midArena = Vector2.Lerp(FindEruptionLeft(), FindEruptionRight(), 0.68f);
                        Teleport(midArena);
                    }

                    Animator.PlayAnimation(ANIM_IDLE);
                    NPC.velocity.X += MathF.Sin(Timer * 0.1f) * 0.2f;
                    NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, -11, 0.1f);
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    if (Timer >= Husk_Prep_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if (Timer < 18)
                    {
                        Animator.PlayAnimation(ANIM_MOUTHOPEN, AnimationParams.Default with { IsLooping = false });
                    }
                    else if (Timer < 36)
                    {
                        Animator.PlayAnimation(ANIM_MOUTH_BIG_OPEN, AnimationParams.Default with { IsLooping = false });
                    }
                    else if (Timer < 64)
                    {
                        Animator.PlayAnimation(ANIM_MOUTH_BIG_OPEN_READY, AnimationParams.Default with { IsLooping = false });
                    }
                    else
                    {
                        ShakeScreenPosition.Shake = 8;
                        Animator.PlayAnimation("EyelessHusk", AnimationParams.Default with { IsLooping = true });
                    }

                    NPC.velocity *= 0.99f;
                    NPC.velocity = NPC.velocity.RotatedBy(0.035f);
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    if (Timer >= Husk_Arch_Time)
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
                        var sound = AssetRegistry.Sounds.Rek.RekBigroar;
                        SoundEngine.PlaySound(sound);
                        //Spawn the husk
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 pos = NPC.Center;
                            pos += NPC.rotation.ToRotationVector2() * 120;
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)pos.X, (int)pos.Y, ModContent.NPCType<RekEye>(), ai0: NPC.whoAmI);
                        }
                    }
                    if(Timer < 60)
                    {
                        if (Timer % 10 == 0)
                        {
                            Particles.RoarDust.Spawn(RoarDustData.Default with { position = NPC.Center, timeLeft = 24 });
                        }
                    }
            
                    _huskAlpha = MathHelper.Lerp(0f, 0.5f, EasingFunction.InOutSine(Timer / 180));
                    Animator.PlayAnimation("EyelessHusk", AnimationParams.Default with { IsLooping = true });
                    NPC.velocity *= 0.97f;
                 //   NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    if (Timer >= 180 && !HasAbaby())
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
                        var sound = AssetRegistry.Sounds.Rek.RekIdleroar;
                        SoundEngine.PlaySound(sound);

                        var sound2 = new SoundStyle("Stellamod/Assets/Sounds/RekSummon");
                        SoundEngine.PlaySound(sound2);

                        ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                        screenShaderSystem.TintScreen(Color.Red, 0.5f, 50);
                    }


                    ShakeScreenPosition.Shake = 8;
                    Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(128, 128);
                    Vector2 vel = pos - NPC.Center;

                    DustParticle sp = Particle<DustParticle>.Spawn(pos, vel * Main.rand.NextFloat(0.1f, 0.3f), Scale: Main.rand.NextFloat(0.5f, 1.5f));
                    sp.gravity = 0f;
                    sp.fast = true;
                    sp.dampening = 0.1f;
                   
                    Animator.PlayAnimation(ANIM_MOUTH_BITE, AnimationParams.Default with { IsLooping = true });
                    _huskAlpha *= 0.94f;
                    NPC.velocity.Y += 0.4f;
                    NPC.velocity.X -= 0.2f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    if (Timer >= Husk_End_Time)
                    {
                        NextState();
                    }
                }
                break;
        }
    }
}

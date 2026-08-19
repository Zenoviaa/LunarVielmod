using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{
    private float Eruption_PrepTime => 90;
    private float Eruption_GraceTime => 40;
    private float Eruption_SinTime => 620;
    private float Eruption_SinHeight => 64;
    private float Eruption_SinFrequency => 0.04f;
    private void AI_Eruption()
    {
        Timer++;
        Vector2 eruptionLeft = FindEruptionLeft();
        Vector2 eruptionRight = FindEruptionRight();
        eruptionLeft.Y -= 384;
        eruptionRight.Y -= 384;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        var sound = new SoundStyle("Stellamod/Assets/Sounds/RekRoar");
                        SoundEngine.PlaySound(sound, NPC.position);
                        ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                        //    screenShaderSystem.DistortScreen(TextureRegistry.NormalNoise1, scrollSpeed: new Vector2(0.0005f), timer: Eruption_SinTime, blend: 0.02f );
                        screenShaderSystem.TintScreen(Color.Red, 0.05f, timer: Eruption_SinTime);
    
                        Teleport(eruptionLeft);
                    }

                    if (Timer >= Eruption_GraceTime)
                    {
                        if (Timer >= Eruption_GraceTime * 5)
                        {
                            this.GetAnimator().PlayAnimation(ANIM_MOUTH_BIG_OPEN_HOLD, AnimationParams.Default with { IsLooping = true });
                        }
                        else if (Timer >= Eruption_GraceTime * 4)
                        {
                            this.GetAnimator().PlayAnimation(ANIM_MOUTH_BIG_OPEN_READY, AnimationParams.Default with { IsLooping = false });
                        }
                        else
                        {
                            this.GetAnimator().PlayAnimation(ANIM_MOUTHOPEN, AnimationParams.Default with { IsLooping = false });
                        }

                        if (Timer % 40 == 0)
                        {
                            if (MultiplayerHelper.IsHost)
                            {
                                //The sound will be on the projectile
                                ProjFirer firer = ProjFirer.From<VulcanEruption>(NPC);
                                int segmentIndex = Main.rand.Next(3, Segments.Length);
                                ref var segment = ref Segments[segmentIndex];
                                firer.position = segment.position;
                                firer.velocity = -Vector2.UnitY * 512;
                                firer.ai0 = NPC.whoAmI;
                                firer.ai1 = segmentIndex;
                                firer.New();
                            }
                        }
                    }

                    Vector2 pointToMoveTo = Vector2.Lerp(eruptionLeft, eruptionRight, Timer / Eruption_SinTime);
                    pointToMoveTo.Y += MathF.Sin(Timer * Eruption_SinFrequency) * Eruption_SinHeight;
                    Vector2 targetVelocity = pointToMoveTo - NPC.Center;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.4f);
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    if (Timer >= Eruption_SinTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    this.GetAnimator().PlayAnimation(ANIM_MOUTH_BITE, AnimationParams.Default with { IsLooping = false });
                    NPC.velocity.X += NPC.direction;
                    NPC.velocity.Y += 0.05f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    if (Timer >= Eruption_PrepTime)
                    {
                        SwitchState(AIState.CoilDash);
                    }
                }
                break;
        }
    }
}

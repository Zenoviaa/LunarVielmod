using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Common.Particles;
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
    private float Eruption_SinTime => 620 * AttackSpeedMultiplier;
    private float Eruption_SinHeight => 64;
    private float Eruption_SinFrequency => 0.04f;

    private int Eruption_Damage => 70;
    private void AI_Eruption()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            AttackCount = Main.rand.NextBool(2) ? -1 : 1;
                        }

                        var sound = new SoundStyle("Stellamod/Assets/Sounds/RekRoar");
                        SoundEngine.PlaySound(sound, NPC.position);
                        ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                        //    screenShaderSystem.DistortScreen(TextureRegistry.NormalNoise1, scrollSpeed: new Vector2(0.0005f), timer: Eruption_SinTime, blend: 0.02f );
                        screenShaderSystem.TintScreen(Color.Red, 0.05f, timer: Eruption_SinTime);

                        var pos = _lavaArenaRectangle.BottomLeft();
                        pos.X -= 384;
                        if(AttackCount == 1)
                        {
                            pos = _lavaArenaRectangle.BottomRight();
                            pos.X += 384;
                        }
                        Teleport(pos);
                    }

                    _globalAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.InSine(Timer / 90F));
                    this.GetAnimator().PlayAnimation(ANIM_IDLE, AnimationParams.Default with { IsLooping = true });

                    if (Timer >= Eruption_GraceTime)
                    {
                        if (Timer % 24 == 0)
                        {
                            if (MultiplayerHelper.IsHost)
                            {
                                //The sound will be on the projectile
                                ProjFirer firer = ProjFirer.From<VulcanEruption>(NPC);
                                int segmentIndex = Main.rand.Next(0, Segments.Length);
                                if(Timer < 300)
                                {
                                    segmentIndex = Main.rand.Next(0, Segments.Length / 4);
                                }
                                ref var segment = ref Segments[segmentIndex];
                                firer.damage = Eruption_Damage;
                                firer.position = segment.position;
                                firer.velocity = -Vector2.UnitY * 512;
                                firer.ai0 = NPC.whoAmI;
                                firer.ai1 = segmentIndex;
                                firer.New();
                            }
                        }
                    }

                    if(Timer == (int)(Eruption_SinTime / 2))
                    {
                        var roarSound = AssetRegistry.Sounds.Rek.RekIdleroar;
                        SoundEngine.PlaySound(roarSound, MyTarget.Center);
                    }
                    Vector2 start = _lavaArenaRectangle.BottomLeft();
                    Vector2 end = _lavaArenaRectangle.BottomRight();

                    start.Y += 64;
                    end.Y += 64;
                    var ratio = Timer / Eruption_SinTime;
                    if (AttackCount == 1)
                        ratio = 1f - ratio;
                    Vector2 pointToMoveTo = Vector2.Lerp(start, end, ratio);
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
                    _globalAlpha = MathHelper.Lerp(1f, 0F, EasingFunction.InSine(Timer / Eruption_PrepTime));
                    this.GetAnimator().PlayAnimation(ANIM_IDLE, AnimationParams.Default with { IsLooping = true });
                    NPC.velocity.X += NPC.direction;
                    NPC.velocity.Y += 0.05f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    if (Timer >= Eruption_PrepTime)
                    {
                        NextState();
                    }
                }
                break;
        }
    }
}

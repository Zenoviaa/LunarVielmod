using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Ishtar;
using Stellamod.Content.Biomes;
using Stellamod.Core.Biomes;
using Stellamod.Core.LunarLightingSystem;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable;

public class FableBiomePlayer : ModPlayer
{
    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        if (!Player.ZoneWitchesFable)
            return;
        if (!Main.hasFocus)
            return;

        if (!Main.dayTime)
        {
            if (Main.rand.NextBool(16))
            {
                var pos = Vector2.Zero;
                pos.Y += Main.screenHeight;
                pos.X += Main.rand.Next(0, Main.screenWidth);
                var vel = new Vector2(2, 0);
                vel *= Main.rand.NextFloat(0.5f, 2f);
                if (Main.rand.NextBool(2))
                {
                    pos.X += Main.screenWidth;
                    vel.X *= -1;
                }

                FableParticles.Lantern.Spawn(new()
                {
                    timeLeft = 1600,
                    color = Color.White,
                    position = pos,
                    velocity = vel,
                    scale = Main.rand.NextFloat(0.4f, 1f)
                });
            }

            if (Main.rand.NextBool(16))
            {
                var pos = Vector2.Zero;
                pos.Y += Main.screenHeight;
                pos.X += Main.rand.Next(0, Main.screenWidth);
                var vel = new Vector2(2, 0);
                vel *= Main.rand.NextFloat(0.5f, 2f);
                if (Main.rand.NextBool(2))
                {
                    pos.X += Main.screenWidth;
                    vel.X *= -1;
                }

                FableParticles.MiniLantern.Spawn(new()
                {
                    timeLeft = 1600,
                    color = Color.White,
                    position = pos,
                    velocity = vel,
                    scale = Main.rand.NextFloat(0.4f, 1f) * 0.5F
                });
            }

            if (Main.rand.NextBool(128))
            {
                var pos = Vector2.Zero;
                pos.Y += Main.screenHeight;
                pos.X += Main.rand.Next(0, Main.screenWidth);
                var vel = new Vector2(2, 0);
                vel *= Main.rand.NextFloat(0.5f, 2f);
                if (Main.rand.NextBool(2))
                {
                    pos.X += Main.screenWidth;
                    vel.X *= -1;
                }

                FableParticles.BigLantern.Spawn(new()
                {
                    timeLeft = 1600,
                    color = Color.White,
                    position = pos,
                    velocity = vel,
                    scale = Main.rand.NextFloat(0.4f, 1f)
                });
            }
        }

        var divMain = 128;
        var divSmall = 64;
        if (!Main.dayTime)
        {
            divMain *= 3;
            divSmall *= 3;
        }
        //Spawn particles
        if (Main.rand.NextBool(divMain))
        {
            var pos = Vector2.Zero;
            pos.Y += Main.rand.Next(0, Main.screenHeight);
            var vel = new Vector2(4, 0);
            vel *= Main.rand.NextFloat(0.5f, 2f);
            if(Main.rand.NextBool(2))
            {
                pos.X += Main.screenWidth;
                vel.X *= -1;
            }

            FableParticles.Witch.Spawn(new()
            {
                timeLeft = 400,
                color = Color.DarkGray,
                position = pos,
                velocity = vel,
                scale = Main.rand.NextFloat(0.4f, 1f)
            });
        }

        if (Main.rand.NextBool(divSmall))
        {
            var pos = Vector2.Zero;
            pos.Y += Main.rand.Next(0, Main.screenHeight / 2);
            var vel = new Vector2(2, 0);
            vel *= Main.rand.NextFloat(0.5f, 2f);
            if (Main.rand.NextBool(2))
            {
                pos.X += Main.screenWidth;
                vel.X *= -1;
            }

            FableParticles.MiniWitch.Spawn(new()
            {
                timeLeft = 400,
                color = Color.DarkGray,
                position = pos,
                velocity = vel,
                scale = Main.rand.NextFloat(0.4f, 1f)
            });
        }
    }
}
public class FableBiome : BaseUrdveilBiome,
    ISunColor
{
    public static Color[] FableSunColors
    {
        get
        {
            field = new Color[]
            {
                  Color.SkyBlue,

                    new Color(255, 173, 63),
                       new Color(255, 173, 63),
                                new Color(255, 173, 63),
                                         new Color(255, 173, 63),
                                            new Color(255, 173, 63),



                    Color.White,
                   Color.White,
                        Color.White,
                             Color.White,
                    Color.White,
                    Color.White,
                   Color.White,
                        Color.White,
                    Color.White,
                    Color.White,

                    new Color(255, 173, 63),
                    new Color(255, 173, 63),
                    new Color(255, 173, 63),
                    new Color(255, 173, 63),
                    new Color(255, 173, 63),
                    Color.SkyBlue
            };
            return field;
        }
    }
    // Select all the scenery
    public override ModWaterStyle WaterStyle => ModContent.GetInstance<IshtarWaterStyle>(); // Sets a water style for when inside this biome
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<NoBackgroundStyle>(); //ModContent.Find<ModSurfaceBackgroundStyle>("Stellamod/FabledBackgroundStyle");
    public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

    public override int Music
    {
        get
        {
            //Put your if statement here


            //Normal music
            if (Main.dayTime)
            {
                return MusicLoader.GetMusicSlot(Mod, "Assets/Music/GetSpookyYaWitch");
            }
            else
            {
                return MusicLoader.GetMusicSlot(Mod, "Assets/Music/LightedFable");
            }
        }
    }

    public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;

    // Populate the Bestiary Filter
    public override string BestiaryIcon => base.BestiaryIcon;
    public override string BackgroundPath => base.BackgroundPath;
    public override Color? BackgroundColor => base.BackgroundColor;
    public override bool IsBiomeActive(Player player) => BiomeTileCounts.InFable;
    public override void SpecialVisuals(Player player, bool isActive)
    {
        string name = "LunarVeil:FableSky";
        if (!SkyManager.Instance[name].IsActive() && isActive)
            SkyManager.Instance.Activate(name, player.Center);
        if (SkyManager.Instance[name].IsActive() && !isActive)
            SkyManager.Instance.Deactivate(name);
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        player.ZoneWitchesFable = true;
        if (Main.netMode == NetmodeID.Server)
            return;
        LunarLightingRenderer.AddSunColor(this);
    }
    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        player.ZoneWitchesFable = false;
        if (Main.netMode == NetmodeID.Server)
            return;
        LunarLightingRenderer.RemoveSunColor(this);
    }

    public void ModifySunColor(ref SkySettings skySettings, float sunInterpolant)
    {
        if (!Main.dayTime)
            skySettings.TargetTintSunColor = FableSunColors[0];
        else
            skySettings.TargetTintSunColor = DrawUtilities.InterpolateColorArray(sunInterpolant, FableSunColors);

        var endDayColorNightColor = DrawUtilities.InterpolateColorArray(0.96f, FableSunColors);
        var endColor = new Color(97, 93, 110);
        var endTintSunColor = new Color(135, 206, 235);
        if (!Main.dayTime)
        {
            skySettings.TargetColorOfTheSkies = endColor;
            skySettings.TargetTintSunColor = endTintSunColor;
        }
        else
        {
            skySettings.TargetColorOfTheSkies = Color.Lerp(skySettings.TargetColorOfTheSkies, skySettings.TargetTintSunColor, 0.25f);
        }

    }
}

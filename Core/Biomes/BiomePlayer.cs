using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Fable;
using Stellamod.Content.Areas.SpringHills;
using Stellamod.Content.Gores.Foreground;
using Stellamod.Core.Foreground;
using Stellamod.Visual.Particles;
using Stellamod.WorldG;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Biomes;


public static class BiomeExtensions
{
    extension(Player player)
    {

        public bool ZoneMoonspiralTower
        {
            get
            {
                return player.GetModPlayer<BiomePlayer>().ZoneMoonspiralTower;
            }
            set
            {
                player.GetModPlayer<BiomePlayer>().ZoneMoonspiralTower = value;
            }
        }
        public bool InDomain
        {
            get => ModContent.GetInstance<DomainExpansionManager>().noRender;
         
        }
        public bool ZoneFable
        {
            get
            {
                return player.GetModPlayer<MyPlayer>().ZoneFable;
            }
            set
            {
                player.GetModPlayer<MyPlayer>().ZoneFable = value;
            }
        }
        public bool ZoneHeatedDepths
        {
            get
            {
                return player.GetModPlayer<BiomePlayer>().ZoneHeatedDepths;
            }
            set
            {
                player.GetModPlayer<BiomePlayer>().ZoneHeatedDepths = value;
            }
        }
        public bool ZoneColloseum
        {
            get
            {
                return player.GetModPlayer<MyPlayer>().ZoneColloseum;
            }
            set
            {
                player.GetModPlayer<MyPlayer>().ZoneColloseum = value;
            }
        }
        public bool ZoneMothlight
        {
            get
            {
                return player.GetModPlayer<MyPlayer>().ZoneMothlight;
            }
            set
            {
                player.GetModPlayer<MyPlayer>().ZoneMothlight = value;
            }
        }
        public bool ZoneMineshaft
        {
            get
            {
                return player.GetModPlayer<MyPlayer>().ZoneMineshaft;
            }
            set
            {
                player.GetModPlayer<MyPlayer>().ZoneMineshaft = value;
            }
        }
        public bool ZoneDeepBelowCoralways
        {
            get
            {
                return player.GetModPlayer<BiomePlayer>().ZoneDeepBelowCoralways;
            }
        }
        public bool ZoneCinder
        {
            get
            {
                return player.GetModPlayer<MyPlayer>().ZoneCinder;
            }
            set
            {
                player.GetModPlayer<MyPlayer>().ZoneCinder = value;
            }
        }
        public bool ZoneDrakonic
        {
            get
            {
                return player.GetModPlayer<MyPlayer>().ZoneDrakonic;
            }
            set
            {
                player.GetModPlayer<MyPlayer>().ZoneDrakonic = value;
            }
        }
        public bool ZoneWonder
        {
            get
            {
                return player.GetModPlayer<MyPlayer>().ZoneWonder;
            }
            set
            {
                player.GetModPlayer<MyPlayer>().ZoneWonder = value;
            }
        }
        public bool ZoneRoyalCapital
        {
            get
            {
                return player.GetModPlayer<MyPlayer>().ZoneAlcadzia;
            }
            set
            {
                player.GetModPlayer<MyPlayer>().ZoneAlcadzia = value;
            }
        }

        public bool ZoneWitchesFable
        {
            get
            {
                return player.GetModPlayer<MyPlayer>().ZoneFable;
            }
            set
            {
                player.GetModPlayer<MyPlayer>().ZoneFable = value;
            }
        }
        public bool ZoneHarmonicCoralways
        {
            get
            {
                return player.GetModPlayer<BiomePlayer>().ZoneHarmonicCoralways;
            }
            set
            {
                player.GetModPlayer<BiomePlayer>().ZoneHarmonicCoralways = value;
            }
        }

        public bool ZoneAurelus
        {
            get
            {
                return player.GetModPlayer<MyPlayer>().ZoneAurelus;
            }
            set
            {
                player.GetModPlayer<MyPlayer>().ZoneAurelus = value;
            }
        }

        public bool ZoneAbyss
        {
            get
            {
                return player.GetModPlayer<MyPlayer>().ZoneAbyss;
            }
            set
            {
                player.GetModPlayer<MyPlayer>().ZoneAbyss = value;
            }
        }

        public bool ZoneCrimsonBridewell
        {
            get => player.GetModPlayer<BiomePlayer>().ZoneCrimsonBridewell;
            set => player.GetModPlayer<BiomePlayer>().ZoneCrimsonBridewell = value;
        }
    }
    public static bool ZoneXixianVillage(this Player player) => player.InModBiome<XixVillageBiome>();
}
public class BiomePlayer : ModPlayer
{
    private float _windCounter;
    public bool ZoneIshtar;
    public bool ZoneSacredUnknowns;
    public bool ZoneEveroseVillage;
    public bool ZoneSpringHills;
    public bool ZoneMistyDungeon;
    public bool ZoneMistyDungeonAnywhere;
    public bool ZoneDesertTown;
    public bool ZoneMarsh;
    public bool ZonePunkerTown;
    public bool ZoneWorldsEnd;
    public bool ZoneMoonspiralTower;
    public bool ZoneForest;
    public bool ZoneJunkyard;
    public bool ZoneHarmonicCoralways;
    public bool ZoneAegislavSurface;
    public bool ZoneHeatedDepths;
    public bool ZoneEdgeoftheMoon;
    public bool ZoneDeepBelowCoralways
    {
        get
        {
            StellaWorld stellaWorld = ModContent.GetInstance<StellaWorld>();
            int heightOffset = 100;
            Rectangle biomeRect =
                new Rectangle(
                stellaWorld.CoralwaysLocation.X,
                stellaWorld.CoralwaysLocation.Y + heightOffset, 1000,
                1800 - heightOffset);
            Rectangle deepRect = new Rectangle
                (stellaWorld.CoralwaysLocation.X,
                biomeRect.Bottom - 400,
                1000,
                400);
            return deepRect.Contains(Player.Center.ToTileCoordinates());
        }
    }
    public bool ZoneCrimsonBridewell;
    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        if (Main.netMode == NetmodeID.Server)
            return;

        if (Player.GetModPlayer<MyPlayer>().ZoneAlcadzia || ZoneWorldsEnd)
        {
            Main.GraveyardVisualIntensity = 0.4f;
        }

        if (Player.whoAmI == Main.myPlayer)
        {

            if (ZoneWorldsEnd)
            {
                PetalStorm s = ScreenShader.GetInstance<PetalStorm>();
                s.alpha = 1;
            }
            AddForegroundOrBackground();
            Player.ManageSpecialBiomeVisuals("Stellamod:Marsh", ZoneMarsh);
            Player.ManageSpecialBiomeVisuals("Stellamod:Aegislav", ZoneAegislavSurface);
            Player.ManageSpecialBiomeVisuals("Stellamod:HeatedDepths", ZoneHeatedDepths);


            if ((Player.ZoneCinder || ZoneHeatedDepths || Player.ZoneUnderworldHeight) && !Player.ZoneWonder)
            {
                WorldDepthGradient depthGradient = ScreenShader.GetInstance<WorldDepthGradient>();
                depthGradient.alpha = 1;


                StellaWorld stellaWorld = ModContent.GetInstance<StellaWorld>();
                float top = stellaWorld.HeatedDepthsStart;
                float end = stellaWorld.HeatedDepthsEnd;
                float steps = end - top;
                float progress = (Player.position.ToTileCoordinates().Y - top) / steps;
                Vector3 gradientStrength = new Vector3();
                gradientStrength.X = MathHelper.Lerp(0f, 0.2f, progress);
                gradientStrength.Y = MathHelper.Lerp(0.4f, 0.8f, progress);
                gradientStrength.Z = 0.18f * 0.5f;
                depthGradient.gradientStrength = gradientStrength;
                depthGradient.gradientColor = Color.Red.ToVector3();
            }

            if (Player.ZoneCinder)
            {
                FlameParticles2();
                return;
            }
            if (ZoneHeatedDepths)
            {
                FlameParticles();
            }
        }
    }
    private void FlameParticles2()
    {

        if (Main.rand.NextBool(2))
        {
            Vector2 pos = new Vector2();
            pos.X = Main.rand.Next(0, Main.screenWidth * 2);
            pos.Y = Main.rand.Next(0, Main.screenHeight);
            pos += Main.screenPosition - Main.screenWidth * Vector2.UnitX;
            PS.FaintSmokeDust.Spawn(FaintSmokeDustData.Default with { position = pos, velocity = -Vector2.UnitY * 0.1f, color = Color.White * 0.15F, timeleft = 180 });
        }
        if (Main.rand.NextBool(6))
        {
            Vector2 pos = new Vector2();
            pos.X = Main.rand.Next(0, Main.screenWidth * 2);
            pos.Y = Main.rand.Next(0, Main.screenHeight);
            pos += Main.screenPosition - Main.screenWidth * Vector2.UnitX;
            PS.FaintSmokeDust.Spawn(FaintSmokeDustData.Default with { position = pos, velocity = -Vector2.UnitY * 0.1f, color = Color.White * 0.15F, timeleft = 180 });
        }
        if (Main.rand.NextBool(2))
        {
            Vector2 pos = new Vector2();
            pos.X = Main.rand.Next(0, Main.screenWidth * 2);
            pos.Y = Main.rand.Next(0, Main.screenHeight);
            pos += Main.screenPosition - Main.screenWidth * Vector2.UnitX;
            PS.CinderEmberDust.Spawn(CinderEmberDustData.Default with { position = pos, velocity = -Vector2.UnitY * 0.1f, parallaxStrength = Main.rand.NextFloat(0.3f, 0.75f) });
        }
        if (Main.rand.NextBool(5))
        {
            Vector2 pos = new Vector2();
            pos.X = Main.rand.Next(0, Main.screenWidth * 2);
            pos.Y = Main.rand.Next(0, Main.screenHeight);
            pos += Main.screenPosition - Main.screenWidth * Vector2.UnitX;
            PS.CinderEmberDust.Spawn(CinderEmberDustData.Default with { position = pos, velocity = -Vector2.UnitY * 0.1f, parallaxStrength = Main.rand.NextFloat(0.3f, 0.75f) });
        }
        if (Main.rand.NextBool(2))
        {
            Vector2 pos = new Vector2();
            pos.X = Main.rand.Next(0, Main.screenWidth * 2);
            pos.Y = Main.rand.Next(0, Main.screenHeight);
            pos += Main.screenPosition - Main.screenWidth * Vector2.UnitX;
            PS.CinderEmberDustBackground.Spawn(CinderEmberDustData.Default with { position = pos, velocity = -Vector2.UnitY * 0.1f });
        }
    }
    private void FlameParticles()
    {
        if (Main.rand.NextBool(12))
        {
            Vector2 pos = new Vector2();
            pos.X = Main.rand.Next(0, Main.screenWidth * 2);
            pos.Y = Main.rand.Next(0, Main.screenHeight);
            pos += Main.screenPosition - Main.screenWidth * Vector2.UnitX;
            UnderworldFlameParticle.Spawn(pos, -Vector2.UnitY * 2 + -Vector2.UnitX, Scale: Main.rand.NextFloat(0.1f, 0.3f));
        }
        if (Main.rand.NextBool(3))
        {
            Vector2 pos = new Vector2();
            pos.X = Main.rand.Next(0, Main.screenWidth * 2);
            pos.Y = Main.rand.Next(0, Main.screenHeight);
            pos += Main.screenPosition - Main.screenWidth * Vector2.UnitX;
            UnderworldSmokeParticle.Spawn(pos, -Vector2.UnitY * 2 + -Vector2.UnitX, Scale: Main.rand.NextFloat(0.5f, 0.8f));
        }
    }
    private void AddForegroundOrBackground()
    {
        MyPlayer myPlayer = Player.GetModPlayer<MyPlayer>();
        if (myPlayer.ZoneIlluria || myPlayer.ZoneIshtar)
        {
            if (Main.rand.NextBool(15))
            {
                ForegroundParticleRenderer.NewParticle<Starstrike>();
            }

            if (Main.rand.NextBool(15))
            {
                ForegroundParticleRenderer.NewParticle<Snowstrike>();
            }
        }

        if (Player.ZoneWonder)
        {

            {
                if(Main.netMode != NetmodeID.Server)
                {
                    //Spawn Foreground particles
                    if(Main.GameUpdateCount % 2 == 0)
                    {
                        float a = Main.rand.NextFloat(0.3f, 0.5F);
                        float r = Main.rand.NextFloat();
                        var color = Color.Violet.Towards(Color.Pink, r);
                        color.ScrollHue(MathHelper.Lerp(-100, 100, r));
                        DarkspaceOrbParticles.ForegroundOrbs.Spawn(new()
                        {
                            position = DrawUtilities.RandomPositionForParticlesAroundPlayerWide(),
                            parallax = -Main.rand.NextFloat(0.5f, 1f),
                            timeLeft = 300,
                            color = color  * a
                        });
                    }

                    if (Main.GameUpdateCount % 2 == 0)
                    {
                        float r = Main.rand.NextFloat();
                        float a = Main.rand.NextFloat(0.3f, 0.5F);
                        var color = Color.Violet.Towards(Color.Pink, r);
                        color.ScrollHue(MathHelper.Lerp(-100, 100, r));
                        DarkspaceOrbParticles.BackgroundOrbs.Spawn(new()
                        {
                            position = DrawUtilities.RandomPositionForParticlesAroundPlayerWide(),
                            parallax = -Main.rand.NextFloat(0.1f, 0.2f),
                            timeLeft = 300,
                            color = color * a
                        });
                    }
                }
            }


            {
                float r = Main.rand.NextFloat();
                float r2 = Main.rand.NextFloat();
                float r3 = MathF.Min(r, r2);
                var color = Color.DarkBlue.Towards(Color.LightBlue, Main.rand.NextFloat());
                color.ScrollHue(MathHelper.Lerp(-25, 25, r3));

                PS.StarryShimmerRiverStars.Spawn(new()
                {
                    position = DrawUtilities.TryFindRandomWetPositionAroundPlayer(16),
                    timeLeft = 120,
                    color = color * r3,
                    velocity = Main.rand.NextVector2Circular(0.3f, 0.3f) + new Vector2(-0.25F + Main.rand.NextFloat(-0.3f, 0f), 0)
                });
            }

        }

        if (Player.ZoneAbyss && Main.rand.NextBool(8))
        {
            PS.AbyssFloatingFlowerDust.Spawn(AbyssFloatingFlowerDustData.Default with
            {
                position = DrawUtilities.RandomScreenPositionForForegroundParticles(),
                parallax = Main.rand.NextFloat(0.2f, 1f),
            });
        }

        if (Main.raining && (Player.ZoneForest || myPlayer.ZoneVillage))
        {
            if (Main.rand.NextBool(5))
            {
                ForegroundParticleRenderer.NewParticle<Cherryblossom>();
            }
        }

        if ((Player.ZoneDesert))
        {
            if (Main.rand.NextBool(5))
            {
                ForegroundParticleRenderer.NewParticle<Sandstrike>();
            }
        }

        if (ZoneMarsh)
        {
            if (Main.rand.NextBool(16))
            {
                ForegroundParticleRenderer.NewParticle<MarshLeaf>();
            }
            if (Main.rand.NextBool(16))
            {
                ForegroundParticleRenderer.NewParticle<MarshPetal>();
            }
        }
        if (ZoneWorldsEnd)
        {
            Main.windSpeedTarget = 50 * 0.01f;
            if (Main.rand.NextBool(32))
            {
                float xPosition = Main.rand.Next(-(int)(Main.screenWidth * 0.25f), (int)(Main.screenWidth * 0.25f));
                float yPosition = Main.rand.NextFloat(-Main.screenHeight * 0.25f, Main.screenHeight * 0.25f);
                Vector2 pos = Main.LocalPlayer.Center + new Vector2(xPosition, yPosition);
                SparkleParticle sp = SparkleParticle.Spawn(pos, Vector2.Zero, Scale: 0.7f);
                sp.flickering = true;
                sp.gravity = 0;
                sp.fast = true;
            }
            if (Main.rand.NextBool(8))
            {
                ForegroundParticleRenderer.NewParticle<GreyPetal>();
            }
        }


        if (ZoneAegislavSurface)
        {
            if (Main.rand.NextBool(8))
            {
                ForegroundParticleRenderer.NewParticle<AegislavStrike>();
            }
        }

        SpringHillsForegroundBackground();
    }

    private void SpringHillsForegroundBackground()
    {
        //Only do this in spring hills
        if (!ZoneSpringHills && !Player.ZoneForest)
            return;
        _windCounter--;
        if (_windCounter <= 0)
        {
            if (Main.rand.NextBool(2))
            {
                Main.windSpeedTarget = Main.rand.Next(-50, -25) * 0.01f;
            }
            else
            {
                Main.windSpeedTarget = Main.rand.Next(25, 50) * 0.01f;
            }

            _windCounter = 1200;
        }
        //CHERRY BLOSSOM
        if (Main.rand.NextBool(20))
        {

            ForegroundParticleRenderer.NewParticle<Cherryblossom>();
        }

        if (Main.rand.NextBool(20))
        {
            ForegroundParticleRenderer.NewParticle<SpringFallingFlower>();
        }
    }
}

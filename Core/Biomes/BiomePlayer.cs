using Stellamod.Assets.Biomes;
using Stellamod.Content.Areas.SpringHills;
using Stellamod.Content.Gores.Foreground;
using Stellamod.Core.Foreground;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Visual.Particles;
using Stellamod.WorldG;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Biomes
{
    public static class BiomeExtensions
    {
        public static bool ZoneFable(this Player player) => player.InModBiome<FableBiome>();
        public static bool ZoneAbyss(this Player player) => player.InModBiome<AbyssBiome>();
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
        public bool ZoneDeepBelowCoralways
        {
            get
            {
                Player localPlayer = Player;
                StellaWorld stellaWorld = ModContent.GetInstance<StellaWorld>();
                int heightOffset = 100;
                Rectangle biomeRect = new Rectangle(stellaWorld.CoralwaysLocation.X, stellaWorld.CoralwaysLocation.Y + heightOffset, 1000, 1800 - heightOffset);
                return localPlayer.Center.ToTileCoordinates().Y > biomeRect.Bottom - 400 && localPlayer.Center.ToTileCoordinates().Y < biomeRect.Bottom;
            }
        }
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
                if (ZoneWorldsEnd)
                {
                    ActivateWorldsEndSky();

                }
                else
                {
                    DeActivateWorldsEndSkyy();
                }

            }
            //  Main.NewText(SkyManager.Instance["Stellamod:WorldsEndSky"].IsActive());

        }
        private void ActivateWorldsEndSky()
        {
            if (!SkyManager.Instance["Stellamod:WorldsEndSky"].IsActive())
            {
                Vector2 targetCenter = Player.Center;
                SkyManager.Instance.Activate("Stellamod:WorldsEndSky", targetCenter);
            }
        }

        private void DeActivateWorldsEndSkyy()
        {
            if (SkyManager.Instance["Stellamod:WorldsEndSky"].IsActive())
            {
                Vector2 targetCenter = Player.Center;
                SkyManager.Instance.Deactivate("Stellamod:WorldsEndSky", targetCenter);
            }
        }
        private void AddForegroundOrBackground()
        {

            MyPlayer myPlayer = Player.GetModPlayer<MyPlayer>();
            if (myPlayer.ZoneIlluria || myPlayer.ZoneIshtar || myPlayer.ZoneAbyss)
            {
                if (Main.rand.NextBool(5))
                {
                    ForegroundParticleRenderer.NewParticle<Starstrike>();
                }

                if (Main.rand.NextBool(5))
                {
                    //Main.NewText("E");
                    ForegroundParticleRenderer.NewParticle<Snowstrike>();
                }
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
}

using Stellamod.Assets.Biomes;
using Stellamod.Content.Areas.SpringHills;
using Stellamod.Content.Gores.Foreground;
using Stellamod.Core.Foreground;
using Terraria;
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
        public bool ZoneSpringHills;
        public bool ZoneMistyDungeon;
        public bool ZoneDesertTown;
        public bool ZoneMarsh;
        public bool ZonePunkerTown;
        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            if (Player.GetModPlayer<MyPlayer>().ZoneAlcadzia)
            {
                Main.GraveyardVisualIntensity = 0.4f;
            }
        
            if(Player.whoAmI == Main.myPlayer)
            {
                AddForegroundOrBackground();
                Player.ManageSpecialBiomeVisuals("Stellamod:Marsh", ZoneMarsh);
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

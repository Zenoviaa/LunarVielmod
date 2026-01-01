using Stellamod.Content.Gores.Foreground;
using Stellamod.Core.Foreground;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Biomes
{
    internal class BiomePlayer : ModPlayer
    {
        private float _windCounter;
        public bool ZoneSpringHills;
        public bool ZoneMistyDungeon;
        public bool ZoneDesertTown;
        public bool ZoneMarsh;
        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            AddForegroundOrBackground();
        }

        private void AddForegroundOrBackground()
        {
            SpringHillsForegroundBackground();
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

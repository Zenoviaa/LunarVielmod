using Microsoft.Xna.Framework;
using Stellamod.Gores.Foreground;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Biomes
{
    internal class BiomePlayer : ModPlayer
    {
        private float _windCounter;
        public bool ZoneSpringHills;
        public override void ResetEffects()
        {
            base.ResetEffects();
        }
        public override void PreUpdate()
        {
            if (Main.hasFocus)
                AddForegroundOrBackground();
        }

        private void AddForegroundOrBackground()
        {
            SpringHillsForegroundBackground();
        }

        private void SpringHillsForegroundBackground()
        {
            //Only do this in spring hills
            if (!ZoneSpringHills)
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
            int spawnChance = -1;
            spawnChance = Cherryblossom.SpawnChance(Player);
            if (spawnChance != -1 && Main.rand.NextBool(spawnChance))
            {
                bool spawnForegroundItem = true;
                bool spawnOnPlayerLayer = true;
                Vector2 pos = Player.Center - new Vector2(Main.rand.Next(-(int)(Main.screenWidth * 2f), (int)(Main.screenWidth * 2f)), Main.screenHeight * 0.52f);
                ForegroundHelper.AddItem(new Cherryblossom(pos), spawnForegroundItem, spawnOnPlayerLayer);
            }

            spawnChance = SpringFallingFlower.SpawnChance(Player);
            if (spawnChance != -1 && Main.rand.NextBool(spawnChance))
            {
                bool spawnForegroundItem = true;
                bool spawnOnPlayerLayer = true;
                Vector2 pos = Player.Center - new Vector2(Main.rand.Next(-(int)(Main.screenWidth * 2f), (int)(Main.screenWidth * 2f)), Main.screenHeight * 0.52f);
                ForegroundHelper.AddItem(new SpringFallingFlower(pos), spawnForegroundItem, spawnOnPlayerLayer);
            }
        }
    }
}

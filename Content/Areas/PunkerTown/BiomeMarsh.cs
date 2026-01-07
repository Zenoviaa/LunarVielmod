using Microsoft.Xna.Framework;
using Stellamod.Assets.Biomes;
using Stellamod.Backgrounds;
using Stellamod.Content.Biomes;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown
{
    public class MarshJungleNPC : GlobalNPC
    {
        public override void EditSpawnRange(Player player, ref int spawnRangeX, ref int spawnRangeY, ref int safeRangeX, ref int safeRangeY)
        {
            base.EditSpawnRange(player, ref spawnRangeX, ref spawnRangeY, ref safeRangeX, ref safeRangeY);
        }
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            base.EditSpawnPool(pool, spawnInfo);
      
            if (spawnInfo.Player.InModBiome<BiomeMarsh>())
            {
                List<int> vanillaNpcs = new List<int>();
                if(Main.dayTime)
                    vanillaNpcs.Add(NPCID.JungleSlime);
                vanillaNpcs.Add(NPCID.JungleBat);
                vanillaNpcs.Add(NPCID.Piranha);
             //   vanillaNpcs.Add(NPCID.Snatcher);

                foreach(var npc in vanillaNpcs)
                {
                    pool.TryAdd(npc, 1f / vanillaNpcs.Count);
                }

          

            }
        }
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            base.EditSpawnRate(player, ref spawnRate, ref maxSpawns);
        }
    }
    public class BiomeMarsh : ModBiome
    {
        public override int Music
        {
            get
            {
                //Put your if statement here

                //Normal music
                if (Main.raining)
                {
                    return MusicLoader.GetMusicSlot(Mod, "Assets/Music/Acidic_Terors");
                }
                else if (!Main.dayTime)
                {
                    return MusicLoader.GetMusicSlot(Mod, "Assets/Music/Sporulent");
                }
                else
                {
                    return -1;
                }
            }
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<AcidWaterStyle>();
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<RainForestBackgroundStyle>();

        public override bool IsBiomeActive(Player player) => (player.ZoneOverworldHeight || player.ZoneDirtLayerHeight) && BiomeTileCounts.InMarsh;
        public override void OnEnter(Player player) => player.GetModPlayer<BiomePlayer>().ZoneMarsh = true;
        public override void OnLeave(Player player) => player.GetModPlayer<BiomePlayer>().ZoneMarsh = false;
    }
}

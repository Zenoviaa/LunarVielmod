using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes
{
    internal class AshotiTemple : ModBiome
    {
        public override int Music => MusicID.Temple;
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InAshotiTemple;
        public override void OnEnter(Player player) => player.GetModPlayer<MyPlayer>().ZoneAshotiTemple = true;
        public override void OnLeave(Player player) => player.GetModPlayer<MyPlayer>().ZoneAshotiTemple = false;
    }

    internal class AshotiTempleNPC : GlobalNPC
    {
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            base.EditSpawnPool(pool, spawnInfo);
            if (spawnInfo.Player.GetModPlayer<MyPlayer>().ZoneAshotiTemple)
            {
                pool[NPCID.Lihzahrd] = 3;
                pool[NPCID.FlyingSnake] = 3;
            }
        }
    }
}


using Terraria.ModLoader;
using System;

using Stellamod.Tiles.Abyss;
using Stellamod.Tiles.Acid;
using Terraria;
using Stellamod.WorldG;

namespace Stellamod.Assets.Biomes
{
    internal class GintzeArmy : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/The_Gintzing_Winds");
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;
        public override bool IsSceneEffectActive(Player player) => EventWorld.Gintzing && player.ZoneForest && (player.ZoneOverworldHeight || player.ZoneSkyHeight);
    }
}
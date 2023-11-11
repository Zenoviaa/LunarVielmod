
using Stellamod.WorldG;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes
{
    internal class GintzeArmy : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/The_Gintzing_Winds");
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;
        public override bool IsSceneEffectActive(Player player) => EventWorld.Gintzing && player.ZoneForest && (player.ZoneOverworldHeight || player.ZoneSkyHeight);
    }
}
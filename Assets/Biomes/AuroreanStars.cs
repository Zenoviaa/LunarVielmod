
using Terraria.ModLoader;
using System;
using Terraria.ModLoader;
using Stellamod.Tiles.Abyss;
using Stellamod.Tiles.Acid;
using Terraria;
using Stellamod.WorldG;

namespace Stellamod.Assets.Biomes
{
    internal class AuroreanStars : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/CountingStars");
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;
        public override bool IsSceneEffectActive(Player player) => EventWorld.Aurorean && player.ZoneForest && (player.ZoneOverworldHeight || player.ZoneSkyHeight);
    }
}
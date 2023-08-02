
using Terraria.ModLoader;
using System;
using Terraria.ModLoader;
using Stellamod.Tiles.Abyss;
using Stellamod.Tiles.Acid;
using Terraria;
using Stellamod.WorldG;

namespace Stellamod.Assets.Biomes
{
    internal class SoulStorm : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SoulStorm");
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;
        public override bool IsSceneEffectActive(Player player) => StellaWorld.SoulStorm && !Main.dayTime && player.ZoneSnow && (player.ZoneOverworldHeight || player.ZoneSkyHeight);
    }
}
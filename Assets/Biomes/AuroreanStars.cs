
using Terraria.ModLoader;
using System;

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
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Stellamod/StarbloomBackgroundStyle");

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Stellamod:Starbloom", isActive, player.Center);

        }
    }
}
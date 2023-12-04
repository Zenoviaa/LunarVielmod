
using Stellamod.WorldG;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes
{
    internal class AuroreanStars : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/CountingStars");
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;
        public override bool IsSceneEffectActive(Player player) => 
            EventWorld.Aurorean && (player.ZoneOverworldHeight || player.ZoneSkyHeight);
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Stellamod/StarbloomBackgroundStyle");

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Stellamod:Starbloom", isActive, player.Center);

        }
    }
}
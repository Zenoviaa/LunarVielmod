
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes
{
    public class AurelusBiome : ModBiome
    {
      
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Hidding_In_The_Shadows");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;


        public override bool IsBiomeActive(Player player) => (player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight) && BiomeTileCounts.InAurelus;
        public override void OnEnter(Player player) => player.GetModPlayer<MyPlayer>().ZoneAurelus = true;
        public override void OnLeave(Player player) => player.GetModPlayer<MyPlayer>().ZoneAurelus = false;
    }
}
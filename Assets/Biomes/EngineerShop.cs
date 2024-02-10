
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes
{
    public class EngineerShop : ModBiome
    {

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Mechanics");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;


        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InMech;
        public override void OnEnter(Player player) => player.GetModPlayer<MyPlayer>().ZoneMechanics = true;
        public override void OnLeave(Player player) => player.GetModPlayer<MyPlayer>().ZoneMechanics = false;
    }
}
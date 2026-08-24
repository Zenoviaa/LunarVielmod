
using Microsoft.Xna.Framework;
using Stellamod.Core.LunarLightingSystem;
using Terraria;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.Tundra.Abyss
{
    public class AbyssBiome : ModBiome,
        IBackLightModifier
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Hidding_In_The_Shadows");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;


        public override bool IsBiomeActive(Player player) => (player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight) && BiomeTileCounts.InAbyss;
        public override void OnEnter(Player player) => player.GetModPlayer<MyPlayer>().ZoneAbyss = true;
        public override void OnLeave(Player player) => player.GetModPlayer<MyPlayer>().ZoneAbyss = false;
        public void ModifyBackLight(ref Color backLightColor)
        {
            backLightColor = Color.Lerp(backLightColor, Color.White, 0.45f);
        }
    }
}

using Microsoft.Xna.Framework;
using Stellamod.WorldG;
using Terraria;
using Terraria.ModLoader;


namespace Stellamod.Assets.Biomes
{
    public class AcidBiome : ModBiome
    {
        //public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("SpiritMod/Biomes/SpiritUgBgStyle");

        public override int Music
        {
            get
            {
                //Put your if statement here
                if (EventWorld.GreenSun)
                {

                    if (Main.dayTime)
                    {
                        return MusicLoader.GetMusicSlot(Mod, "Assets/Music/GreenSun");
                    }

                    else
                    {
                        //Change this if needbe
                        return MusicLoader.GetMusicSlot(Mod, "Assets/Music/GreenSun");
                    }


                }

                //Normal music
                if (Main.dayTime)
                {
                    return MusicLoader.GetMusicSlot(Mod, "Assets/Music/Acidic_Terors");
                }
                else
                {
                    return MusicLoader.GetMusicSlot(Mod, "Assets/Music/Acidic_Nightmares");
                }
            }
        }
     
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Stellamod/AcidWaterStyle");

        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Stellamod/MarrowSurfaceBackgroundStyle");

        public override bool IsBiomeActive(Player player) => (player.ZoneOverworldHeight || player.ZoneDirtLayerHeight) && BiomeTileCounts.InAcid;
        public override void OnEnter(Player player) => player.GetModPlayer<MyPlayer>().ZoneAcid = true;
        public override void OnLeave(Player player) => player.GetModPlayer<MyPlayer>().ZoneAcid = false;
    }
}
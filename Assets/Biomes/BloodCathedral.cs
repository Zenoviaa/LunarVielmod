using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace Stellamod.Assets.Biomes
{
    public class BloodCathedral : ModBiome
    {
        //public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("SpiritMod/Biomes/SpiritUgBgStyle");

        public override int Music
        {
            get
            {
                //Put your if statement here


                //Normal music
                if (!Main.dayTime)
                {
                    return MusicLoader.GetMusicSlot(Mod, "Assets/Music/BloodCathedral");
                }
                else
                {
                    return -1;
                }
            }
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Stellamod/IshtarWaterStyle");
        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InBloodCathedral;
        public override void OnEnter(Player player) => player.GetModPlayer<MyPlayer>().ZoneBloodCathedral = true;
        public override void OnLeave(Player player) => player.GetModPlayer<MyPlayer>().ZoneBloodCathedral = false;
    }
}

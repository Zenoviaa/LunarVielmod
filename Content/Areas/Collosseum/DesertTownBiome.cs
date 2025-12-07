using Microsoft.Xna.Framework;
using Stellamod.Content.Biomes;
using Stellamod.Core.Biomes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum
{
    public class DesertTownBiome : BaseUrdveilBiome
    {
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;
        // Select Music
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override int Music
        {
            get
            {
                return -1;
            }
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {

        }

        public override bool IsBiomeActive(Player player)
        {
            bool inDesertTown = BiomeTileCounts.InDesertTown;
            return inDesertTown;
        }

        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;


        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.GetModPlayer<BiomePlayer>().ZoneDesertTown = true;

        }
        public override void OnLeave(Player player)
        {
            base.OnLeave(player);
            player.GetModPlayer<BiomePlayer>().ZoneDesertTown = false;
        }
    }
}

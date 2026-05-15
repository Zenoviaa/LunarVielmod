using Microsoft.Xna.Framework;
using Stellamod.Backgrounds;
using Stellamod.Content.Biomes;
using Stellamod.Core.Biomes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Dungeon
{
    public class MistyDungeonSurfaceBiome : BaseUrdveilBiome
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
            bool inMistyDungeon = BiomeTileCounts.InMistyDungeon;
            return inMistyDungeon;
        }
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;

        public override bool ShowTitleCard()
        {
            return false;
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.GetModPlayer<BiomePlayer>().ZoneMistyDungeonAnywhere = true;

        }
        public override void OnLeave(Player player)
        {
            base.OnLeave(player);
            player.GetModPlayer<BiomePlayer>().ZoneMistyDungeonAnywhere = false;
        }
    }
    public class MistyDungeonBiome : BaseUrdveilBiome
    {
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;
        // Select Music
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override int Music
        {
            get
            {
                return MusicLoader.GetMusicSlot(Mod, "Assets/Music/ManorWorld");
            }
        }
        public override void SpecialVisuals(Player player, bool isActive)
        {

        }

        public override bool IsBiomeActive(Player player)
        {
            bool inMistyDungeon = BiomeTileCounts.InMistyDungeon;
            Point point = player.position.ToTileCoordinates();
            bool lowEnough = (double)point.Y > Main.worldSurface + 16;
            return inMistyDungeon && lowEnough;
        } 
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;


        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.GetModPlayer<BiomePlayer>().ZoneMistyDungeon = true;

        }
        public override void OnLeave(Player player)
        {
            base.OnLeave(player);
            player.GetModPlayer<BiomePlayer>().ZoneMistyDungeon = false;
        }
    }
}

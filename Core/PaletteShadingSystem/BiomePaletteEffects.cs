using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Illuria.BossesIL.EStyr;
using Stellamod.Content.Areas.Ishtar.BossesIS.SanguineSingularity;
using Stellamod.Content.Biomes;
using Stellamod.Core.Palettes;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.PaletteShadingSystem
{
    public class MoonspiralTowerPaletteEffect : PaletteEffect
    {
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override PalettePriority Priority => PalettePriority.Medium;
        public override bool IsActive(Player player)
        {
            BiomePlayer myPlayer = player.GetModPlayer<BiomePlayer>();
            return myPlayer.ZoneMoonspiralTower;
        }
    }
    public class AbyssPaletteEffect : PaletteEffect
    {
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override PalettePriority Priority => PalettePriority.Medium;
        public override bool IsActive(Player player)
        {
            MyPlayer myPlayer = player.GetModPlayer<MyPlayer>();
            if (myPlayer.ZoneAbyss)
                return true;
            if (myPlayer.ZoneAurelus)
                return true;

            return false;
        }
    }
    public class VilepipesNGardenPaletteEffect : PaletteEffect
    {
        public override PalettePriority Priority => PalettePriority.Medium;
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override bool IsActive(Player player)
        {
            MyPlayer myPlayer = player.GetModPlayer<MyPlayer>();
            if (myPlayer.ZoneGovheil)
                return true;
            if (myPlayer.ZoneAcid)
                return true;

            return false;
        }
    }

    public class HellPaletteEffect : PaletteEffect
    {
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override bool IsActive(Player player)
        {
            MyPlayer myPlayer = player.GetModPlayer<MyPlayer>();
            if (myPlayer.ZoneCinder)
                return true;
            if (myPlayer.ZoneDrakonic)
                return true;
            if (player.ZoneUnderworldHeight)
                return true;

            return false;
        }
    }

    public class RoyalCapitalPaletteEffect : PaletteEffect
    {
        public override PalettePriority Priority => PalettePriority.Medium;
        public override PaletteType PaletteType => PaletteType.LunarShader;

        public override bool IsActive(Player player)
        {
            MyPlayer myPlayer = player.GetModPlayer<MyPlayer>();
            return myPlayer.ZoneAlcadzia;
        }
    }

    public class DungeonPaletteEffect : PaletteEffect
    {
        public override PalettePriority Priority => PalettePriority.Low;
        public override PaletteType PaletteType => PaletteType.VanillaShader;
        public override bool IsActive(Player player)
        {
            if (player.ZoneDungeon)
                return true;
            return false;
        }
    }


    public class DesertPaletteEffect : PaletteEffect
    {
        public override PalettePriority Priority => PalettePriority.Low;
        public override PaletteType PaletteType => PaletteType.VanillaShader;
        public override bool IsActive(Player player)
        {
            if (player.ZoneUndergroundDesert && !Main.LocalPlayer.ZoneOverworldHeight)
                return true;
            return false;
        }
    }

    public class DesertTopPaletteEffect : PaletteEffect
    {
        public override PalettePriority Priority => PalettePriority.Low;
        public override PaletteType PaletteType => PaletteType.VanillaShader;

        public override bool IsActive(Player player)
        {
            MyPlayer myPlayer = player.GetModPlayer<MyPlayer>();
            if (myPlayer.ZoneAshotiTemple)
                return true;
            if (player.GetModPlayer<MyPlayer>().ZoneColloseum)
                return true;
            if (player.ZoneUndergroundDesert)
                return false;
            if (player.ZoneDesert)
                return true;

            return false;
        }

    }

    public class FablePaletteEffect : PaletteEffect
    {
        public override PalettePriority Priority => PalettePriority.Medium;
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override bool IsActive(Player player)
        {
            MyPlayer myPlayer = player.GetModPlayer<MyPlayer>();
            if (myPlayer.ZoneFable)
                return true;
            return false;
        }
    }

    public class IllurianMistyDungeonPaletteEffect : PaletteEffect
    {
        public override PalettePriority Priority => PalettePriority.Medium;
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override bool IsActive(Player player)
        {
            return player.GetModPlayer<BiomePlayer>().ZoneMistyDungeon;
        }
    }

    public class BloodHoundPaletteEffect : PaletteEffect
    {
        public override PalettePriority Priority => PalettePriority.Medium;
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override bool IsActive(Player player)
        {
            MyPlayer myPlayer = player.GetModPlayer<MyPlayer>();
            if (Main.dayTime)
                return false;
            if (myPlayer.ZoneBloodCathedral)
                return true;
            return false;
        }
    }

    public class SanguineSingularityPaletteEffect : PaletteEffect
    {
        public override PalettePriority Priority => PalettePriority.Highest;
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override bool IsActive(Player player)
        {
            return base.IsActive(player) && NPC.AnyNPCs(ModContent.NPCType<SanguineSingularity>());
        }
    }

    /// <summary>
    /// A palette shader effect, these don't use terraria's normal post-processing because we don't want it over EVERY single layer
    /// </summary>
    public abstract class PaletteEffect : ModType
    {
        public virtual PaletteType PaletteType { get; }
        public virtual PalettePriority Priority { get; }
        public sealed override void SetupContent()
        {
            base.SetupContent();
            SetStaticDefaults();
        }
        public float fade;
        protected override void Register()
        {
            ModTypeLookup<PaletteEffect>.Register(this);
        }

        public virtual string GetPaletteFile()
        {
            return this.GetType().Name.Replace("PaletteEffect", ".pal");
        }

        public Effect GetShader()
        {
            string palFile = GetPaletteFile();
            PalettizerShader palettizerShader = PalettizerShader.Instance;
            palettizerShader.PaletteTexture = PaletteHelper.GetColorSpectrum(palFile);
            palettizerShader.Progress = fade;
            palettizerShader.Dither = ModContent.GetInstance<LunarVeilClientConfig>().Dither;
            palettizerShader.ImageSize = new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
            return palettizerShader.Effect;
        }

        public virtual bool IsActive(Player player)
        {
            return true;
        }
    }


}

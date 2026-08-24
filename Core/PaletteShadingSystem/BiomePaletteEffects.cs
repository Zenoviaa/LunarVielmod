using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Ishtar.BossesIS.SanguineSingularity;
using Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox;
using Stellamod.Content.Biomes;
using Stellamod.Core.Palettes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.PaletteShadingSystem
{
    public class VilepiperPaletteEffect : PaletteEffect
    {
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override PalettePriority Priority => PalettePriority.Medium;
        public override bool IsActive(Player player)
        {
            return false;
        }
    }

    public class EvildemonsPaletteEffect : PaletteEffect
    {
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override PalettePriority Priority => PalettePriority.Medium;
        public override bool IsActive(Player player)
        {
            return false;
        }
    }

    public class BloodyChasmsPaletteEffect : PaletteEffect
    {
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override PalettePriority Priority => PalettePriority.Medium;
        public override bool IsActive(Player player)
        {
            return false;
        }
    }

    public class PerfectPaletteEffect : PaletteEffect
    {
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override PalettePriority Priority => PalettePriority.Medium;
        public override bool IsActive(Player player)
        {
    
            BiomePlayer myPlayer = player.GetModPlayer<BiomePlayer>();
            return myPlayer.ZoneEdgeoftheMoon;
        }
    }

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
            //  return true;
            MyPlayer myPlayer = player.GetModPlayer<MyPlayer>();
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
    public class CindersparkPaletteEffect : PaletteEffect
    {
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override bool IsActive(Player player)
        {
            return false;
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

    public class HellPaletteEffect : PaletteEffect
    {
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override bool IsActive(Player player)
        {

            return false;
            MyPlayer myPlayer = player.GetModPlayer<MyPlayer>();
            if (myPlayer.ZoneCinder)
                return false;
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
            return myPlayer.ZoneAlcadzia || NPC.AnyNPCs(ModContent.NPCType<RoyalFox>());
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
 
            return player.GetModPlayer<BiomePlayer>().ZoneMistyDungeon || player.GetModPlayer<BiomePlayer>().ZoneMistyDungeonAnywhere;
        }
    }

    public class BloodHoundPaletteEffect : PaletteEffect
    {
        public override PalettePriority Priority => PalettePriority.Medium;
        public override PaletteType PaletteType => PaletteType.LunarShader;
        public override bool IsActive(Player player)
        {
      
            //       return true;
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
            return this.GetType().Name.Replace("PaletteEffect", string.Empty);
        }

        public Effect GetShader()
        {
            string palFile = GetPaletteFile();
            PalettizerShader palettizerShader = PalettizerShader.Instance;
            palettizerShader.PaletteTexture = PaletteAssets.FromPaletteFile(palFile).Value.ColorAtlas;
            palettizerShader.Progress = fade;
            palettizerShader.Dither = ModContent.GetInstance<LunarVeilClientConfig>().Dither;
            palettizerShader.ImageSize = new Vector2(Main.screenWidth, Main.screenHeight);
            palettizerShader.DitherTexture = AssetManager.Dithering.Dither8x8.Asset.Value;
            palettizerShader.DitherAlpha = 0.05f;
            palettizerShader.ScreenOffset = Main.screenPosition;
            return palettizerShader.Effect;
        }

        public virtual bool IsActive(Player player)
        {
            return true;
        }
    }
}

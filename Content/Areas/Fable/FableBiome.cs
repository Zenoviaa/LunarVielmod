using Microsoft.Xna.Framework;
using Stellamod.Backgrounds;
using Stellamod.Content.Areas;
using Stellamod.Content.Areas.Ishtar;
using Stellamod.Content.Biomes;
using Stellamod.Core.Biomes;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable;

// Shows setting up two basic biomes. For a more complicated example, please request.
public class FableBiome : BaseUrdveilBiome
{
    // Select all the scenery
    public override ModWaterStyle WaterStyle => ModContent.GetInstance<IshtarWaterStyle>(); // Sets a water style for when inside this biome
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<NoBackgroundStyle>(); //ModContent.Find<ModSurfaceBackgroundStyle>("Stellamod/FabledBackgroundStyle");
    public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

    public override int Music
    {
        get
        {
            //Put your if statement here


            //Normal music
            if (Main.dayTime)
            {
                return MusicLoader.GetMusicSlot(Mod, "Assets/Music/GetSpookyYaWitch");
            }
            else
            {
                return MusicLoader.GetMusicSlot(Mod, "Assets/Music/LightedFable");
            }
        }
    }

    public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;

    // Populate the Bestiary Filter
    public override string BestiaryIcon => base.BestiaryIcon;
    public override string BackgroundPath => base.BackgroundPath;
    public override Color? BackgroundColor => base.BackgroundColor;
    public override bool IsBiomeActive(Player player) => BiomeTileCounts.InFable;
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        player.ZoneWitchesFable = true;
    }
    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        player.ZoneWitchesFable = false;
    }
}

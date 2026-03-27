using Stellamod.Assets.Biomes;
using Stellamod.Content.Biomes;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.WorldG;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide;

public class HarmonicCoralwaysBiome : ModBiome,
    IBackLightModifier
{
    //   public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<NoBackgroundStyle>();
    public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

    // Select Music
    public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

    public override int Music
    {
        get
        {
            return MusicLoader.GetMusicSlot(Mod, "Assets/Music/HarmonicCoralways");
        }
    }


    public override string BestiaryIcon => base.BestiaryIcon;
    public override string BackgroundPath => base.BackgroundPath;
    public override Color? BackgroundColor => base.BackgroundColor;
    public override ModWaterStyle WaterStyle => ModContent.GetInstance<HarmonicWaterStyle>();
    public override bool IsBiomeActive(Player player)
    {
        StellaWorld stellaWorld = ModContent.GetInstance<StellaWorld>();
        Rectangle biomeRect = new Rectangle(stellaWorld.CoralwaysLocation.X, stellaWorld.CoralwaysLocation.Y, 1000, 1800);
        return biomeRect.Contains(player.Center.ToTileCoordinates());
    }

    public void ModifyBackLight(ref Color backLightColor)
    {
        backLightColor = Color.Lerp(backLightColor, Color.White, 0.9f);
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        player.GetModPlayer<BiomePlayer>().ZoneHarmonicCoralways = true;
        if (Main.netMode == NetmodeID.Server)
            return;

        LunarLightingRenderer.AddBackLight(this);
    }
    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        player.GetModPlayer<BiomePlayer>().ZoneHarmonicCoralways = false;
        if (Main.netMode == NetmodeID.Server)
            return;

        LunarLightingRenderer.RemoveBackLight(this);
    }
}

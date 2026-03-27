using Stellamod.Assets.Biomes;
using Stellamod.Content.Biomes;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.WorldG;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide;

public class HarmonicCoralwaysTileGlow : GlobalTile
{
    public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
    {
        base.ModifyLight(i, j, type, ref r, ref g, ref b);
        var biomePlayer = Main.LocalPlayer.GetModPlayer<BiomePlayer>();
        if (!biomePlayer.ZoneHarmonicCoralways)
            return;

        Tile tile = Main.tile[i, j];
        if(WorldGen.TileIsExposedToAir(i, j))
        {
            r = 0.5f;
            g =0.51f;
            b = 1;
        }
    }
}
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

        int heightOffset = 100;
        Rectangle biomeRect = new Rectangle(stellaWorld.CoralwaysLocation.X, stellaWorld.CoralwaysLocation.Y + heightOffset, 1000, 1800 - heightOffset);
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

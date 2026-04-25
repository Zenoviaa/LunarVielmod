using Stellamod.Content.Biomes;
using Stellamod.Core.Biomes;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror;

public class AegislavBiome : BaseUrdveilBiome
{
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<NoBackgroundStyle>();
    public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

    // Select Music
    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

    public override int Music
    {
        get
        {
            return MusicLoader.GetMusicSlot(Mod, "Assets/Music/Aegislav");
        }
    }


    public override string BestiaryIcon => base.BestiaryIcon;
    public override string BackgroundPath => base.BackgroundPath;
    public override Color? BackgroundColor => base.BackgroundColor;

    public override bool IsBiomeActive(Player player) => BiomeTileCounts.InAegislav;
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        player.GetModPlayer<BiomePlayer>().ZoneAegislavSurface = true;
        if (Main.netMode == NetmodeID.Server)
            return;

        SkyManager.Instance.Activate("Stellamod:AegislavSky", player.Center);
    }
    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        player.GetModPlayer<BiomePlayer>().ZoneAegislavSurface = false;
        if (Main.netMode == NetmodeID.Server)
            return;

        SkyManager.Instance.Deactivate("Stellamod:AegislavSky", player.Center);
    }
}

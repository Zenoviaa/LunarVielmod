using Microsoft.Xna.Framework;
using Stellamod.Content.Biomes;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes;


// Shows setting up two basic biomes. For a more complicated example, please request.
public class AlcadziaBiome : ModBiome
{
    // Select all the scenery
    public override ModWaterStyle WaterStyle => ModContent.GetInstance<StarbloomWaterStyle>();// ModContent.Find<ModWaterStyle>("Stellamod/StarbloomWaterStyle"); // Sets a water style for when inside this biome
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<AlcadziaBackgroundStyle>();// ModContent.Find<ModSurfaceBackgroundStyle>("Stellamod/AlcadziaBackgroundStyle");
    public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

    // Select Music
    public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;



    public override int Music => AssetReferences.Assets.Music.AlcadizHurricane.Slot;
    public override void SpecialVisuals(Player player, bool isActive)
    {
        string name = "LunarVeil:RoyalCapitalSky";
        if (!SkyManager.Instance[name].IsActive() && isActive)
            SkyManager.Instance.Activate(name, player.Center);
        if (SkyManager.Instance[name].IsActive() && !isActive)
            SkyManager.Instance.Deactivate(name);
    }


    // Populate the Bestiary Filter

    public override bool IsBiomeActive(Player player) => BiomeTileCounts.InRoyalCapital;

    public override string BestiaryIcon => base.BestiaryIcon;
    public override string BackgroundPath => base.BackgroundPath;
    public override Color? BackgroundColor => base.BackgroundColor;

    // Use SetStaticDefaults to assign the display name
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("Cathedral of the Moon");
    }


    public override void OnEnter(Player player)
    {
        player.ZoneRoyalCapital = true;
    }
    public override void OnLeave(Player player)
    {
        player.ZoneRoyalCapital = false;
    }
    // Calculate when the biome is active.


}

using Microsoft.Xna.Framework;
using Stellamod.Assets.Biomes;
using Stellamod.Common.Particles;
using Stellamod.Content.Biomes;
using Stellamod.Core.Biomes;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using Stellamod.WorldG;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace;

public class WonderousPlayer : ModPlayer
{
    public override void Load()
    {
        base.Load();
        On_Player.CanSeeShimmerEffects += RemoveShimmer;
    }


    public override void Unload()
    {
        base.Unload();
        On_Player.CanSeeShimmerEffects -= RemoveShimmer;
    }

    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        if (Main.netMode == NetmodeID.Server)
            return;
        if (Main.gameMenu)
            return;
        if (!Main.LocalPlayer.ZoneWonder)
            return;
        if (MoonUtils.Rand.NextBool(5))
        {
            float xRand = MoonUtils.Rand.NextFloat(-1000, 1000);
            float yRand = MoonUtils.Rand.NextFloat(-1000, 1000);
            PS.FallingDarkspaceStar.Spawn(new()
            {
                position = Main.LocalPlayer.Center + new Vector2(xRand, yRand),
                color = Color.White,
                timeLeft = 180
            });
        }
    }

    private bool RemoveShimmer(On_Player.orig_CanSeeShimmerEffects orig, Player self)
    {
        if (Main.gameMenu)
            return false;
        if (self.ZoneWonder)
            return false;
        return orig(self);
    }

}
public class WonderousDarkspcaeTileGlow : GlobalTile
{
    public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
    {
        base.ModifyLight(i, j, type, ref r, ref g, ref b);
        if (!Main.LocalPlayer.ZoneWonder)
            return;

        Tile tile = Main.tile[i, j];
        if (WorldGen.TileIsExposedToAir(i, j) && tile.LiquidAmount > 0)
        {
            r = 0.25f;
            g = 0.71f;
            b = 0.8f;
        }
    }
}
public class WonderousDarkspaceBiome : BaseUrdveilBiome,
    IBackLightModifier
{

    // Select all the scenery
    public override ModWaterStyle WaterStyle => ModContent.GetInstance<StarbloomWaterStyle>(); // Sets a water style for when inside this biome
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<AlcadziaBackgroundStyle>();// ModContent.Find<ModSurfaceBackgroundStyle>("Stellamod/AlcadziaBackgroundStyle");
    //  public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<BlankUndergroundBackgroundStyle>();
    public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

    // Select Music
    public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;



    public override int Music => AssetReferences.Assets.Music.WondrousDarkspace.Slot;
    public override void SpecialVisuals(Player player, bool isActive)
    {
        string name = "LunarVeil:DarkspaceSky";
        if (!SkyManager.Instance[name].IsActive() && isActive)
            SkyManager.Instance.Activate(name, player.Center);
        if (SkyManager.Instance[name].IsActive() && !isActive)
            SkyManager.Instance.Deactivate(name);
    }


    // Populate the Bestiary Filter

    public override bool IsBiomeActive(Player player)
    {
        StellaWorld stellaWorld = ModContent.GetInstance<StellaWorld>();

        return BiomeTileCounts.InDarkspace && !player.ZoneOverworldHeight && !player.ZoneSkyHeight && player.position.ToTileCoordinates().Y > stellaWorld.DarkspaceStart;
    }

    public override string BestiaryIcon => base.BestiaryIcon;
    public override string BackgroundPath => base.BackgroundPath;
    public override Color? BackgroundColor => base.BackgroundColor;

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        player.ZoneWonder = true;
        if (Main.netMode == NetmodeID.Server)
            return;

        ModContent.GetInstance<LunarLightingRenderer>().AddBackLight(this);
    }
    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        player.ZoneWonder = false;
        if (Main.netMode == NetmodeID.Server)
            return;

        ModContent.GetInstance<LunarLightingRenderer>().RemoveBackLight(this);
    }

    public void ModifyBackLight(ref Color backLightColor)
    {
        backLightColor = Color.Lerp(backLightColor, Color.White, 0.8f);
    }
}

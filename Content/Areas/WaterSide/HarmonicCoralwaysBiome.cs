using Stellamod.Assets.Biomes;
using Stellamod.Content.Biomes;
using Stellamod.Core.Backgrounds;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Visual.Particles;
using Stellamod.WorldG;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide;

public class HarmonicCoralwaysEffects : ModPlayer
{
    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        if (Main.myPlayer != Player.whoAmI)
            return;

        BiomePlayer biomePlayer = Player.GetModPlayer<BiomePlayer>();
        if (!biomePlayer.ZoneHarmonicCoralways)
            return;

        if (!Main.rand.NextBool(100))
            return;

        Rectangle spawnRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        Vector2 pos = new Vector2();
        pos.X = Main.rand.NextFloat(Main.screenPosition.X, Main.screenPosition.X + Main.screenWidth);
        pos.Y = Main.rand.NextFloat(Main.screenPosition.Y, Main.screenPosition.Y + Main.screenHeight);
        var bp = BubbleParticle.Spawn(pos, -Vector2.UnitY, Scale: Main.rand.NextFloat(0.4f, 0.8f));
        bp.gravity = 0;
        bp.longBubble = true;
    }
}
public class HarmonicCoralwaysSpawnRates : GlobalNPC
{
    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        base.EditSpawnRate(player, ref spawnRate, ref maxSpawns);
        if (player.InModBiome<HarmonicCoralwaysBiome>())
        {
            spawnRate = (int)(spawnRate * 0.06f);
            maxSpawns *= (int)(maxSpawns * 2f);
        }
    }
}
public class HarmonicCoralwaysTileGlow : GlobalTile
{
    public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
    {
        base.ModifyLight(i, j, type, ref r, ref g, ref b);
        var biomePlayer = Main.LocalPlayer.GetModPlayer<BiomePlayer>();
        if (!biomePlayer.ZoneHarmonicCoralways)
            return;
        if (biomePlayer.ZoneDeepBelowCoralways)
            return;
        Tile tile = Main.tile[i, j];
        if(WorldGen.TileIsExposedToAir(i, j))
        {
            r = 0.5f;
            g = 0.51f;
            b = 0.8f;
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
            Player localPlayer = Main.LocalPlayer;
            if (IsDeepBelow)
            {
                return MusicLoader.GetMusicSlot(Mod, "Assets/Music/SongsDeepBelow");
            } 
            else
            {
                return MusicLoader.GetMusicSlot(Mod, "Assets/Music/HarmonicCoralways");
            }
        }
    }


    public override string BestiaryIcon => base.BestiaryIcon;
    public override string BackgroundPath => base.BackgroundPath;
    public override Color? BackgroundColor => base.BackgroundColor;
    public override ModWaterStyle WaterStyle => ModContent.GetInstance<HarmonicWaterStyle>();
    public bool IsDeepBelow
    {
        get
        {
            return Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneDeepBelowCoralways;
        }
    }
    public override bool IsBiomeActive(Player player)
    {
        StellaWorld stellaWorld = ModContent.GetInstance<StellaWorld>();
        int heightOffset = 100;
        Rectangle biomeRect = new Rectangle(stellaWorld.CoralwaysLocation.X, stellaWorld.CoralwaysLocation.Y + heightOffset, 1000, 1800 - heightOffset);
        return biomeRect.Contains(player.Center.ToTileCoordinates());
    }

    public void ModifyBackLight(ref Color backLightColor)
    {
        if (IsDeepBelow)
        {
            //backLightColor = Color.Lerp(backLightColor, Color.White, 0.3f);
            ModContent.GetInstance<CustomBGManager>().darkenBGColor = Color.Lerp(Color.White, Color.Black, 0.25f);
            return;
        }
        backLightColor = Color.Lerp(backLightColor, Color.White, 0.9f);
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        player.GetModPlayer<BiomePlayer>().ZoneHarmonicCoralways = true;
        if (Main.netMode == NetmodeID.Server)
            return;

        ModContent.GetInstance<LunarLightingRenderer>().AddBackLight(this);
    }
    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        player.GetModPlayer<BiomePlayer>().ZoneHarmonicCoralways = false;
        if (Main.netMode == NetmodeID.Server)
            return;

        ModContent.GetInstance<LunarLightingRenderer>().RemoveBackLight(this);
    }
}

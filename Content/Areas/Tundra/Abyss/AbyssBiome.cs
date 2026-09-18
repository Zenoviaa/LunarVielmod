using Stellamod.Assets.Biomes;
using Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;
using Stellamod.Content.Biomes;
using Stellamod.Core.Biomes;
using Stellamod.Core.LunarLightingSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.Tundra.Abyss;

public class AbyssBiome : BaseUrdveilBiome,
    IBackLightModifier
{
    public override int Music
    {
        get
        {
            int music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/ArtInTheShadows");
            if (NPC.AnyNPCs(ModContent.NPCType<TheWhisperer>()))
            {
                music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/HeWhoWhispss");
                return music;
            } 
            else if (BellFlowerSystem.Whispering)
            {
                music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/HeWhoWhispss");
                return music;
            }

            return music;
        }
    }

    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
    public override string BestiaryIcon => base.BestiaryIcon;
    public override string BackgroundPath => MapBackground;
    public override Color? BackgroundColor => base.BackgroundColor;
    public override ModWaterStyle WaterStyle => ModContent.GetInstance<AcidWaterStyle>();

    public override bool IsBiomeActive(Player player) => 
        (player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight) && BiomeTileCounts.InAbyss && !player.InModBiome<AurelusBiome>();
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        player.GetModPlayer<MyPlayer>().ZoneAbyss = true;
        player.GetModPlayer<BiomePlayer>().justEnteredAbyss = true;
        if (Main.netMode == NetmodeID.Server)
            return;

        ModContent.GetInstance<LunarLightingRenderer>().AddBackLight(this);
    }
    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        player.GetModPlayer<MyPlayer>().ZoneAbyss = false;
        if (Main.netMode == NetmodeID.Server)
            return;

        ModContent.GetInstance<LunarLightingRenderer>().RemoveBackLight(this);
    }

    public void ModifyBackLight(ref Color backLightColor)
    {
        backLightColor = Color.Lerp(backLightColor, Color.White, 0.8f);
    }
}
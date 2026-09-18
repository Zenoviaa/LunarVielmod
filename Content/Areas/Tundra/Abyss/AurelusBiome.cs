
using Microsoft.Xna.Framework;
using Stellamod.Core.Biomes;
using Stellamod.WorldG;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss;

public class AurelusBiome : BaseUrdveilBiome
{
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/AurelusTemple");
    public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
    public override string BestiaryIcon => base.BestiaryIcon;
    public override string BackgroundPath => MapBackground;
    public override Color? BackgroundColor => base.BackgroundColor;


    public override bool IsBiomeActive(Player player)
    {
        if (SavedGenerationParameters.AbyssTempleRectangle == Rectangle.Empty)
            return BiomeTileCounts.InAurelus;
        return SavedGenerationParameters.AbyssTempleRectangle.Contains(player.position.ToTileCoordinates());
    }
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        player.GetModPlayer<MyPlayer>().ZoneAurelus = true;
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        player.GetModPlayer<MyPlayer>().ZoneAurelus = false;
    }
}
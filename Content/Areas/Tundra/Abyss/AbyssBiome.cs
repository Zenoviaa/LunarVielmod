
using Microsoft.Xna.Framework;
using Stellamod.Core.Biomes;
using Stellamod.Core.LunarLightingSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.Tundra.Abyss
{
    public class AbyssBiome : BaseUrdveilBiome,
        IBackLightModifier
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Hidding_In_The_Shadows");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;


        public override bool IsBiomeActive(Player player) => (player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight) && BiomeTileCounts.InAbyss;
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.GetModPlayer<MyPlayer>().ZoneAbyss = true;
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
}
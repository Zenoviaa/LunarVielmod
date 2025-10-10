using Microsoft.Xna.Framework;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes
{
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
            if (Main.LocalPlayer == null)
                return;

            if (Main.rand.NextBool(5) && Main.LocalPlayer.GetModPlayer<MyPlayer>().ZoneWonder)
            {
                float xRand = Main.rand.NextFloat(-1000, 1000);
                float yRand = Main.rand.NextFloat(-1000, 1000);
                Particle.NewParticle<StarParticle>(Main.LocalPlayer.Center + new Vector2(xRand, yRand), Vector2.Zero);
            }
        }
        private bool RemoveShimmer(On_Player.orig_CanSeeShimmerEffects orig, Player self)
        {
            if (Main.LocalPlayer == null)
                return false;

            if (Main.LocalPlayer.GetModPlayer<MyPlayer>().ZoneWonder)
                return false;
            return orig(self);
        }

    }
    public class WonderousDarkspaceBiome : ModBiome
    {

        // Select all the scenery
        public override ModWaterStyle WaterStyle => ModContent.GetInstance<StarbloomWaterStyle>(); // Sets a water style for when inside this biome
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Stellamod/AlcadziaBackgroundStyle");
        //  public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<BlankUndergroundBackgroundStyle>();
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

        // Select Music
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;



        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/WondrousDarkspace");
        public override void SpecialVisuals(Player player, bool isActive)
        {
            string name = "LunarVeil:DarkspaceSky";
            if (!SkyManager.Instance[name].IsActive() && isActive)
                SkyManager.Instance.Activate(name, player.Center);
            if (SkyManager.Instance[name].IsActive() && !isActive)
                SkyManager.Instance.Deactivate(name);
        }


        // Populate the Bestiary Filter

        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InDarkspace;

        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;

        public override void OnEnter(Player player) => player.GetModPlayer<MyPlayer>().ZoneWonder = true;
        public override void OnLeave(Player player) => player.GetModPlayer<MyPlayer>().ZoneWonder = false;
    }
}

using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Biomes;
using Stellamod.Core.Foreground;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Backgrounds;

public class MagicalFish : ForegroundLayer
{
    private HologramShader HologramShader;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        tilingInBothAxes = true;
        HologramShader = new HologramShader();
        showWhenNotGrounded = true;
    }
    public override bool IsActive()
    {
        return
            Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneHarmonicCoralways || 
            Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneMoonspiralTower;
    }

    public override void SetLayering(ref float zLayer, ref Vector2 parallax)
    {
        base.SetLayering(ref zLayer, ref parallax);
        
        shader = HologramShader;
        HologramShader.NoiseTexture = AssetManager.Noise.AuroraRays.Value;
        HologramShader.Time = Main.GlobalTimeWrappedHourly * 1;
        parallax.X = 1.2f;
        parallax.Y = 1.2f;
    }
}

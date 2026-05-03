using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.MoonspiralTower.VerliaBoss;
using Stellamod.Content.Biomes;
using Stellamod.Core.Foreground;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Backgrounds;

public class MagicalFish : ForegroundLayer
{
    private Asset<Texture2D> _glowMask;
    private HologramShader HologramShader;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        tilingInBothAxes = true;
        HologramShader = new HologramShader();
        showWhenNotGrounded = true;
    }
    public override void Unload()
    {
        base.Unload();
        _glowMask = null;
    }

    public override bool IsActive()
    {
        return
            Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneMoonspiralTower;
    }

    public override void SetLayering(ref float zLayer, ref Vector2 parallax)
    {
        base.SetLayering(ref zLayer, ref parallax);

        shader = HologramShader;
        _glowMask ??= ModContent.Request<Texture2D>(Texture + "_GlowMask");
        HologramShader.NoiseTexture = _glowMask.Value;

        Vector2 right = Vector2.UnitX;
        right = right.RotatedBy(MathHelper.ToRadians(24));
        HologramShader.Time = right * Main.GlobalTimeWrappedHourly * -0.08f;//new Vector2(Main.GlobalTimeWrappedHourly * -0.08f);

        parallax.X = 1.5f;
        parallax.Y = 1.5f;
        if (NPC.AnyDanger())
        {
            drawAlpha = MathHelper.Lerp(drawAlpha, 0f, 0.1f);
        }
        else
        {
            drawAlpha = MathHelper.Lerp(drawAlpha, 1f, 0.1f);
        }

    }
}

public class HarmonicMagicalFish : ForegroundLayer
{
    private Asset<Texture2D> _glowMask;
    private HologramShader HologramShader;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        tilingInBothAxes = true;
        HologramShader = new HologramShader();
        showWhenNotGrounded = true;
    }
    public override void Unload()
    {
        base.Unload();
        _glowMask = null;
    }

    public override bool IsActive()
    {
        return
            Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneHarmonicCoralways;
    }

    public override void SetLayering(ref float zLayer, ref Vector2 parallax)
    {
        base.SetLayering(ref zLayer, ref parallax);
        
        shader = HologramShader;
        _glowMask ??= ModContent.Request<Texture2D>(Texture + "_GlowMask");
        HologramShader.NoiseTexture = _glowMask.Value;
        HologramShader.Time = new Vector2(Main.GlobalTimeWrappedHourly * -0.08f);
        parallax.X = 1.2f;
        parallax.Y = 1.2f;
    }
}

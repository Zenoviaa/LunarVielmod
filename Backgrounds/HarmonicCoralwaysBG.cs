using Stellamod.Content.Biomes;
using Stellamod.Core.Backgrounds;
using Terraria;

namespace Stellamod.Backgrounds;

public class HarmonicCoralwaysBG : CustomBG
{
    public CustomBGLayer BackLayer;
    public CustomBGLayer MidLayer;
    public CustomBGLayer FrontLayer;

    public float FarParallax => 0.08f;
    public float MidParallax => 0.11f;
    public float CloseParallax => 0.20f;

    private void AddFarLayer()
    {
        BackLayer = new CustomBGLayer();
        BackLayer.SetTexture("Assets/Textures/Backgrounds/HarmonicCoralwaysFar");
        BackLayer.Parallax = FarParallax;
        BackLayer.DrawOffset = Vector2.Zero;
        AddLayer(BackLayer);
    }

    private void AddMidLayer()
    {
        MidLayer = new CustomBGLayer();
        MidLayer.SetTexture("Assets/Textures/Backgrounds/HarmonicCoralwaysMid");
        MidLayer.Parallax = MidParallax;
        MidLayer.DrawOffset = Vector2.Zero;
        AddLayer(MidLayer);
    }

    private void AddCloseLayer()
    {
        FrontLayer = new CustomBGLayer();
        FrontLayer.SetTexture("Assets/Textures/Backgrounds/HarmonicCoralwaysClose");
        FrontLayer.Parallax = CloseParallax;
        FrontLayer.DrawOffset = Vector2.Zero;
        AddLayer(FrontLayer);
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        DrawScale = 1.5f;
        AddFarLayer();
        AddMidLayer();
        AddCloseLayer();
    }

    public override bool IsActive()
    {
        //   Main.LocalPlayer.ZoneDesert = false; Main.LocalPlayer.ZoneUndergroundDesert = false;
        BackLayer.ParallaxOffset = new Vector2(750, 0);
        NoSurfaceLight = true;
        parallaxInBothWays = true;
        NoSurfaceOffset = true;
        DrawScale = 1;
        DrawOffset = new Vector2(0, 100);
        return Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneHarmonicCoralways;
    }
}

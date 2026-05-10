using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.WaterSide.BossesWS;
using Stellamod.Content.Biomes;
using Stellamod.Core.Backgrounds;
using Stellamod.Core.Effects;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Backgrounds;

public class HarmonicCoralwaysBG : CustomBG
{
    public HologramShader Shader;
    public CustomBGLayer BackLayer;
    public CustomBGLayer MidLayer;
    public CustomBGLayer FrontLayer;
    public CustomBGLayer HoloLayer;
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
    private void AddHoloLayer()
    {
        HoloLayer = new CustomBGLayer();
        HoloLayer.SetTexture("Assets/Textures/Backgrounds/HarmonicCoralwaysFish");
        HoloLayer.Parallax = CloseParallax;
        HoloLayer.DrawOffset = Vector2.Zero;
       

        AddLayer(HoloLayer);
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        DrawScale = 1.5f;
        AddFarLayer();
        AddMidLayer();
        AddCloseLayer();
        //AddHoloLayer();
    }

    public override bool IsActive()
    {
        /*
        Shader = HologramShader.Instance;
        Shader.NoiseTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/CloudNoise3").Value;
        HoloLayer.Shader = Shader;
        Shader.Time = Main.GlobalTimeWrappedHourly;*/
        //Main.LocalPlayer.ZoneDesert = false; Main.LocalPlayer.ZoneUndergroundDesert = false;
        BackLayer.ParallaxOffset = new Vector2(750, 0);
        NoSurfaceLight = true;
        parallaxInBothWays = true;
        NoSurfaceOffset = true;
        DrawScale = 1.2f;
        DrawOffset = new Vector2(0, 100);
        if (NPC.AnyNPCs(ModContent.NPCType<LeviathanEel>()))
        {
            DrawColor = Color.Lerp(DrawColor, Color.Lerp(Color.White, Color.Black, 0.85f), 0.1f);
        }
        else
        {
            DrawColor = Color.Lerp(DrawColor, Color.White, 0.1f);
        }
     
        return Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneHarmonicCoralways;
    }
}

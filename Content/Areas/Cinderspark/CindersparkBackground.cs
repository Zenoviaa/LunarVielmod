using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Backgrounds;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark;

public class CindersparkBackground : CustomBG
{
    private Asset<Texture2D> _backgroundTextureAsset;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _backgroundTextureAsset = AssetManager.LoadBackground("Cinderspark");
    }

    public override bool UseCustomDrawing()
    {
        return true;
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        Color fadeToColor = Color.OrangeRed;
        fadeToColor.A = 75;
        BackgroundHelper.DrawHeatDistortedAtlassedBackground(spriteBatch, BackgroundHelper.AtlassedBackgroundDraw.Default with
        {
            cameraMovement = CameraMovement,
            bg = _backgroundTextureAsset,
            numBackgrounds = 3,
            fadeToColor = fadeToColor,
            alpha = Alpha,
            parallax = new Vector2(0.01f, 0f),
            baseColor = Color.White,
        }, 
            time: Main.GlobalTimeWrappedHourly * 0, 
            heatDistortion: 0.012f, ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/NormalNoise1").Value);
    }

    public override bool IsActive()
    {
        return Main.LocalPlayer.GetModPlayer<MyPlayer>().ZoneCinder;
    }
}
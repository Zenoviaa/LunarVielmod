using Stellamod.Content.Biomes;
using Stellamod.Core.Backgrounds;
using Terraria;

namespace Stellamod.Content.Areas.WondrousDarkspace;

public class WonderousDarkspaceBackground : CustomBG
{

    public override bool UseCustomDrawing()
    {
        return true;
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        Color fadeToColor = Color.White;
        fadeToColor.A = 0;
        BackgroundHelper.DrawSimpleAtlassedBackgroundStretchedToScreen(spriteBatch, BackgroundHelper.AtlassedBackgroundDraw.Default with
        {
            cameraMovement = CameraMovement,
            bg = AssetReferences.Assets.Textures.Backgrounds.Darkspace.Asset,
            numBackgrounds = 4,
            fadeToColor = fadeToColor,
            alpha = Alpha,
            parallax = new Vector2(0.01f, 0f),
            baseColor = Color.White
        });
    }

    public override bool IsActive()
    {
        return Main.LocalPlayer.ZoneWonder;
    }
}
using Stellamod.Core;
using Stellamod.Core.Backgrounds;
using Terraria;

namespace Stellamod.Content.Areas.Tundra.Abyss;

public class AbyssBackground : CustomBG
{
    public override bool UseCustomDrawing()
    {
        return true;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        BackgroundHelper.DrawSimpleAtlassedBackground(spriteBatch, BackgroundHelper.AtlassedBackgroundDraw.Default with
        {
            fadeToColor = Color.Transparent,
            numBackgrounds = 3,
            parallax = new Vector2(0.003f, 0),
            bg = AssetReferences.Assets.Textures.Backgrounds.Abyss.Asset,
            cameraMovement = CameraMovement,
            alpha = Alpha
        });
    }

    public override bool IsActive()
    {
        return Main.LocalPlayer.GetModPlayer<MyPlayer>().ZoneAbyss;
    }
}

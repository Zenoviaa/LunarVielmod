using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Core.Backgrounds;
using Terraria;

namespace Stellamod.Content.Areas.Tundra.Abyss;

public class AbyssBackground : CustomBG
{
    private Asset<Texture2D> _backgroundTextureAsset;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _backgroundTextureAsset = AssetManager.LoadBackground("Abyss");
    }

    public override bool UseCustomDrawing()
    {
        return true;
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        BackgroundHelper.DrawWrappedAtlassedBackground(spriteBatch, BackgroundHelper.AtlassedBackgroundDraw.Default with
        {
            fadeToColor = Color.Transparent,
            numBackgrounds = 3,
            parallax = new Vector2(0.003f, 0),
            bg = _backgroundTextureAsset,
            cameraMovement = CameraMovement,
            alpha = Alpha
        });
    }

    public override bool IsActive()
    {
        return Main.LocalPlayer.GetModPlayer<MyPlayer>().ZoneAbyss;
    }
}

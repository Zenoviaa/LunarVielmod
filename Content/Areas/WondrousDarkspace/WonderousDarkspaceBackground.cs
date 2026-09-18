using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Content.Biomes;
using Stellamod.Core.Backgrounds;
using Stellamod.Core.Effects;
using Terraria;

namespace Stellamod.Content.Areas.WondrousDarkspace;

public class WonderousDarkspaceBackground : CustomBG
{
    private Asset<Texture2D> _backgroundTextureAsset;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _backgroundTextureAsset = AssetManager.LoadBackground("Darkspace");
    }

    public override bool UseCustomDrawing()
    {
        return true;
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        Color fadeToColor = Color.White;
        fadeToColor.A = 0;
        BackgroundHelper.DrawSimpleAtlassedBackground(spriteBatch, BackgroundHelper.AtlassedBackgroundDraw.Default with
        {
            cameraMovement = CameraMovement,
            bg = _backgroundTextureAsset,
            numBackgrounds = 6,
            fadeToColor = fadeToColor,
            alpha = Alpha,
            parallax = new Vector2(0.01f, 0f),
            baseColor = Color.White
        });
    }

    public override bool IsActive()
    {
        return false;
    }
}


public class DarkspaceBG : CustomBG
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        DrawScale = 1f;
        DrawOffset = Vector2.Zero;
        NoSurfaceOffset = true;
        NoSurfaceLight = true;
        CustomBGLayer backLayer = new CustomBGLayer();
        backLayer.SetTexture("Assets/Textures/Backgrounds/DarkspaceBottom");
        backLayer.Parallax = 0.2f;
        backLayer.DrawOffset = Vector2.Zero;
        AddLayer(backLayer);


        CustomBGLayer midLayer = new CustomBGLayer();
        midLayer.SetTexture("Assets/Textures/Backgrounds/DarkspaceMid");
        midLayer.Parallax = 0.35f;
        midLayer.DrawOffset = Vector2.Zero;
        AddLayer(midLayer);

        CustomBGLayer midFogLayer = new CustomBGLayer();
        midFogLayer.SetTexture("Assets/Textures/Backgrounds/DarkspaceMidGradient");
        midFogLayer.Parallax = 0.35f;
        midFogLayer.DrawOffset = Vector2.Zero;

        MistShader midMistShader = new MistShader();
        midMistShader.StartColor = Color.Purple * 0.25f;
        midMistShader.EndColor = Color.Transparent;
        midFogLayer.Shader = midMistShader;
        AddLayer(midFogLayer);

        CustomBGLayer frontLayer = new CustomBGLayer();
        frontLayer.SetTexture("Assets/Textures/Backgrounds/DarkspaceFront");
        frontLayer.Parallax = 0.4f;
        frontLayer.DrawOffset = Vector2.Zero;
        AddLayer(frontLayer);

        CustomBGLayer front2Layer = new CustomBGLayer();
        front2Layer.SetTexture("Assets/Textures/Backgrounds/DarkspaceFrontGradient");
        front2Layer.Parallax = 0.5f;
        front2Layer.DrawOffset = Vector2.Zero;
        //  AddLayer(front2Layer);

        CustomBGLayer frontFogLayer = new CustomBGLayer();
        frontFogLayer.SetTexture("Assets/Textures/Backgrounds/RainforestFrontGradient");
        frontFogLayer.Parallax = 0.5f;
        frontFogLayer.DrawOffset = Vector2.Zero;

        MistShader frontMistShader = new MistShader();
        frontMistShader.StartColor = Color.Pink * 0.5f;
        frontMistShader.EndColor = Color.Transparent;
        frontFogLayer.Shader = frontMistShader;
        AddLayer(frontFogLayer);
    }
    public override int GetParallaxYStartHeight()
    {

        int yMax = (Main.UnderworldLayer - (Main.maxTilesY / 6));
        int yMin = yMax - 12;
        int yMid = (yMin + yMax) / 2;
        return (int)(yMid * 16);
    }

    public override bool IsActive()
    {
        DrawOffset = new Vector2(0, 64);
        return Main.LocalPlayer.GetModPlayer<MyPlayer>().ZoneWonder;
    }
}
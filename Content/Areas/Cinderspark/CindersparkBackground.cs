using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Backgrounds;
using Terraria;

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

        int numBackgrounds = 3;
        Vector2[] parallax = new Vector2[10];
        Vector2[] offsets = new Vector2[10];
        for (int i = 0; i < numBackgrounds; i++)
        {
            offsets[i] = Vector2.Lerp(new Vector2(0f, 0), new Vector2(0f, 1f), i / (float)numBackgrounds);
            parallax[i] = Vector2.Lerp(new Vector2(0.01f, 0f), Vector2.Zero, i / (float)numBackgrounds) * (CameraMovement) * 0.01f;
        }

        AtlassedParallaxingBackgroundShader backgroundShader = AtlassedParallaxingBackgroundShader.Instance;
        backgroundShader.Parallax = parallax;
        backgroundShader.Offsets = offsets;
        backgroundShader.Tiling = new Vector2(1f, numBackgrounds);

        Color fadeToColor = Color.OrangeRed;
        fadeToColor.A = 75;
        backgroundShader.FadeToColor = fadeToColor;
        spriteBatch.Begin(SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            backgroundShader.Effect);

        Color baseColor = Color.Lerp(Color.White, Color.Black, 0.0f);
        //  baseColor = Color.Lerp(baseColor, Main.ColorOfTheSkies, 0.5f);
        Color drawColor = baseColor * Alpha;

        Rectangle drawRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        for (int i = numBackgrounds; i >= 0; i--)
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(_backgroundTextureAsset, Main.screenPosition);
            drawer.drawOrigin = Vector2.Zero;
            drawer.scale = Vector2.One * 2;
            drawer.color = drawColor;
            drawer.VerticalFrame(i, numBackgrounds);
            drawer.dstRect = drawRect;
            spriteBatch.Draw(drawer);
        }

        spriteBatch.End();
    }

    public override bool IsActive()
    {
        return Main.LocalPlayer.GetModPlayer<MyPlayer>().ZoneCinder;
    }
}
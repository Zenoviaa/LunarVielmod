using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Backgrounds;

public static class BackgroundHelper
{
    public record struct AtlassedBackgroundDraw(Asset<Texture2D> bg, int numBackgrounds, Color baseColor, Color fadeToColor, float alpha, Vector2 cameraMovement, Vector2 parallax)
    {
        public static readonly AtlassedBackgroundDraw Default = new()
        {
            numBackgrounds = 3,
            baseColor = Color.White,
            fadeToColor = new Color(255, 255, 255, 25),
            parallax = new Vector2(0.1f),

        };
    }
    public static void DrawSimpleAtlassedBackground(SpriteBatch spriteBatch, 
        Asset<Texture2D> bg, 
        int numBackgrounds,
        Color fadeToColor, 
        float alpha, 
        Vector2 cameraMovement)
    {
        Vector2[] parallax = new Vector2[10];
        Vector2[] offsets = new Vector2[10];
        for (int i = 0; i < numBackgrounds; i++)
        {
            offsets[i] = Vector2.Lerp(new Vector2(0f, 0), new Vector2(0f, 1f), (float)i / (float)numBackgrounds);
            parallax[i] = Vector2.Lerp(new Vector2(0.01f, 0.01f), Vector2.Zero, (float)i / (float)numBackgrounds) * (cameraMovement) * 0.01f;
        }

        AtlassedParallaxingBackgroundShader backgroundShader = AtlassedParallaxingBackgroundShader.Instance;
        backgroundShader.Parallax = parallax;
        backgroundShader.Offsets = offsets;
        backgroundShader.Tiling = new Vector2(1f, numBackgrounds);
        backgroundShader.FadeToColor = fadeToColor;
        spriteBatch.Begin(SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            backgroundShader.Effect);

        Color baseColor = Color.Lerp(Color.White, Color.Black, 0.75f);
        //  baseColor = Color.Lerp(baseColor, Main.ColorOfTheSkies, 0.5f);
        Color drawColor = baseColor * alpha;

        Rectangle drawRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        for (int i = numBackgrounds; i >= 0; i--)
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(bg, Main.screenPosition);
            drawer.drawOrigin = Vector2.Zero;
            drawer.scale = Vector2.One * 2;
            drawer.color = drawColor;
            drawer.VerticalFrame(i, numBackgrounds);
            drawer.dstRect = drawRect;
            spriteBatch.Draw(drawer);
        }

        spriteBatch.End();
    }
    public static void DrawWrappedAtlassedBackground(SpriteBatch spriteBatch, AtlassedBackgroundDraw draw)
    {
        Vector2[] parallax = new Vector2[10];
        Vector2[] offsets = new Vector2[10];

        int numBackgrounds = draw.numBackgrounds;
        Color fadeToColor = draw.fadeToColor;
        Vector2 cameraMovement = draw.cameraMovement;
        float alpha = draw.alpha;
        var bg = draw.bg;
        for (int i = 0; i < numBackgrounds; i++)
        {
            offsets[i] = Vector2.Lerp(new Vector2(0f, 0), new Vector2(0f, 1f), (float)i / (float)numBackgrounds);
            parallax[i] = Vector2.Lerp(draw.parallax, Vector2.Zero, (float)i / (float)numBackgrounds) * (cameraMovement) * 0.01f;
        }

        AtlassedParallaxingBackgroundShader backgroundShader = AtlassedParallaxingBackgroundShader.Instance;
        backgroundShader.Parallax = parallax;
        backgroundShader.Offsets = offsets;
        backgroundShader.Tiling = new Vector2(1f, numBackgrounds);
        backgroundShader.FadeToColor = fadeToColor;
        backgroundShader.Effect.CurrentTechnique = backgroundShader.Effect.Techniques["SpriteDrawing"];
        spriteBatch.Begin(SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointWrap,
            DepthStencilState.None,
            RasterizerState.CullNone,
            backgroundShader.Effect);

        Color baseColor = draw.baseColor;
        //  baseColor = Color.Lerp(baseColor, Main.ColorOfTheSkies, 0.5f);
        Color drawColor = baseColor * alpha;


        SpritebatchDrawer drawer2 = SpritebatchDrawer.FromTextureAsset(bg, Main.screenPosition);
        drawer2.VerticalFrame(0, numBackgrounds);
        Rectangle frameSize = drawer2.sourceRect.Value;


        float scale = 1.5f;
        float width = frameSize.Width * scale;
        float height = frameSize.Height * scale;
        int xRepeats = (int)(Main.screenWidth / width) + 1;
        int yRepeats = (int)(Main.screenHeight / height) + 1;


        for (int i = numBackgrounds; i >= 0; i--)
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(bg, Main.screenPosition);
            drawer.drawOrigin = Vector2.Zero;
            drawer.scale = Vector2.One;
            drawer.color = drawColor;
            drawer.VerticalFrame(i, numBackgrounds);
            for (int x = 0; x < xRepeats; x++)
            {
                for (int y = 0; y < yRepeats; y++)
                {
                    Rectangle drawRect = new Rectangle((int)(x * width), (int)(y * height), (int)width, (int)height);
                    drawer.dstRect = drawRect;
                    spriteBatch.Draw(drawer);
                }
            }
 
        }

        spriteBatch.End();
    }
    public static void DrawSimpleAtlassedBackground(SpriteBatch spriteBatch, AtlassedBackgroundDraw draw)
    {
        Vector2[] parallax = new Vector2[10];
        Vector2[] offsets = new Vector2[10];

        int numBackgrounds = draw.numBackgrounds;
        Color fadeToColor = draw.fadeToColor;
        Vector2 cameraMovement = draw.cameraMovement;
        float alpha = draw.alpha;
        var bg = draw.bg;
        for (int i = 0; i < numBackgrounds; i++)
        {
            offsets[i] = Vector2.Lerp(new Vector2(0f, 0), new Vector2(0f, 1f), (float)i / (float)numBackgrounds);
            parallax[i] = Vector2.Lerp(draw.parallax, Vector2.Zero, (float)i / (float)numBackgrounds) * (cameraMovement) * 0.01f;
        }

        AtlassedParallaxingBackgroundShader backgroundShader = AtlassedParallaxingBackgroundShader.Instance;
        backgroundShader.Parallax = parallax;
        backgroundShader.Offsets = offsets;
        backgroundShader.Tiling = new Vector2(1f, numBackgrounds);
        backgroundShader.FadeToColor = fadeToColor;
        backgroundShader.Effect.CurrentTechnique = backgroundShader.Effect.Techniques["SpriteDrawing"];
        spriteBatch.Begin(SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            backgroundShader.Effect);

        Color baseColor = draw.baseColor;
        //  baseColor = Color.Lerp(baseColor, Main.ColorOfTheSkies, 0.5f);
        Color drawColor = baseColor * alpha;

        Rectangle drawRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        for (int i = numBackgrounds; i >= 0; i--)
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(bg, Main.screenPosition);
            drawer.drawOrigin = Vector2.Zero;
            drawer.scale = Vector2.One * 2;
            drawer.color = drawColor;
            drawer.VerticalFrame(i, numBackgrounds);
            drawer.dstRect = drawRect;
            spriteBatch.Draw(drawer);
        }

        spriteBatch.End();
    }
    public static void DrawHeatDistortedAtlassedBackground(SpriteBatch spriteBatch, 
        AtlassedBackgroundDraw draw, float time, float heatDistortion, Texture2D heatNoise)
    {
        Vector2[] parallax = new Vector2[10];
        Vector2[] offsets = new Vector2[10];

        int numBackgrounds = draw.numBackgrounds;
        Color fadeToColor = draw.fadeToColor;
        Vector2 cameraMovement = draw.cameraMovement;
        float alpha = draw.alpha;
        var bg = draw.bg;
        for (int i = 0; i < numBackgrounds; i++)
        {
            offsets[i] = Vector2.Lerp(new Vector2(0f, 0), new Vector2(0f, 1f), (float)i / (float)numBackgrounds);
            parallax[i] = Vector2.Lerp(draw.parallax, Vector2.Zero, (float)i / (float)numBackgrounds) * (cameraMovement) * 0.01f;
        }

        AtlassedParallaxingBackgroundShader backgroundShader = AtlassedParallaxingBackgroundShader.Instance;
        backgroundShader.Parallax = parallax;
        backgroundShader.Offsets = offsets;
        backgroundShader.Tiling = new Vector2(1f, numBackgrounds);
        backgroundShader.FadeToColor = fadeToColor;
        backgroundShader.Time = time;
        backgroundShader.HeatDistortion = heatDistortion;
        backgroundShader.NormalNoise1 = heatNoise;
        backgroundShader.Effect.CurrentTechnique = backgroundShader.Effect.Techniques["HeatDrawing"];
        spriteBatch.Begin(SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            backgroundShader.Effect);

        Color baseColor = draw.baseColor;
        //  baseColor = Color.Lerp(baseColor, Main.ColorOfTheSkies, 0.5f);
        Color drawColor = baseColor * alpha;

        Rectangle drawRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        for (int i = numBackgrounds; i >= 0; i--)
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(bg, Main.screenPosition);
            drawer.drawOrigin = Vector2.Zero;
            drawer.scale = Vector2.One * 2;
            drawer.color = drawColor;
            drawer.VerticalFrame(i, numBackgrounds);
            drawer.dstRect = drawRect;
            spriteBatch.Draw(drawer);
        }

        spriteBatch.End();
    }
}
public abstract class CustomBG : ModType
{
    public int Type;
    public List<CustomBGLayer> Layers = new List<CustomBGLayer>();
    public Vector2 startParallaxPosition;
    public Vector2 CameraMovement => Main.Camera.Center - startParallaxPosition;
    public int Priority;
    public float Alpha;
    public float DrawScale;
    public Vector2 DrawOffset;
    public bool NoSurfaceOffset;
    public bool NoSurfaceLight;
    public float ParallaxYOffset;
    public bool NoParallaxY;
    public float ParallaxYFactor;
    public bool parallaxInBothWays;
    public Color DrawColor = Color.White;
    public bool IgnorePaletteShader;
    public float LocalParallaxSpeed=1f;
    public bool ignoreSkyColor;
    public override void Unload()
    {
        base.Unload();
        Layers = null;
    }

    public virtual bool IsActive()
    {
        return false;
    }
    public virtual void SetDrawDefaults()
    {
        if (Alpha <= 0)
            DrawColor = Color.White;
    }

    public virtual bool UseCustomDrawing() => false;
    /// <summary>
    /// The spritebatch must be begun and ended within this method, only executes if UseCustomDrawing returns true
    /// </summary>
    /// <param name="spriteBatch"></param>
    public virtual void Draw(SpriteBatch spriteBatch)
    {

    }
    public virtual int GetParallaxYStartHeight()
    {
        return (int)(Main.worldSurface * 16);
    }
    public void AddLayer(CustomBGLayer layer)
    {
        Layers.Add(layer);
    }

    public void AddFogLayer(Color startColor, Color endColor)
    {

    }

    public sealed override void SetupContent()
    {
        base.SetupContent();
        DrawScale = 1;
        SetStaticDefaults();
    }

    protected sealed override void Register()
    {
        ModTypeLookup<CustomBG>.Register(this);
    }

}

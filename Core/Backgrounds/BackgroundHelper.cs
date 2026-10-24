using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using Stellamod.Common.Shaders;
using System.Diagnostics;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Backgrounds;

public static class BackgroundHelper
{
    public record struct AtlassedBackgroundDraw(
        Asset<Texture2D> bg, int numBackgrounds, Color baseColor, Color fadeToColor, float alpha, Vector2 cameraMovement, Vector2 parallax, Vector2 parallaxOffset)
    {
        public static readonly AtlassedBackgroundDraw Default = new()
        {
            numBackgrounds = 3,
            baseColor = Color.White,
            fadeToColor = new Color(255, 255, 255, 25),
            parallax = new Vector2(0.1f),
            parallaxOffset = Vector2.Zero
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
            offsets[i] = Vector2.Lerp(new Vector2(0f, 0), new Vector2(0f, 1f), i / (float)numBackgrounds);
            parallax[i] = Vector2.Lerp(new Vector2(0.01f, 0.01f), Vector2.Zero, i / (float)numBackgrounds) * (cameraMovement) * 0.01f;
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
    public static void DrawWrappedAtlassedBackground(SpriteBatch spriteBatch, AtlassedBackgroundDraw draw, float scale = 1f)
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
            offsets[i] = Vector2.Lerp(new Vector2(0f, 0), new Vector2(0f, 1f), i / (float)numBackgrounds);
            parallax[i] = Vector2.Lerp(draw.parallax, Vector2.Zero, i / (float)numBackgrounds) * (cameraMovement) * 0.01f;
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
    public struct SurfaceWrappingBackground
    {
        public float Alpha;
        public Asset<Texture2D> Mid;
        public Asset<Texture2D> Close;
        public Asset<Texture2D> Far;
        public Asset<Texture2D> Underground;
        public int yParallaxStartHeight;
        public Vector2 ParallaxSpeed;
    }
    public static void DrawSurfaceWrappingBackgroundClampHeight(SpriteBatch spriteBatch, SurfaceWrappingBackground draw, int y)
    {
        LunarBackgroundShader backgroundShader = LunarBackgroundShader.Instance;
        Color fadeToColor = Color.White;
        fadeToColor.A = 0;
        backgroundShader.FadeToColor = fadeToColor;
        backgroundShader.Time = Main.GlobalTimeWrappedHourly * -1 * 0.1f;
        GraphicsDevice gDevice = Main.graphics.GraphicsDevice;

        //Prepare sampler states
        gDevice.Textures[1] = draw.Mid.Value;
        gDevice.SamplerStates[1] = SamplerState.PointClamp;

        gDevice.Textures[2] = draw.Far.Value;
        gDevice.SamplerStates[2] = SamplerState.PointClamp;

        gDevice.Textures[3] = draw.Underground.Value;
        gDevice.SamplerStates[3] = SamplerState.PointClamp;


        Vector2 closeParallax = new Vector2();
        closeParallax.X = Main.screenPosition.X * draw.ParallaxSpeed.X * 0.0002f;

 
        int worldSurfaceY = draw.yParallaxStartHeight;
     
        worldSurfaceY -= 0;
        int diffY = (int)(worldSurfaceY - Main.screenPosition.Y);
   
     
        closeParallax.Y = -diffY * 0.0001f;

        closeParallax.Y += 0.2f;

        Vector2 midParallax = new Vector2();
        midParallax.X = Main.screenPosition.X * draw.ParallaxSpeed.X * 0.0002f * 0.5f;
        midParallax.Y = closeParallax.Y;

        Vector2 farParallax = new Vector2();
        farParallax.X = Main.screenPosition.X * draw.ParallaxSpeed.X * 0.0002f * 0.25f + 0.25f;
        farParallax.Y = closeParallax.Y;
        farParallax.X += 0.35f;
        //Set up parallax
        Vector2[] parallax = new Vector2[3];
        parallax[0] = farParallax;
        parallax[1] = midParallax;
        parallax[2] = closeParallax;
        backgroundShader.Parallax = parallax;
        backgroundShader.DustTexture = AssetReferences.Assets.NoiseTextures.Clouds.Asset.Value;

        spriteBatch.Begin(SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.LinearWrap,
            DepthStencilState.None,
            RasterizerState.CullNone,
            backgroundShader.Effect);

        Color baseColor = Color.White;
        baseColor = Color.Lerp(baseColor, Main.ColorOfTheSkies, 0.5f);
        Color drawColor = baseColor * draw.Alpha;
        Vector2 drawScale = Vector2.One * 1.5f;
        Vector2 yLock = new Vector2(0, diffY);
        Rectangle drawRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        spriteBatch.Draw(
            draw.Close.Value,
            yLock,
            drawRect,
            drawColor,
            0f,
            default,
            scale: drawScale,
            SpriteEffects.None,
            0f
        );

        spriteBatch.End();
    }
    public static void DrawSurfaceWrappingBackground(SpriteBatch spriteBatch, SurfaceWrappingBackground draw)
    {
        LunarBackgroundShader backgroundShader = LunarBackgroundShader.Instance;
        Color fadeToColor = Color.White;
        fadeToColor.A = 0;
        backgroundShader.FadeToColor = fadeToColor;
        backgroundShader.Time = Main.GlobalTimeWrappedHourly * -1 * 0.1f;
        GraphicsDevice gDevice = Main.graphics.GraphicsDevice;

        //Prepare sampler states
        gDevice.Textures[1] = draw.Mid.Value;
        gDevice.SamplerStates[1] = SamplerState.PointClamp;

        gDevice.Textures[2] = draw.Far.Value;
        gDevice.SamplerStates[2] = SamplerState.PointClamp;

        gDevice.Textures[3] = draw.Underground.Value;
        gDevice.SamplerStates[3] = SamplerState.PointClamp;


        Vector2 closeParallax = new Vector2();
        closeParallax.X = Main.screenPosition.X * draw.ParallaxSpeed.X * 0.0002f;



        int worldSurfaceY = draw.yParallaxStartHeight;
        worldSurfaceY -= 0;
        int diffY = (int)(worldSurfaceY - Main.screenPosition.Y);
        closeParallax.Y = -diffY * 0.0001f;
        closeParallax.Y += 0.2f;

        Vector2 midParallax = new Vector2();
        midParallax.X = Main.screenPosition.X * draw.ParallaxSpeed.X * 0.0002f * 0.5f;
        midParallax.Y = closeParallax.Y;

        Vector2 farParallax = new Vector2();
        farParallax.X = Main.screenPosition.X * draw.ParallaxSpeed.X * 0.0002f * 0.25f + 0.25f;
        farParallax.Y = closeParallax.Y;
        farParallax.X += 0.35f;
        //Set up parallax
        Vector2[] parallax = new Vector2[3];
        parallax[0] = farParallax;
        parallax[1] = midParallax;
        parallax[2] = closeParallax;
        backgroundShader.Parallax = parallax;
        backgroundShader.DustTexture = AssetReferences.Assets.NoiseTextures.Clouds.Asset.Value;

        spriteBatch.Begin(SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.LinearWrap,
            DepthStencilState.None,
            RasterizerState.CullNone,
            backgroundShader.Effect);

        Color baseColor = Color.White;
        baseColor = Color.Lerp(baseColor, Main.ColorOfTheSkies, 0.5f);
        Color drawColor = baseColor * draw.Alpha;
        Vector2 drawScale = Vector2.One * 1.5f;

        Rectangle drawRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        spriteBatch.Draw(
            draw.Close.Value,
            Vector2.Zero,
            drawRect,
            drawColor,
            0f,
            default,
            scale: drawScale,
            SpriteEffects.None,
            0f
        );

        spriteBatch.End();
    }
    public static void DrawSurfaceAtlassedBackground(SpriteBatch spriteBatch, AtlassedBackgroundDraw draw)
    {
        Vector2[] parallax = new Vector2[10];
        Vector2[] offsets = new Vector2[10];

        int numBackgrounds = draw.numBackgrounds;
        Color fadeToColor = draw.fadeToColor;
        Vector2 cameraMovement = draw.cameraMovement;
        float alpha = draw.alpha;

        var bg = GaussianBlurHelper.ApplyBlurIfNecessary(draw.bg.Value);

        for (int i = 0; i < numBackgrounds; i++)
        {
            offsets[i] = Vector2.Lerp(new Vector2(0f, 0), new Vector2(0f, 1f), i / (float)numBackgrounds);
            parallax[i] = Vector2.Lerp(draw.parallax, Vector2.Zero, i / (float)numBackgrounds) * (cameraMovement) * 0.01f;
            parallax[i] += draw.parallaxOffset;
        }

        AtlassedParallaxingBackgroundShader backgroundShader = AtlassedParallaxingBackgroundShader.Instance;
        backgroundShader.Parallax = parallax;
        backgroundShader.Offsets = offsets;
        backgroundShader.Tiling = new Vector2(1f, numBackgrounds);
        backgroundShader.FadeToColor = fadeToColor;
        backgroundShader.Effect.CurrentTechnique = backgroundShader.Effect.Techniques["SurfaceDrawing"];


        spriteBatch.Begin(SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointWrap,
            DepthStencilState.None,
            RasterizerState.CullNone,
            backgroundShader.Effect,
            Main.GameViewMatrix.TransformationMatrix);

        Color baseColor = draw.baseColor;
        //  baseColor = Color.Lerp(baseColor, Main.ColorOfTheSkies, 0.5f);
        Color drawColor = baseColor * alpha;

        //view size scales with zoom scaling, by factoring this in the texture will always wrap much more than whatever your adapter is
        Rectangle drawRect = new Rectangle(0, 0, (int)Main.ViewSize.X, (int)Main.ViewSize.Y);
        for (int i = numBackgrounds - 1; i >= 0; i--)
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(bg, Main.screenPosition);
            drawer.drawOrigin = Vector2.Zero;
            drawer.scale = Vector2.One * 2;
            drawer.color = drawColor;
            drawer.VerticalFrame(i, numBackgrounds);
 //           drawer.sourceRect = drawRect;
            spriteBatch.Draw(drawer);
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

        var bg = GaussianBlurHelper.ApplyBlurIfNecessary(draw.bg.Value);

        for (int i = 0; i < numBackgrounds; i++)
        {
            offsets[i] = Vector2.Lerp(new Vector2(0f, 0), new Vector2(0f, 1f), i / (float)numBackgrounds);
            parallax[i] = Vector2.Lerp(draw.parallax, Vector2.Zero, i / (float)numBackgrounds) * (cameraMovement) * 0.01f;
            parallax[i] += draw.parallaxOffset;
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
            backgroundShader.Effect,
            Main.GameViewMatrix.TransformationMatrix);

        Color baseColor = draw.baseColor;
        //  baseColor = Color.Lerp(baseColor, Main.ColorOfTheSkies, 0.5f);
        Color drawColor = baseColor * alpha;

        //view size scales with zoom scaling, by factoring this in the texture will always wrap much more than whatever your adapter is
        Rectangle drawRect = new Rectangle(0, 0, (int)Main.ViewSize.X, (int)Main.ViewSize.Y);
        for (int i = numBackgrounds; i >= 0; i--)
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(bg, Main.screenPosition);
            drawer.drawOrigin = Vector2.Zero;
            drawer.scale = Vector2.One * 2;
            drawer.color = drawColor;
            drawer.VerticalFrame(i, numBackgrounds);
            drawer.sourceRect = drawRect;
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
            offsets[i] = Vector2.Lerp(new Vector2(0f, 0), new Vector2(0f, 1f), i / (float)numBackgrounds);
            parallax[i] = Vector2.Lerp(draw.parallax, Vector2.Zero, i / (float)numBackgrounds) * (cameraMovement) * 0.01f;
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

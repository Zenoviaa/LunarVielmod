using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.ZTileSystem;
using System;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Stellamod.Helpers;


public static class TextureExtensions
{
    public static Vector2 GetTexelSize(this Asset<Texture2D> textureAsset) => GetTexelSize(textureAsset.Value);
    public static Vector2 GetTexelSize(this Texture2D texture) => Vector2.One / texture.Size();
}

public static class ColorExtensions
{
    public static Vector3 ToHSV(this Color color) => DrawUtilities.RgbToHsv(color);
    public static void ScrollHue(this ref Color color, float degrees)
    {
        DrawUtilities.IncreaseHueBy(ref color, degrees);
    }
    public static Color Towards(this Color color, Color target, float lerp)
    {
        return Color.Lerp(color, target, lerp);
    }
}

/// <summary>
/// The section of the background to draw and the offset with it, make sure to use with a wrapping mode
/// </summary>
/// <param name="SourceRectangle"></param>
/// <param name="DrawOffset"></param>
public record struct BackgroundDrawParameters(Rectangle SourceRectangle, Vector2 DrawOffset);


/// <summary>
/// A collection of utility functions for drawing simple visual effects
/// </summary>
public static class DrawUtilities
{
    public delegate Color GetTrailColor(float completionRatio);
    public delegate float GetTrailWidth(float completionRatio);
    public static short[] PrepareIndicesForDrawingWrappedAround(int length)
    {
        int connectIndex = 0;
         short[] indicesSpan = new short[length * 6 ];
        int vertexCount = length * 4;
        for (int i = 0; i < indicesSpan.Length; i += 6)
        {
            indicesSpan[i] = (short)(connectIndex + 0);
            indicesSpan[i + 1] = (short)(connectIndex + 1);
            indicesSpan[i + 2] = (short)(connectIndex + 2);
            indicesSpan[i + 3] = (short)(connectIndex + 2);
            indicesSpan[i + 4] = (short)(connectIndex + 3);
            indicesSpan[i + 5] = (short)(connectIndex + 1);
            connectIndex += 4;
        }


        return indicesSpan;
    }

    /// <summary>
    /// Prepares indices for a set of quads
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static short[] PrepareIndicesForDrawing(int length)
    {
        int connectIndex = 0;
        Span<short> indicesSpan = stackalloc short[length * 6];
        for (int i = 0; i < indicesSpan.Length; i += 6)
        {
            indicesSpan[i] = (short)(connectIndex + 0);
            indicesSpan[i + 1] = (short)(connectIndex + 1);
            indicesSpan[i + 2] = (short)(connectIndex + 2);
            indicesSpan[i + 3] = (short)(connectIndex + 2);
            indicesSpan[i + 4] = (short)(connectIndex + 3);
            indicesSpan[i + 5] = (short)(connectIndex + 1);
            connectIndex += 4;
        }
        return indicesSpan.ToArray();
    }

    public static void DrawOutlinedRectangle(SpriteBatch spriteBatch, in Rectangle rectangle, in Color color, in int outlineSize)
    {
        var top = new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, outlineSize);
        var bottom = new Rectangle(rectangle.Left, rectangle.Bottom - outlineSize, rectangle.Width, outlineSize);
        var left = new Rectangle(rectangle.Left, rectangle.Top, outlineSize, rectangle.Height);
        var right = new Rectangle(rectangle.Right - outlineSize, rectangle.Top, outlineSize, rectangle.Height);

        var square = AssetReferences.Assets.GlowMasks.WhiteSquare.Asset.Value;
        spriteBatch.Draw(square, top, color);
        spriteBatch.Draw(square, bottom, color);
        spriteBatch.Draw(square, left, color);
        spriteBatch.Draw(square, right, color);

    }

    /// <summary>
    /// Draws indexed primitives with an effect then reverts back to the previous graphics device state afterward
    /// </summary>
    /// <typeparam name="VertexType"></typeparam>
    /// <param name="arr"></param>
    /// <param name="indices"></param>
    /// <param name="effect"></param>
    public static void DrawUserIndexedPrimitivesWithEffect<VertexType>(VertexType[] arr, short[] indices, Effect effect)
        where VertexType : struct, IVertexType
    {
        GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
        foreach (var pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();
        }
        graphicsDevice.DrawUserIndexedPrimitives<VertexType>(
          PrimitiveType.TriangleList, arr, 0, arr.Length, indices, 0, indices.Length / 3);
    }
    public static void DrawUserIndexedPrimitivesWithEffect<VertexType>(VertexType[] arr, int[] indices, Effect effect)
        where VertexType : struct, IVertexType
    {
        GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
        foreach (var pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();
        }
        graphicsDevice.DrawUserIndexedPrimitives<VertexType>(
          PrimitiveType.TriangleList, arr, 0, arr.Length, indices, 0, arr.Length / 2);
    }

    /// <summary>
    /// Prepares vertices for a basic trail
    /// </summary>
    /// <param name="oldPos"></param>
    /// <param name="colorFunc"></param>
    /// <param name="widthFunc"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static VertexPositionColorTexture[] PrepareSimpleTrailing(
        Vector2[] oldPos,
        Func<float, Color> colorFunc,
        Func<float, float> widthFunc, 
        Vector2? offset = null)
    {


        Vector2 trailOffset = offset == null ? Vector2.Zero : (Vector2)offset;
        float numPoints = oldPos.Length * 2;

        oldPos = DrawUtilities.PruneFarPoints(oldPos);

        if (oldPos.Length <= 2)
            return new VertexPositionColorTexture[4];

        numPoints = oldPos.Length * 2;
        Vector2[] trailingPoints = CommonDrawing.CatmullRomSplineInterpolation(oldPos, numPoints);
        return TrailVertexHelper.FillVertexArray(trailingPoints, colorFunc, widthFunc, trailOffset);
    }
    public static VertexPositionColorTexture[] PrepareSimpleTrailingNoSmoothing(
        Vector2[] oldPos,
        Func<float, Color> colorFunc,
        Func<float, float> widthFunc,
        Vector2? offset = null)
    {


        Vector2 trailOffset = offset == null ? Vector2.Zero : (Vector2)offset;
        float numPoints = oldPos.Length * 2;

        oldPos = DrawUtilities.PruneFarPoints(oldPos);

        if (oldPos.Length <= 2)
            return new VertexPositionColorTexture[4];


//        Vector2[] trailingPoints = CommonDrawing.CatmullRomSplineInterpolation(oldPos, numPoints);
        return TrailVertexHelper.FillVertexArray(oldPos, colorFunc, widthFunc, trailOffset);
    }
    public static VertexPositionColorTexture[] PreparedWrappedTrailing(
        Vector2[] oldPos,
        Func<float, Color> colorFunc,
        Func<float, float> widthFunc,
        Vector2? offset = null)
    {


        Vector2 trailOffset = offset == null ? Vector2.Zero : (Vector2)offset;


        oldPos = DrawUtilities.PruneFarPoints(oldPos);

        if (oldPos.Length <= 2)
            return new VertexPositionColorTexture[4];
              

        //        Vector2[] trailingPoints = CommonDrawing.CatmullRomSplineInterpolation(oldPos, numPoints);
        return TrailVertexHelper.FillVertexArrayWrapped(oldPos, colorFunc, widthFunc, trailOffset);
    }
    public static BackgroundDrawParameters CalculateScaledBackgroundDraw(Vector2 textureSize)
    {
        Vector2 drawOrigin = textureSize * 0.5f;
        int sw = Main.screenWidth;
        int sh = Main.screenHeight;
        Rectangle drawRectangle = new Rectangle(0, 0, sw * 2, sh * 2);
        return new BackgroundDrawParameters(drawRectangle, -new Vector2(sw / 2, sh / 2));
    }

    public static Vector2 RandomScreenPositionForForegroundParticles()
    {
        float xPosition = Main.rand.Next(-(int)(Main.screenWidth * 0.52f), (int)(Main.screenWidth * 0.52f));
        float yPosition = Main.rand.NextFloat(-Main.screenHeight * 0.52f, 0);
        Vector2 pos = Main.LocalPlayer.Center + new Vector2(xPosition, yPosition);
        return pos; 
    }

    public static Vector2 CalculateScreenOffset(Rectangle drawLocation, float scale = 1f)
    {
        Vector2 texelSize = Vector2.One / new Vector2(drawLocation.Width, drawLocation.Height);
        Vector2 screenoffset = Main.screenPosition * texelSize;
        screenoffset *= (1f / scale);
        return screenoffset;
    }
    public static Vector2[] PruneFarPoints(Vector2[] oldPos)
    {
        float tooFar = 1000 * 1000;
        List<Vector2> prunedPoints = new List<Vector2>();
        Vector2 prevAddedPoint = oldPos[0];
        for (int i = 0; i < oldPos.Length - 1; i++)
        {
            Vector2 cur = oldPos[i];
            Vector2 next = oldPos[i + 1];
            float d = Vector2.DistanceSquared(cur, next);
            if (cur == Vector2.Zero || d > tooFar)
            {
                break;
            }
            else
            {
                float d2 = Vector2.DistanceSquared(cur, prevAddedPoint);
                if (d2 < 4)
                    continue;

                prevAddedPoint = cur;
     
                prunedPoints.Add(cur);
            }

        }
        return prunedPoints.ToArray();

    }
    public static Vector2[] PruneFarPointsAddStartPoint(Vector2[] oldPos)
    {
        float tooFar = 1000 * 1000;
        List<Vector2> prunedPoints = new List<Vector2>();
        Vector2 prevAddedPoint = oldPos[0];
        for (int i = 0; i < oldPos.Length - 1; i++)
        {
            Vector2 cur = oldPos[i];
            Vector2 next = oldPos[i + 1];
            float d = Vector2.DistanceSquared(cur, next);
            if (cur == Vector2.Zero || d > tooFar)
            {
                break;
            }
            else
            {
                float d2 = Vector2.DistanceSquared(cur, prevAddedPoint);
                if (d2 < 4)
                    continue;

                prevAddedPoint = cur;

                prunedPoints.Add(cur);
            }

        }
        if(prunedPoints.Count > 0)
            prunedPoints.Add(prunedPoints[0]);
        return prunedPoints.ToArray();

    }
    /// <summary>
    /// Takes in a value between 0-1 and interpolates between colors throughout the input array
    /// </summary>
    /// <param name="lerpValue"></param>
    /// <param name="colors"></param>
    /// <returns></returns>
    public static Color InterpolateColorArray(float lerpValue, params Color[] colors)
    {
        int currentIndex = (int)MathF.Floor(lerpValue * colors.Length) % colors.Length;
        int nextIndex = (currentIndex + 1) % colors.Length;
        float stepSize = 1f / (float)colors.Length;

        Color currentColor = colors[currentIndex];
        Color nextColor = colors[nextIndex];

        float localProgress = (lerpValue - (stepSize * currentIndex)) / stepSize;
        Color interpolatedColor = Color.Lerp(currentColor, nextColor, localProgress);
        return interpolatedColor;
    }
    public static int[] QuadIndices(int vertexCount)
    {
        int connectIndex = 0;
        int[] indices = new int[vertexCount * 6];
        for (int i = 0; i < indices.Length; i += 6)
        {
            indices[i] = connectIndex + 0;
            indices[i + 1] = connectIndex + 1;
            indices[i + 2] = connectIndex + 2;
            indices[i + 3] = connectIndex + 2;
            indices[i + 4] = connectIndex + 3;
            indices[i + 5] = connectIndex + 1;
            connectIndex += 4;
        }
        return indices;
    }

    public static void IncreaseHueBy(ref Color color, float value)
    {
        float h, s, v;

        Vector3 hsv = RgbToHsv(color.R, color.G, color.B);
        hsv.X += value;

        float r, g, b;

        Vector3 rgb = HsvToRgb(hsv);


        color.R = (byte)(rgb.X);
        color.G = (byte)(rgb.Y);
        color.B = (byte)(rgb.Z);
    }

    //Reference: https://www.geeksforgeeks.org/dsa/program-change-rgb-color-model-hsv-color-model/

    public static Vector3 RgbToHsv(Color color) => RgbToHsv(color.ToVector3());
    public static Vector3 RgbToHsv(Vector3 rgb) => RgbToHsv(rgb.X, rgb.Y, rgb.Z);
    public static Vector3 RgbToHsv(float r, float g, float b)
    {
        Vector3 hsv;
        float min, max, delta;
        min = Math.Min(Math.Min(r, g), b);
        max = Math.Max(Math.Max(r, g), b);
        hsv.Z = max;         
        delta = max - min;
        if (max != 0)
        {
            hsv.Y = delta / max;     

            if (r == max)
                hsv.X = (g - b) / delta;      
            else if (g == max)
                hsv.X = 2 + (b - r) / delta; 
            else
                hsv.X = 4 + (r - g) / delta;   
            hsv.X *= 60;
            if (hsv.X < 0)
                hsv.X += 360;
        }
        else
        {
            hsv.Y = 0;
            hsv.X = -1;
        }
        return hsv;

    }
    public static Vector3 HsvToRgb(Vector3 hsv)
    {
        float h = hsv.X;
        float s = hsv.Y;
        float v = hsv.Z;

        float r, g, b;


        h = h - ((int)(h / 360) * 360);

        int i;
        float f, p, q, t;
        if (s == 0)
        {
            r = g = b = v;
            return new Vector3(r, g, b);
        }
        h /= 60;          

        i = (int)h;
        f = h - i;     
        p = v * (1 - s);
        q = v * (1 - s * f);
        t = v * (1 - s * (1 - f));
        switch (i)
        {
            case 0:
                r = v;
                g = t;
                b = p;
                break;
            case 1:
                r = q;
                g = v;
                b = p;
                break;
            case 2:
                r = p;
                g = v;
                b = t;
                break;
            case 3:
                r = p;
                g = q;
                b = v;
                break;
            case 4:
                r = t;
                g = p;
                b = v;
                break;
            default:    
                r = v;
                g = p;
                b = q;
                break;
        }
        return new Vector3(r, g, b);
    }

    public static Vector2[] InterpolateBetweenPoints(Vector2 start, Vector2 end, float numPoints)
    {
        Vector2[] points = new Vector2[(int)numPoints];
        for(int i = 0; i < points.Length; i++)
        {
            ref Vector2 p = ref points[i];
            p = Vector2.Lerp(start, end, (float)i / (float)points.Length);
        }
        return points;
    }
    public static void InterpolateBetweenPointsNonAlloc(ref Vector2[] points, Vector2 start, Vector2 end)
    {
        for (int i = 0; i < points.Length; i++)
        {
            ref Vector2 p = ref points[i];
            p = Vector2.Lerp(start, end, (float)i / (float)points.Length);
        }
    }
    public static void DrawSpriteAfterImage(SpriteBatch spriteBatch, Projectile projectile, Color startColor, Color endColor, float alpha)
    {
        SpritebatchDrawer spriteDrawer = SpritebatchDrawer.FromProjectile(projectile);
        DrawSpriteAfterImage(spriteBatch, spriteDrawer, projectile.oldPos, projectile.oldRot, startColor, endColor, alpha, projectile.Size * 0.5f);
    }

    public static void DrawSpriteAfterImage(SpriteBatch spriteBatch, SpritebatchDrawer spriteDrawer, Vector2[] oldPos, float[] oldRot, Color startColor, Color endColor, float alpha, Vector2? offset = null)
    {
        Vector2 o = offset.HasValue ? offset.Value : Vector2.Zero;
        for (int i = 0; i < oldPos.Length; i++)
        {
            Vector2 pos = oldPos[i] + o;
            spriteDrawer.rotation = oldRot[i];
            spriteDrawer.worldPosition = pos;
            float ratio = (float)i / (float)oldPos.Length;
            spriteDrawer.color = Color.Lerp(startColor, endColor, ratio) * alpha;
            spriteBatch.Draw(spriteDrawer);
        }
    }

    public static Vector2[] TrailLocalRectanglePoints(in Vector2[] oldPos, in Vector2 center, Rectangle worldRectangle)
    {

        Vector2[] particles = new Vector2[oldPos.Length];
        for (int i = 0; i < particles.Length; i++)
        {
            ref Vector2 particle = ref particles[i];
            particle = oldPos[i];
            particle = DrawUtilities.WorldToScreenCoordinates(particle, worldRectangle);
        };
        return (particles);
    }


    public static Vector2[] TrailLocalRectanglePoints(in Vector2[] oldPos, in Vector2 center, Rectangle worldRectangle, Vector2 offset)
    {

        Vector2[] particles = new Vector2[oldPos.Length];
        for (int i = 0; i < particles.Length; i++)
        {
            ref Vector2 particle = ref particles[i];
            particle = oldPos[i] + offset;
            particle = DrawUtilities.WorldToScreenCoordinates(particle, worldRectangle);
        }
        ;
        return (particles);
    }

    /// <summary>
    /// Returns normalized trail coordinates between 0-1 within the rectangle boundaries of the projectile
    /// This allow for rendering a trail in a single quad with some shaders, no vertices required!
    /// </summary>
    /// <param name="projectile"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    public static (Vector2[], Rectangle) TrailLocalRectanglePoints(Projectile projectile, float padding = 32)
    {
        return TrailLocalRectanglePoints(projectile.oldPos, projectile.Center, padding);
    }

    /// <summary>
    /// Returns normalized trail coordinates between 0-1 within the rectangle boundaries of the projectile
    /// This allow for rendering a trail in a single quad with some shaders, no vertices required!
    /// </summary>
    /// <param name="projectile"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    public static (Vector2[], Rectangle) TrailLocalRectanglePoints(in Vector2[] oldPos, in Vector2 center, in float padding = 32)
    {
        Vector2 min = center;
        Vector2 max = center;
        for (int i = 0; i < oldPos.Length; i++)
        {
            min = Vector2.Min(min, oldPos[i]);
            max = Vector2.Max(max, oldPos[i]);
        }
        min -= new Vector2(padding);
        max += new Vector2(padding);

        int sizeX = (int)(max.X - min.X);
        int sizeY = (int)(max.Y - min.Y);
        int size = Math.Max(sizeX, sizeY);

        Rectangle worldRectangle = new Rectangle(
            (int)min.X, (int)min.Y, size, size);

        Vector2[] particles = new Vector2[oldPos.Length];
        for (int i = 0; i < particles.Length; i++)
        {
            ref Vector2 particle = ref particles[i];
            particle = oldPos[i];
            particle = DrawUtilities.WorldToScreenCoordinates(particle, worldRectangle);
        }
        Rectangle screenRectangle = worldRectangle;
        screenRectangle.X -= (int)Main.screenPosition.X;
        screenRectangle.Y -= (int)Main.screenPosition.Y;
        return (particles, screenRectangle);
    }
    public static Vector2 RandomPositionInNPCRect(this NPC npc)
    {
        Vector2 pos = new Vector2();
        pos.X = Main.rand.Next(0, npc.width);
        pos.Y = Main.rand.Next(0, npc.height);
        pos += npc.position;
        return pos;
    }

    public static Vector2 TexelSize => Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight);
    public static Vector2 WorldToScreenCoordinates(Vector2 worldPos)
    {
        Vector2 screenPos = new Vector2();
        screenPos.X = (worldPos.X - Main.screenPosition.X) / (float)Main.screenWidth;
        screenPos.Y = (worldPos.Y - Main.screenPosition.Y) / (float)Main.screenHeight;
        return screenPos;
    }
    public static Vector2 WorldToScreenCoordinates(Vector2 worldPos, Rectangle worldDrawRect)
    {
        Vector2 screenPos = new Vector2();
        screenPos.X = (worldPos.X - worldDrawRect.X) / (float)worldDrawRect.Width;
        screenPos.Y = (worldPos.Y - worldDrawRect.Y) / (float)worldDrawRect.Height;
        return screenPos;
    }
    public static void DrawScreenRectangle(this SpriteBatch sb, Color? overrideColor = null)
    {
        Color drawColor = overrideColor.HasValue ? overrideColor.Value : Color.White;
        Rectangle drawRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        sb.Draw(TextureAssets.BlackTile.Value, drawRect, drawColor);
    }

    public static Rectangle CenterRectangle(Rectangle rectangle, int newWidth, int newHeight)
    {
        Vector2 center = rectangle.Center();
        return CenterRectangle(center, newWidth, newHeight);
    }
    public static Rectangle CenterRectangle(Vector2 worldPosition, int width, int height)
    {
        int top = (int)worldPosition.Y - height / 2;
        int left = (int)worldPosition.X - width / 2;
        return new Rectangle(left, top, width, height);
    }

    public static void DrawScreenRectangle(this SpriteBatch sb, Rectangle screenRect, Color? overrideColor = null)
    {
        Color drawColor = overrideColor.HasValue ? overrideColor.Value : Color.White;
        sb.Draw(TextureAssets.BlackTile.Value, screenRect, drawColor);
    }

    public static SpritebatchDrawer GetDrawer(this Asset<Texture2D> textureAsset, Vector2 worldPosition)
    {
        return SpritebatchDrawer.FromTextureAsset(textureAsset, worldPosition);
    }

    public static void Draw(this SpriteBatch spriteBatch, SpritebatchDrawer drawer)
    {
        if (drawer.dstRect.HasValue)
        {
            spriteBatch.Draw(drawer.texture, drawer.dstRect.Value, drawer.sourceRect, drawer.color, drawer.rotation, drawer.drawOrigin, drawer.spriteEffects, 0);
            return;
        }
        spriteBatch.Draw(drawer.texture, drawer.worldPosition - Main.screenPosition, drawer.sourceRect, drawer.color, drawer.rotation, drawer.drawOrigin, drawer.scale, drawer.spriteEffects, 0);
    }

    /// <summary>
    /// Draws an after image trail
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="modProjectile"></param>
    public static void DrawBasicAfterImage(SpriteBatch spriteBatch, Projectile projectile, GetTrailColor getTrailColor, GetTrailWidth getTrailWidth)
    {
        Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
        SpritebatchDrawer spritebatchDrawer = SpritebatchDrawer.FromProjectile(projectile);

        //Create an after image effect
        //Gonna extract this to a function
        for (int i = 0; i < projectile.oldPos.Length; i++)
        {
            float ratio = i / (float)projectile.oldPos.Length;
            Color afterImageColor = getTrailColor(ratio);
            float afterImageScale = getTrailWidth(ratio);

            spritebatchDrawer.worldPosition = projectile.oldPos[i] + projectile.Size * 0.5f;
            spritebatchDrawer.color = afterImageColor;
            spritebatchDrawer.scale = Vector2.One * afterImageScale;
            spritebatchDrawer.rotation = projectile.oldRot[i];
            spriteBatch.Draw(spritebatchDrawer);
        }
    }
    public static void DrawBasicAfterImage(SpriteBatch spriteBatch, NPC npc, GetTrailColor getTrailColor, GetTrailWidth getTrailWidth, SpritebatchDrawer spritebatchDrawer)
    {
        //Create an after image effect
        //Gonna extract this to a function
        for (int i = 0; i < npc.oldPos.Length; i++)
        {
            float ratio = i / (float)npc.oldPos.Length;
            Color afterImageColor = getTrailColor(ratio);
            float afterImageScale = getTrailWidth(ratio);

            spritebatchDrawer.worldPosition = npc.oldPos[i] + npc.Size * 0.5f;
            spritebatchDrawer.color = afterImageColor;
            spritebatchDrawer.scale = Vector2.One * afterImageScale;
            spritebatchDrawer.rotation = npc.oldRot[i];
            spriteBatch.Draw(spritebatchDrawer);
        }
    }
    public static void DrawBasicAfterImage(SpriteBatch spriteBatch, Projectile projectile, GetTrailColor getTrailColor, GetTrailWidth getTrailWidth, SpritebatchDrawer spritebatchDrawer)
    {
        Texture2D texture = TextureAssets.Projectile[projectile.type].Value;

        //Create an after image effect
        //Gonna extract this to a function
        for (int i = 0; i < projectile.oldPos.Length; i++)
        {
            float ratio = i / (float)projectile.oldPos.Length;
            Color afterImageColor = getTrailColor(ratio);
            float afterImageScale = getTrailWidth(ratio);

            spritebatchDrawer.worldPosition = projectile.oldPos[i] + projectile.Size * 0.5f;
            spritebatchDrawer.color = afterImageColor;
            spritebatchDrawer.scale = Vector2.One * afterImageScale;
            spritebatchDrawer.rotation = projectile.oldRot[i];
            spriteBatch.Draw(spritebatchDrawer);
        }
    }

    public static void DrawBasicGlow(SpriteBatch spriteBatch, Vector2 position, float scale, Color color)
    {
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, position);
        glowDrawer.color = color;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= scale;
        spriteBatch.Draw(glowDrawer);
    }
}

public static class SpriteBatchExtensions
{
    /// <summary>
    /// Ends the spritebatch and spits out the parameters it was using to draw
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="parameters"></param>
    public static void EndOut(this SpriteBatch spriteBatch, out SpritebatchParams parameters)
    {
        parameters = spriteBatch.Parameters;
        spriteBatch.End();
   
    }

    extension(SpriteBatch spriteBatch)
    {
        /// <summary>
        /// Retrieves the parameters from the sprite batch
        /// </summary>
        public SpritebatchParams Parameters => SpritebatchParams.FromSpritebatch(spriteBatch);
    }
}

/// <summary>
/// A collection of helper function for the sprite batch
/// </summary>
public static class SB
{
    public static SpritebatchParams InWorldScaled
    {
        get
        {
            SpritebatchParams starter = new SpritebatchParams();
            starter.blendState = BlendState.AlphaBlend;
            starter.samplerState = SamplerState.PointClamp;
            starter.sortMode = SpriteSortMode.Deferred;
            starter.depthStencilState = DepthStencilState.None;
            starter.effect = null!;
            starter.matrix = Main.GameViewMatrix.TransformationMatrix;
            starter.rasterizerState = Main.Rasterizer;
            return starter;
        }
    }
    public static SpritebatchParams InWorldUnscaled
    {
        get
        {
            SpritebatchParams starter = new SpritebatchParams();
            starter.blendState = BlendState.AlphaBlend;
            starter.samplerState = SamplerState.PointClamp;
            starter.sortMode = SpriteSortMode.Deferred;
            starter.depthStencilState = DepthStencilState.None;
            starter.effect = null!;
            starter.matrix =Matrix.identity;
            starter.rasterizerState = Main.Rasterizer;
            return starter;
        }
    }
}
/// <summary>
/// Accesses the current parameters of the spritebatch
/// </summary>
public struct SpritebatchParams
{
    public BlendState blendState;
    public SamplerState samplerState;
    public RasterizerState rasterizerState;
    public DepthStencilState depthStencilState;
    public Effect effect;
    public SpriteSortMode sortMode;
    public Matrix matrix;
    public static SpritebatchParams FromSpritebatch(SpriteBatch spriteBatch)
    {
        SpritebatchParams starter = new SpritebatchParams();
        starter.blendState = spriteBatch.blendState;
        starter.samplerState = spriteBatch.samplerState;
        starter.sortMode = spriteBatch.sortMode;
        starter.depthStencilState = spriteBatch.depthStencilState;
        starter.effect = spriteBatch.customEffect;
        starter.matrix = spriteBatch.transformMatrix;
        starter.rasterizerState = spriteBatch.rasterizerState;
        return starter;
    }


    public void Begin(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(
            sortMode,
            blendState,
            samplerState,
            depthStencilState,
            rasterizerState,
            effect,
            matrix);
    }



    public static SpritebatchParams InWorldAndZoomed()
    {
        SpritebatchParams starter = new SpritebatchParams();
        starter.blendState = BlendState.AlphaBlend;
        starter.samplerState = SamplerState.PointClamp;
        starter.sortMode = SpriteSortMode.Deferred;
        starter.depthStencilState = DepthStencilState.None;
        starter.effect = null!;
        starter.matrix = Main.GameViewMatrix.TransformationMatrix;
        starter.rasterizerState = Main.Rasterizer;
        return starter;
    }
    public static SpritebatchParams UI
    {
        get
        {
            SpritebatchParams starter = new SpritebatchParams();
            starter.blendState = BlendState.AlphaBlend;
            starter.samplerState = SamplerState.LinearClamp;
            starter.sortMode = SpriteSortMode.Deferred;
            starter.depthStencilState = DepthStencilState.None;
            starter.effect = null!;
            starter.matrix = Main.UIScaleMatrix;
            starter.rasterizerState = Main.Rasterizer;
            return starter;
        }
    }

}

public static class SpritebatchDrawExtensions
{
    public static void Begin(this SpriteBatch spriteBatch, SpritebatchParams spritebatchParams) => spritebatchParams.Begin(spriteBatch);
    public static SpritebatchContext Ctx(this SpriteBatch spriteBatch, SpritebatchParams requiredParameters) => new SpritebatchContext(spriteBatch, requiredParameters);
}

public struct SpritebatchContext : IDisposable
{
    private SpritebatchParams? _oldParameters;
    private SpriteBatch? _spriteBatch;

    public SpritebatchParams spriteBatchParameters;
    public SpritebatchContext(SpriteBatch spriteBatch, SpritebatchParams requiredParameters)
    {
        spriteBatchParameters = requiredParameters;
        _spriteBatch = spriteBatch;
        bool beginCalled = spriteBatch.beginCalled;
        if (beginCalled)
        {
            _oldParameters = SpritebatchParams.FromSpritebatch(spriteBatch);
            spriteBatch.End();
        }
        spriteBatch.Begin(spriteBatchParameters);
    }


    public void EndAndTryResume(SpriteBatch spriteBatch)
    {
        //This should only be used with a using statement, which means this will always be called immediately after it exits scope
        //So begin can be assumed to have been called here
        spriteBatch.End();

        //If there's old parameters that means a batch is being interuppted, so we shouldresume it right after
        //It remembers the old parameters so it doesn't matter where this is being called!
        if (_oldParameters.HasValue)
        {
            spriteBatch.Begin(_oldParameters.Value);
            _oldParameters = null;
        }
    }

    public void Dispose()
    {
        EndAndTryResume(_spriteBatch!);
    }
}

/// <summary>
/// Encapsulates parameters for starting a spritebatch so we don't have to call begin and end everytime
/// This should only be used with a using statement after using one of the static .Begin() functions
/// </summary>
public struct SpritebatchStarter :
    IDisposable
{
    private SpritebatchParams? _oldParameters;
    private SpriteBatch? _spriteBatch;

    public required SpritebatchParams spriteBatchParameters;

    //TODO: check if parameters match and do not restart the spritebatch if they do

    /// <summary>
    /// Begins a spritebatch with these parameters, if begin has already been called it will be ended
    /// </summary>
    /// <param name="spriteBatch"></param>
    public void Begin(SpriteBatch spriteBatch)
    {
        _spriteBatch = spriteBatch;
        bool beginCalled = spriteBatch.beginCalled;
        if (beginCalled)
        {
            _oldParameters = SpritebatchParams.FromSpritebatch(spriteBatch);
            spriteBatch.End();
        }
        spriteBatch.Begin(spriteBatchParameters);
    }

    public void EndAndTryResume(SpriteBatch spriteBatch)
    {
        //This should only be used with a using statement, which means this will always be called immediately after it exits scope
        //So begin can be assumed to have been called here
        spriteBatch.End();

        //If there's old parameters that means a batch is being interuppted, so we shouldresume it right after
        //It remembers the old parameters so it doesn't matter where this is being called!
        if (_oldParameters.HasValue)
        {
            spriteBatch.Begin(_oldParameters.Value);
            _oldParameters = null;
        }
    }

    public static SpritebatchStarter Begin(SpriteBatch spriteBatch, SpritebatchParams spritebatchParams)
    {
        SpritebatchStarter starter = new SpritebatchStarter()
        {
            spriteBatchParameters = spritebatchParams
        };
        starter._oldParameters = null;
        starter.Begin(spriteBatch);
        return starter;
    }

    public void Dispose()
    {
        EndAndTryResume(_spriteBatch!);
    }
}

/// <summary>  
/// Helper struct for using the spritebatch to draw things
/// </summary>
public struct SpritebatchDrawer
{
    public Texture2D texture;
    public Vector2 worldPosition;
    public Rectangle? dstRect;
    public Rectangle? sourceRect;
    public Color color;
    public float rotation;
    public Vector2 drawOrigin;
    public SpriteEffects spriteEffects;
    public Vector2 scale;
    public void Flip(ref float xPosition)
    {
        xPosition = sourceRect.Value.Width - xPosition;
    }

    public void VerticalFrame(int frameIndex, int frameCount)
    {
        sourceRect = texture.GetFrame(frameIndex, frameCount);
    }

    public void LeftCenterOrigin()
    {
        Vector2 normalizedOrigin = new Vector2(0f, 0.5f);
        if (sourceRect.HasValue)
        {
            Rectangle rectangle = sourceRect.Value;
            drawOrigin = new Vector2(rectangle.Width, rectangle.Height) * normalizedOrigin;
        }
        else
        {
            drawOrigin = new Vector2(texture.Width, texture.Height) * normalizedOrigin;
        }
    }

    public void BottomLeftOrigin()
    {
        Vector2 normalizedOrigin = new Vector2(0f, 1f);
        if (sourceRect.HasValue)
        {
            Rectangle rectangle = sourceRect.Value;
            drawOrigin = new Vector2(rectangle.Width, rectangle.Height) * normalizedOrigin;
        }
        else
        {
            drawOrigin = new Vector2(texture.Width, texture.Height) * normalizedOrigin;
        }
    }
    public void RightCenterOrigin()
    {
        Vector2 normalizedOrigin = new Vector2(1f, 0.5f);
        if (sourceRect.HasValue)
        {
            Rectangle rectangle = sourceRect.Value;
            drawOrigin = new Vector2(rectangle.Width, rectangle.Height) * normalizedOrigin;
        }
        else
        {
            drawOrigin = new Vector2(texture.Width, texture.Height) * normalizedOrigin;
        }
    }
    public void BottomCenterOrigin()
    {
        if (sourceRect.HasValue)
        {
            Rectangle rectangle = sourceRect.Value;
            drawOrigin = new Vector2(rectangle.Width * 0.5f, rectangle.Height);
        }
        else
        {
            drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height);
        }
    }
    public void Origin(float xPct, float yPct)
    {
        if (sourceRect.HasValue)
        {
            Rectangle rectangle = sourceRect.Value;
            drawOrigin = new Vector2(rectangle.Width * xPct, rectangle.Height * yPct);
        }
        else
        {
            drawOrigin = new Vector2(texture.Width * xPct, texture.Height * yPct);
        }
    }
    public void TopCenterOrigin()
    {
        if (sourceRect.HasValue)
        {
            Rectangle rectangle = sourceRect.Value;
            drawOrigin = new Vector2(rectangle.Width * 0.5f, 0);
        }
        else
        {
            drawOrigin = new Vector2(texture.Width * 0.5f, 0);
        }
    }
    public void CenterOrigin()
    {
        if (sourceRect.HasValue)
        {
            Rectangle rectangle = sourceRect.Value;
            drawOrigin = new Vector2(rectangle.Width * 0.5f, rectangle.Height * 0.5f);
        }
        else
        {
            drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
        }
    }

    public static SpritebatchDrawer FromZTileDraw(Asset<Texture2D> textureAsset, ZTileDrawData drawData)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(textureAsset, drawData.drawPosition + Main.screenPosition);
        drawer.sourceRect = drawData.frame;
        drawer.color = drawData.drawColor;
        drawer.rotation = drawData.drawRotation;
        drawer.drawOrigin = drawData.drawOrigin;
        drawer.scale = drawData.drawScale;
        drawer.spriteEffects = drawData.spriteEffects;
        return drawer;
    }

    public static SpritebatchDrawer FromTextureAsset(Asset<Texture2D> textureAsset, Vector2 worldPosition)
    {
        SpritebatchDrawer spritebatchDrawer = new SpritebatchDrawer();
        spritebatchDrawer.texture = textureAsset.Value;
        spritebatchDrawer.worldPosition = worldPosition;
        spritebatchDrawer.sourceRect = null;
        spritebatchDrawer.color = Color.White.MultiplyRGB(Lighting.GetColor(worldPosition.ToTileCoordinates()));
        spritebatchDrawer.rotation = 0;
        spritebatchDrawer.drawOrigin = textureAsset.Size() * 0.5f;
        spritebatchDrawer.spriteEffects = SpriteEffects.None;
        spritebatchDrawer.scale = Vector2.One;
        return spritebatchDrawer;
    }
    public static SpritebatchDrawer FromTextureAsset(Texture2D textureAsset, Vector2 worldPosition)
    {
        SpritebatchDrawer spritebatchDrawer = new SpritebatchDrawer();
        spritebatchDrawer.texture = textureAsset;
        spritebatchDrawer.worldPosition = worldPosition;
        spritebatchDrawer.sourceRect = null;
        spritebatchDrawer.color = Color.White.MultiplyRGB(Lighting.GetColor(worldPosition.ToTileCoordinates()));
        spritebatchDrawer.rotation = 0;
        spritebatchDrawer.drawOrigin = textureAsset.Size() * 0.5f;
        spritebatchDrawer.spriteEffects = SpriteEffects.None;
        spritebatchDrawer.scale = Vector2.One;
        return spritebatchDrawer;
    }


    public static SpritebatchDrawer FromProjectile(Projectile projectile)
    {
        SpritebatchDrawer spritebatchDrawer = new SpritebatchDrawer();
        spritebatchDrawer.texture = TextureAssets.Projectile[projectile.type].Value;
        spritebatchDrawer.worldPosition = projectile.Center;
        spritebatchDrawer.sourceRect = projectile.Frame();
        spritebatchDrawer.color = Color.White.MultiplyRGB(Lighting.GetColor(projectile.position.ToTileCoordinates()));
        spritebatchDrawer.rotation = projectile.rotation;
        spritebatchDrawer.drawOrigin = spritebatchDrawer.sourceRect.Value.Size() * 0.5f;
        spritebatchDrawer.spriteEffects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        spritebatchDrawer.scale = Vector2.One * projectile.scale;
        return spritebatchDrawer;
    }


    public static SpritebatchDrawer FromNPC(NPC npc)
    {
        SpritebatchDrawer spritebatchDrawer = new SpritebatchDrawer();
        spritebatchDrawer.texture = TextureAssets.Npc[npc.type].Value;
        spritebatchDrawer.worldPosition = npc.Center;
        spritebatchDrawer.sourceRect = npc.frame;
        spritebatchDrawer.color = Color.White.MultiplyRGB(Lighting.GetColor(npc.position.ToTileCoordinates()));
        spritebatchDrawer.rotation = npc.rotation;
        spritebatchDrawer.drawOrigin = spritebatchDrawer.sourceRect.Value.Size() * 0.5f;
        spritebatchDrawer.spriteEffects = npc.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        spritebatchDrawer.scale = Vector2.One * npc.scale;
        return spritebatchDrawer;
    }
}
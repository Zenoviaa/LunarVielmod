using ReLogic.Content;
using System;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
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
/// A collection of utility functions for drawing simple visual effects
/// </summary>
public static class DrawUtilities
{
    public delegate Color GetTrailColor(float completionRatio);
    public delegate float GetTrailWidth(float completionRatio);


    public static Vector2[] PruneFarPoints(Vector2[] oldPos)
    {

        List<Vector2> prunedPoints = new List<Vector2>();
        Vector2 prevAddedPoint = oldPos[0];
        for (int i = 0; i < oldPos.Length - 1; i++)
        {
            Vector2 cur = oldPos[i];
            Vector2 next = oldPos[i + 1];
            float d = Vector2.Distance(cur, next);
            if (cur == Vector2.Zero || d > 1000)
            {
                break;
            }
            else
            {
                float d2 = Vector2.DistanceSquared(cur, prevAddedPoint);
                prevAddedPoint = cur;
                prunedPoints.Add(cur);
            }

        }
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
        if (drawer.blackIsTransparency)
            drawer.color.A = 0;
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
}

/// <summary>
/// Accesses the current parameters of the spritebatch
/// </summary>
public struct SpritebatchParams
{
    private readonly static FieldInfo _blendStateField;
    private readonly static FieldInfo _samplerStateField;
    private readonly static FieldInfo _depthStencilStateField;
    private readonly static FieldInfo _rasterizerStateField;
    private readonly static FieldInfo _matrixField;
    private readonly static FieldInfo _effectField;
    private readonly static FieldInfo _beginCalledInfoBackingField;
    private readonly static FieldInfo _sortModeField;
    static SpritebatchParams()
    {
        //Cache reflection fields
        _sortModeField = GetPrivateSpritebatchField("sortMode");
        _beginCalledInfoBackingField = GetPrivateSpritebatchField("beginCalled");
        _effectField = GetPrivateSpritebatchField("customEffect");
        _matrixField = GetPrivateSpritebatchField("transformMatrix");
        _rasterizerStateField = GetPrivateSpritebatchField("rasterizerState");
        _depthStencilStateField = GetPrivateSpritebatchField("depthStencilState");
        _samplerStateField = GetPrivateSpritebatchField("samplerState");
        _blendStateField = GetPrivateSpritebatchField("blendState");
        _sortModeField = GetPrivateSpritebatchField("sortMode");
    }

    public BlendState blendState;
    public SamplerState samplerState;
    public RasterizerState rasterizerState;
    public DepthStencilState depthStencilState;
    public Effect effect;
    public SpriteSortMode sortMode;
    public Matrix matrix;
    private static FieldInfo GetPrivateSpritebatchField(string name)
    {
        return typeof(SpriteBatch).GetField(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)!;
    }

    public static SpriteSortMode GetSortMode(SpriteBatch spriteBatch)
    {
        return (SpriteSortMode)_sortModeField.GetValue(spriteBatch)!;
    }

    public static BlendState GetBlendState(SpriteBatch spriteBatch)
    {
        return (BlendState)_blendStateField.GetValue(spriteBatch)!;
    }

    public static SamplerState GetSamplerState(SpriteBatch spriteBatch)
    {
        return (SamplerState)_samplerStateField.GetValue(spriteBatch)!;
    }

    public static DepthStencilState GetDepthStencilState(SpriteBatch spriteBatch)
    {
        return (DepthStencilState)_depthStencilStateField.GetValue(spriteBatch)!;
    }

    public static RasterizerState GetRasterizerState(SpriteBatch spriteBatch)
    {
        return (RasterizerState)_rasterizerStateField.GetValue(spriteBatch)!;
    }

    public static Matrix GetTransformMatrix(SpriteBatch spriteBatch)
    {
        return (Matrix)_matrixField.GetValue(spriteBatch)!;
    }

    public static Effect GetEffect(SpriteBatch spriteBatch)
    {
        return (Effect)_effectField.GetValue(spriteBatch)!;
    }

    public static bool GetBeginCalled(SpriteBatch spriteBatch)
    {
        bool beginCalled = (bool)_beginCalledInfoBackingField.GetValue(spriteBatch)!;
        return beginCalled;
    }
    public static SpritebatchParams FromSpritebatch(SpriteBatch spriteBatch)
    {
        SpritebatchParams starter = new SpritebatchParams();
        starter.blendState = GetBlendState(spriteBatch);
        starter.samplerState = GetSamplerState(spriteBatch);
        starter.sortMode = GetSortMode(spriteBatch);
        starter.depthStencilState = GetDepthStencilState(spriteBatch);
        starter.effect = GetEffect(spriteBatch);
        starter.matrix = GetTransformMatrix(spriteBatch);
        starter.rasterizerState = GetRasterizerState(spriteBatch);
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
}

public static class SpritebatchDrawExtensions
{
    public static void Begin(this SpriteBatch spriteBatch, SpritebatchParams spritebatchParams) => spritebatchParams.Begin(spriteBatch);
}

public class SpritebatchContext : IDisposable
{
    private SpritebatchParams? _oldParameters;
    private SpriteBatch? _spriteBatch;

    public SpritebatchParams spriteBatchParameters;
    public SpritebatchContext(SpriteBatch spriteBatch, SpritebatchParams requiredParameters)
    {
        spriteBatchParameters = requiredParameters;
        _spriteBatch = spriteBatch;
        bool beginCalled = SpritebatchParams.GetBeginCalled(spriteBatch);
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
        bool beginCalled = SpritebatchParams.GetBeginCalled(spriteBatch);
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
    public bool blackIsTransparency;
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
using Microsoft.CodeAnalysis.Operations;
using ReLogic.Content;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;

namespace Stellamod.Helpers;

/// <summary>
/// A collection of utility functions for drawing simple visual effects
/// </summary>
public static class DrawUtilities
{
    public delegate Color GetTrailColor(float completionRatio);
    public delegate float GetTrailWidth(float completionRatio);

    public static Vector2 RandomPositionInNPCRect(this NPC npc)
    {
        Vector2 pos = new Vector2();
        pos.X = Main.rand.Next(0, npc.width);
        pos.Y = Main.rand.Next(0, npc.height);
        pos += npc.position;
        return pos;
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
            float ratio = (float)i / (float)projectile.oldPos.Length;
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
            float ratio = (float)i / (float)npc.oldPos.Length;
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
            float ratio = (float)i / (float)projectile.oldPos.Length;
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
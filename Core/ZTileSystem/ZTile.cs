using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using System.Text;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.ZTileSystem;

public struct ZTileDrawParams
{
    public ZTilePosition tilePosition;
    public ZTileInstanceData tileData;
    public Color lightColor;
}

/// <summary>
/// Base class for a purely decorative tile asset
/// </summary>
public abstract class ZTile : ModTexturedType, ILocalizedModType
{
    private Asset<Texture2D> _tileTextureAsset;
    private Asset<Texture2D> _outlineTextureAsset;
    public ushort type;
    public TilePlacementRules placementRules;
    public TileDrawOrigin drawOrigin;
    public Vector2 parallaxAmount;
    public int frameCount = 1;
    public float rotateSpeed;
    public float windSwayOffset;
    public float windSwayMagnitude;
    public float windSwaySpeed;
    public bool interactable;
    public string LocalizationCategory => "ZTiles";
    public string DisplayName
    {
        get
        {
            return LangText.ZTile(this, "DisplayName");
        }
    }

    protected override void Register()
    {
        ModTypeLookup<ZTile>.Register(this);
    }

    public override void Unload()
    {
        base.Unload();
        _tileTextureAsset = null;
        _outlineTextureAsset = null;
    }
    public sealed override void SetupContent()
    {
        base.SetupContent();
        SetStaticDefaults();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        this.GetLocalization(nameof(DisplayName), () => RegexHelper.SplitByCapital(Name));
    }

    public void DrawIcon(SpriteBatch spriteBatch, Vector2 iconCenterPos, float maxSize)
    {
        _tileTextureAsset  ??= ModContent.Request<Texture2D>(Texture);
        Rectangle? frame = new Rectangle(0, 0, _tileTextureAsset.Width(), _tileTextureAsset.Height() / frameCount);
        Rectangle rect = frame.Value;
        float xMultiplier = maxSize / (float)rect.Width;
        float yMultiplier = maxSize / (float)rect.Height;
        Vector2 scale = new Vector2(xMultiplier, yMultiplier);
        spriteBatch.Draw(_tileTextureAsset.Value, iconCenterPos, frame, Color.White, 0, frame.Value.Size() / 2f, scale, SpriteEffects.None, 0);
    }

    public void DrawIcon2(SpriteBatch spriteBatch, Vector2 iconCenterPos, int frameNumber)
    {
        _tileTextureAsset ??= ModContent.Request<Texture2D>(Texture);
        int frameHeight = _tileTextureAsset.Height() / frameCount;
        Rectangle? frame = new Rectangle(0, frameHeight * frameNumber, _tileTextureAsset.Width(), frameHeight);
        Rectangle rect = frame.Value;
        spriteBatch.Draw(_tileTextureAsset.Value, iconCenterPos, frame, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
    }

    public float GetLeafSway(float offset, float magnitude, float speed)
    {
        return (float)Math.Sin(Main.GameUpdateCount * speed + offset) * magnitude;
    }

    public virtual void Update(Vector2 worldPosition)
    {

    }
    public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        _tileTextureAsset ??= ModContent.Request<Texture2D>(Texture);
        //Calculate frame;
        int frameHeight = _tileTextureAsset.Height() / frameCount;
        int frameWidth = _tileTextureAsset.Width();
        int yOffset = frameHeight * drawParams.tileData.frameNumber;
        Rectangle frame = new Rectangle(0, yOffset, frameWidth, frameHeight);

        //Calculate hte draworigin
        Vector2 drawOrigin = new Vector2(frame.Width / 2, frame.Height / 2);
        Vector2 drawOffset = Vector2.Zero;
        switch (this.drawOrigin)
        {
            default:
            case TileDrawOrigin.BottomUp:
                drawOffset = new Vector2(0, -frameHeight / 2f);
                break;
            case TileDrawOrigin.Center:
                drawOffset = Vector2.Zero;
                break;
            case TileDrawOrigin.TopDown:
                drawOffset = new Vector2(0, 0);
                drawOrigin = new Vector2(frame.Width / 2, 0);
                break;

        }


        //Since it's gonne default to 0 on old worlds
        //We'll make 255 be black
        Color valueColor = Color.Lerp(Color.White, Color.Black, (float)drawParams.tileData.value / 255f);
        Color drawColor = drawParams.lightColor.MultiplyRGB(valueColor);

        float drawRotation;
        switch (drawParams.tileData.rotation)
        {
            default:
            case Rotation.Degrees_0:
                drawRotation = 0;
                break;
            case Rotation.Degrees_90:
                drawRotation = MathHelper.PiOver2;
                break;
            case Rotation.Degrees_180:
                drawRotation = MathHelper.Pi;
                break;
            case Rotation.Degrees_270:
                drawRotation = MathHelper.Pi + MathHelper.PiOver2;
                break;
        }

        if(rotateSpeed > 0)
        {
            drawRotation += Main.GlobalTimeWrappedHourly * rotateSpeed * 24;
        }

        //Calculate wind if any
        if(windSwayMagnitude > 0)
        {
            drawRotation += GetLeafSway(windSwayOffset + drawParams.tilePosition.x, windSwayMagnitude, windSwaySpeed);
        }


        VelocityMap velocityMap = ModContent.GetInstance<VelocityMap>();

        //Convert to world coordinates
        Point point = new Point(drawParams.tilePosition.x, drawParams.tilePosition.y);
        Vector2 worldCoordinates = point.ToWorldCoordinates();

        /*
        Vector2 accumVelocity = velocityMap.GetDecayingVelocity(worldCoordinates - new Vector2(32), 64, 64);
        drawRotation += accumVelocity.ToRotation() * 0.2f;*/
        Vector2 drawPosition = worldCoordinates - screenPos;
        drawPosition += new Vector2(8);

        SpriteEffects spriteEffects = drawParams.tileData.flipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        if (drawParams.tileData.flipX)
            drawRotation *= -1;
        bool doDraw = PreDraw(spriteBatch, drawPosition + drawOffset, screenPos, drawParams);
        if(doDraw)
            spriteBatch.Draw(_tileTextureAsset.Value, drawPosition + drawOffset, frame, drawColor, drawRotation, drawOrigin, drawParams.tileData.scale, spriteEffects, 0);
        PostDraw(spriteBatch, drawPosition + drawOffset, screenPos, drawParams);
    }

    public virtual void DrawOutline(SpriteBatch spriteBatch, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        _outlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");
        //Calculate frame;
        int frameHeight = _outlineTextureAsset.Height() / frameCount;
        int frameWidth = _outlineTextureAsset.Width();
        int yOffset = frameHeight * drawParams.tileData.frameNumber;
        Rectangle frame = new Rectangle(0, yOffset, frameWidth, frameHeight);

        //Calculate hte draworigin
        Vector2 drawOrigin = new Vector2(frame.Width / 2, frame.Height / 2);
        Vector2 drawOffset = Vector2.Zero;
        switch (this.drawOrigin)
        {
            default:
            case TileDrawOrigin.BottomUp:
                drawOffset = new Vector2(0, -frameHeight / 2f);
                break;
            case TileDrawOrigin.Center:
                drawOffset = Vector2.Zero;
                break;
            case TileDrawOrigin.TopDown:
                drawOffset = new Vector2(0, 0);
                drawOrigin = new Vector2(frame.Width / 2, 0);
                break;

        }


        //Since it's gonne default to 0 on old worlds
        //We'll make 255 be black
       // Color valueColor = Color.Lerp(Color.White, Color.Black, (float)drawParams.tileData.value / 255f);
        Color drawColor = Color.White;
        drawColor *= (int)ExtraMath.Osc(0f, 3f, speed: 6);
        float drawRotation;
        switch (drawParams.tileData.rotation)
        {
            default:
            case Rotation.Degrees_0:
                drawRotation = 0;
                break;
            case Rotation.Degrees_90:
                drawRotation = MathHelper.PiOver2;
                break;
            case Rotation.Degrees_180:
                drawRotation = MathHelper.Pi;
                break;
            case Rotation.Degrees_270:
                drawRotation = MathHelper.Pi + MathHelper.PiOver2;
                break;
        }

        if (rotateSpeed > 0)
        {
            drawRotation += Main.GlobalTimeWrappedHourly * rotateSpeed * 24;
        }

        //Calculate wind if any
        if (windSwayMagnitude > 0)
        {
            drawRotation += GetLeafSway(windSwayOffset + drawParams.tilePosition.x, windSwayMagnitude, windSwaySpeed);
        }


        VelocityMap velocityMap = ModContent.GetInstance<VelocityMap>();

        //Convert to world coordinates
        Point point = new Point(drawParams.tilePosition.x, drawParams.tilePosition.y);
        Vector2 worldCoordinates = point.ToWorldCoordinates();

        /*
        Vector2 accumVelocity = velocityMap.GetDecayingVelocity(worldCoordinates - new Vector2(32), 64, 64);
        drawRotation += accumVelocity.ToRotation() * 0.2f;*/
        Vector2 drawPosition = worldCoordinates - screenPos;
        drawPosition += new Vector2(8);

        SpriteEffects spriteEffects = drawParams.tileData.flipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        if (drawParams.tileData.flipX)
            drawRotation *= -1;
        bool doDraw = PreDraw(spriteBatch, drawPosition + drawOffset, screenPos, drawParams);
        if (doDraw)
            spriteBatch.Draw(_outlineTextureAsset.Value, drawPosition + drawOffset, frame, drawColor, drawRotation, drawOrigin, drawParams.tileData.scale, spriteEffects, 0);
        PostDraw(spriteBatch, drawPosition + drawOffset, screenPos, drawParams);
    }

    public virtual bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        return true;
    }

    public virtual void PostDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {

    }

    public virtual (int,int) GetBounds()
    {
        return (0, 0);
    }
    /// <summary>
    /// If interactable is set to true, you can right click the tile
    /// </summary>
    public virtual void RightClick()
    {

    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.ZTileSystem;

public struct ZTileDrawParams
{
    public ZTilePosition tilePosition;
    public ZTileInstanceData tileData;
    public Color multiplyColor;
}

/// <summary>
/// Base class for a purely decorative tile asset
/// </summary>
public abstract class ZTile : ModTexturedType
{
    public ushort type;
    public TilePlacementRules placementRules;
    public TileDrawOrigin drawOrigin;
    public Vector2 parallaxAmount;
    public int frameCount = 1;
    protected override void Register()
    {
        ModTypeLookup<ZTile>.Register(this);
    }

    public sealed override void SetupContent()
    {
        base.SetupContent();
        SetStaticDefaults();
    }

    public void DrawIcon(SpriteBatch spriteBatch, Vector2 iconCenterPos, float maxSize)
    {
        Asset<Texture2D> tileTextureAsset = ModContent.Request<Texture2D>(Texture);
        Rectangle? frame = new Rectangle(0, 0, tileTextureAsset.Width(), tileTextureAsset.Height() / frameCount);
        Rectangle rect = frame.Value;
        float xMultiplier = maxSize / (float)rect.Width;
        float yMultiplier = maxSize / (float)rect.Height;
        Vector2 scale = new Vector2(xMultiplier, yMultiplier);
        spriteBatch.Draw(tileTextureAsset.Value, iconCenterPos, frame, Color.White, 0, frame.Value.Size() / 2f, scale, SpriteEffects.None, 0);
    }

    public void DrawIcon2(SpriteBatch spriteBatch, Vector2 iconCenterPos, int frameNumber)
    {
        Asset<Texture2D> tileTextureAsset = ModContent.Request<Texture2D>(Texture);

        int frameHeight = tileTextureAsset.Height() / frameCount;
        Rectangle? frame = new Rectangle(0, frameHeight * frameNumber, tileTextureAsset.Width(), frameHeight);
        Rectangle rect = frame.Value;
        spriteBatch.Draw(tileTextureAsset.Value, iconCenterPos, frame, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        //TODO: index array instead of modcontent.request
        Asset<Texture2D> tileTextureAsset = ModContent.Request<Texture2D>(Texture);

        //Calculate frame;
        int frameHeight = tileTextureAsset.Height() / frameCount;
        int frameWidth = tileTextureAsset.Width();
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
                drawOffset = new Vector2(0, frameHeight / 2f);
                break;

        }

        Color drawColor = Color.White;
        Color lightingColor = Lighting.GetColor(drawParams.tilePosition.x, drawParams.tilePosition.y);
        drawColor = drawColor.MultiplyRGB(lightingColor);
        drawColor = drawColor.MultiplyRGBA(drawParams.multiplyColor);

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

        //Convert to world coordinates
        Point point = new Point(drawParams.tilePosition.x, drawParams.tilePosition.y);
        Vector2 worldCoordinates = point.ToWorldCoordinates();
        Vector2 drawPosition = worldCoordinates - screenPos;
        drawPosition += new Vector2(8);

        SpriteEffects spriteEffects = drawParams.tileData.flipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        spriteBatch.Draw(tileTextureAsset.Value, drawPosition + drawOffset, frame, drawColor, drawRotation, drawOrigin, drawParams.tileData.scale, spriteEffects, 0);
    }
}

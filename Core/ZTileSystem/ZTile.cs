using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.ZTileSystem;

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
        float xMultiplier = maxSize/  (float)rect.Width;
        float yMultiplier = maxSize / (float)rect.Height;
        Vector2 scale = new Vector2(xMultiplier, yMultiplier);  
        spriteBatch.Draw(tileTextureAsset.Value, iconCenterPos, frame, Color.White, 0, frame.Value.Size() / 2f, scale, SpriteEffects.None, 0);
    }
}

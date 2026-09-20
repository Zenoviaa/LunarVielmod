using Terraria.ModLoader;

namespace Stellamod.Core.TileOverlaySystem;

public abstract class TileOverlayType : ModType
{
    public int Type { get; set; }
    public byte FrameCount { get; protected set; }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public sealed override void SetupContent()
    {
        base.SetupContent();
        SetStaticDefaults();
    }

    protected override void Register()
    {
        ModTypeLookup<TileOverlayType>.Register(this);
    }

    public virtual void PlaceTile(in int x, in int y, in TileOverlayPlacer placer)
    {
        Point point = new Point(x, y);
        TileOverlayData overlayData = new();
        overlayData.overlayType = (byte)Type;
        overlayData.overlayFrame = placer.frame;
        TileOverlayUtility.PlacedTileOverlays[point] = overlayData;
    }

    public void PlaceStyleBlock2x2(in int x, in int y, in TileOverlayPlacer placer)
    {

    }

    public abstract void BeginSpritebatch(SpriteBatch spriteBatch);
    public abstract void DrawTileOverlay(in TileOverlayDrawData drawData, SpriteBatch spriteBatch);
}

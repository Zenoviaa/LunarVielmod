using ReLogic.Content;
using System;
using Terraria;

namespace Stellamod.WorldG;

/// <summary>
/// Encapsulates a texture for world generation purposes, in most cases we're just going to use the texture as a mask for erasing tiles.
/// </summary>
public class GenerationPrefab : IDisposable
{
    public GenerationPrefab(string name, Asset<Texture2D> textureAsset)
    {
        Name = name;
        TextureAsset = textureAsset;
        Pixels = new Color[Width * Height];
        TextureAsset.Value.GetData(Pixels);
    }

    public string Name { get; private set; }
    public Color[] Pixels { get; private set; }
    public Asset<Texture2D> TextureAsset { get; private set; }
    public int Width => TextureAsset.Width();
    public int Height => TextureAsset.Height();

    public void Dispose()
    {
        TextureAsset = null;
    }

    public Color Sample(int localX, int localY)
    {
        return TextureUtilities.GetPixelColor(TextureAsset.Value, localX, localY, Pixels);
    }


    private void PasteEraseInner(in int originX, in int originY)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                int tileX = originX + x;
                int tileY = originY + y;
                if (!WorldGen.InWorld(tileX, tileY))
                    continue;

                Color c = Sample(x, y);
                if (c.R > 125)
                {
                    Tile t = Main.tile[tileX, tileY];
                    t.ClearEverything();
                }
            }
        }
    }
    private void PasteEraseInner(in int originX, in int originY, Action<int, int, Color> manipulator)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                int tileX = originX + x;
                int tileY = originY + y;
                manipulator(tileX, tileY, Sample(x, y));
            }
        }
    }
    public void PasteErase(int originX, int originY, Point pixelOrigin)
    {
        originX -= pixelOrigin.X;
        originY -= pixelOrigin.Y;
        PasteEraseInner(originX, originY);
    }
    public void PasteErase(Point origin, PrefabPlacementType placementType, Action<int, int, Color> manipulator = null)
    {
        PasteErase(origin.X, origin.Y, placementType, manipulator);
    }
    public Rectangle GetBounds(int originX, int originY, PrefabPlacementType placementType)
    {
        switch (placementType)
        {
            case PrefabPlacementType.FromTopLeft:
                break;
            case PrefabPlacementType.FromTopCenter:
                originX -= Width / 2;
                break;
            case PrefabPlacementType.FromCenter:
                originX -= Width / 2;
                originY -= Height / 2;
                break;
            case PrefabPlacementType.FromTopRight:
                originX -= Width;
                break;

        }

        //Clamp to world bounds to prevent index out of bounds exceptions
        Rectangle rectangle = new Rectangle(originX, originY, Width, Height);
        rectangle.X = (int)MathHelper.Clamp(rectangle.X, 0, Main.maxTilesX - 1);
        rectangle.Y = (int)MathHelper.Clamp(rectangle.Y, 0, Main.maxTilesY - 1);

        int maxRight = (int)MathHelper.Clamp(rectangle.X + rectangle.Width, 0, Main.maxTilesX - 1);
        int maxWidth = maxRight - rectangle.Left;
        rectangle.Width = (int)MathHelper.Min(rectangle.Width, maxWidth);

        int maxBottom = (int)MathHelper.Clamp(rectangle.Y + rectangle.Height, 0, Main.maxTilesY - 1);
        int maxHeight = maxBottom - rectangle.Top;
        rectangle.Height = (int)MathHelper.Min(rectangle.Height, maxHeight);
        return rectangle;
    }
    public void PasteErase(int originX, int originY, PrefabPlacementType placementType, Action<int, int, Color> manipulator = null)
    {
        switch (placementType)
        {
            case PrefabPlacementType.FromTopLeft:
                break;
            case PrefabPlacementType.FromTopCenter:
                originX -= Width / 2;
                break;
            case PrefabPlacementType.FromCenter:
                originX -= Width / 2;
                originY -= Height / 2;
                break;
            case PrefabPlacementType.FromTopRight:
                originX -= Width;
                break;

        }

        if(manipulator != null)
        {
            PasteEraseInner(originX, originY, manipulator);
        }
        else
        {
            PasteEraseInner(originX, originY);
        }

    }


}

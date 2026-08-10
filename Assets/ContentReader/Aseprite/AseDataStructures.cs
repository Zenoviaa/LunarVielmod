using Stellamod.Core.NPCHelpers;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Assets.ContentReader.Aseprite;


public class AseSprite
{
    public AseSprite()
    {
        layers = new List<AseLayer>();
        frames = new List<AseFrame>();
        tags = new List<AseTags>();
    }

    public List<AseLayer> layers;
    public List<AseFrame> frames;
    public List<AseTags> tags;
    public Texture2D sheet;

    public Vector2 Size => new Vector2(FrameWidth, FrameHeight);
    public int FrameWidth => frames[0].width;
    public int FrameHeight => frames[0].height;
    
    public SpritebatchDrawer GetSprite(int frameIndex, Vector2 worldPosition)
    {
        Rectangle srcRect = GetSrcRect(frameIndex);
        return SpritebatchDrawer.FromTextureAsset(sheet, worldPosition) with { sourceRect = srcRect, drawOrigin = srcRect.Size() * 0.5f  };
    }

    public Rectangle GetSrcRect(int frameIndex)
    {
        return new Rectangle(0, frameIndex * FrameHeight, FrameWidth, FrameHeight);
    }

    public Texture2D CreateVerticalSpriteSheet()
    {
        //For now just export the sheet vertically
        int spriteSize = frames[0].width * frames[0].height;
        Texture2D texture = new Texture2D(Main.instance.GraphicsDevice, frames[0].width, frames[0].height * frames.Count);
        Color[] pixels = new Color[texture.Width * texture.Height];
        int pixelIndex = 0;
        for(int i = 0; i < frames.Count; i++)
        {
            for(int j = 0; j < frames[i].pixels.Length; j++)
            {
                pixels[pixelIndex++] = frames[i].pixels[j];
            }
        }
        texture.SetData(pixels);
        return texture;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var layer in layers)
        {
            sb.AppendLine(layer.ToString());
        }

        foreach (var frame in frames)
        {
            sb.AppendLine(frame.ToString());
        }
        return sb.ToString();
    }
}

public enum AseAnimationDirection
{
    Forward = 0,
    Reverse = 1,
    Ping_Pong = 2,
    Ping_Pong_Reverse = 3
}

public class AseTags
{
    public ushort from;
    public ushort to;
    public AseAnimationDirection animationDirection;
    public ushort repeats;
    public string name;
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"{name} From: {from} To: {to}, Repeats: {repeats} {animationDirection}");
        return sb.ToString();
    }
}

public class AseLayer
{
    public AseLayerFlags flags;
    public AseLayerType type;
    public ushort layerChildLevel;
    public AseBlendMode blendMode;
    public byte opacity;
    public string name;
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Aseprite Layer");
        sb.AppendLine($"Flags {flags}");
        sb.AppendLine($"Type {type}");
        sb.AppendLine($"Layer Child Level {layerChildLevel}");
        sb.AppendLine($"Blend Mode {blendMode}");
        return sb.ToString();
    }
}

public enum AseBlendMode
{
    Normal = 0,
    Multiply = 1,
    Screen = 2,
    Overlay = 3,
    Darken = 4,
    Lighten = 5,
    Color_Dodge = 6,
    Color_Burn = 7,
    Hard_Light = 8,
    Soft_Light = 9,
    Difference = 10,
    Exclusion = 11,
    Hue = 12,
    Saturation = 13,
    Color = 14,
    Luminosity = 15,
    Addition = 16,
    Subtract = 17,
    Divide = 18
}
public enum AseLayerFlags
{
    Visible = 1,
    Editable = 2,
    LockMovement = 4,
    Background = 8,
    PreferLinkedCels = 16,
    LayerGroupShouldBeDisplayedCollapsed = 32,
    ReferenceLayer = 64,
}

public enum AseLayerType
{
    Normal = 0,
    Group = 1,
    Tilemap = 2
}

public class AseFrame
{
    public int width;
    public int height;
    public ushort frameDurationInMilliseconds;
    public float frameTime;
    public Color[] pixels;
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"{frameDurationInMilliseconds}");
        return sb.ToString();
    }
}

public enum AseCelType
{
    RawImageData = 0,
    LinkedCel = 1,
    CompressedImage = 2,
    CompressedTilemap = 3
}


public enum AseChunkType
{
    Cel = 0x2005,
    Layer = 0x2004,
    Tags = 0x2018
}

public enum AseFlags
{
    LayerOpacityHasValidValue = 1,
    LayerBlendModeOpacityValidForGroups = 2,
    LayersHaveUUID = 4
}

public enum AseColorDepth
{
    Indexed = 8,
    Grayscale = 16,
    RGBA = 32
}

public class AseCel
{
    public AseSprite sprite;
    public AseLayer Layer => sprite.layers[layerIndex];
    public ushort layerIndex;
    public short xPosition;
    public short yPosition;
    public byte opacity;
    public AseCelType celType;
    public short z;
    public ushort pixelWidth;
    public ushort pixelHeight;
    public Color[] pixels;
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"[CEL]");
        sb.AppendLine($"X: {xPosition}");
        sb.AppendLine($"Y: {yPosition}");
        sb.AppendLine($"Opacity: {opacity}");
        sb.AppendLine($"Cel Type: {celType}");
        sb.AppendLine($"Z Layer: {z}");
        sb.AppendLine($"Pixel Width/Height: {pixelWidth} {pixelHeight}");
        // sb.AppendLine($"Pixels Length {pixels.Length}");
        return sb.ToString();
    }
}

public class AseHeader
{
    public uint fileSize;
    public ushort magicNumber;
    public ushort frames;
    public ushort widthInPixels;
    public ushort heightInPixels;
    public AseColorDepth colorDepth;
    public AseFlags flags;
    public ushort speed;
    public ushort numberOfColors;
    public byte pixelRatioWidth;
    public byte pixelRatioHeight;
    public short xGridPos;
    public short yGridPos;
    public ushort gridWidth;
    public ushort gridHeight;
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("[Aseprite File HEADER]");
        sb.AppendLine($"File Size: {fileSize}");
        sb.AppendLine($"Frames: {frames}");
        sb.AppendLine($"Width In Pixels: {widthInPixels}");
        sb.AppendLine($"Height In Pixels: {heightInPixels}");
        sb.AppendLine($"Color Depth: {colorDepth}");
        sb.AppendLine($"Flags: {flags}");
        sb.AppendLine($"Speed: {speed}");
        sb.AppendLine($"Number of Colors {numberOfColors}");
        sb.AppendLine($"Pixel Ratio {pixelRatioWidth} / {pixelRatioHeight}");
        sb.AppendLine($"Grid Position {xGridPos}, {yGridPos}");
        sb.AppendLine($"Grid Size {gridWidth}, {gridHeight}");
        return sb.ToString();
    }
}
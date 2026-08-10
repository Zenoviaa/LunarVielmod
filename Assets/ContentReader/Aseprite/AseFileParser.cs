using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Assets.ContentReader.Aseprite;

// Refs: https://gist.github.com/NoelFB/778d190e5d17f1b86ebf39325346fcc5
// https://github.com/aseprite/aseprite/blob/main/docs/ase-file-specs.md
public static class AseFileParser
{
    static byte BYTE(BinaryReader reader) => reader.ReadByte();
    static ushort WORD(BinaryReader reader) => reader.ReadUInt16();
    static short SHORT(BinaryReader reader) => reader.ReadInt16();
    static uint DWORD(BinaryReader reader) => reader.ReadUInt32();
    static int LONG(BinaryReader reader) => reader.ReadInt32();

    //The heck is this one?
    static int FIXED(BinaryReader reader) => reader.ReadInt32();
    static float FLOAT(BinaryReader reader) => reader.ReadSingle();
    static double DOUBLE(BinaryReader reader) => reader.ReadDouble();
    static ulong QWORD(BinaryReader reader) => reader.ReadUInt64();
    static long LONG64(BinaryReader reader) => reader.ReadInt64();
    static byte[] BYTES(BinaryReader reader, int n) => reader.ReadBytes(n);
    static string STRING(BinaryReader reader) { return Encoding.UTF8.GetString(BYTES(reader, WORD(reader))); }
    static byte[] PIXEL(BinaryReader reader, int width, int height, AseColorDepth depth)
    {
        switch (depth)
        {
            case AseColorDepth.RGBA:
                return BYTES(reader, width * height * 4);
            case AseColorDepth.Grayscale:
                return BYTES(reader, width * height * 2);
            case AseColorDepth.Indexed:
                return BYTES(reader, width * height);
            default:
                throw new Exception("Unknown Color Depth Format");
        }
    }


    static AseHeader ParseHeader(BinaryReader reader)
    {
        AseHeader header = new AseHeader();
        //A 128-byte header (same as FLC/FLI header, but with other magic number):
        header.fileSize = DWORD(reader);
        header.magicNumber = WORD(reader);
        header.frames = WORD(reader);
        header.widthInPixels = WORD(reader);
        header.heightInPixels = WORD(reader);
        header.colorDepth = (AseColorDepth)WORD(reader);
        header.flags = (AseFlags)DWORD(reader);
        header.speed = WORD(reader);

        DWORD(reader); // Set be 0
        DWORD(reader); // Set be 0

        BYTE(reader); // Palette Entry
        BYTES(reader, 3); // Ignore these bytes
        header.numberOfColors = WORD(reader);

        //0 colors is 256 for old sprites.
        if (header.numberOfColors == 0)
        {
            header.numberOfColors = 256;
        }
        header.pixelRatioWidth = BYTE(reader);
        header.pixelRatioHeight = BYTE(reader);
        header.xGridPos = SHORT(reader);
        header.yGridPos = SHORT(reader);
        header.gridWidth = WORD(reader);
        header.gridHeight = WORD(reader);
        //Dead space for the future
        BYTES(reader, 84);
        return header;
    }

    private static void BytesToPixels(byte[] bytes, Color[] pixels, AseColorDepth mode, Color[] palette)
    {
        int len = pixels.Length;
        if (mode == AseColorDepth.RGBA)
        {
            for (int p = 0, b = 0; p < len; p++, b += 4)
            {
                pixels[p].R = (byte)(bytes[b + 0] * bytes[b + 3] / 255);
                pixels[p].G = (byte)(bytes[b + 1] * bytes[b + 3] / 255);
                pixels[p].B = (byte)(bytes[b + 2] * bytes[b + 3] / 255);
                pixels[p].A = bytes[b + 3];
            }
        }
        else if (mode == AseColorDepth.Grayscale)
        {
            for (int p = 0, b = 0; p < len; p++, b += 2)
            {
                pixels[p].R = pixels[p].G = pixels[p].B = (byte)(bytes[b + 0] * bytes[b + 1] / 255);
                pixels[p].A = bytes[b + 1];
            }
        }
        else if (mode == AseColorDepth.Indexed)
        {
            for (int p = 0, b = 0; p < len; p++, b += 1)
                pixels[p] = palette[bytes[b]];
        }
    }

    public static AseSprite Parse(Stream stream)
    {
        MemoryStream ms = new MemoryStream();
        stream.CopyTo(ms);
        ms.Seek(0, SeekOrigin.Begin);
        var reader = new BinaryReader(ms);
        var header = ParseHeader(reader);
        var sprite = ParseFrames(header, reader);
        sprite.sheet = sprite.CreateVerticalSpriteSheet();
        reader.Dispose();
        ms.Dispose();
        return sprite;
    }

    private static AseSprite ParseFrames(AseHeader header, BinaryReader reader)
    {
        AseSprite sprite = new AseSprite();
        byte[] buffer = new byte[header.widthInPixels * header.heightInPixels * 4];
        for (int j = 0; j < header.frames; j++)
        {
            AseFrame frame = new AseFrame();
            frame.width = header.widthInPixels;
            frame.height = header.heightInPixels;
            frame.pixels = new Color[frame.width * frame.height];


            //Read the frame header
            long frameStart = reader.BaseStream.Position;
            uint bytesInFrame = DWORD(reader); //number of bytes in this frame
            long frameEnd = frameStart + bytesInFrame;

            WORD(reader); //magic number (always 0xF1FA


            ushort oldChunkNumber = WORD(reader); // old field for number of chunks
            frame.frameDurationInMilliseconds = WORD(reader); // in milliseconds
            frame.frameTime = (float)frame.frameDurationInMilliseconds / 1000f;
            BYTES(reader, 2); // for the future
            uint newChunkNumber = DWORD(reader);

            sprite.frames.Add(frame);
            //If new chunk number is 0, then we use the old chunk number.
            var chunkNumber = newChunkNumber == 0 ? oldChunkNumber : newChunkNumber;
            for (uint i = 0; i < chunkNumber; i++)
            {
                long start = reader.BaseStream.Position;
                uint chunkSize = DWORD(reader);
                AseChunkType chunkType = (AseChunkType)WORD(reader);
                long chunkEnd = start + chunkSize;
                switch (chunkType)
                {
                    case AseChunkType.Tags:
                        {
                            ushort numberOfTags = WORD(reader);
                            //For future set to zero
                            BYTES(reader, 8);

                            //foreach tag 
                            for (uint k = 0; k < numberOfTags; k++)
                            {
                                AseTags tags = new AseTags();
                                tags.from = WORD(reader);
                                tags.to = WORD(reader);
                                tags.animationDirection = (AseAnimationDirection)BYTE(reader);
                                tags.repeats = WORD(reader);
                                //For the future, set to zero
                                BYTES(reader, 6);

                                //RGB values of tag color
                                BYTES(reader, 3);

                                //extra byte zero
                                BYTE(reader);

                                tags.name = STRING(reader);
                                sprite.tags.Add(tags);
                            }

                        }
                        break;
                    case AseChunkType.Layer:
                        {
                            AseLayer layer = new AseLayer();
                            layer.flags = (AseLayerFlags)WORD(reader);
                            layer.type = (AseLayerType)WORD(reader);
                            layer.layerChildLevel = WORD(reader);
                            //default layer width/height, ignore.
                            WORD(reader);
                            WORD(reader);

                            layer.blendMode = (AseBlendMode)WORD(reader);
                            layer.opacity = BYTE(reader);
                            //for future (set to zero)
                            BYTES(reader, 3);
                            layer.name = STRING(reader);
                            sprite.layers.Add(layer);
                        }
                        break;
                    case AseChunkType.Cel:
                        {

                            AseCel cel = new AseCel();
                            cel.sprite = sprite;
                            cel.layerIndex = WORD(reader);
                            cel.xPosition = SHORT(reader);
                            cel.yPosition = SHORT(reader);
                            cel.opacity = BYTE(reader);
                            cel.celType = (AseCelType)WORD(reader);
                            cel.z = SHORT(reader);
                            BYTES(reader, 5); //For future set to zero

                            switch (cel.celType)
                            {
                                case AseCelType.RawImageData:

                                    cel.pixelWidth = WORD(reader);
                                    cel.pixelHeight = WORD(reader);
                                    cel.pixels = new Color[cel.pixelWidth * cel.pixelHeight];
                                    BytesToPixels(PIXEL(reader, cel.pixelWidth, cel.pixelWidth, header.colorDepth), cel.pixels, header.colorDepth, null);
                                    break;
                                case AseCelType.LinkedCel:
                                    break;
                                case AseCelType.CompressedImage:
                                    {
                                        cel.pixelWidth = WORD(reader);
                                        cel.pixelHeight = WORD(reader);

                                        using var deflate = new ZLibStream(reader.BaseStream, CompressionMode.Decompress, true);
                                        int readBytes;
                                        var totalBytesRead = 0;
                                        do
                                        {
                                            readBytes = deflate.Read(buffer, totalBytesRead, buffer.Length - totalBytesRead);
                                            totalBytesRead += readBytes;
                                        } while (readBytes > 0);

                                        //TODO: add pallette
                                        cel.pixels = new Color[cel.pixelWidth * cel.pixelHeight];
                                        BytesToPixels(buffer, cel.pixels, header.colorDepth, null);
                                        CelToFrame(frame, cel);
                                    }

                                    break;
                                case AseCelType.CompressedTilemap:
                                    break;
                            }
                        }
                        break;

                }

                reader.BaseStream.Position = chunkEnd;
            }
            reader.BaseStream.Position = frameEnd;
        }

        return sprite;
    }


    /// <summary>
    /// Applies a Cel's pixels to the Frame, using its Layer's BlendMode & Alpha
    /// </summary>
    private static void CelToFrame(AseFrame frame, AseCel cel)
    {
        var opacity = (byte)((cel.opacity * cel.Layer.opacity) * 255);
        var blend = AseBlending.BlendModes[(int)cel.Layer.blendMode];

        for (int sx = 0; sx < cel.pixelWidth; sx++)
        {
            int dx = cel.xPosition + sx;
            int dy = cel.yPosition * frame.width;

            for (int i = 0, sy = 0; i < cel.pixelHeight; i++, sy += cel.pixelWidth, dy += frame.width)
                blend(ref frame.pixels[dx + dy], cel.pixels[sx + sy], opacity);
        }
    }
}

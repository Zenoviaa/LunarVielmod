using Microsoft.Xna.Framework.Media;
using ReLogic.Content;
using ReLogic.Content.Readers;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using static Theorafile;

namespace Stellamod.Assets.Videos;


//NOTES:
//This is the imeplementation written by Mirsario for Terraria Overhaul to read a .ogv file into memory, since the Video class can only read form external URLs which is sad
//I tried writing my own implementation for this, but couldn't figure out why I was getting a protected memory exception when parsing the asset,
//I need to read up more on memory management lol
//I'll come back to this later
//TODO: Investigate internal workings of this code more and try again
//Also having an actual h.264 system with like ffmpeg or something would be really cool, .ogv seems like a quite outdated video format.

[Autoload(false)]
public class VideoReader :
    IAssetReader,
    ILoadable
{


    private const BindingFlags ReflectionFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    private static readonly Dictionary<IntPtr, UnmanagedMemoryStream> memoryStreams = new();
    // Stores delegates in heap so that they don't get eaten by the GC.
    private static readonly tf_callbacks callbacks = new()
    {
        read_func = ReadFunction,
        seek_func = SeekFunction,
        close_func = CloseFunction,
    };
    // Reflection
    private static readonly Type videoType = typeof(Video);
    private static readonly FieldInfo videoTheora = videoType.GetField("theora", ReflectionFlags)!;
    private static readonly FieldInfo videoYWidth = videoType.GetField("yWidth", ReflectionFlags)!;
    private static readonly FieldInfo videoYHeight = videoType.GetField("yHeight", ReflectionFlags)!;
    private static readonly FieldInfo videoUvWidth = videoType.GetField("uvWidth", ReflectionFlags)!;
    private static readonly FieldInfo videoUvHeight = videoType.GetField("uvHeight", ReflectionFlags)!;
    private static readonly FieldInfo videoFps = videoType.GetField("fps", ReflectionFlags)!;
    private static readonly FieldInfo videoNeedsDurationHack = videoType.GetField("needsDurationHack", ReflectionFlags)!;
    private static readonly PropertyInfo videoDuration = videoType.GetProperty(nameof(Video.Duration), ReflectionFlags)!;
    private static readonly PropertyInfo videoGraphicsDevice = videoType.GetProperty("GraphicsDevice", ReflectionFlags)!;

    public async ValueTask<T> FromStream<T>(Stream stream, MainThreadCreationContext mainThreadCtx) where T : class
    {
        if (typeof(T) != videoType)
        {
            throw AssetLoadException.FromInvalidReader<VideoReader, T>();
        }

        await mainThreadCtx;

        var result = CreateVideo(stream);

        return (result as T)!;
    }

    private unsafe Video CreateVideo(Stream stream)
    {
        // This is created only to get a length without accessing stream.Length,
        // because 'stream' may be 'DeflateStream', and that doesn't implement it.
        // Could be avoided.
        using var memoryStream = new MemoryStream();

        stream.CopyTo(memoryStream);

        int numBytes = (int)memoryStream.Position;
        nint dataPtr = Marshal.AllocHGlobal(numBytes);
        var unmanagedStream = new UnmanagedMemoryStream((byte*)dataPtr, numBytes, numBytes, FileAccess.ReadWrite);

        memoryStream.Seek(0, SeekOrigin.Begin);
        memoryStream.CopyTo(unmanagedStream, numBytes);
        unmanagedStream.Seek(0L, SeekOrigin.Begin);

        // Keep track of streams and the data pointers.
        memoryStreams[dataPtr] = unmanagedStream;

        // Video's constructors are useless - they're internal, and take an OS file path rather than a data pointer.
        // Here we assemble the Video instance completely by ourselves.
        // Good thing that we no longer have to account for different frameworks being used.
        // - Mirsario.
        var result = (Video)RuntimeHelpers.GetUninitializedObject(videoType);

        int openResult = tf_open_callbacks(dataPtr, out nint theoraPtr, callbacks);

        if (openResult != 0)
        {
            throw new InvalidOperationException($"Theorafile returned code '{openResult}' when trying to load data.");
        }

        tf_videoinfo(theoraPtr, out int yWidth, out int yHeight, out double fps, out var fmt);

        int uvWidth;
        int uvHeight;

        if (fmt == th_pixel_fmt.TH_PF_420)
        {
            uvWidth = yWidth / 2;
            uvHeight = yHeight / 2;
        }
        else if (fmt == th_pixel_fmt.TH_PF_422)
        {
            uvWidth = yWidth / 2;
            uvHeight = yHeight;
        }
        else if (fmt == th_pixel_fmt.TH_PF_444)
        {
            uvWidth = yWidth;
            uvHeight = yHeight;
        }
        else
        {
            throw new NotSupportedException("Unrecognized YUV format!");
        }

        videoGraphicsDevice.SetValue(result, Main.graphics.GraphicsDevice);
        videoTheora.SetValue(result, theoraPtr);
        videoYWidth.SetValue(result, yWidth);
        videoYHeight.SetValue(result, yHeight);
        videoUvWidth.SetValue(result, uvWidth);
        videoUvHeight.SetValue(result, uvHeight);
        videoFps.SetValue(result, fps);

        videoDuration.SetValue(result, TimeSpan.MaxValue);
        videoNeedsDurationHack.SetValue(result, true);

        return result;
    }
    private static unsafe IntPtr ReadFunction(IntPtr ptr, IntPtr size, IntPtr nmemb, IntPtr dataSource)
    {
        if (!memoryStreams.TryGetValue(dataSource, out var stream))
        {
            return IntPtr.Zero;
        }

        int numBytes = (int)(nmemb * size);
        var span = new Span<byte>((void*)ptr, numBytes);
        int numRead = stream.Read(span);

        return numRead;
    }

    private static int SeekFunction(IntPtr dataSource, long offset, SeekWhence whence)
    {
        if (!memoryStreams.TryGetValue(dataSource, out var stream))
        {
            return 0;
        }

        var seekOrigin = whence switch
        {
            SeekWhence.TF_SEEK_SET => SeekOrigin.Begin,
            SeekWhence.TF_SEEK_CUR => SeekOrigin.Current,
            SeekWhence.TF_SEEK_END => SeekOrigin.End,
            _ => throw new InvalidDataException($"{nameof(SeekWhence)} value made no sense"),
        };
        long newPosition = stream.Seek(offset, seekOrigin);

        return (int)newPosition;
    }

    private static int CloseFunction(IntPtr dataSource)
    {
        if (!memoryStreams.Remove(dataSource, out var stream))
        {
            return 0;
        }

        stream.Dispose();
        Marshal.FreeHGlobal(dataSource);

        return 1;
    }

    public const string FILE_EXTENSION = ".ogv";
    public void Load(Mod mod)
    {

        var readers = Main.instance.Services.Get<AssetReaderCollection>();
        if (!readers.TryGetReader(FILE_EXTENSION, out var reader) || reader != this)
        {
            Console.WriteLine("REGISTER READER");
            readers.RegisterReader(this, FILE_EXTENSION);
        }
    }

    public void Unload()
    {
        //guh kinda messy
        var readers = Main.instance.Services.Get<AssetReaderCollection>();
        Dictionary<string, IAssetReader> readersByExtension = (Dictionary<string, IAssetReader>)
            typeof(AssetReaderCollection).GetField("_readersByExtension", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .GetValue(readers);
        if (readersByExtension.ContainsKey(FILE_EXTENSION))
        {
            Console.WriteLine("REMOVEEEEEEEEEEEE READER");
            readersByExtension.Remove(FILE_EXTENSION);
            typeof(AssetReaderCollection).GetField("_extensions", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .SetValue(readers, readersByExtension.Keys.ToArray());
        }
    }
}

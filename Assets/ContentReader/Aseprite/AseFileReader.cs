using Microsoft.Xna.Framework.Media;
using ReLogic.Content;
using ReLogic.Content.Readers;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Assets.ContentReader.Aseprite;

[Autoload(false)]
public class AseFileReader :
    IAssetReader,
    ILoadable
{
    public const string FILE_EXTENSION = ".aseprite";
    private static readonly Type spriteType = typeof(AseSprite);
    public async ValueTask<T> FromStream<T>(Stream stream, MainThreadCreationContext mainThreadCtx) where T : class
    {
        if (typeof(T) != spriteType)
        {
            throw AssetLoadException.FromInvalidReader<AseFileReader, T>();
        }

        await mainThreadCtx;

        var result = CreateSprite(stream);

        return (result as T)!;
    }

    private AseSprite CreateSprite(Stream stream)
    {
        return AseFileParser.Parse(stream);
    }

    public void Load(Mod mod)
    {
        var readers = Main.instance.Services.Get<AssetReaderCollection>();
        if (!readers.TryGetReader(FILE_EXTENSION, out var reader) || reader != this)
        {
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
            readersByExtension.Remove(FILE_EXTENSION);
            typeof(AssetReaderCollection).GetField("_extensions", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .SetValue(readers, readersByExtension.Keys.ToArray());
        }
    }
}

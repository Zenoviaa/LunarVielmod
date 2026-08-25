using ReLogic.Content;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities;

public class LazyAsset<T>
    where T : class 
{
    public LazyAsset(string path)
    {
        Path = path;
    }
    private Asset<T> _assetBackingField;
    public readonly string Path;
    public T Value
    {
        get
        {
            return Asset.Value;
        }
    }
    public Asset<T> Asset
    {
        get
        {
            _assetBackingField ??= ModContent.Request<T>(Path);
            return _assetBackingField;
          
        }
        private set
        {
            _assetBackingField = null;
        }
    }

    public void Unload()
    {
        _assetBackingField = null;
    }


    public static implicit operator T(LazyAsset<T> lazy) => lazy.Asset.Value;
    public static implicit operator Asset<T>(LazyAsset<T> lazy) => lazy.Asset;
}

using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.WorldG;

[Autoload(Side = ModSide.Client)]
public class GenerationTextureManager : ModSystem
{
    public Dictionary<string, GenerationPrefab> Prefabs { get; private set; }
    public override void Load()
    {
        base.Load();
        Main.QueueMainThreadAction(LoadPrefabAssets);
    }
    public override void Unload()
    {
        base.Unload();
        Main.QueueMainThreadAction(UnloadPrefabAssets);
    }

    private void UnloadPrefabAssets()
    {

    }
    private void LoadPrefabAssets()
    {
        Prefabs = new Dictionary<string, GenerationPrefab>();
        Mod mod = Stellamod.Instance;
        foreach (var file in mod.GetFileNames())
        {
            if (file.Contains("WorldGenTextures/"))
            {
                string path = "Stellamod/" + file;
                path = path.Replace(".rawimg", "");
                Asset<Texture2D> worldGenTexture = ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad);
                GenerationPrefab prefab = new GenerationPrefab(Path.GetFileNameWithoutExtension(file), worldGenTexture);
                Console.WriteLine($"Prefab {prefab.Name}");
                Prefabs.Add(prefab.Name, prefab);
            }
        }
    }

    public GenerationPrefab GetPrefab(string name) => Prefabs[name];
}

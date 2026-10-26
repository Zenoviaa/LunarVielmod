using ReLogic.Content;
using System;
using System.IO;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Common.Shaders;

public class CrystalShaderRegistry
{
    public static AssetRepository Assets => Stellamod.Instance.Assets;
    public static void LoadShaders()
    {
        foreach (string filePath in ModContent.GetInstance<Stellamod>().GetFileNames())
        {
            if (!filePath.Contains(".fxc"))
                continue;

            string assetPath = filePath.Replace(".fxc", "");
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            Asset<Effect> miscShader = Assets.Request<Effect>(assetPath, AssetRequestMode.ImmediateLoad);
            GameShaders.Misc[$"LunarVeil:{fileName}"] = new MiscShaderData(miscShader, miscShader.Value.Techniques[0].Passes[0].Name);
        }
    }
}

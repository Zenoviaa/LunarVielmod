using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria.Graphics.Effects;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Stellamod.Common.Shaders
{
    internal class CrystalShaderRegistry
    {
        public static AssetRepository Assets => Stellamod.Instance.Assets;
        public static void RegisterMiscShader(string name, string pass)
        {
            string assetPath = $"Effects/CrystalShaders/{name}";
            Asset<Effect> miscShader = Assets.Request<Effect>(assetPath, AssetRequestMode.ImmediateLoad);
            GameShaders.Misc[$"LunarVeil:{name}"] = new MiscShaderData(miscShader, pass);
        }

        public static void LoadShaders()
        {
            //Automatically Load All Base Shaders
            foreach (Type type in Stellamod.Instance.Code.GetTypes())
            {
                //Only if it inherits BaseShader
                if (!type.IsAbstract && type.IsSubclassOf(typeof(BaseShader)))
                {
                    //This automatically loads shaders that inherits from BaseShader, so we don't have to keep manually updating the Registry and can just use
                    //The custom classes that we made :)
                    object instance = Activator.CreateInstance(type);
                    BaseShader shader = (BaseShader)instance;
                    string name = shader.EffectPath;
                    string assetPath = $"Effects/CrystalShaders/{name}";
                    Asset<Effect> miscShader = Assets.Request<Effect>(assetPath, AssetRequestMode.ImmediateLoad);
                    GameShaders.Misc[$"LunarVeil:{name}"] = new MiscShaderData(miscShader, miscShader.Value.Techniques[0].Passes[0].Name);
                }
            }
            var miscShader9 = new Ref<Effect>(Stellamod.Instance.Assets.Request<Effect>("Effects/CrystalShaders/Water", AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene["LunarVeil:Water"] = new Filter(new ScreenShaderData(miscShader9, "PrimitivesPass"), EffectPriority.VeryHigh);
            Filters.Scene["LunarVeil:Water"].Load();

            var miscShader7 = new Ref<Effect>(Stellamod.Instance.Assets.Request<Effect>("Effects/CrystalShaders/WaterBasic", AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene["LunarVeil:WaterBasic"] = new Filter(new ScreenShaderData(miscShader7, "PrimitivesPass"), EffectPriority.VeryHigh);
            Filters.Scene["LunarVeil:WaterBasic"].Load();

        }
    }
}

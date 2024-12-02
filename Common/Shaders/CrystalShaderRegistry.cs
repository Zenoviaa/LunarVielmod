using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria.Graphics.Shaders;

namespace Stellamod.Common.Shaders
{
    internal class CrystalShaderRegistry
    {
        public static AssetRepository Assets => Stellamod.Instance.Assets;
        private static void RegisterMiscShader(string name, string pass)
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
        }
    }
}

using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod
{
    internal static class ShaderLoader
    {
        private static void LoadShader(Mod mod, string path, string name)
        {
            Asset<Effect> shader = mod.Assets.Request<Effect>(path);

            // To bind a miscellaneous, non-filter effect, use this.
            // If you're actually using this, you probably already know what you're doing anyway.
            // This type of shader needs an additional parameter: float4 uShaderSpecificData;
            GameShaders.Misc[$"{mod.Name}:{name}"] = new MiscShaderData(shader, "Pass0");
        }
        public static void LoadShaders(Mod mod)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            LoadShader(mod, "Assets/Effects/Trails/SlashEffect", "SlashEffect");
            LoadShader(mod, "Assets/Effects/GlowCircle", "GlowCircleShader");
            LoadShader(mod, "Assets/Effects/GlowCircle", "GlowCircleShader");
        }
    }
}

using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Effects
{
    internal class PaletteShaderSystem : ModSystem
    {
        private Color[] _currentPalette;
        private FilterManager FilterManager => Terraria.Graphics.Effects.Filters.Scene;

        public void UsePalette(Color[] palette)
        {
            if(_currentPalette != palette)
            {
                Vector3[] p = new Vector3[palette.Length];
                for (int i = 0; i < palette.Length; i++)
                {
                    p[i] = palette[i].ToVector3();
                }

                _currentPalette = palette;
                var shaderData = FilterManager[ShaderRegistry.Screen_Palette].GetShader();
                shaderData.Shader.Parameters["Palette"].SetValue(p);
                shaderData.Shader.Parameters["PaletteLength"].SetValue(p.Length);
            }
            if (!FilterManager[ShaderRegistry.Screen_Palette].IsActive())
            {
                FilterManager.Activate(ShaderRegistry.Screen_Palette);
            }
        }

        public bool DisablePalette(Color[] palette)
        {
            if(_currentPalette == palette)
            {
                if (FilterManager[ShaderRegistry.Screen_Palette].IsActive())
                {
                    FilterManager.Deactivate(ShaderRegistry.Screen_Palette);
                    return true;
                }
            
            }

            return false;
        }
    }
}

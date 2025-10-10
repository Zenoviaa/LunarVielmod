using Terraria.Graphics.Shaders;

namespace Stellamod.Helpers
{
    public static class ShaderExtensions
    {
        public static void UseIntensity(this MiscShaderData miscShaderData, float intensity)
        {
            miscShaderData.Shader.Parameters["uIntensity"].SetValue(intensity);
        }
    }
}

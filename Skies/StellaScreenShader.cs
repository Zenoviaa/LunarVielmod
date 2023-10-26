using Terraria.Graphics.Shaders;

namespace Stellamod.Skies
{
    //basically just a general screenshader for all of the mod's screen tints
    public class StellaScreenShader : ScreenShaderData
    {
        public StellaScreenShader(string passName) : base(passName)
        {
        }

        private void UpdateSpookyIndex()
        {
        }

        public override void Apply()
        {
            UpdateSpookyIndex();
            base.Apply();
        }
    }
}
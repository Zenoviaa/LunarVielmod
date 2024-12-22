using Terraria;
using Terraria.Graphics.Shaders;

namespace Stellamod
{
    public class VeriplantScreenShaderData : ScreenShaderData
    {
        private int GintzeIndex;

        public VeriplantScreenShaderData(string passName)
            : base(passName)
        {
        }

        private void UpdateMirageIndex()
        {

        }

        public override void Apply()
        {
            UpdateMirageIndex();
            if (GintzeIndex != -1)
            {
                base.UseTargetPosition(Main.npc[GintzeIndex].Center);
            }
            base.Apply();
        }
    }
}

using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod
{
    public class VerliaScreenShaderData : ScreenShaderData
    {
        private int GintzeIndex;
        private bool VerliaBIndex;

        public VerliaScreenShaderData(string passName)
            : base(passName)
        {
        }

        private void UpdateMirageIndex()
        {
            if (VerliaBIndex)
            {
                return;
            }
            GintzeIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (VerliaBIndex)
                {
                    GintzeIndex = i;
                    break;
                }
            }
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
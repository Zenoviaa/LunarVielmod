using Stellamod.NPCs.Bosses.Verlia;
using Stellamod.WorldG;
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
            VerliaBIndex = NPC.AnyNPCs(ModContent.NPCType<VerliaB>());
            if (VerliaBIndex)
            {
                return;
            }
            this.GintzeIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (VerliaBIndex)
                {
                    this.GintzeIndex = i;
                    break;
                }
            }
        }

        public override void Apply()
        {
            this.UpdateMirageIndex();
            if (this.GintzeIndex != -1)
            {
                base.UseTargetPosition(Main.npc[this.GintzeIndex].Center);
            }
            base.Apply();
        }
    }
}
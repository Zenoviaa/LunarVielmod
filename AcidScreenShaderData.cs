using Stellamod.WorldG;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Stellamod
{
    public class GintzeScreenShaderData : ScreenShaderData
	{
		private int GintzeIndex;

		public GintzeScreenShaderData(string passName)
			: base(passName)
		{
		}

		private void UpdateMirageIndex()
		{
	
			if (EventWorld.Gintzing)
			{
				return;
			}
			GintzeIndex = -1;
			for (int i = 0; i < Main.npc.Length; i++)
			{
				if (EventWorld.Gintzing)
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

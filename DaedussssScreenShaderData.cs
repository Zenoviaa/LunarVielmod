using Stellamod.WorldG;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod
{
	public class DaedussssScreenShaderData : ScreenShaderData
	{
		private int GintzeIndex;

		public AbyssScreenShaderData(string passName)
			: base(passName)
		{
		}

		private void UpdateMirageIndex()
		{
	
			if (EventWorld.Gintzing)
			{
				return;
			}
			this.GintzeIndex = -1;
			for (int i = 0; i < Main.npc.Length; i++)
			{
				if (EventWorld.Gintzing)
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

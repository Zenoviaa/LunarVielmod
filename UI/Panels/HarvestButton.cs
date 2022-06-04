using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.Panels
{
	public class HarvestButton : UIState
	{
		public UIImageButton harvestButton;

		public override void OnInitialize()
		{
			Top.Set(-48, 1f);
			Height.Set(48, 0f);
			Width.Set(48, 0f);
			Left.Set(0, 0.6f);

			harvestButton = new UIImageButton(ModContent.Request<Texture2D>("Stellamod/UI/Panels/buttonharvest"));
			harvestButton.Left.Set(0, 0.6f);
			harvestButton.Top.Set(-96, 1f);
			harvestButton.Width.Set(96f, 0f);
			harvestButton.Height.Set(96f, 0f);
			harvestButton.SetVisibility(1f, 1f);
			harvestButton.OnClick += OnClick;

			Append(harvestButton);
		}
		public void OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			for (int i = 0; i < Main.npc.Length; i++)
			{
				NPC npc = Main.npc[i];
				if (npc.active && npc.HasBuff<Harvester>())
				{
					npc.StrikeNPC(9999, 1, 1, false, false, true);
					SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/clickk"));
				}
			}
		}
	}
}

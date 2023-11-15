using Terraria.UI;

namespace Stellamod.UI.Panels
{
    public class HarvestButton : UIState
	{
		/* public UIImageButton harvestButton;

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
		public new void OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			NPC closestNpc = Main.npc.Where(x => x.active).OrderBy(x => x.Center.DistanceSQ(Main.LocalPlayer.Center)).FirstOrDefault();
			float closestDist = float.MaxValue;
			for (int i = 0; i < Main.npc.Length; i++)
			{
				float dist = Main.npc[i].Center.DistanceSQ(Main.LocalPlayer.Center);
				if (!Main.npc[i].active || dist >= closestDist)
				{
					continue;
				}

				closestNpc = Main.npc[i];
				closestDist = dist;

				
				if (closestNpc.active && closestNpc.HasBuff<Harvester>())
				{
					closestNpc.StrikeNPC(9999, 1, 1, false, false, true);
					SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/clickk"));
				}
			}
			
			
		}
	*/}
}

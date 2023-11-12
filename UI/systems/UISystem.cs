using Microsoft.Xna.Framework;
using Stellamod.UI.Panels;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.Systems
{
    public class UISystem : ModSystem
	{
		public UserInterface HarvestButtonLayer;
		public HarvestButton HarvestButtonElement;

		internal Gamble gambled;
		private UserInterface _gambled;

		private GameTime _lastUpdateUiGameTime;
		internal UserInterface userInterface;
		internal HarvestButton uiState;
		public override void Unload()
		{
	
			Gamble.Choice1 = null;
			Gamble.Choice2 = null;
			Gamble.Choice3 = null;
			Gamble.Choice4 = null;
			Gamble.Choice5 = null;
			Gamble.Choice6 = null;
		}
		public override void Load()
		{
			
			if (!Main.dedServ && Main.netMode != NetmodeID.Server)
			{
				uiState = new HarvestButton();
				userInterface = new UserInterface();
				userInterface.SetState(uiState);
				
				
				_gambled = new UserInterface();
				gambled = new Gamble();
				gambled.Activate();
				_gambled.SetState(gambled);
			}
		}
		public override void UpdateUI(GameTime gameTime)
		{
			_lastUpdateUiGameTime = gameTime;
			if (userInterface?.CurrentState != null)
				userInterface.Update(gameTime);


			if (_gambled?.CurrentState != null)
				gambled.Update(gameTime);
			
			
			Player player = Main.LocalPlayer;
		
			MyPlayer GamblePlayer = player.GetModPlayer<MyPlayer>();

			

			if (Gamble.visible) _gambled?.Update(gameTime);

		
		}
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int interfaceLayer = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Cursor"));
			if (interfaceLayer != -1)
			{
				layers.Insert(interfaceLayer, new LegacyGameInterfaceLayer("Stellamod: Bag",
					delegate
					{
						if (_lastUpdateUiGameTime != null && userInterface?.CurrentState != null)
							userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);

						return true;
					},
					InterfaceScaleType.UI));
			}
			int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseTextIndex != -1)
			{
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
					"Stellamod: Gamble Buttons",
					delegate
					{
						if (Gamble.visible) _gambled.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.Game)
				);
			}
		}
	}
}
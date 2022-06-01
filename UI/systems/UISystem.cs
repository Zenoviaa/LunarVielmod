using Microsoft.Xna.Framework;
using Stellamod.UI.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.systems
{
    public class UISystem : ModSystem
    {
        public UserInterface HarvestButtonLayer;
        public HarvestButton HarvestButtonElement;


        private GameTime _lastUpdateUiGameTime;
        internal UserInterface userInterface;
        internal HarvestButton uiState;

        public override void Load()
        {
            if (!Main.dedServ && Main.netMode != NetmodeID.Server)
            {
                uiState = new HarvestButton();
                userInterface = new UserInterface();
                userInterface.SetState(uiState);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (userInterface?.CurrentState != null)
                userInterface.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            
            int interfaceLayer = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Cursor"));
            if (interfaceLayer != -1)
            {
                layers.Insert(interfaceLayer, new LegacyGameInterfaceLayer("Player Swapper: Cursor",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && userInterface?.CurrentState != null)
                            userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);

                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}
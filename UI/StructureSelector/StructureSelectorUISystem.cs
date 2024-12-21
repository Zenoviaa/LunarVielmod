using Microsoft.Xna.Framework;
using Stellamod.UI.ToolsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.StructureSelector
{
    [Autoload(Side = ModSide.Client)]
    internal class StructureSelectorUISystem : ModSystem
    {
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        public static string RootTexturePath => "Stellamod/UI/StructureSelector/";

        public StructureSelectorUIState selectorUIState;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            selectorUIState = new StructureSelectorUIState();
            selectorUIState.Activate();
            _userInterface.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            //Close if inventory isn't open lol
            _lastUpdateUiGameTime = gameTime;
            if (_userInterface?.CurrentState != null)
            {
                _userInterface.Update(gameTime);
            }
        }

        internal void ToggleUI()
        {
            if (_userInterface?.CurrentState != null)
            {
                CloseUI();
            }
            else
            {
                OpenUI();
            }
        }

        internal void OpenUI()
        {
            _userInterface.SetState(selectorUIState);
        }

        private void CloseUI()
        {
            _userInterface.SetState(null);
        }


        public override void PreSaveAndQuit()
        {
            //Calls Deactivate and drops the item
            if (_userInterface.CurrentState != null)
            {
                _userInterface.SetState(null);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "LunarVeil: Structure Selector UI",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && _userInterface?.CurrentState != null)
                        {
                            _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}

using Microsoft.Xna.Framework;
using System.Collections.Generic;
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
        private UserInterface _saveUserInterface;
        public static string RootTexturePath => "Stellamod/UI/StructureSelector/";

        public StructureSelectorUIState selectorUIState;
        public StructureSaveUIState saveUIState;
        public MagicWandUIState magicWandUIState;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _saveUserInterface = new UserInterface();
            _userInterface = new UserInterface();
            selectorUIState = new StructureSelectorUIState();
            selectorUIState.Activate();

            saveUIState = new StructureSaveUIState();
            saveUIState.Activate();

            magicWandUIState = new MagicWandUIState();
            magicWandUIState.Activate();


            _userInterface.SetState(null);
            _saveUserInterface.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            //Close if inventory isn't open lol
            _lastUpdateUiGameTime = gameTime;
            if (_userInterface?.CurrentState != null)
            {
                _userInterface.Update(gameTime);
            }

            if (_saveUserInterface?.CurrentState != null)
            {
                _saveUserInterface.Update(gameTime);
            }
        }

        internal void ToggleUI()
        {
            if (_userInterface?.CurrentState != null)
            {
                CloseUI();
            }
            else if (_userInterface?.CurrentState == null)
            {
                OpenUI();
            }
        }

        internal void OpenMagicWandUI()
        {
            _saveUserInterface.SetState(magicWandUIState);
        }
        internal void CloseMagicWandUI()
        {
            _saveUserInterface.SetState(null);
        }

        internal void OpenSaveUI()
        {

            _saveUserInterface.SetState(saveUIState);
        }
        internal void CloseSaveUI()
        {
            saveUIState.ui.Textbox.Unfocus();
            _saveUserInterface.SetState(null);
        }
        internal void OpenUI()
        {
            selectorUIState.ui.Refresh();
            _userInterface.SetState(selectorUIState);
        }
        public void CloseUI()
        {
            saveUIState.ui.Textbox.Unfocus();
            _userInterface.SetState(null);
        }


        public override void PreSaveAndQuit()
        {
            //Calls Deactivate and drops the item
            if (_userInterface.CurrentState != null)
            {
                _userInterface.SetState(null);
            }
            if (_saveUserInterface.CurrentState != null)
            {
                _saveUserInterface.SetState(null);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Stellamod: Structure Selector UI",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && _userInterface?.CurrentState != null)
                        {
                            _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        if (_lastUpdateUiGameTime != null && _saveUserInterface?.CurrentState != null)
                        {
                            _saveUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}

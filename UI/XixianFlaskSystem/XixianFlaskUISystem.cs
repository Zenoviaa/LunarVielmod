using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.XixianFlaskSystem
{

    [Autoload(Side = ModSide.Client)]
    internal class XixianFlaskUISystem : ModSystem
    {
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        public static string RootTexturePath => "Stellamod/UI/XixianFlaskSystem/";

        public XixianFlaskUIState xixianFlaskUIState;

        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            xixianFlaskUIState = new XixianFlaskUIState();
            xixianFlaskUIState.Activate();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            //Close if inventory isn't open lol
            if (!Main.playerInventory && _userInterface.CurrentState != null)
            {
                CloseUI();
            }

            _lastUpdateUiGameTime = gameTime;
            if (_userInterface?.CurrentState != null)
            {
                _userInterface.Update(gameTime);
            }
        }

        internal void ToggleUI()
        {
            if (_userInterface.CurrentState != null)
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
            //Set State
            xixianFlaskUIState.xixianFlaskUI.slot.OpenUI();
            _userInterface.SetState(xixianFlaskUIState);
        }

        internal void CloseUI()
        {
            _userInterface.SetState(null);
        }

        public override void PreSaveAndQuit()
        {
            //Calls Deactivate and drops the item
            if (_userInterface.CurrentState != null)
            {
                CloseUI();
                _userInterface.SetState(null);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "LunarVeil: Xixian Flask UI",
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

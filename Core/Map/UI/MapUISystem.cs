using Microsoft.Xna.Framework;
using Stellamod.Core.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.Map.UI
{
    [Autoload(Side = ModSide.Client)]
    internal class MapUISystem : BaseUISystem
    {
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        private UserInterface _minimapUserInterface;
        public MapIconUIState mapIconUIState;
        public MapUIState mapUIState;
        public override int uiSlot => Slot_MajorUI;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            _minimapUserInterface = new UserInterface();
            mapUIState = new MapUIState();
            mapUIState.Activate();
            mapIconUIState = new MapIconUIState();
            mapIconUIState.Activate();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (!Main.playerInventory && _userInterface.CurrentState != null)
            {
                CloseUI();
            }

            if (!Main.playerInventory && _minimapUserInterface.CurrentState != null)
            {
                CloseHudUI();
            }
            else if (Main.playerInventory && _minimapUserInterface.CurrentState == null)
            {
                OpenHudUI();
            }
            if (_userInterface?.CurrentState != null)
            {
                _userInterface.Update(gameTime);
            }
            if (_minimapUserInterface?.CurrentState != null)
            {
                _minimapUserInterface.Update(gameTime);
            }
        }

        public override void CloseThis()
        {
            base.CloseThis();
            CloseUI();
        }

        internal void OpenHudUI()
        {
            _minimapUserInterface.SetState(mapIconUIState);
        }

        internal void CloseHudUI()
        {
            _minimapUserInterface.SetState(null);
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
            TakeSlot();
            _userInterface.SetState(mapUIState);
        }

        internal void CloseUI()
        {
            ClearSlot();
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
                    "Urdveil: Map UI",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && _minimapUserInterface?.CurrentState != null)
                        {
                            _minimapUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
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
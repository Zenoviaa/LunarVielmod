using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Stellamod.Helpers;
using Stellamod.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.ItemBrowser
{
    [Autoload(Side = ModSide.Client)]
    public class ItemBrowserSystem : BaseUISystem
    {
        //Alright, so what we're going to do is basically replace the item browser from hero's mod cause it sucks for finding items for our mod
        //We just need better tabs and it to be bigger tbh


        //Here's how it'll work
        //Since the player's inventory is on the left we'll naturally right align it but you can drag the window anywhere
        private bool _pressed;
        private UserInterface _userInterface;
        private GameTime _lastUpdateUiGameTime;
        public ItemBrowserUIState browserUIState;
        public override int uiSlot => Slot_MajorUI;

        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            browserUIState = new();
            browserUIState.Activate();
        }


        public void OpenUI()
        {
            //Create a new editing context
            //Set the state of the interface.
            browserUIState.browser.ResetCategories();
            browserUIState.browser.AddElements();
            _userInterface.SetState(browserUIState);
        }

        public void CloseUI()
        {

            _userInterface.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (InputHelper.KeyDown(Keys.F1))
            {
                _pressed = true;
            }
            else if (_pressed && InputHelper.KeyUp(Keys.F1))
            {
                if (_userInterface.CurrentState == null)
                    OpenUI();
                else
                    CloseUI();

                _pressed = false;
            }
            _lastUpdateUiGameTime = gameTime;
            if (_userInterface?.CurrentState != null)
            {
                _userInterface.Update(gameTime);
            }
        }

        public override void PreSaveAndQuit()
        {
            //Calls Deactivate and drops the item
            if (_userInterface.CurrentState != null)
            {
                //   RenamePetUI.saveItemInUI = true;
                _userInterface.SetState(null);
            }
        }


        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Scarlet Sun: Item Browser",
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

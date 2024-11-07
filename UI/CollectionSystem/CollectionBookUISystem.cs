using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.CollectionSystem
{
    [Autoload(Side = ModSide.Client)]
    internal class CollectionBookUISystem : ModSystem
    {
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        private UserInterface _hudUserInterface;
        private UserInterface _tabsUserInterface;
        public static string RootTexturePath => "Stellamod/UI/CollectionSystem/";

        public CollectionBookUIState collectionBookUI;
        public CollectionBookIconUIState collectionBookIconUI;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            _hudUserInterface = new UserInterface();
            _tabsUserInterface = new UserInterface();

            collectionBookUI = new CollectionBookUIState();
            collectionBookUI.Activate();

            collectionBookIconUI = new CollectionBookIconUIState();
            collectionBookIconUI.Activate();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            //Close if inventory isn't open lol
            if (!Main.playerInventory && _userInterface.CurrentState != null)
            {
                CloseBookUI();
            }

            if (!Main.playerInventory && _hudUserInterface.CurrentState != null)
            {
                CloseHudUI();
            }
            else if (Main.playerInventory && _hudUserInterface.CurrentState == null)
            {
                OpenHudUI();
            }

            _lastUpdateUiGameTime = gameTime;
 
        
            if (_userInterface?.CurrentState != null)
            {
                _userInterface.Update(gameTime);
            }
    
            if (_hudUserInterface?.CurrentState != null)
            {
                _hudUserInterface.Update(gameTime);
            }
        }

        internal void ToggleUI()
        {
            if (_userInterface.CurrentState != null)
            {
                CloseBookUI();
            }
            else
            {
                OpenBookUI();
            }
        }

        internal void OpenBookUI()
        {
            //Set State
            _userInterface.SetState(collectionBookUI);
            collectionBookUI.bookUI.book.Open();
        }

        internal void CloseBookUI()
        {
            collectionBookUI.bookUI.book.Close();
        }


        internal void ReallyCloseBookUI()
        {
            _userInterface.SetState(null);
            _tabsUserInterface.SetState(null);
        }

        internal void OpenHudUI()
        {
            _hudUserInterface.SetState(collectionBookIconUI);
        }

        internal void CloseHudUI()
        {
            _hudUserInterface.SetState(null);
        }

        internal void OpenCollectionUI()
        {

        }

        internal void OpenQuestUI()
        {

        }

        internal void OpenLoreUI()
        {

        }


        public override void PreSaveAndQuit()
        {
            //Calls Deactivate and drops the item
            if (_userInterface.CurrentState != null)
            {
                CloseBookUI();
                _userInterface.SetState(null);
                _tabsUserInterface.SetState(null);
            }
            if (_hudUserInterface.CurrentState != null)
            {
                CloseHudUI();
                _hudUserInterface.SetState(null);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "LunarVeil: Collection Book UI",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && _hudUserInterface?.CurrentState != null)
                        {
                            _hudUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
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

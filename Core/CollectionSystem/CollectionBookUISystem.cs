﻿using Microsoft.Xna.Framework;
using Stellamod.Core.CollectionSystem.Quests;
using Stellamod.Core.QuestSystem;
using Stellamod.Core.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.CollectionSystem
{
    [Autoload(Side = ModSide.Client)]
    internal class CollectionBookUISystem : BaseUISystem
    {
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        private UserInterface _hudUserInterface;
        private UserInterface _tabsUserInterface;
        private UserInterface _rightInfoUserInterface;
        public static string RootTexturePath => "Stellamod/Core/CollectionSystem/";

        public CollectionBookUIState collectionBookUI;
        public CollectionBookIconUIState collectionBookIconUI;
        public CollectionItemTabUIState collectionItemTabUI;
        public CollectionItemTabRecipeUIState collectionRecipeInfoUI;


        public QuestTabUIState questTabUIState;
        public ActiveQuestUIState activeQuestUIState;
        public FragmentsTabUIState fragmentsTabUIState;
        public FragmentDescriptionTabUIState fragmentDescriptionTabUIState;
        public override int uiSlot => Slot_MajorUI;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            _hudUserInterface = new UserInterface();
            _tabsUserInterface = new UserInterface();
            _rightInfoUserInterface = new UserInterface();

            collectionBookUI = new CollectionBookUIState();
            collectionBookUI.Activate();

            collectionBookIconUI = new CollectionBookIconUIState();
            collectionBookIconUI.Activate();

            collectionItemTabUI = new CollectionItemTabUIState();
            collectionItemTabUI.Activate();

            collectionRecipeInfoUI = new CollectionItemTabRecipeUIState();
            collectionRecipeInfoUI.Activate();

            questTabUIState = new QuestTabUIState();
            questTabUIState.Activate();

            activeQuestUIState = new ActiveQuestUIState();
            activeQuestUIState.Activate();

            fragmentsTabUIState = new FragmentsTabUIState();
            fragmentsTabUIState.Activate();

            fragmentDescriptionTabUIState = new FragmentDescriptionTabUIState();
            fragmentDescriptionTabUIState.Activate();

            _userInterface.SetState(null);
            _hudUserInterface.SetState(null);
            _tabsUserInterface.SetState(null);
            _rightInfoUserInterface.SetState(null);
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

            if (_tabsUserInterface?.CurrentState != null)
            {
                _tabsUserInterface.Update(gameTime);
            }
            if (_rightInfoUserInterface?.CurrentState != null)
            {
                _rightInfoUserInterface.Update(gameTime);
            }
        }

        public override void CloseThis()
        {
            base.CloseThis();
            CloseBookUI();
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
            TakeSlot();
            _userInterface.SetState(collectionBookUI);
            collectionBookUI.bookUI.book.Open();
        }

        internal void CloseBookUI()
        {
            ClearSlot();
            collectionBookUI.bookUI.book.Close();
            CloseRightUI();
            CloseTabUI();
        }


        internal void ReallyCloseBookUI()
        {
            _userInterface.SetState(null);
            _tabsUserInterface.SetState(null);
            _rightInfoUserInterface.SetState(null);
        }

        internal void OpenHudUI()
        {
            _hudUserInterface.SetState(collectionBookIconUI);
        }

        internal void CloseHudUI()
        {
            _hudUserInterface.SetState(null);
        }

        internal void OpenCollectionTabUI()
        {
            collectionItemTabUI.ui.Glow = 1f;
            _tabsUserInterface.SetState(collectionItemTabUI);
            _rightInfoUserInterface.SetState(null);
        }

        internal void OpenRecipesInfoUI(Item item)
        {
            collectionRecipeInfoUI.ui.Material = item;
            collectionRecipeInfoUI.ui.Glow = 1f;
            collectionRecipeInfoUI.Recalculate();
            _rightInfoUserInterface.SetState(collectionRecipeInfoUI);
        }

        internal void OpenQuestsTabUI()
        {
            questTabUIState.ui.Glow = 1f;
            _tabsUserInterface.SetState(questTabUIState);
            _rightInfoUserInterface.SetState(null);
        }
        internal void OpenFragmentsTabUI()
        {
            questTabUIState.ui.Glow = 1f;
            _tabsUserInterface.SetState(fragmentsTabUIState);
            _rightInfoUserInterface.SetState(null);
        }


        internal void OpenQuestInfoUI(Quest quest)
        {
            activeQuestUIState.ui.Quest = quest;
            activeQuestUIState.ui.Glow = 1f;
            activeQuestUIState.Recalculate();
            _rightInfoUserInterface.SetState(activeQuestUIState);
        }

        internal void OpenLoreTabUI()
        {

        }

        internal void OpenDescriptionUI()
        {
            _rightInfoUserInterface.SetState(fragmentDescriptionTabUIState);
        }

        internal void CloseTabUI()
        {
            _tabsUserInterface.SetState(null);
        }

        internal void CloseRightUI()
        {
            _rightInfoUserInterface.SetState(null);
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
                    "Urdveil: Collection Book UI",
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
                        if (_lastUpdateUiGameTime != null && _tabsUserInterface?.CurrentState != null)
                        {
                            _tabsUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        if (_lastUpdateUiGameTime != null && _rightInfoUserInterface?.CurrentState != null)
                        {
                            _rightInfoUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}

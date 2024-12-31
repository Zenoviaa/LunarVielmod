using Stellamod.Items.MoonlightMagic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.AdvancedMagicSystem
{
    [Autoload(Side = ModSide.Client)]
    internal class AdvancedMagicUISystem : BaseUISystem
    {
        private UserInterface _backpackInterface;
        private UserInterface _staffInterface;
        private UserInterface _btnInterface;

        public StaffUIState staffUIState;
        public ItemUIState itemUIState;
        public ButtonUIState buttonUIState;

        public static BaseStaff Staff { get; private set; }
        private GameTime _lastUpdateUiGameTime;

        public override int uiSlot => Slot_MajorUI;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _backpackInterface = new UserInterface();
            _staffInterface = new UserInterface();
            _btnInterface = new UserInterface();

            staffUIState = new StaffUIState();
            itemUIState = new ItemUIState();
            buttonUIState = new ButtonUIState();

          
            staffUIState.Activate();
            itemUIState.Activate();
            buttonUIState.Activate();
        }

        public override void Unload()
        {
            base.Unload();
            Staff = null;
        }

        internal void Recalculate()
        {
            staffUIState?.staffUI?.Recalculate();
            staffUIState?.elementUI?.Recalculate();
            itemUIState?.itemUI?.Recalculate();
        }



        internal void OpenUI(BaseStaff staff)
        {
   
            if(Staff == staff)
            {
                CloseStaffUI();
                CloseBackpackUI();
            }
            else
            {
                Staff = staff;
                staffUIState.staffUI.OpenUI(staff);
                staffUIState.elementUI.ElementSlot.OpenUI(staff);
                OpenStaffUI();
                //Recalculate();
    
                if (_backpackInterface.CurrentState == null)
                {
                    OpenBackpackUI();
                }
            }
        }

        internal void ToggleUI()
        {
            if (_backpackInterface.CurrentState != null)
            {
                CloseBackpackUI();
            }
            else
            {
                OpenBackpackUI();
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if(_btnInterface?.CurrentState == null && Main.playerInventory)
            {
                OpenButtonUI();
            }
            else if(_btnInterface?.CurrentState != null && !Main.playerInventory)
            {
                CloseButtonUI();
            }

            if(!Main.playerInventory && _backpackInterface?.CurrentState != null)
            {
                CloseBackpackUI();
            }

            if (!Main.playerInventory && _staffInterface?.CurrentState != null)
            {
                CloseStaffUI();
            }

            _lastUpdateUiGameTime = gameTime;
            if (_backpackInterface?.CurrentState != null)
            {
                _backpackInterface.Update(gameTime);
            }
            if (_staffInterface?.CurrentState != null)
            {
                _staffInterface.Update(gameTime);
            }
            if (_btnInterface?.CurrentState != null)
            {
                _btnInterface.Update(gameTime);
            }
        }

        public override void PreSaveAndQuit()
        {
            //Calls Deactivate and drops the item
            if (_backpackInterface.CurrentState != null)
            {
             //   RenamePetUI.saveItemInUI = true;
                _backpackInterface.SetState(null);
                _staffInterface.SetState(null);
                _btnInterface.SetState(null);
            }
        }

        internal void OpenButtonUI()
        {
            //Set State
            _btnInterface.SetState(buttonUIState);
        }
        public override void CloseThis()
        {
            base.CloseThis();
            CloseStaffUI();
            CloseBackpackUI();
        }

        internal void CloseButtonUI()
        {
            //Kill
            _btnInterface.SetState(null);
        }

        internal void OpenStaffUI()
        {
            //Set State
            _staffInterface.SetState(staffUIState);
        }

        internal void CloseStaffUI()
        {
            _staffInterface.SetState(null);
            Staff = null;
        }

        internal void OpenBackpackUI()
        {
            //Set State
            TakeSlot();
            _backpackInterface.SetState(itemUIState);
        }

        internal void CloseBackpackUI()
        {
            ClearSlot();
            _backpackInterface.SetState(null);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "LunarVeil: Advanced Magic UI",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && _backpackInterface?.CurrentState != null)
                        {
                            _backpackInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        if (_lastUpdateUiGameTime != null && _staffInterface?.CurrentState != null)
                        {
                            _staffInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        if (_lastUpdateUiGameTime != null && _btnInterface?.CurrentState != null)
                        {
                            _btnInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}

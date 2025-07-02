using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Stellamod.Core.ScorpionMountSystem;
using Stellamod.Core.UI;

namespace Stellamod.Core.GunHolsterSystem.UI
{
    [Autoload(Side = ModSide.Client)]
    internal class ScorpionHolsterUISystem : BaseUISystem
    {
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;

        public ScorpionHolsterUIState scorpionHolsterUIState;
        public BaseScorpionItem scorpionItem;
        public override int uiSlot => Slot_MinorUI;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            scorpionHolsterUIState = new ScorpionHolsterUIState();
            scorpionHolsterUIState.Activate();
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

        public override void CloseThis()
        {
            base.CloseThis();
            CloseUI();
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
            scorpionHolsterUIState.ui.OpenUI(scorpionItem);
            _userInterface.SetState(scorpionHolsterUIState);
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
                    "Urdveil: Scorpion Gun Holster UI",
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

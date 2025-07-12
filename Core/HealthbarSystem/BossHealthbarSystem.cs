using Microsoft.Xna.Framework;
using Stellamod.Core.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.HealthbarSystem
{
    [Autoload(Side = ModSide.Client)]
    internal class BossHealthbarSystem : BaseUISystem
    {
        public override int uiSlot => Slot_MinorUI;

        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;

        public BossHealthbarUIState uiState;
        public List<ScarletBoss> ActiveBosses = new List<ScarletBoss>();
        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            uiState = new();
            uiState.Activate();
        }

        public void Add(ScarletBoss boss)
        {
            if (ActiveBosses.Contains(boss))
                return;
            ActiveBosses.Add(boss);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (ActiveBosses.Count > 0)
            {
                uiState.ui.TrackingNpc = ActiveBosses[0];
            }
            else
            {
                uiState.ui.TrackingNpc = null;
            }
            ActiveBosses.Clear();

            //Close if inventory isn't open lol
            if (_userInterface.CurrentState != null && !uiState.ui.IsTracking())
            {
                CloseUI();
            }
            else if (uiState.ui.IsTracking())
            {
                OpenUI();
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
            _userInterface.SetState(uiState);
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
                    "Scarlet Sun: Boss Health UI",
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

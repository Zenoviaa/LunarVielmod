using Microsoft.Xna.Framework;
using Stellamod.Items.Weapons.Igniters;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.PowderSystem
{
    [Autoload(Side = ModSide.Client)]
    internal class PowderUISystem : ModSystem
    {
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        public PowderUIState powderUIState;
        public BaseIgniterCard Card { get; set; }
        public static string RootTexturePath => "Stellamod/UI/PowderSystem/";
        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            powderUIState = new PowderUIState();
            powderUIState.Activate();
        }

        public override void UpdateUI(GameTime gameTime)
        {
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

        internal void Recalculate()
        {
            powderUIState?.powderUI?.Recalculate();
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
            Recalculate();
            _userInterface.SetState(powderUIState);
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
                _userInterface.SetState(null);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "LunarVeil: Powder UI",
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
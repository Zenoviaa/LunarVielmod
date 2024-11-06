using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.GunHolsterSystem
{

    [Autoload(Side = ModSide.Client)]
    internal class GunHolsterUISystem : ModSystem
    {
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        public static string RootTexturePath => "Stellamod/UI/GunHolsterSystem/";

        public GunHolsterUIState gunHolsterUIState;

        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            gunHolsterUIState = new GunHolsterUIState();
            gunHolsterUIState.Activate();
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
            _userInterface.SetState(gunHolsterUIState);
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
                    "LunarVeil: Gun Holster UI",
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

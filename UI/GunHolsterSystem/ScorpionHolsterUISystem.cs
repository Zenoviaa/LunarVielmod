using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using Stellamod.Common.ScorpionMountSystem;

namespace Stellamod.UI.GunHolsterSystem
{
    [Autoload(Side = ModSide.Client)]
    internal class ScorpionHolsterUISystem : ModSystem
    {
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        public static string RootTexturePath => "Stellamod/UI/GunHolsterSystem/";

        public ScorpionHolsterUIState scorpionHolsterUIState;
        public BaseScorpionItem scorpionItem;

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
            _userInterface.SetState(scorpionHolsterUIState);
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
                    "LunarVeil: Scorpion Gun Holster UI",
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

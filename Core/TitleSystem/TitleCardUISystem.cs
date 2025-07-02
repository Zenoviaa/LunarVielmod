using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;


namespace Stellamod.Core.TitleSystem
{
    [Autoload(Side = ModSide.Client)]
    internal class TitleCardUISystem : ModSystem
    {
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        public TitleCardUIState titleUIState;
        public static string RootTexturePath => "Stellamod/Core/TitleSystem/";

        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            titleUIState = new TitleCardUIState();
            titleUIState.Activate();

        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (_userInterface?.CurrentState != null)
            {
                if (titleUIState.titleCardUI.IsFinished)
                {
                    CloseUI();
                }
            }

            if (_userInterface?.CurrentState != null)
            {
                _userInterface.Update(gameTime);
            }
        }

        internal void OpenUI(string text, float duration)
        {
            //Set State
            titleUIState.titleCardUI.ShowWave(text, duration);
            _userInterface.SetState(titleUIState);
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
                    "Urdveil: Title Card UI",
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
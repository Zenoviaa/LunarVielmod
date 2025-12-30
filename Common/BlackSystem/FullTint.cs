using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.BlackSystem
{

    [Autoload(Side = ModSide.Client)]
    public class FullTint : ModSystem
    {
        private UserInterface _userInterface;
        private GameTime _lastUpdateUiGameTime;
        private BlackUIState _black;
        public static Color ScreenTintColor;
        public static float Alpha;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            _black = new BlackUIState();
            _black.Activate();
        }

        public static void SetColor(Color tintColor, float alpha)
        {
            //UI code does not run on the server.
            if (Main.netMode == NetmodeID.Server)
                return;

            ScreenTintColor = tintColor;
            Alpha = alpha;
        }

        public override void UpdateUI(GameTime gameTime)
        {

            if (Alpha > 0)
            {
                _black.ui.Color = ScreenTintColor * Alpha;
                _userInterface.SetState(_black);
            }
            else
            {
                _userInterface.SetState(null);
            }

            if (Alpha > 0)
                Alpha -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            _lastUpdateUiGameTime = gameTime;
            if (_userInterface?.CurrentState != null)
            {
                _userInterface.Update(gameTime);
            }
        }

        public override void PreSaveAndQuit()
        {
            if (_userInterface.CurrentState != null)
            {
                _userInterface.SetState(null);
            }
        }


        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Interface Logic 4"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Scarlet Sun: Black Transition",
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

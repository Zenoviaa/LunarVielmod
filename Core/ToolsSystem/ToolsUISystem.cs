using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.ToolsSystem
{
    [Autoload(Side = ModSide.Client)]
    internal class ToolsUISystem : ModSystem
    {
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        private UserInterface _tilePainterUserInterface;
        public static string RootTexturePath => "Stellamod/Core/ToolsSystem/";

        public ToolsUIState toolsUIState;
        public TilePainterUIState tilePainterUIState;
        public bool ShouldDraw => _tilePainterUserInterface.CurrentState != null;
        public bool ShowHitboxes;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            _tilePainterUserInterface = new UserInterface();

            toolsUIState = new ToolsUIState();
            toolsUIState.Activate();

            tilePainterUIState = new TilePainterUIState();
            tilePainterUIState.Activate();
            _userInterface.SetState(null);
            _tilePainterUserInterface.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            //Close if inventory isn't open lol
            _lastUpdateUiGameTime = gameTime;
            if (_userInterface?.CurrentState != null)
            {
                _userInterface.Update(gameTime);
            }
            if (_tilePainterUserInterface?.CurrentState != null)
            {
                _tilePainterUserInterface.Update(gameTime);
            }



        }

        internal void ToggleUI()
        {
            if (_userInterface?.CurrentState != null)
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
            _userInterface.SetState(toolsUIState);
        }

        public void CloseUI()
        {
            _userInterface.SetState(null);
        }

        public void ToggleTilePainterUI()
        {
            if (_tilePainterUserInterface?.CurrentState != null)
            {
                CloseTilePainterUI();
            }
            else
            {
                OpenTilePainterUI();
            }
        }
        public void OpenTilePainterUI()
        {
            _tilePainterUserInterface.SetState(tilePainterUIState);
        }

        public void CloseTilePainterUI()
        {
            _tilePainterUserInterface.SetState(null);
        }
        internal void ToggleTilePainterUI(bool isOn)
        {
            if (_tilePainterUserInterface?.CurrentState != null && !isOn)
            {
                CloseTilePainterUI();
            }
            else if (_tilePainterUserInterface?.CurrentState == null && isOn)
            {
                OpenTilePainterUI();
            }
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
                    "Urdveil: Tools UI",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && _userInterface?.CurrentState != null)
                        {
                            _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        if (_lastUpdateUiGameTime != null && _tilePainterUserInterface?.CurrentState != null)
                        {
                            _tilePainterUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}

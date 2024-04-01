using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.Dialogue
{
    [Autoload(Side = ModSide.Client)]
    internal class DialogueSystem : ModSystem
    {
        private UserInterface _panel;
        private bool _isVisible;

        public DialoguePanel Panel { get; private set; }

        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
                if (_isVisible)
                {
                    ShowMyUI();
                }
                else
                {
                    HideMyUI();
                }
            }
        }

        public override void Load()
        {
            Panel = new DialoguePanel();
            Panel.Activate();
            _panel = new UserInterface();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _panel?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "YourMod: A Description",
                    delegate
                    {
                        _panel.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public void ShowMyUI()
        {
            _panel?.SetState(Panel);
        }

        public void HideMyUI()
        {
            _panel?.SetState(null);
        }
    }
}

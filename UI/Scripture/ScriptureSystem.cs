using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.Scripture
{
    public class ScripturePlayer : ModPlayer 
    {
        public bool hasScripture;
        public override void ResetEffects()
        {
            hasScripture = false;
        }

        public override void PostUpdateEquips()
        {
            ScriptureSystem scriptureSystem = ModContent.GetInstance<ScriptureSystem>();
            if (Main.myPlayer == Main.LocalPlayer.whoAmI && !hasScripture && scriptureSystem.IsVisible)
            {
                scriptureSystem.IsVisible = false;
            }
        }
    }

    [Autoload(Side = ModSide.Client)]
    public class ScriptureSystem : ModSystem
    {
        private UserInterface _panel;
        private bool _isVisible;

        public ScripturePanel Panel { get; private set; }

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
            Panel = new ScripturePanel();
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

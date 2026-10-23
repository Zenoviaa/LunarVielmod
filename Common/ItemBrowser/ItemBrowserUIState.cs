using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.UI;

namespace Stellamod.Common.ItemBrowser
{
    public class ItemBrowserUIState : UIState
    {
        public ItemBrowserWindow browser;
        public ItemBrowserUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            browser = new ItemBrowserWindow();
            Append(browser);
        }

        public override void Recalculate()
        {
            base.Recalculate();

        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Recalculate();
        }
    }
}

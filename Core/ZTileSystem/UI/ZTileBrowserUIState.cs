using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.UI;

namespace Stellamod.Core.ZTileSystem.UI;

public class ZTileBrowserUIState : UIState
{
    public ZTileBrowserWindow browser;
    public ZTileToolbar toolbar;
    public ZTileBrowserUIState() : base()
    {

    }

    public override void OnInitialize()
    {
        browser = new ZTileBrowserWindow();
        Append(browser);

        toolbar = new ZTileToolbar();
        Append(toolbar);
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
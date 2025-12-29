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
        if (browser == null)
            return;

        //Resize the main panels height based on resolution
        //Recalculate size of the UI based on the resolution, so it's dynamic
        const float size = 706;
        float height = Main.graphics.GraphicsDevice.Viewport.Height;
        float subHeight = height - 32;
        float targetSize = Math.Min(subHeight, size);
        browser.Height.Pixels = targetSize;
        browser.Width.Pixels = targetSize;
    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Recalculate();
    }
}
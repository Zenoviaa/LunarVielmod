using Stellamod.Core.ZTileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellamod.Content.Areas.Cinderspark.TilesCS;

public class RekPillar : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        drawOrigin = TileDrawOrigin.BottomUp;
        frameCount = 1;
    }
}
public class RekPot : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        drawOrigin = TileDrawOrigin.Center;
        frameCount = 1;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        return base.PreDraw(spriteBatch, drawPosition, screenPos, drawParams);
    }
    public override void PostDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        base.PostDraw(spriteBatch, drawPosition, screenPos, drawParams);
    }
}

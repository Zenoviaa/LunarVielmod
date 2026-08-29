using Stellamod.Common.Particles;
using Stellamod.Core.ZTileSystem;
using Terraria;
using Terraria.ID;

namespace Stellamod.Content.Areas.SpringHills.TilesSH;

public class WoodenZui : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        base.PostDraw(spriteBatch, drawPosition, screenPos, drawParams);

    }
}
public class QueenCalamitous : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        base.PostDraw(spriteBatch, drawPosition, screenPos, drawParams);

    }
}
public class TheGreatDoor : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        base.PostDraw(spriteBatch, drawPosition, screenPos, drawParams);

    }
}

public class StoneSword1 : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        base.PostDraw(spriteBatch, drawPosition, screenPos, drawParams);

    }
}

public class StoneSword2 : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }


    public override void PostDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        base.PostDraw(spriteBatch, drawPosition, screenPos, drawParams);

    }
}

public class StoneFurnace : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }

    public override void Update(Vector2 worldPosition)
    {
        base.Update(worldPosition);
        Lighting.AddLight(worldPosition + new Vector2(0, -16), Color.OrangeRed.ToVector3() * 4);
        if (Main.GameUpdateCount % 2 == 0)
        {
            Particles.RagingFlameDust.Spawn(RagingFlameDustData.Default with { position = worldPosition, timeleft = 70 });

        }
    }
}
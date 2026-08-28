using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace Stellamod.Core.Utilities;

public struct HatDrawParameters
{
    public static readonly HatDrawParameters Default = new()
    {
        hatOffset = new Vector2(0, -16),
        wiggleSpeed = 2
    };

    public Vector2 hatOffset;
    public float wiggleSpeed;
}
public static class CostumeUtilities
{
    public static DrawData GetHatDrawData(ref PlayerDrawSet drawInfo, Asset<Texture2D> hatTextureAsset, in HatDrawParameters parameters)
    {
        var position = drawInfo.Center - Main.screenPosition;
        position = new Vector2((int)position.X, (int)position.Y);
        position += parameters.hatOffset;

        Rectangle bodyFrame = drawInfo.drawPlayer.bodyFrame;
        float yOsc = MathF.Sin(bodyFrame.Y) * 0.5f + 0.5f;
        position.Y += yOsc * parameters.wiggleSpeed;
        float rotation = yOsc * MathHelper.ToRadians(5);
        SpriteEffects spriteEffects = drawInfo.drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        var drawData = new DrawData(
            hatTextureAsset.Value,
            position,
            null,
            drawInfo.colorArmorHead,
            rotation,
            hatTextureAsset.Size() * 0.5f,
            1f,
            spriteEffects,
            0
        );
        drawData.shader = drawInfo.cHead;
        return drawData;
    }
}

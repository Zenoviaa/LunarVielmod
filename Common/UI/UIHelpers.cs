using Microsoft.Xna.Framework.Graphics.PackedVector;
using Stellamod.Core.Tooltips;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Stellamod.Common.UI;

public static class UIHelpers
{
    public static float CalculateTooltipsHeight(List<TooltipLine> lines)
    {
        float height = 0;
        List<DrawableTooltipLine> list2 = lines.Select((TooltipLine x, int index) => new DrawableTooltipLine(x, index, 0, 0, Color.White)).ToList();
        for (int num19 = 0; num19 < list2.Count; num19++)
        {
            height += FontAssets.MouseText.Value.MeasureString(list2[num19].Text).Y;
        }
        return height;
    }

    public static void DrawTooltips(SpriteBatch spriteBatch, List<TooltipLine> lines, Vector2 position, int width, float alpha)
    {
        float height = 0;
        List<DrawableTooltipLine> list2 = lines.Select((TooltipLine x, int index) => new DrawableTooltipLine(x, index, 0, 0, Color.White)).ToList();
        for (int num19 = 0; num19 < list2.Count; num19++)
        {
            height += FontAssets.MouseText.Value.MeasureString(list2[num19].Text).Y;
        }

        Rectangle backgroundRect = ExpandableTooltip.GetBGRectangle((int)position.X, (int)position.Y, width, (int)(height + 32));
        Utils.DrawInvBG(spriteBatch, backgroundRect, new Color(23, 25, 81, 255) * 0.925f);

        float yOffset = 42;
        for (int num19 = 0; num19 < list2.Count; num19++)
        {
            float x = position.X;
            float y = position.Y + yOffset;
            Color color = list2[num19].OverrideColor != null ? list2[num19].OverrideColor.Value : list2[num19].Color;
            color *= alpha;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, list2[num19].Font, list2[num19].Text, new Vector2(x, y), color, list2[num19].Rotation, list2[num19].Origin, list2[num19].BaseScale, list2[num19].MaxWidth, list2[num19].Spread);
            yOffset += FontAssets.MouseText.Value.MeasureString(list2[num19].Text).Y;
        }
    }

    public static Vector2 ScreenOffset(Vector2 dimensions, Vector2 normalizedOrigin, Vector2 offset, Vector2? parentDimensions = null)
    {
        if (parentDimensions == null)
        {
            parentDimensions = new Vector2(Main.screenWidth, Main.screenHeight);
        }
        float relativeLeft = parentDimensions.Value.X * normalizedOrigin.X - (dimensions.X / 2) + offset.X;
        float relativeTop = parentDimensions.Value.Y * normalizedOrigin.Y - (dimensions.Y / 2) + offset.Y;
        return new Vector2(relativeLeft, relativeTop);
    }
}

using Stellamod.Common.StaminaSystem;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Stellamod.Core.Utilities;

public static class UIUtils
{
    public static string GetStaminaIconString(in int staminaCost)
    {
        var s = "";
        var type = ModContent.ItemType<StaminaBerry>();
        s += $"{staminaCost}[i:{type}]";
        return s;
    }

    public static void DrawBuffStacksOverlay(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams, int stacks)
    {
        ChatManager.DrawColorCodedStringWithShadow(
            spriteBatch,
            FontAssets.ItemStack.Value,
            $"{stacks}x",
            drawParams.TextPosition,
            drawParams.DrawColor,
            rotation: 0,
            origin: new Vector2(0f),
            baseScale: new Vector2(0.85f));
    }
}

using ReLogic.Content;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Core.UI;

[Autoload(Side = ModSide.Client)]
public class FontLoader : ModSystem
{
    private static bool _replacedFonts;
    public static Asset<DynamicSpriteFont> DeathText;
    public static Asset<DynamicSpriteFont> MouseText;
    private static string FontName => "KleeOne";
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();

    }

    public override void OnModLoad()
    {
        base.OnModLoad();
        DeathText = ModContent.Request<DynamicSpriteFont>($"Stellamod/Assets/Fonts/{FontName}DeathText", AssetRequestMode.ImmediateLoad);
        MouseText = ModContent.Request<DynamicSpriteFont>($"Stellamod/Assets/Fonts/{FontName}MouseText", AssetRequestMode.ImmediateLoad);
        HandleFontActionBasedOnConfig();
    }
    public override void PostSetupContent()
    {
        base.PostSetupContent();
 
    }

    public override void Unload()
    {
        base.Unload();
        UnloadFonts();
    }

    public static void HandleFontActionBasedOnConfig()
    {
        if (LunarVeilClientConfig.Instance.fontReplace && !_replacedFonts)
        {
            ReplaceFonts();
        }
        else if (!LunarVeilClientConfig.Instance.fontReplace && _replacedFonts)
        {
            UnloadFonts();
        }
    }

    private static void ReplaceFonts()
    {
        if (!LunarVeilClientConfig.Instance.fontReplace)
            return;
        if (DeathText == null || MouseText == null)
            return;

        FontAssets.DeathText = DeathText;
        FontAssets.MouseText = MouseText;
        _replacedFonts = true;
    }

    private static void UnloadFonts()
    {
        if (!_replacedFonts)
            return;

        FontAssets.DeathText = ModContent.Request<DynamicSpriteFont>("Terraria/Fonts/Death_Text");
        FontAssets.MouseText = ModContent.Request<DynamicSpriteFont>("Terraria/Fonts/Mouse_Text");
        _replacedFonts = false;
    }
}

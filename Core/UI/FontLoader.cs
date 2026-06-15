using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Core.UI;

[Autoload(Side = ModSide.Client)]
public class FontLoader : ModSystem
{
    private Asset<DynamicSpriteFont> _deathText;
    private Asset<DynamicSpriteFont> _mouseText;
    private string FontName => "KleeOne";
    public override void OnModLoad()
    {
        base.OnModLoad();
        _deathText = ModContent.Request<DynamicSpriteFont>($"Stellamod/Assets/Fonts/{FontName}DeathText", AssetRequestMode.ImmediateLoad);
        _mouseText = ModContent.Request<DynamicSpriteFont>($"Stellamod/Assets/Fonts/{FontName}MouseText", AssetRequestMode.ImmediateLoad);
        On_Main.Update += LoadFonts;
    }

    public override void Unload()
    {
        base.Unload();
        UnloadFonts();
    }
    private void UnloadFonts()
    {
        FontAssets.DeathText = ModContent.Request<DynamicSpriteFont>("Terraria/Fonts/Death_Text");
        FontAssets.MouseText = ModContent.Request<DynamicSpriteFont>("Terraria/Fonts/Mouse_Text");
    }
    private void LoadFonts(On_Main.orig_Update orig, Main self, GameTime gameTime)
    {
        orig(self, gameTime);
        if (_deathText == null || _mouseText == null)
            return;
        if (!_deathText.IsLoaded || !_mouseText.IsLoaded)
            return;
  

        var config = ModContent.GetInstance<LunarVeilClientConfig>();
        if (FontAssets.DeathText == _deathText && (!config.fontReplace))
        {
            UnloadFonts();
        }
        else if (FontAssets.DeathText != _deathText && config.fontReplace)
        {
            FontAssets.DeathText = _deathText;
            FontAssets.MouseText = _mouseText;
        }
    }
}

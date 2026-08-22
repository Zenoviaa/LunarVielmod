using ReLogic.Content;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Core.UI;

[Autoload(Side = ModSide.Client)]
public class FontLoader : ModSystem
{
    private bool _canReplace;
    private bool _replacedFonts;
    private Asset<DynamicSpriteFont> _deathText;
    private Asset<DynamicSpriteFont> _mouseText;
    private string FontName => "KleeOne";
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();

    }
    public override void PostAddRecipes()
    {
        base.PostAddRecipes();
        _canReplace = true;
    }
    public override void OnModLoad()
    {
        base.OnModLoad();
        _canReplace = false;
        _deathText = ModContent.Request<DynamicSpriteFont>($"Stellamod/Assets/Fonts/{FontName}DeathText");
        _mouseText = ModContent.Request<DynamicSpriteFont>($"Stellamod/Assets/Fonts/{FontName}MouseText");
        On_Main.Update += CheckForFontReplacement;
    }

    private void CheckForFontReplacement(On_Main.orig_Update orig, Main self, GameTime gameTime)
    {
        orig(self, gameTime);
        if (!_canReplace)
            return;

        var config = ModContent.GetInstance<LunarVeilClientConfig>();
        if (config.fontReplace)
        {
            if (!_replacedFonts)
            {
                FontAssets.DeathText = _deathText;
                FontAssets.MouseText = _mouseText;
                _replacedFonts = true;
            }
        }
        else
        {
            if (_replacedFonts)
            {
                UnloadFonts();
                _replacedFonts = false;
            }
        }
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

    public override void UpdateUI(GameTime gameTime)
    {
        base.UpdateUI(gameTime);

    }
    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();


    }
}

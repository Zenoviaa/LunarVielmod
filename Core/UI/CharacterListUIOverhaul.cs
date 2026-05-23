using ReLogic.Content;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.IO;
using Terraria.ModLoader;

namespace Stellamod.Core.UI;

[Autoload(Side = ModSide.Client)]
public class CharacterListUIOverhaul : ModSystem
{
    private Asset<Texture2D> _buttonExclamationTexture;
    private Asset<Texture2D> _scrollbarTexture;
    private Asset<Texture2D> _buttonSeedTexture;
    private Asset<Texture2D> _buttonCloudActiveTexture;
    private Asset<Texture2D> _buttonCloudInactiveTexture;
    private Asset<Texture2D> _buttonFavoriteActiveTexture;
    private Asset<Texture2D> _buttonFavoriteInactiveTexture;
    private Asset<Texture2D> _buttonPlayTexture;
    private Asset<Texture2D> _buttonRenameTexture;
    private Asset<Texture2D> _buttonDeleteTexture;
    private Asset<Texture2D> _newInnerPanelTextureAsset;

    private Asset<Texture2D> LoadTexture(string name)
    {
        return ModContent.Request<Texture2D>($"Stellamod/Assets/Textures/UI/{name}");
    }

    public override void Load()
    {
        base.Load();
        _scrollbarTexture = LoadTexture("Scrollbar");
        _newInnerPanelTextureAsset = LoadTexture("InnerPanelBackground");
        _buttonDeleteTexture = LoadTexture("ButtonDelete");
        _buttonRenameTexture = LoadTexture("ButtonRename");
        _buttonPlayTexture = LoadTexture("ButtonPlay");
        _buttonFavoriteInactiveTexture = LoadTexture("ButtonFavoriteInactive");
        _buttonFavoriteActiveTexture = LoadTexture("ButtonFavoriteActive");
        _buttonCloudInactiveTexture = LoadTexture("ButtonCloudInactive");
        _buttonCloudActiveTexture = LoadTexture("ButtonCloudActive");
        _buttonSeedTexture = LoadTexture("ButtonSeed");
        _buttonExclamationTexture = LoadTexture("ButtonExclamation");
        On_UIScrollbar.DrawSelf += ReplaceScrollbarAsset;
        On_UICharacterListItem.InitializeTmlFields += ReplaceAsset;
        On_UIWorldListItem.LoadTmlTextures += ReplaceAssets;

    }

    public override void Unload()
    {
        base.Unload();
        _newInnerPanelTextureAsset = null;
        _buttonRenameTexture = null;
        _buttonCloudActiveTexture = null;
        _buttonCloudInactiveTexture = null;
        _buttonPlayTexture = null;
        _buttonFavoriteActiveTexture = null;
        _buttonFavoriteInactiveTexture = null;
        _buttonCloudInactiveTexture = null;
        _buttonCloudActiveTexture = null;
        _scrollbarTexture = null;
        _buttonSeedTexture = null;
        _buttonExclamationTexture = null;

        On_UIScrollbar.DrawSelf -= ReplaceScrollbarAsset;
        On_UICharacterListItem.InitializeTmlFields -= ReplaceAsset;
        On_UIWorldListItem.LoadTmlTextures -= ReplaceAssets;
    }

    private void ReplaceScrollbarAsset(On_UIScrollbar.orig_DrawSelf orig, UIScrollbar self, SpriteBatch spriteBatch)
    {
        if (ModContent.GetInstance<MainMenuOverhaul>().IsMenuActive && Main.gameMenu)
        {
            typeof(UIScrollbar).GetField("_texture", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(self, _scrollbarTexture);
        }


        orig(self, spriteBatch);

        if (ModContent.GetInstance<MainMenuOverhaul>().IsMenuActive && Main.gameMenu)
        {
            typeof(UIScrollbar).GetField("_texture", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(self, ModContent.Request<Texture2D>("Terraria/Images/UI/Scrollbar"));
        }
    }

    private void ReplaceAssets(On_UIWorldListItem.orig_LoadTmlTextures orig, UIWorldListItem self)
    {
        orig(self);
        if (!ModContent.GetInstance<MainMenuOverhaul>().IsMenuActive)
            return;
        typeof(UIWorldListItem).GetField("_buttonSeedTexture", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(self, _buttonSeedTexture);
        typeof(UIWorldListItem).GetField("_innerPanelTexture", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(self, _newInnerPanelTextureAsset);
        typeof(UIWorldListItem).GetField("_buttonDeleteTexture", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(self, _buttonDeleteTexture);
        typeof(UIWorldListItem).GetField("_buttonRenameTexture", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(self, _buttonRenameTexture);
        typeof(UIWorldListItem).GetField("_buttonPlayTexture", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(self, _buttonPlayTexture);
        typeof(UIWorldListItem).GetField("_buttonPlayTexture", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(self, _buttonPlayTexture);
        typeof(UIWorldListItem).GetField("_buttonFavoriteInactiveTexture", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(self, _buttonFavoriteInactiveTexture);
        typeof(UIWorldListItem).GetField("_buttonFavoriteActiveTexture", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(self, _buttonFavoriteActiveTexture);
        typeof(UIWorldListItem).GetField("_buttonFavoriteActiveTexture", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(self, _buttonFavoriteActiveTexture);
        typeof(UIWorldListItem).GetField("_buttonCloudInactiveTexture", BindingFlags.Instance | BindingFlags.NonPublic)
           .SetValue(self, _buttonCloudInactiveTexture);
        typeof(UIWorldListItem).GetField("_buttonCloudActiveTexture", BindingFlags.Instance | BindingFlags.NonPublic)
          .SetValue(self, _buttonCloudActiveTexture);
    }

    private void ReplaceAsset(On_UICharacterListItem.orig_InitializeTmlFields orig, UICharacterListItem self, PlayerFileData data)
    {
        orig(self, data);
        if (!ModContent.GetInstance<MainMenuOverhaul>().IsMenuActive)
            return;

        typeof(UICharacterListItem).GetField("_innerPanelTexture", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(self, _newInnerPanelTextureAsset);
        typeof(UICharacterListItem).GetField("_buttonDeleteTexture", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(self, _buttonDeleteTexture);
        typeof(UICharacterListItem).GetField("_buttonRenameTexture", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(self, _buttonRenameTexture);
        typeof(UICharacterListItem).GetField("_buttonPlayTexture", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(self, _buttonPlayTexture);
        typeof(UICharacterListItem).GetField("_buttonPlayTexture", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(self, _buttonPlayTexture);
        typeof(UICharacterListItem).GetField("_buttonFavoriteInactiveTexture", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(self, _buttonFavoriteInactiveTexture);
        typeof(UICharacterListItem).GetField("_buttonFavoriteActiveTexture", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(self, _buttonFavoriteActiveTexture);
        typeof(UICharacterListItem).GetField("_buttonFavoriteActiveTexture", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(self, _buttonFavoriteActiveTexture);
        typeof(UICharacterListItem).GetField("_buttonCloudInactiveTexture", BindingFlags.Instance | BindingFlags.NonPublic)
           .SetValue(self, _buttonCloudInactiveTexture);
        typeof(UICharacterListItem).GetField("_buttonCloudActiveTexture", BindingFlags.Instance | BindingFlags.NonPublic)
          .SetValue(self, _buttonCloudActiveTexture);
    }

    private void UnloadTMLCommon()
    {
        //    UICommon.ButtonCollapsedTexture = LoadEmbeddedTexture("Config.UI.ButtonCollapsed");
        //       UICommon.ButtonExpandedTexture = LoadEmbeddedTexture("Config.UI.ButtonExpanded");
        //   UICommon.ButtonErrorTexture = LoadEmbeddedTexture("UI.ButtonError");
        //       UICommon.ButtonExclamationTexture = LoadEmbeddedTexture("UI.ButtonExclamation");
    }



}

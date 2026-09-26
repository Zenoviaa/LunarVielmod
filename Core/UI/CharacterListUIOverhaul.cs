using ReLogic.Content;
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
            self._texture = _scrollbarTexture;
        }

        orig(self, spriteBatch);

        if (ModContent.GetInstance<MainMenuOverhaul>().IsMenuActive && Main.gameMenu)
        {
            self._texture = ModContent.Request<Texture2D>("Terraria/Images/UI/Scrollbar");
        }
    }

    private void ReplaceAssets(On_UIWorldListItem.orig_LoadTmlTextures orig, UIWorldListItem self)
    {
        orig(self);
        if (!ModContent.GetInstance<MainMenuOverhaul>().IsMenuActive)
            return;
        self._buttonSeedTexture = _buttonSeedTexture;
        self._innerPanelTexture = _newInnerPanelTextureAsset;
        self._buttonDeleteTexture = _buttonDeleteTexture;
        self._buttonRenameTexture = _buttonRenameTexture;
        self._buttonPlayTexture = _buttonPlayTexture;
        self._buttonFavoriteInactiveTexture = _buttonFavoriteInactiveTexture;
        self._buttonFavoriteActiveTexture = _buttonFavoriteActiveTexture;
        self._buttonCloudInactiveTexture = _buttonCloudInactiveTexture;
        self._buttonCloudActiveTexture = _buttonCloudActiveTexture;
    }

    private void ReplaceAsset(On_UICharacterListItem.orig_InitializeTmlFields orig, UICharacterListItem self, PlayerFileData data)
    {
        orig(self, data);
        if (!ModContent.GetInstance<MainMenuOverhaul>().IsMenuActive)
            return;

        self._innerPanelTexture = _newInnerPanelTextureAsset;
        self._buttonDeleteTexture = _buttonDeleteTexture;
        self._buttonRenameTexture = _buttonRenameTexture;
        self._buttonPlayTexture = _buttonPlayTexture;
        self._buttonFavoriteInactiveTexture = _buttonFavoriteInactiveTexture;
        self._buttonFavoriteActiveTexture = _buttonFavoriteActiveTexture;
        self._buttonCloudInactiveTexture = _buttonCloudInactiveTexture;
        self._buttonCloudActiveTexture = _buttonCloudActiveTexture;
    }
}

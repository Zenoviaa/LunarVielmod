using ReLogic.Content;
using Stellamod.Common.UI;
using System;
using Terraria;

namespace Stellamod.Common.ArmorShop.UI;

public  struct BannerShopParameters()
{
    public Func<Item[]> AvailableItemsFunction;
    public Action<Item> SelectItemFunction;
    public Func<Item, bool> ViewItemFunction;
    public Func<Item, bool> SelectedItemFunction;
    public string TitleKey;
    public string TooltipKey;
    public Action<SpriteBatch, Item, BannerDrawParameters> DrawFunction;
    public Action<Item> HoverTooltipFunction;
    public Action BuyFunction;
    public Asset<Texture2D> SlotTextureOverride;
    public Action<SpriteBatch, Item, BannerDrawParameters> DrawWhitesFunction;
}

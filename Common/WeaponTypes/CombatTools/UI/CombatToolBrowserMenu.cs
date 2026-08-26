using ReLogic.Content;
using Stellamod.Common.ArmorShop.UI;
using Stellamod.Common.MagicSystem.UI;
using Stellamod.Common.UI;
using Stellamod.Core;
using Stellamod.Helpers;
using Stellamod.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI.Chat;

namespace Stellamod.Common.WeaponTypes.CombatTools.UI;

/// <summary>
/// Creates a menu of all the items in the mod
/// </summary>
public class CombatToolBrowserMenu : UIPanel
{
    private bool _initItems;
    public CombatToolBrowserMenu()
    {
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
    }
    public BannerItemBrowserView View { get; private set; }
    public float BannerWidth => View.Width.Pixels;
    public float BannerHeight => View.Height.Pixels;
    public float textAlpha;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 428;
        Height.Pixels = 236;
        // Append(_view);
    }

    private void Refresh()
    {
        if (Main.gameMenu)
            return;
        if (!_initItems || View == null)
        {
            RemoveAllChildren();
            List<Item> itemList = new List<Item>();

            var items = ModContent.GetContent<ModItem>();

            foreach (var item in items)
            {
                if (item.Item.TryGetGlobalItem<CombatTool>(out var t))
                {
                    if (t.isCombatTool)
                    {
                        itemList.Add(new Item(item.Type));
                    }
                }
              
            }
            var shopParameters = new BannerShopParameters();
            shopParameters.SelectItemFunction = SelectCombatTool;
            shopParameters.ViewItemFunction = ViewCombatTool;
            View = new(itemList.ToArray(), shopParameters);
            View.Width.Pixels = Width.Pixels;
            View.Height.Pixels = Height.Pixels;
            View.Activate();
            _initItems = true;
            Append(View);
        }

        base.Recalculate();
    }


    private void SelectCombatTool(Item item)
    {
        CombatToolPlayer combatToolPlayer = Main.LocalPlayer.GetModPlayer<CombatToolPlayer>();
        if (!combatToolPlayer.HasUnlocked(item))
            return;

        combatToolPlayer.SelectedTool = item;
        combatToolPlayer.SelectedTool.GetGlobalItem<CombatTool>().ammoCount = (int)((float)combatToolPlayer.carryingCapacity * (float)combatToolPlayer.SelectedTool.GetGlobalItem<CombatTool>().maxAmmoCount);
    }

    private bool ViewCombatTool(Item item)
    {
        return Main.LocalPlayer.GetModPlayer<CombatToolPlayer>().HasUnlocked(item);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (View == null)
            Refresh();

        Width.Pixels = BannerWidth;
        Height.Pixels = BannerHeight;
    }
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        string text = LangText.Common("DragHelp");
        Vector2 size = FontAssets.MouseText.Value.MeasureString(text);
        Vector2 centerPos = new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
        centerPos.Y += 256;

        string text2 = LangText.Common("CombatToolHelp");
        Vector2 size2 = FontAssets.MouseText.Value.MeasureString(text2);
        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text2,
            centerPos + new Vector2(0, -32), Color.White * ExtraMath.Osc(0.8f, 1f, speed: 3) * textAlpha, 0f, size2 * 0.5f, new Vector2(1f), -1f, 1f);

        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text,
            centerPos, Color.Lerp(Color.White, Color.Black, 0.5f) * ExtraMath.Osc(0.8f, 1f, speed: 3) * textAlpha, 0f, size * 0.5f, new Vector2(1f), -1f, 1f);

        string text3 = LangText.Common("CombatTool");
        Vector2 size3 = FontAssets.DeathText.Value.MeasureString(text3);
        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, text3,
            centerPos + new Vector2(0, -384), Color.White * textAlpha, 0f, size3 * 0.5f, new Vector2(1f), -1f, 1f);


    }
}



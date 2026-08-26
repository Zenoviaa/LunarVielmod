using Stellamod.Common.ArmorShop.UI;
using Stellamod.Common.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Stellamod.Common.ClassReworkSystem.AmmoRework.UI;

/// <summary>
/// Creates a menu of all the items in the mod
/// </summary>
public class AmmoToolBrowserMenu : UIPanel
{
    private bool _initItems;

    public AmmoToolBrowserMenu()
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
            ActSystem actSystem = ModContent.GetInstance<ActSystem>();
            itemList.AddRange(ItemHelper.Act1Ammos);
            if (actSystem.act2)
                itemList.AddRange(ItemHelper.Act2Ammos);
            if (actSystem.act3)
                itemList.AddRange(ItemHelper.Act3Ammos);
            BannerShopParameters shopParameters = new();
            shopParameters.AvailableItemsFunction = () => itemList.ToArray();
            shopParameters.SelectItemFunction = SelectCombatTool;
            shopParameters.ViewItemFunction = ViewCombatTool;
            shopParameters.SelectedItemFunction = HasSelectedCombatTool;
            View = new(shopParameters.AvailableItemsFunction(), shopParameters);
            View.Width.Pixels = Width.Pixels;
            View.Height.Pixels = Height.Pixels;
            View.Activate();
            _initItems = true;
            Append(View);
        }

        base.Recalculate();
    }

    private bool HasSelectedCombatTool(Item item)
    {
        ClassReworkPlayer combatToolPlayer = Main.LocalPlayer.GetModPlayer<ClassReworkPlayer>();
        if (combatToolPlayer.QuiverAmmoItem == null)
            return false;
        if (combatToolPlayer.QuiverAmmoItem.type == item.type)
            return true;
        return false;
    }

    private void SelectCombatTool(Item item)
    {
        ClassReworkPlayer combatToolPlayer = Main.LocalPlayer.GetModPlayer<ClassReworkPlayer>();
        if (!combatToolPlayer.HasAmmo(item))
            return;

        combatToolPlayer.QuiverAmmoItem = item.Clone();
        combatToolPlayer.QuiverAmmoItem.stack = 9999;
        SoundStyle selectedSound = new SoundStyle("Stellamod/Assets/Sounds/Gun/GunReload");
        SoundEngine.PlaySound(selectedSound);
    }

    private bool ViewCombatTool(Item item)
    {
        return Main.LocalPlayer.GetModPlayer<ClassReworkPlayer>().HasAmmo(item);
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
        Rectangle r = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        SpritebatchDrawer d = SpritebatchDrawer.FromTextureAsset(TextureAssets.BlackTile.Value, Vector2.Zero);
        d.dstRect = r;
        d.drawOrigin = Vector2.Zero;
        d.color = Color.Black * textAlpha * 0.8f;
        spriteBatch.Draw(d);

        string text = LangText.Common("DragHelp");
        Vector2 size = FontAssets.MouseText.Value.MeasureString(text);
        Vector2 centerPos = new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
        centerPos.Y += 256;

        string text2 = LangText.Common("QuiverHelp");
        Vector2 size2 = FontAssets.MouseText.Value.MeasureString(text2);
        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text2,
            centerPos + new Vector2(0, -412), Color.White * ExtraMath.Osc(0.8f, 1f, speed: 3) * textAlpha, 0f, size2 * 0.5f, new Vector2(1f), -1f, 1f);

        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text,
            centerPos + new Vector2(0, -382), Color.Lerp(Color.White, Color.Black, 0.5f) * ExtraMath.Osc(0.8f, 1f, speed: 3) * textAlpha, 0f, size * 0.5f, new Vector2(1f), -1f, 1f);

        string text3 = LangText.Common("MagicQuiver");
        Vector2 size3 = FontAssets.DeathText.Value.MeasureString(text3);
        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, text3,
            centerPos + new Vector2(0, -452), Color.White * textAlpha, 0f, size3 * 0.5f, new Vector2(1f), -1f, 1f);


    }
}

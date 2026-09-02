using ReLogic.Content;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.WeaponTypes.CombatTools.UI;

public class CombatToolSlot : UIElement
{
    private readonly int _context;
    private readonly float _scale;

    private UIText _countText;
    private UIText _keybindText;
    private Asset<Texture2D> _slotTextureAsset;
    public CombatToolSlot(int context = ItemSlot.Context.InventoryItem, float scale = 1f)
    {
        _context = context;
        _scale = scale;

        _slotTextureAsset = ModContent.Request<Texture2D>(
            $"Stellamod/Common/WeaponTypes/CombatToolSlot", AssetRequestMode.ImmediateLoad);
        _countText = new UIText("0");
        _keybindText = new UIText("");
        Width.Set(_slotTextureAsset.Width() * scale, 0f);
        Height.Set(_slotTextureAsset.Height() * scale, 0f);
        OnLeftClick += OpenUI;
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        _countText.Left.Set(0, 0.1f);
        _countText.Top.Set(0, 0.5f);
        Append(_countText);
        Append(_keybindText);
    }

    private void OpenUI(UIMouseEvent evt, UIElement listeningElement)
    {
        //Don't open if haven't unlocked
        CombatToolUISystem uiSystem = ModContent.GetInstance<CombatToolUISystem>();
        uiSystem.ToggleUI();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        List<string> keys = LunarVeilKeybinds.ToolKeybind.GetAssignedKeys();
        if (keys.Count > 0)
        {
            _keybindText.SetText(keys[0]);
        }
        else
        {
            _keybindText.SetText("");
        }
        _keybindText.Left.Set(0, 0.75f);
        _keybindText.Top.Set(0, 0.75f);
        Player player = Main.LocalPlayer;
        int flaskBuffType = ModContent.BuffType<CannotUseFlask>();
        int buffIndex = player.FindBuffIndex(flaskBuffType);
        if (buffIndex == -1)
        {
            _countText.SetText("");
            return;
        }

        int remainingTime = player.buffTime[buffIndex];
        float ticks = remainingTime;
        float seconds = ticks / 60f;
        _countText.SetText(seconds.ToString("#.#"));
    }
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        Color color2 = Main.inventoryBack;
        Rectangle rectangle = GetDimensions().ToRectangle();
        Vector2 pos = rectangle.TopLeft();
        Texture2D backingTexture = _slotTextureAsset.Value;
        int offset = (int)(backingTexture.Size().Y / 2);
        Vector2 centerPos = pos + rectangle.Size() / 2f;
        spriteBatch.Draw(backingTexture, rectangle.TopLeft(), null, color2, 0f, default, _scale, SpriteEffects.None, 0f);


        Item item = Main.LocalPlayer.GetModPlayer<CombatToolPlayer>().SelectedTool;
        if (item == null)
            return;
        if(item.IsAir)
            return;
        CombatTool combatTool = item.GetGlobalItem<CombatTool>();
        if (combatTool == null)
            return;

        int ammoCount = combatTool.ammoCount;
        _countText.SetText($"x{ammoCount}");
        float oldScale = Main.inventoryScale;
        Main.inventoryScale = _scale;

        bool contains = ContainsPoint(Main.MouseScreen);
        if (contains && !PlayerInput.IgnoreMouseInterface)
        {
            Main.LocalPlayer.mouseInterface = true;
            Main.hoverItemName = item.HoverName;
            Main.HoverItem = item;
        }

        //Draw Backing

        Color itemColor = Color.White;
        if(ammoCount <= 0)
        {
            color2 = Color.Lerp(color2, Color.Black, 0.75f);
            itemColor = Color.Lerp(itemColor, Color.Black, 0.75f);
        }

        ItemSlot.DrawItemIcon(item, _context, spriteBatch, centerPos, _scale, 32, itemColor);
        Main.inventoryScale = oldScale;
    }
}


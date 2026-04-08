using ReLogic.Content;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.MagicSystem.UI;
using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Core.Utilities;
using Stellamod.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Content.Armors.GovheilQueen;

#region Sub Wand UI
[Autoload(Side = ModSide.Client)]
public class SubWandUISystem : BaseUISystem
{
    private GameTime _lastUpdateUiGameTime;
    private UserInterface _hudUserInterface;

    public SubWandSlotUIState subWandSlotUIState;
    public override int uiSlot => Slot_MajorUI;
    public override void OnModLoad()
    {
        base.OnModLoad();
        _hudUserInterface = new UserInterface();
        _hudUserInterface.SetState(null);


    }

    public override void UpdateUI(GameTime gameTime)
    {
        //Close if inventory isn't open lol
        if (_hudUserInterface.CurrentState == null)
        {
            OpenHudUI();
        }

        _lastUpdateUiGameTime = gameTime;
        if (_hudUserInterface?.CurrentState != null)
        {
            _hudUserInterface.Update(gameTime);
        }
    }

    public override void CloseThis()
    {
        base.CloseThis();
        CloseUI();
    }

    public void OpenHudUI()
    {
        subWandSlotUIState = new();
        subWandSlotUIState.Activate();
        _hudUserInterface.SetState(subWandSlotUIState);
    }

    public void CloseHudUI()
    {
        _hudUserInterface.SetState(null);
    }
    public void OpenUI()
    {
        //Set State
        TakeSlot();
    }

    public void CloseUI()
    {
        ClearSlot();
    }

    public override void PreSaveAndQuit()
    {
        if (_hudUserInterface.CurrentState != null)
        {
            CloseHudUI();
            _hudUserInterface.SetState(null);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex != -1)
        {
            layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                "Stellamod: Sub Wand UI",
                delegate
                {
                    if (_lastUpdateUiGameTime != null && _hudUserInterface?.CurrentState != null)
                    {
                        _hudUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);

                    }

                    return true;
                },
                InterfaceScaleType.UI));
        }
    }

}
public class SubWandSlotUIState : UIState
{
    public SubWandSlotPanel panel;
    public SubWandSlotUIState() : base()
    {

    }

    public override void OnInitialize()
    {
        panel = new();
        Append(panel);
    }
}

public class SubWandSlotPanel : UIPanel
{
    private UIPanel _panel;
    public SubWandSlot slot;

    public const int width = 432;
    public const int height = 280;

    public int RelativeLeft
    {
        get
        {
            if (!Main.playerInventory || !Main.LocalPlayer.GetModPlayer<AdvancedMagicPlayer>().hasMiniWand)
            {
                return 11600;
            }
            return 800;
        }
    }
    public int RelativeTop => 8;

    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 48 * 5f;
        Height.Pixels = 48 * 16;
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;

        _panel = new UIPanel();
        _panel.Width.Pixels = Width.Pixels;
        _panel.Height.Pixels = Height.Pixels;
        _panel.BackgroundColor = Color.Transparent;
        _panel.BorderColor = Color.Transparent;
        Append(_panel);

        slot = new();
        _panel.Append(slot);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        //Constantly lock the UI in the position regardless of resolution changes
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
    }
}

public class SubWandSlot : UIElement
{
    private readonly int _context;
    private readonly float _scale;
    private Asset<Texture2D> _slotTextureAsset;
    public SubWandSlot(int context = ItemSlot.Context.InventoryItem, float scale = 1f)
    {
        _context = context;
        _scale = scale;
        _slotTextureAsset = ModContent.Request<Texture2D>(
            this.GetTypeDirectoryWithSlash() + "SubWandSlot", AssetRequestMode.ImmediateLoad);
        Width.Set(_slotTextureAsset.Width() * scale, 0f);
        Height.Set(_slotTextureAsset.Height() * scale, 0f);
        OnLeftClick += OpenUI;
    }

    public override void OnInitialize()
    {
        base.OnInitialize();


    }

    public override void OnActivate()
    {
        base.OnActivate();
        AdvancedMagicPlayer magicPlayer = Main.LocalPlayer.GetModPlayer<AdvancedMagicPlayer>();
        if (magicPlayer.miniWand.IsAir)
        {
            magicPlayer.miniWand = ModContent.GetModItem(ModContent.ItemType<SubWand>()).Item.Clone();
        }
    }

    private void OpenUI(UIMouseEvent evt, UIElement listeningElement)
    {
        AdvancedMagicPlayer magicPlayer = Main.LocalPlayer.GetModPlayer<AdvancedMagicPlayer>();
        ModContent.GetInstance<MagicUISystem>().OpenUI(magicPlayer.miniWand.ModItem as AbstractMagicWand);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        AdvancedMagicPlayer magicPlayer = Main.LocalPlayer.GetModPlayer<AdvancedMagicPlayer>();
        Item item = magicPlayer.miniWand;
        float oldScale = Main.inventoryScale;
        Main.inventoryScale = _scale;
        Rectangle rectangle = GetDimensions().ToRectangle();
        bool contains = ContainsPoint(Main.MouseScreen);
        if (contains && !PlayerInput.IgnoreMouseInterface)
        {
            Main.LocalPlayer.mouseInterface = true;
            Main.hoverItemName = item.HoverName;
            Main.HoverItem = item;
        }

        //Draw Backing
        Color color2 = Main.inventoryBack;
        Vector2 pos = rectangle.TopLeft();

        Texture2D backingTexture = _slotTextureAsset.Value;
        int offset = (int)(backingTexture.Size().Y / 2);
        Vector2 centerPos = pos + rectangle.Size() / 2f;
        spriteBatch.Draw(backingTexture, rectangle.TopLeft(), null, color2, 0f, default, _scale, SpriteEffects.None, 0f);

        ItemSlot.DrawItemIcon(item, _context, spriteBatch, centerPos, _scale, 32, Color.White);
        Main.inventoryScale = oldScale;
    }
}

#endregion
public class SubWand : AbstractMagicWand
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 35;
        Item.shootSpeed = 10;
        Item.useTime = 18;
        Item.useAnimation = 36;
        Size = 8;
        TrailLength = 16;
        normalSlotCount = 2;
        timedSlotCount = 1;
    }
}

[AutoloadEquip(EquipType.Head)]
public class GovheilMask : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        ArmorSetSystem.RegisterArmorSet<GovheilMask, GovheilBreastplate, GovheilQueenThighs>();
    }

    public override void SetDefaults()
    {
        Item.width = 18; // Width of the item
        Item.height = 18; // Height of the item
        Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
        Item.rare = ItemRarityID.LightRed; // The rarity of the item
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.wandNormalEnchantmentSlots += 2;
        stats.defenseBonus += 6;
        stats.accessorySlots++;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<GovheilBreastplate>() && legs.type == ModContent.ItemType<GovheilQueenThighs>();
    }


    public override void UpdateArmorSet(Player player)
    {
        player.GetModPlayer<AdvancedMagicPlayer>().hasMiniWand = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class GovheilBreastplate : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.width = 18; // Width of the item
        Item.height = 18; // Height of the item
        Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
        Item.rare = ItemRarityID.LightRed; // The rarity of the item
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.wandCastTime += 0.5f;
        stats.defenseBonus += 8;
        stats.accessorySlots += 1;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class GovheilQueenThighs : ModItem
{
    public override void SetStaticDefaults()
    {
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.width = 18; // Width of the item
        Item.height = 18; // Height of the item
        Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
        Item.rare = ItemRarityID.LightRed; // The rarity of the item
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.defenseBonus += 4;
        stats.accessorySlots += 1;
    }
}
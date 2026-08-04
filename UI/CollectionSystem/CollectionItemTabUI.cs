using Stellamod.Common.ArmorRework;
using Stellamod.Common.ArmorShop;
using Stellamod.Common.Shaders;
using Stellamod.Core.Tooltips;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials.Molds;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.UI.CollectionSystem;


public class CollectionItemTabCraft : UIElement
{
    public Item Item;
    public CollectionItemTabCraft()
    {
        Item = new Item();
        Item.SetDefaults(ItemID.None);

        var value = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}CollectionTabSlot",
            ReLogic.Content.AssetRequestMode.ImmediateLoad);
        Width.Set(value.Width(), 0f);
        Height.Set(value.Height(), 0f);
    }

    public ArmorSet armorSet;
    public int slotType;
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }


    private void DrawBrewingCraft(SpriteBatch spriteBatch)
    {
        Rectangle rectangle = GetDimensions().ToRectangle();
        if (IsMouseHovering && !PlayerInput.IgnoreMouseInterface)
        {
            Main.LocalPlayer.mouseInterface = true;
        }
        //  Main.NewText(IsMouseHovering);

        bool contains = ContainsPoint(Main.MouseScreen);
        //Draw Backing
        Color color2 = Main.inventoryBack;
        Vector2 pos = rectangle.TopLeft();
        Vector2 centerPos = pos + rectangle.Size() / 2f;

        var cauldronPlayer = Main.LocalPlayer.GetModPlayer<CauldronPlayer>();
        var cauldron = ModContent.GetInstance<Cauldron>();
        Color drawColor = Color.White;
        if (!cauldronPlayer.HasMadeItem(Item))
        {
            drawColor = Color.Black;
            if (contains)
            {
                MoldTooltipItem t = ModContent.GetModItem(ModContent.ItemType<MoldTooltipItem>()) as MoldTooltipItem;
                if (t.MoldNeeded == null)
                {
                    t.MoldNeeded ??= new Item();
                    t.MoldNeeded.SetDefaults(ItemID.None);
                }

                t.MoldNeeded = cauldron.FindMold(new Item(Item.type));
                Main.hoverItemName = "Testing Testing 123";
                Main.HoverItem = t.Item;
            }
        }
        else
        {
            if (contains)
            {
                var tooltipLines = new List<TooltipLine>();
                TooltipLine helpLine = new TooltipLine(Stellamod.Instance, "ArmorHelp", Item.Name);
                helpLine.OverrideColor = Color.Goldenrod;
                tooltipLines.Add(helpLine);

                var renderer = ModContent.GetInstance<ExpandableTooltipRenderer>();
                renderer.SetTooltipsToDraw(tooltipLines, 28, 16);
            }
        }

        ItemSlot.DrawItemIcon(Item, ItemSlot.Context.InventoryItem, spriteBatch, centerPos, 1f, 32, drawColor);
    }
    public override int CompareTo(object obj)
    {
        if (obj is CollectionItemTabCraft other)
        {
            if (other.slotType == 1 && slotType == 1)
            {
                return armorSet.act.CompareTo(other.armorSet.act);
            }
            else
            {
                return slotType.CompareTo(other.slotType);
            }

        }

        return base.CompareTo(obj);
    }

    private void DrawArmor(SpriteBatch spriteBatch)
    {
        Height.Pixels = 72;

        Rectangle rectangle = GetDimensions().ToRectangle();
        Vector2 pos = rectangle.TopLeft();
        Texture2D value = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}CollectionTabSlotArmor").Value;
        Vector2 centerPos = rectangle.TopLeft() + value.Size() * 0.5f + new Vector2(-10, -10);
        spriteBatch.Draw(value, rectangle.TopLeft(), null, Color.White, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);


        /*
        ArmorRenderSystem armorRenderSystem = ModContent.GetInstance<ArmorRenderSystem>();
        ArmorSetSystem.GetArmorSet(armorSet, out Item helm, out Item armor, out Item leggings);
        Rectangle frame = armorRenderSystem.armorFrames[helm.type];
        spriteBatch.Draw(armorRenderSystem.armorRT, centerPos, frame, Color.White, 0, frame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
        */

        RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
        SamplerState anisotropicClamp = SamplerState.PointClamp;
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);

        DiscoveredArmorsPlayer armorsPlayer = Main.LocalPlayer.GetModPlayer<DiscoveredArmorsPlayer>();
        ArmorSetSystem.GetArmorSet(armorSet, out Item helm, out Item armor, out Item leggings);
        ArmorReworkPlayerRenderer.silhouette =
            !armorsPlayer.IsAnyDiscovered(helm.type, armor.type, leggings.type);
        ExpandableTooltip.DrawArmorPreview(centerPos, helm, armor, leggings);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);

        if (IsMouseHovering)
        {
            var tooltipLines = new List<TooltipLine>();
            string text = LangText.Armor(helm.ModItem, "Set");
            string combinedText = $"{text} {LangText.Common("Armor")}";
            TooltipLine helpLine = new TooltipLine(Stellamod.Instance, "ArmorHelp", combinedText);
            helpLine.OverrideColor = Color.Goldenrod;
            tooltipLines.Add(helpLine);


            TooltipLine helpLine3 = new TooltipLine(Stellamod.Instance, "ArmorDesc", LangText.Armor(helm.ModItem, "Type"));
            helpLine3.OverrideColor = Color.Gray;
            tooltipLines.Add(helpLine3);

            ArmorShopGroups armorShopGroups = ModContent.GetInstance<ArmorShopGroups>();
            ArmorShopSet armorShopSet = armorShopGroups.FindSet(helm);
            if(armorShopSet != null)
            {
                string craftString = LangText.Common("ArmorCraft", armorShopSet.material.Name);
                TooltipLine helpLine2 = new TooltipLine(Stellamod.Instance, "ArmorHelp", craftString);
                helpLine2.OverrideColor = Color.White;
                tooltipLines.Add(helpLine2);
            }


            var renderer = ModContent.GetInstance<ExpandableTooltipRenderer>();
            renderer.SetTooltipsToDraw(tooltipLines, 28, 16);
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        switch (slotType)
        {
            case 0:
                DrawBrewingCraft(spriteBatch);
                break;
            case 1:
                DrawArmor(spriteBatch);
                break;
        }
    }
}



/// <summary>
/// The individual slot that you click on to open up all the items you can create with that material
/// </summary>
public class CollectionItemTabSlot : UIElement
{
    private Vector2 _hoverScale;
    public CollectionItemTabSlot()
    {
        item = new Item();
        item.SetDefaults(ItemID.None);

        var value = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}CollectionTabSlot",
            ReLogic.Content.AssetRequestMode.ImmediateLoad);
        Width.Set(value.Width() * 16, 0f);
        Height.Set(value.Height(), 0f);
        OnLeftClick += OnButtonClick;
        OnMouseOver += OnMouseHover;
    }

    public Item item;
    public ArmorSet armorSet;
    public int slotType;
    private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
    {
        CollectionBookUISystem uiSystem = ModContent.GetInstance<CollectionBookUISystem>();
        switch (slotType)
        {
            case 0:

                uiSystem.OpenRecipesInfoUI(item);
                break;
            case 1:
                uiSystem.OpenArmorInfoUI();
                break;
        }

    }

    private void OnMouseHover(UIMouseEvent evt, UIElement listeningElement)
    {

    }
    public override int CompareTo(object obj)
    {
        if (obj is CollectionItemTabSlot other)
        {
            if (other.slotType == 0 && slotType == 0)
            {
                Item itemA = item;
                Item itemB = other.item;
                return Cauldron.MaterialOrder[itemA.type].CompareTo(Cauldron.MaterialOrder[itemB.type]);
            }
            else if (other.slotType == 1 && slotType == 1)
            {
                return armorSet.act.CompareTo(other.armorSet.act);
            }
            else
            {
                return other.CompareTo(slotType);
            }

        }

        return base.CompareTo(obj);
    }

    private void DrawArmorSet(SpriteBatch spriteBatch)
    {
        Height.Pixels = 100;
        Rectangle rectangle = GetDimensions().ToRectangle();
        Vector2 pos = rectangle.TopLeft();
        Texture2D value = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}CollectionTabSlotArmor").Value;
        Vector2 centerPos = rectangle.TopLeft() + value.Size() * 0.5f + new Vector2(-10, -10);
        RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
        SamplerState anisotropicClamp = SamplerState.PointClamp;
        SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;
        if (IsMouseHovering)
        {
            float outlineOffset = 2;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, whiteShader.Effect, Main.UIScaleMatrix);

            for (float f = 0; f < MathHelper.TwoPi; f += MathHelper.PiOver2)
            {
                Vector2 offset = f.ToRotationVector2() * outlineOffset;
                spriteBatch.Draw(value, rectangle.TopLeft() + offset, null, Color.White, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, default, Main.UIScaleMatrix);
        }


        spriteBatch.Draw(value, rectangle.TopLeft(), null, Color.White, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);


        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);

        ArmorSetSystem.GetArmorSet(armorSet, out Item helm, out Item armor, out Item leggings);
        ExpandableTooltip.DrawArmorPreview(centerPos, helm, armor, leggings);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);


        //Drawing the number of things you have unlocked
        {
            var armorPlayer = Main.LocalPlayer.GetModPlayer<DiscoveredArmorsPlayer>();
            var armorSetSystem = ModContent.GetInstance<ArmorSetSystem>();
            string amountYouHave = $"{armorPlayer.CountDiscoveredArmors()} / {ArmorSetSystem.GetArmorSets().Length}";
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(item.Name);
            Vector2 origin = new Vector2(0f, textSize.Y * 0.5f);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, amountYouHave, centerPos + Vector2.UnitX * 48,
                Color.White, 0, origin, Vector2.One);
        }

        //Draw the name of the item
        {
            string drawString = "Armors";
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(drawString);
            Vector2 origin = new Vector2(0f, textSize.Y * 0.5f);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, drawString, centerPos + Vector2.UnitX * 128 + Vector2.UnitY * 13,
                Color.White, 0, origin, Vector2.One);
        }

    }

    private void DrawBrewingMaterial(SpriteBatch spriteBatch)
    {
        Rectangle rectangle = GetDimensions().ToRectangle();

        //Draw Backing
        Vector2 pos = rectangle.TopLeft();
        Texture2D value = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}CollectionTabSlot").Value;
        Vector2 centerPos = rectangle.TopLeft() + new Vector2(18, rectangle.Height * 0.5f);
        spriteBatch.Draw(value, rectangle.TopLeft(), null, Color.White, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);

        if (IsMouseHovering)
        {
            var whiteShader = SpriteWhiteShader.Instance;
            float outlineOffset = 2;
            RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
            SamplerState anisotropicClamp = SamplerState.PointClamp;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, whiteShader.Effect, Main.UIScaleMatrix);

            for (float f = 0; f < MathHelper.TwoPi; f += MathHelper.PiOver2)
            {
                Vector2 offset = f.ToRotationVector2() * outlineOffset;
                ItemSlot.DrawItemIcon(item, ItemSlot.Context.InventoryItem, spriteBatch, centerPos + offset, _hoverScale.X, 32, Color.White);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, default, Main.UIScaleMatrix);
        }
        SamplerState p = SpritebatchParams.GetSamplerState(spriteBatch);
        if (p != SamplerState.AnisotropicClamp)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);

        }
        ItemSlot.DrawItemIcon(item, ItemSlot.Context.InventoryItem, spriteBatch, centerPos, _hoverScale.X, 32, Color.White);


        //Drawing the number of things you have unlocked
        {
            var cauldronPlayer = Main.LocalPlayer.GetModPlayer<CauldronPlayer>();
            var cauldron = ModContent.GetInstance<Cauldron>();
            string amountYouHave = $"{cauldronPlayer.CountCraftsInMaterial(item.type)} / {cauldron.CountCraftsInMaterial(item.type)}";
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(item.Name);
            Vector2 origin = new Vector2(0f, textSize.Y * 0.5f);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, amountYouHave, centerPos + Vector2.UnitX * 32,
                Color.White, 0, origin, Vector2.One);
        }

        //Draw the name of the item
        {
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(item.Name);
            Vector2 origin = new Vector2(0f, textSize.Y * 0.5f);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, item.Name, centerPos + Vector2.UnitX * 128,
                Color.White, 0, origin, Vector2.One);
        }

        //Hovering text
        if (IsMouseHovering)
        {
            Main.hoverItemName = item.Name;
            Main.HoverItem = item;
        }
    }
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {

        Vector2 targetHoverScale = IsMouseHovering ? new Vector2(1.2f) : Vector2.One;
        _hoverScale = Vector2.Lerp(_hoverScale, targetHoverScale, 0.25f);
        switch (slotType)
        {
            case 0:
                DrawBrewingMaterial(spriteBatch);
                break;
            case 1:
                DrawArmorSet(spriteBatch);
                break;
        }
        this.QuickMouseInteraction();
    }
}

public class CollectionItemRecipesUI : UIPanel
{
    private UIList _uiList;
    private UIPanel _panel;
    private UIGrid _slotGrid;
    private FancyScrollbar _scrollbar;

    public const int width = 480;
    public const int height = 155;

    public int RelativeLeft => Main.screenWidth / 2 - width / 2 + 280;
    public int RelativeTop => Main.screenHeight / 2 - height / 2 - 196;

    public CollectionItemRecipesUI() : base()
    {
        //Set to air
        Material = new Item();
        Material.SetDefaults(ItemID.None);
        Glow = 1f;
    }

    public Item Material { get; set; }
    public float Glow { get; set; }
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 48 * 6f;
        Height.Pixels = 48 * 9;
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

        _slotGrid = new UIGrid();
        _slotGrid.Width.Set(0, 1f);
        _slotGrid.Height.Set(0, 1f);
        _slotGrid.ListPadding = 2f;
        _panel.Append(_slotGrid);

        _scrollbar = new FancyScrollbar();
        _scrollbar.Width.Set(20, 0);
        _scrollbar.Height.Set(340, 0);
        _scrollbar.Left.Set(0, 0.93f);
        _scrollbar.Top.Set(0, 0.05f);

        float maxViewSize = 48 * 8f;
        _scrollbar.SetView(0, maxViewSize);
        Append(_scrollbar);


        _uiList = new UIList();
        _uiList.Width.Pixels = Width.Pixels;
        _uiList.Height.Pixels = Height.Pixels;
        _uiList.Add(_panel);
        _uiList.SetScrollbar(_scrollbar);
        Append(_uiList);
    }

    public override void Recalculate()
    {
        //Recalculate the UI when there is some sort of update
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
        _slotGrid?.Clear();
        if (Main.gameMenu)
            return;

        //We just need to get the number of unique materials since that's how we're sorting things

        var cauldron = ModContent.GetInstance<Cauldron>();
        Item[] crafts = cauldron.GetCraftsFromMaterial(Material.type);
        for (int i = 0; i < crafts.Length; i++)
        {
            Item craft = crafts[i];
            CollectionItemTabCraft slot = new CollectionItemTabCraft();
            slot.Item = craft;
            _slotGrid.Add(slot);
        }

        _slotGrid.Recalculate();
        base.Recalculate();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        //Constantly lock the UI in the position regardless of resolution changes
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
        Glow *= 0.95f;

        _panel.Height.Pixels = _slotGrid.GetTotalHeight() + 32;
        float progress = _panel.Height.Pixels / Height.Pixels;
        progress = MathHelper.Clamp(progress, 0f, 1f);
        _scrollbar.Height.Set(Height.Pixels * progress, 0);

        //Hacky way to get invisible scrollbar when there's no need for it
        if (_panel.Height.Pixels < Height.Pixels)
        {
            _scrollbar.Top.Set(500000, 0f);
        }
        else
        {
            _scrollbar.Top.Set(0.05f, 0f);
        }
    }
}
public class CollectionItemTabUI : UIPanel
{
    private UIList _uiList;
    private UIPanel _panel;
    private UIGrid _slotGrid;
    private FancyScrollbar _scrollbar;

    public const int width = 480;
    public const int height = 155;

    public int RelativeLeft => Main.screenWidth / 2 - width / 2 - 128;
    public int RelativeTop => Main.screenHeight / 2 - height / 2 - 196;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 48 * 8;
        Height.Pixels = 48 * 9;
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

        _slotGrid = new UIGrid();
        _slotGrid.Width.Set(0, 1f);
        _slotGrid.Height.Set(0, 1f);
        _slotGrid.ListPadding = 2f;

        _panel.Append(_slotGrid);

        _scrollbar = new FancyScrollbar();
        _scrollbar.Width.Set(20, 0);
        _scrollbar.Height.Set(340, 0);
        _scrollbar.Left.Set(0, 0.94f);
        _scrollbar.Top.Set(0, 0.05f);

        float maxViewSize = 48 * 8f;
        _scrollbar.SetView(0, maxViewSize);
        Append(_scrollbar);


        _uiList = new UIList();
        _uiList.Width.Pixels = Width.Pixels;
        _uiList.Height.Pixels = Height.Pixels;
        _uiList.Add(_panel);
        _uiList.SetScrollbar(_scrollbar);
        Append(_uiList);
    }

    public override void OnActivate()
    {
        base.OnActivate();
        _slotGrid.Clear();
        //Armors need to show before the brewing materials
        ArmorSet[] armorSets = ArmorSetSystem.GetArmorSets();
        //When the ui activates on the load screen, there's no armors
        //Maybe we shouldn't activate UIs there??
        if (armorSets.Length <= 0)
            return;
        if (Main.gameMenu)
            return;

        CollectionItemTabSlot slot2 = new CollectionItemTabSlot();
        slot2.armorSet = armorSets[0];
        slot2.slotType = 1;
        _slotGrid.Add(slot2);

        var cauldron = ModContent.GetInstance<Cauldron>();
        Item[] materialsYouCanCraftWith = cauldron.GetMaterials();
        for (int i = 0; i < materialsYouCanCraftWith.Length; i++)
        {
            Item mat = materialsYouCanCraftWith[i];
            CollectionItemTabSlot slot = new CollectionItemTabSlot();
            slot.item = mat;
            _slotGrid.Add(slot);
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        //Constantly lock the UI in the position regardless of resolution changes
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;

        _panel.Height.Pixels = _slotGrid.GetTotalHeight() + 32;
        float progress = _panel.Height.Pixels / Height.Pixels;
        progress = MathHelper.Clamp(progress, 0f, 1f);
        _scrollbar.Height.Set(Height.Pixels * progress, 0);

        //Hacky way to get invisible scrollbar when there's no need for it
        if (_panel.Height.Pixels < Height.Pixels)
        {
            _scrollbar.Top.Set(500000, 0f);
        }
        else
        {
            _scrollbar.Top.Set(0, 0f);
        }

    }
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {

        base.DrawSelf(spriteBatch);

    }
}



public class CollectionArmorInfoUI : UIPanel
{
    private UIList _uiList;
    private UIPanel _panel;
    private UIGrid _slotGrid;
    private FancyScrollbar _scrollbar;

    public const int width = 480;
    public const int height = 155;

    public int RelativeLeft => Main.screenWidth / 2 - width / 2 + 280;
    public int RelativeTop => Main.screenHeight / 2 - height / 2 - 196;

    public CollectionArmorInfoUI() : base()
    {
        //Set to air
        Material = new Item();
        Material.SetDefaults(ItemID.None);
    }

    public Item Material { get; set; }
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 48 * 8;
        Height.Pixels = 48 * 9;
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

        _slotGrid = new UIGrid();
        _slotGrid.Width.Set(0, 1f);
        _slotGrid.Height.Set(0, 1f);
        _slotGrid.ListPadding = 2f;
        _panel.Append(_slotGrid);

        _scrollbar = new FancyScrollbar();
        _scrollbar.Width.Set(20, 0);
        _scrollbar.Height.Set(340, 0);
        _scrollbar.Left.Set(0, 0.86f);
        _scrollbar.Top.Set(0, 0.05f);

        float maxViewSize = 48 * 8f;
        _scrollbar.SetView(0, maxViewSize);
        Append(_scrollbar);


        _uiList = new UIList();
        _uiList.Width.Pixels = Width.Pixels;
        _uiList.Height.Pixels = Height.Pixels;
        _uiList.Add(_panel);
        _uiList.SetScrollbar(_scrollbar);
        Append(_uiList);
    }

    public override void OnActivate()
    {
        base.OnActivate();
        _slotGrid?.Clear();
        if (Main.gameMenu)
            return;

        //We just need to get the number of unique materials since that's how we're sorting things
        ArmorSet[] armorSets = ArmorSetSystem.GetArmorSets();
        foreach (ArmorSet armorSet in armorSets)
        {
            CollectionItemTabCraft slot = new CollectionItemTabCraft();
            slot.armorSet = armorSet;
            slot.slotType = 1;
            slot.Width.Pixels = 48;
            _slotGrid.Add(slot);
        }

        _slotGrid.Recalculate();
    }


    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        //Constantly lock the UI in the position regardless of resolution changes
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
        Width.Pixels = 48 * 8;
        _slotGrid.ListPadding = 16;
        _slotGrid.PaddingLeft = 16;
        _slotGrid.PaddingRight = 52;
        _panel.Height.Pixels = _slotGrid.GetTotalHeight() + 32;
        float progress = _panel.Height.Pixels / Height.Pixels;
        progress = MathHelper.Clamp(progress, 0f, 1f);
        _scrollbar.Height.Set(Height.Pixels * progress, 0);

        //Hacky way to get invisible scrollbar when there's no need for it
        if (_panel.Height.Pixels < Height.Pixels)
        {
            _scrollbar.Top.Set(500000, 0f);
        }
        else
        {
            _scrollbar.Top.Set(0.05f, 0f);
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials.Molds;
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
    private readonly float _scale;
    private readonly int _context;
    public CollectionItemTabCraft(int context = ItemSlot.Context.InventoryItem, float scale = 1f)
    {
        _scale = scale;
        _context = context;
        Item = new Item();
        Item.SetDefaults(ItemID.None);

        var value = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}CollectionTabSlot",
            ReLogic.Content.AssetRequestMode.ImmediateLoad);
        Width.Set(value.Width() * scale, 0f);
        Height.Set(value.Height() * scale, 0f);
    }

    public float Glow { get; set; }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        float oldScale = Main.inventoryScale;
        Main.inventoryScale = _scale;
        Rectangle rectangle = GetDimensions().ToRectangle();

        bool contains = ContainsPoint(Main.MouseScreen);
        if (contains && !PlayerInput.IgnoreMouseInterface)
        {
            Main.LocalPlayer.mouseInterface = true;
        }

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
                Main.hoverItemName = Item.Name;
                Main.HoverItem = Item;
            }
        }

        ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos, _scale, 32, drawColor);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, default, default, default, default, Main.UIScaleMatrix);

        for (int i = 0; i < 8f; i++)
        {
            Color glowColor = Color.White * Glow;
            float progress = (float)i / 8f;
            float rot = progress * MathHelper.TwoPi;
            Vector2 offset = rot.ToRotationVector2() * 8 * Glow;
            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos + offset, _scale, 32, glowColor);
        }

        spriteBatch.End();
        spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
        Main.inventoryScale = oldScale;
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
    private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
    {
        CollectionBookUISystem uiSystem = ModContent.GetInstance<CollectionBookUISystem>();
        uiSystem.OpenRecipesInfoUI(item);
    }

    private void OnMouseHover(UIMouseEvent evt, UIElement listeningElement)
    {

    }
    public override int CompareTo(object obj)
    {
        if (obj is CollectionItemTabSlot other)
        {
            Item itemA = item;
            Item itemB = other.item;
            return Cauldron.MaterialOrder[itemA.type].CompareTo(Cauldron.MaterialOrder[itemB.type]);
        }

        return base.CompareTo(obj);
    }
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        Rectangle rectangle = GetDimensions().ToRectangle();

        bool contains = ContainsPoint(Main.MouseScreen);
        if (contains && !PlayerInput.IgnoreMouseInterface)
        {
            Main.LocalPlayer.mouseInterface = true;
        }

        Vector2 targetHoverScale = contains ? new Vector2(1.2f) : Vector2.One;
        _hoverScale = Vector2.Lerp(_hoverScale, targetHoverScale, 0.25f);

        //Draw Backing
        Vector2 pos = rectangle.TopLeft();
        Texture2D value = ModContent.Request<Texture2D>($"{CollectionBookUISystem.RootTexturePath}CollectionTabSlot").Value;
        Vector2 centerPos = rectangle.TopLeft() + new Vector2(18, rectangle.Height * 0.5f);
        spriteBatch.Draw(value, rectangle.TopLeft(), null, Color.White, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);

        if (contains)
        {
            var whiteShader = SpriteWhiteShader.Instance;
            float outlineOffset = 2;
            RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
            SamplerState anisotropicClamp = SamplerState.PointClamp;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, Main.Rasterizer, whiteShader.Effect, Main.UIScaleMatrix);
   
            for(float f = 0; f < MathHelper.TwoPi; f += MathHelper.PiOver2)
            {
                Vector2 offset = f.ToRotationVector2() * outlineOffset;
                ItemSlot.DrawItemIcon(item, ItemSlot.Context.InventoryItem, spriteBatch, centerPos + offset, _hoverScale.X, 32, Color.White);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, default, Main.UIScaleMatrix);
        }

        ItemSlot.DrawItemIcon(item, ItemSlot.Context.InventoryItem, spriteBatch, centerPos, _hoverScale.X, 32, Color.White);


        //Drawing the number of things you have unlocked
        {
            var cauldronPlayer = ModContent.GetInstance<CauldronPlayer>();
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
        if (contains)
        {
            Main.hoverItemName = item.Name;
            Main.HoverItem = item;
        }
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
            slot.Glow = Glow;
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
}

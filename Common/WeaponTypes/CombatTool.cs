using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.MagicSystem.UI;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Helpers;
using Stellamod.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Common.WeaponTypes
{
    #region Game Mechanics
    public class CombatToolPlayer : ModPlayer
    {
        private Item _selectedToolBackingField;
        public Item SelectedTool
        {
            get
            {
                if (_selectedToolBackingField == null)
                {
                    _selectedToolBackingField = new Item();
                    _selectedToolBackingField.SetDefaults(0);
                }
                return _selectedToolBackingField;
            }
            set
            {
                _selectedToolBackingField = value;
            }
        }
        private List<Item> _unlockedToolsBackingField;
        public List<Item> UnlockedTools
        {
            get
            {
                if (_unlockedToolsBackingField == null)
                {
                    _unlockedToolsBackingField = new List<Item>();
                }

                return _unlockedToolsBackingField;
            }
            set
            {
                _unlockedToolsBackingField = value;
            }
        }
        public void Unlock(Item item)
        {
            UnlockedTools.Add(item);
        }

        public bool HasUnlocked(Item item)
        {
            return UnlockedTools.Find(x => x.type == item.type) != null;
        }

        public override bool PreItemCheck()
        {
            if (Main.myPlayer == Player.whoAmI)
            {
                if (LunarVeilKeybinds.ToolKeybind.JustReleased)
                {
                    CombatTool combatTool = SelectedTool.GetGlobalItem<CombatTool>();
                    combatTool.ammoCount = 1;
                    if (combatTool.isCombatTool)
                    {
                        if (combatTool.ammoCount > 0)
                        {
                            combatTool.ammoCount--;
                            ItemLoader.Shoot(SelectedTool, Player, new EntitySource_ItemUse_WithAmmo(Player, SelectedTool, -1),
                                Player.Center, (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.Zero) * SelectedTool.shootSpeed, SelectedTool.shoot, Player.GetWeaponDamage(SelectedTool), Player.GetWeaponKnockback(SelectedTool));
                        }
                    }
                }
            }
            return base.PreItemCheck();
        }
        public override void UpdateDead()
        {
            base.UpdateDead();
            CombatTool combatTool = SelectedTool.GetGlobalItem<CombatTool>();
            combatTool.ammoCount = combatTool.maxAmmoCount;
        }
        public override void PostItemCheck()
        {
            base.PostItemCheck();

        }
        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["tool"] = SelectedTool;
            tag["unlocked"] = UnlockedTools;
        }
        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            SelectedTool = tag.Get<Item>("tool");
            UnlockedTools = tag.Get<List<Item>>("unlocked");
        }
    }
    public class CombatToolProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public float bossDamagePercent;
        public float enemyDamagePercent;
        public override void SetDefaults(Projectile entity)
        {
            base.SetDefaults(entity);
            bossDamagePercent = 0;
            enemyDamagePercent = 0;
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(projectile, bitWriter, binaryWriter);
            binaryWriter.Write(bossDamagePercent);
            binaryWriter.Write(enemyDamagePercent);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(projectile, bitReader, binaryReader);
            bossDamagePercent = binaryReader.ReadSingle();
            enemyDamagePercent = binaryReader.ReadSingle();
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(projectile, target, ref modifiers);
            if (target.boss)
            {
                float bonusDamage = target.lifeMax * bossDamagePercent;
                modifiers.FlatBonusDamage += bonusDamage;
            }
            else
            {
                float bonusDamage = target.lifeMax * enemyDamagePercent;
                modifiers.FlatBonusDamage += bonusDamage;
            }
        }
    }

    public class CombatTool : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public bool isCombatTool;
        public float bossDamagePercent;
        public float enemyDamagePercent;
        public int ammoCount;
        public int maxAmmoCount;
       
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            if (isCombatTool)
            {
                TooltipLine line = new TooltipLine(Mod, "CombatToolWeaponType", LangText.Common("CombatTool"));
                line.OverrideColor = Color.LightGoldenrodYellow;
                tooltips.Add(line);

                line = new TooltipLine(Mod, "CarryingCapacity", LangText.Common("CombatToolCount", maxAmmoCount));
                line.OverrideColor = Color.White;

                tooltips.Add(line);
                string esp = string.Format("{0:P2}", enemyDamagePercent);
                line = new TooltipLine(Mod, "EnemyDamagePercent", LangText.Common("EnemyDamagePercent", esp));
                line.OverrideColor = Color.IndianRed;
                tooltips.Add(line);

                string bsp = string.Format("{0:P2}", bossDamagePercent);
                line = new TooltipLine(Mod, "BossDamagePercent", LangText.Common("BossDamagePercent", bsp));
                line.OverrideColor = Color.IndianRed;
                tooltips.Add(line);
            }
        }
        public override bool OnPickup(Item item, Player player)
        {
            if (isCombatTool)
            {
                CombatToolPlayer toolPlayer = player.GetModPlayer<CombatToolPlayer>();
                toolPlayer.Unlock(item);
                PopupText.NewText(PopupTextContext.SonarAlert, item, 1, longText: true);
                return false;
            }
            else
            {
                return base.OnPickup(item, player);
            }

        }
        public override void UpdateInventory(Item item, Player player)
        {
            base.UpdateInventory(item, player);
            if (isCombatTool)
            {
                CombatToolPlayer toolPlayer = player.GetModPlayer<CombatToolPlayer>();
                toolPlayer.Unlock(item);
             //   PopupText.NewText(PopupTextContext.SonarAlert, item, 1, longText: true);
                for (int i = 0; i < player.inventory.Length; i++)
                {
                    Item inv = player.inventory[i];
                    if (item == inv)
                    {
                        player.inventory[i] = new Item();
                        player.inventory[i].SetDefaults(0);
                    }
                }
            }
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (isCombatTool)
            {

                Projectile p = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
                
                CombatToolProjectile combatToolProjectile = p.GetGlobalProjectile<CombatToolProjectile>();
                combatToolProjectile.bossDamagePercent = bossDamagePercent;
                combatToolProjectile.enemyDamagePercent = enemyDamagePercent;
                return false;
            }
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }
    }
    #endregion

    #region UI
    public class CombatToolMeunUIState : UIState
    {
        public CombatToolBrowserWindow xixianFlaskUI;
        public CombatToolMeunUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            xixianFlaskUI = new CombatToolBrowserWindow();
            Append(xixianFlaskUI);
        }
    }

    public class CombatToolUIState : UIState
    {
        public CombatToolSlotPanel panel;
        public CombatToolUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            panel = new();
            Append(panel);
        }
    }
    public class CombatToolSlotPanel : UIPanel
    {
        private UIPanel _panel;
        public CombatToolSlot slot;


        public const int width = 432;
        public const int height = 280;

        public int RelativeLeft
        {
            get
            {
                if (!Main.playerInventory)
                {
                    return 412 + 64 + 64;
                }
                return 555 + 64 + 64;
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

    /// <summary>
    /// Creates a menu of all the items in the mod
    /// </summary>
    public class CombatToolBrowserMenu : UIPanel
    {
        private InventoryBackground _inventoryBackground;
        private UIGrid _grid;
        private UIPanel _panel;
        private UIScrollbar _scrollbar;
        private UIList _uiList;
        private CombatToolBrowserView _view;
        private XButton _xButton;
        public CombatToolBrowserMenu(UIScrollbar scrollbar)
        {
            _xButton = new XButton(Close);
            _inventoryBackground = new InventoryBackground();
            _panel = new UIPanel();
            _grid = new UIGrid();
            _scrollbar = scrollbar;
            _uiList = new UIList();
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 428;
            Height.Pixels = 236;
            Append(_inventoryBackground);

            _panel.Width.Pixels = Width.Pixels;
            _panel.Height.Pixels = Height.Pixels;
            _panel.BackgroundColor = Color.Transparent;
            _panel.BorderColor = Color.Transparent;
            Append(_panel);

            _grid.Left.Pixels = 10;
            _grid.Width.Set(0, 1f);
            _grid.Height.Set(0, 1f);
            _grid.HAlign = 0.5f;
            _grid.VAlign = 0.5f;
            _grid.ListPadding = 2;
            _panel.Append(_grid);



            _uiList.Width.Pixels = Width.Pixels;
            _uiList.Height.Pixels = Height.Pixels;
            _uiList.Add(_panel);
            _uiList.SetScrollbar(_scrollbar);
            Append(_uiList);
          //  Append(_xButton);

        }
        private string _lastSearchFilter;
        public void SetSearchFilter(string searchFilter)
        {
            //Set the text filter for items
            if (_lastSearchFilter == searchFilter)
                return;
            _lastSearchFilter = searchFilter;
            Refresh();
        }

        private void Refresh()
        {
            if (Main.gameMenu)
                return;


            _grid.Clear();

            var items = ModContent.GetContent<ModItem>();
            List<Item> itemList = new List<Item>();
            foreach (var item in items)
            {
                if (item.Item.GetGlobalItem<CombatTool>().isCombatTool)
                    itemList.Add(item.Item);
            }
           
            _view = new(itemList.ToArray());
            _view.SearchFilter = _lastSearchFilter;
            _view.Width.Pixels = Width.Pixels;
            _view.Height.Pixels = Height.Pixels;
            _view.Activate();
            _grid.Add(_view);

            _grid.Recalculate();
            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
  
            if (_view == null)
                Refresh();
            _xButton.Left.Pixels = 0;
            _xButton.Top.Pixels = 0;
            _panel.Height.Pixels = _view.Height.Pixels + 32;
            float progress = _panel.Height.Pixels / Height.Pixels;
            progress = MathHelper.Clamp(progress, 0f, 1f);

            _inventoryBackground.drawColor = Color.Lerp(Color.White, Color.Black, 0.8f);
            _scrollbar.Height.Set(Height.Pixels * progress, 0);
            float scrollRatio = _scrollbar.ViewPosition;

            if (_view != null)
            {
                _view.ViewPosition = scrollRatio;
            }


            //Hacky way to get invisible scrollbar when there's no need for it
            if (_panel.Height.Pixels < Height.Pixels)
            {
                _scrollbar.Top.Set(500000, 0f);
            }
            else
            {
                _scrollbar.Top.Set(0, 0.2f);
            }
            _scrollbar.Left.Set(0, 0.83f);

            _grid.ListPadding = 16;
        }

        private void Close()
        {
            CombatToolUISysten uiSystem = ModContent.GetInstance<CombatToolUISysten>();
            uiSystem.CloseUI();
        }
    }

    /// <summary>
    /// The full window of the item browser
    /// </summary>
    public class CombatToolBrowserWindow : UIPanel
    {
        private UIScrollbar _scrollbar;
        private XButton _xButton;
        private CombatToolBrowserMenu _inventoryMenu;
        private UIInputTextField _textBox;

        public CombatToolBrowserWindow() : base()
        {
            _scrollbar = new FancyScrollbar();
            _xButton = new XButton(Close);
            _inventoryMenu = new CombatToolBrowserMenu(_scrollbar);
            _textBox = new UIInputTextField("Search...");
        }

        public string SearchFilter => _textBox.Text;

        public int RelativeLeft => ScreenHelper.TrueScreenWidth / 2 - (int)Width.Pixels / 2;
        public int RelativeTop => ScreenHelper.TrueScreenHeight / 2 - (int)Height.Pixels / 2;

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 704;
            Height.Pixels = 704;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;


            _inventoryMenu.HAlign = 0.5f;
            _inventoryMenu.VAlign = 0.5f;
            Append(_inventoryMenu);
            Append(_xButton);

            //Scrollbar
            _scrollbar.Width.Set(20, 0);
            _scrollbar.Height.Set(340, 0);
            _scrollbar.Left.Set(0, 0.95f);
            _scrollbar.Top.Set(0, 0f);

            float maxViewSize = 48 * 8f;
            _scrollbar.SetView(0, maxViewSize);
            Append(_scrollbar);

            _textBox.HAlign = 0.5f;
            _textBox.VAlign = 0.1f;
            _textBox.Width.Pixels = 128;
            Append(_textBox);
            SetPos();
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
            if (!Main.gameMenu)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
        }

        private void SetPos()
        {
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;

            _inventoryMenu.HAlign = 0.5f;
            _inventoryMenu.VAlign = 0.25f;
            _xButton.Top.Pixels = 64;
            _xButton.Left.Pixels = 164;

        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //Constantly lock the UI in the position regardless of resolution changes
            _inventoryMenu.SetSearchFilter(SearchFilter);
            SetPos();
        }

        private void Close()
        {
            CombatToolUISysten uiSystem = ModContent.GetInstance<CombatToolUISysten>();
            uiSystem.CloseUI();
        }
    }

    [Autoload(Side = ModSide.Client)]
    public class CombatToolUISysten : BaseUISystem
    {
        private GameTime _lastUpdateUiGameTime;
        private UserInterface _userInterface;
        private UserInterface _hudUserInterface;
        public static string RootTexturePath => typeof(CombatToolUISysten).DirectoryHere() + "/";

        public CombatToolMeunUIState menuUIState;
        public CombatToolUIState slotUIState;
        public override int uiSlot => Slot_MajorUI;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            _hudUserInterface = new UserInterface();
            menuUIState = new();
            menuUIState.Activate();
            slotUIState = new();
            slotUIState.Activate();

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
            if (_userInterface?.CurrentState != null)
            {
                _userInterface.Update(gameTime);
            }
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

        public void ToggleUI()
        {
            if (_userInterface.CurrentState != null)
            {
                SoundStyle soundStyle = SoundID.MenuClose;
                SoundEngine.PlaySound(soundStyle);
                CloseUI();
            }
            else
            {
                SoundStyle soundStyle = SoundID.MenuOpen;
                SoundEngine.PlaySound(soundStyle);
                OpenUI();
            }
        }
        public void OpenHudUI()
        {
            _hudUserInterface.SetState(slotUIState);
        }

        public void CloseHudUI()
        {
            _hudUserInterface.SetState(null);
        }
        public void OpenUI()
        {
            //Set State
            TakeSlot();
            _userInterface.SetState(menuUIState);
        }

        public void CloseUI()
        {
            ClearSlot();
            _userInterface.SetState(null);
        }

        public override void PreSaveAndQuit()
        {
            //Calls Deactivate and drops the item
            if (_userInterface.CurrentState != null)
            {
                CloseUI();
                _userInterface.SetState(null);
            }
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
                    "Stellamod: Combat Tool UI",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && _userInterface?.CurrentState != null)
                        {
                            _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);

                        }
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
                $"{this.GetType().DirectoryHere()}/CombatToolSlot", AssetRequestMode.ImmediateLoad);
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
            CombatToolUISysten uiSystem = ModContent.GetInstance<CombatToolUISysten>();
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
            Item item = Main.LocalPlayer.GetModPlayer<CombatToolPlayer>().SelectedTool;
            int ammoCount = item.GetGlobalItem<CombatTool>().ammoCount;
            _countText.SetText($"x{ammoCount}");
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
            Color itemColor = Color.White;
            if(ammoCount <= 0)
            {
                color2 = Color.Lerp(color2, Color.Black, 0.75f);
                itemColor = Color.Lerp(itemColor, Color.Black, 0.75f);
            }
            Vector2 pos = rectangle.TopLeft();

            Texture2D backingTexture = _slotTextureAsset.Value;
            int offset = (int)(backingTexture.Size().Y / 2);
            Vector2 centerPos = pos + rectangle.Size() / 2f;
            spriteBatch.Draw(backingTexture, rectangle.TopLeft(), null, color2, 0f, default, _scale, SpriteEffects.None, 0f);

            ItemSlot.DrawItemIcon(item, _context, spriteBatch, centerPos, _scale, 32, itemColor);
            Main.inventoryScale = oldScale;
        }
    }


    /// <summary>
    /// Setups a view that lets you look over a massive grid of items
    /// </summary>
    public class CombatToolBrowserView : UIPanel
    {
        private float _scale;
        private int _context;
        private string _oldSearchFilter;
        //Basically, instead of ceratgin 6800 slots or whatever
        //We have a single view that takes an array of items
        //Uses that to calculate draw offsets for each item and draws them
        public CombatToolBrowserView(Item[] items)
        {
            _scale = 1f;
            _context = ItemSlot.Context.BankItem;
            ElementsPerRow = 9;

            //Set up the items we're going to iterate over
            Items = items;
            HoveringItem = new Item();
            HoveringItem.SetDefaults(0);

            //Setup mouse interactions
            OnLeftClick += SpawnItem;

            //Setup drawing
            string texturePath = this.GetType().DirectoryHere() + "/ItemBrowserSlot";
            SlotTextureAsset = ModContent.Request<Texture2D>(texturePath, AssetRequestMode.AsyncLoad);
            Width.Set(32, 0f);
            Height.Set(32, 0f);
        }

        public Item[] Items;
        public Item[] SearchFilterItems;
        public Item HoveringItem;
        public Asset<Texture2D> SlotTextureAsset;
        public string SearchFilter;
        public float ViewPosition;
        public int ElementsPerRow;

        private void SpawnItem(UIMouseEvent evt, UIElement listeningElement)
        {
            CombatToolPlayer combatToolPlayer = Main.LocalPlayer.GetModPlayer<CombatToolPlayer>();
            if (!combatToolPlayer.HasUnlocked(HoveringItem))
                return;

            CombatTool combatTool = combatToolPlayer.SelectedTool.GetGlobalItem<CombatTool>();
            float ammoCapacity = (float)combatTool.ammoCount / (float)combatTool.maxAmmoCount;
            combatToolPlayer.SelectedTool = HoveringItem;
            combatToolPlayer.SelectedTool.GetGlobalItem<CombatTool>().ammoCount = (int)(ammoCapacity * combatToolPlayer.SelectedTool.GetGlobalItem<CombatTool>().maxAmmoCount);
        }

        private bool NeedsUpdateCollection()
        {
            return _oldSearchFilter != SearchFilter;
        }

        private void UpdateCollection()
        {
            IEnumerable<Item> collection = Items;
            string filter = string.Empty;
            if (!string.IsNullOrEmpty(SearchFilter))
            {
                filter = SearchFilter.TrimStart().ToLower();
                collection = collection.Where(x => x.Name.ToLower().Contains(filter));
            }


            SearchFilterItems = collection.ToArray();
            _oldSearchFilter = SearchFilter;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;
            Rectangle rectangle = GetDimensions().ToRectangle();
            if (IsMouseHovering && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (IsMouseHovering)
            {
                Main.HoverItem = HoveringItem;
                Main.hoverItemName = HoveringItem.HoverName;
            }

            Vector2 topLeft = rectangle.TopLeft();
            float availableWidth = GetInnerDimensions().Width;
            float listPadding = 10;
            Rectangle outerDimensions = new Rectangle(0, 0, 32, 32);
            Point mousePoint = Main.MouseScreen.ToPoint();
            string filter = string.Empty;
            if (!string.IsNullOrEmpty(SearchFilter))
                filter = SearchFilter.TrimStart().ToLower();
            bool useFilter = !string.IsNullOrEmpty(filter);

            //We're basically just reusing the grid code here lol
            //There's currently 9 items per row
            //To optimize this, we can calculate the placement of an element with some simple math based on its index
            //Instead of using left and top variables
            //So let's do that


            //We only want to draw the items that are actually in view
            //So we should calculate a starting inde


            //Define our width variables

            if (NeedsUpdateCollection())
            {
                UpdateCollection();
            }
            Item[] itemArr = SearchFilterItems;
            int elementsPerRow = ElementsPerRow;
            float elementWidth = outerDimensions.Width;
            float viewWidth = availableWidth;
            float elementHeight = outerDimensions.Height;

            //Calculate the maximum height of the grid
            int itemRows = (itemArr.Length / elementsPerRow);
            float maximumHeight = itemRows * (elementHeight + listPadding);
            Height.Pixels = maximumHeight + 32;


            Texture2D slotTexture = SlotTextureAsset.Value;
            Color drawColor = Color.Lerp(Color.White, Color.Black, 0.75f);
            float drawScale = 1.2f;
            Vector2 drawOrigin = slotTexture.Size() / 2;

            //The view position is the y offset of the scrollbar
            //So to figure out where to start from
            //We just divide the offset by 
            //Caculate a starting and ending index for which items to draw
            int numRowsDownward = (int)(ViewPosition / (elementHeight + listPadding));
            int startIndex = numRowsDownward * elementsPerRow;
            int endIndex = startIndex + elementsPerRow * 6;


            //Now we're only loading the items that are in view! Yippee! Optimization!
            for (int i = startIndex; i < endIndex && i < itemArr.Length; i++)
            {
                Item item = itemArr[i];

                //Remmeber 9 elements per row
                //We can use the modulus operator to get this to keep looping, since all elements are the same size
                float leftOffset = i % elementsPerRow * (elementWidth + listPadding);
                float topOffset = i / elementsPerRow * (elementHeight + listPadding);

                //Enchantment Card
                Vector2 tl = topLeft;
                tl.X += leftOffset;
                tl.Y += topOffset;
                Vector2 centerPos = tl + new Vector2(16);

                Vector2 iconCenterPos = tl + slotTexture.Size() / 2;

                bool isUnlocked = Main.LocalPlayer.GetModPlayer<CombatToolPlayer>().HasUnlocked(item);
              

                spriteBatch.Draw(slotTexture, iconCenterPos, null, drawColor, 0f, drawOrigin, _scale, SpriteEffects.None, 0f);

                Color iconColor = Color.White;
                if (!isUnlocked)
                    iconColor = Color.Lerp(iconColor, Color.Black, 0.8f);
                ItemSlot.DrawItemIcon(item, _context, spriteBatch, centerPos, drawScale, 32, iconColor);
                if (HoveringItem.stack > 1)
                {
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, item.stack.ToString(),
                        centerPos + new Vector2(0, 2) * _scale, Color.White, 0f, Vector2.Zero, new Vector2(_scale), -1f, _scale);
                }

                //Check if hovering for tooltip
                Rectangle hoverRectangle = new Rectangle((int)tl.X, (int)tl.Y, 32, 32);
                if (hoverRectangle.Contains(mousePoint))
                {
                    HoveringItem = item;
                    Main.HoverItem = item;
                    Main.hoverItemName = item.HoverName;
                }
            }

            Main.inventoryScale = oldScale;
        }
    }
    #endregion
}

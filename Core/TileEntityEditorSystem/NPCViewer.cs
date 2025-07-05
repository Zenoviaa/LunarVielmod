using Microsoft.Xna.Framework;
using Stellamod.Core.StructureSelector;
using Stellamod.Core.UI;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Stellamod.Core.TileEntityEditorSystem
{
    internal class NPCViewerSlot : UIPanel
    {
        private readonly ModNPC _npc;
        private readonly Action<ModNPC> _action;
        public NPCViewerSlot(ModNPC npc, Action<ModNPC> action)
        {
            _npc = npc;
            _action = action;
            Width.Pixels = 256;
            Height.Pixels = 48;
            BackgroundColor = Color.Blue * 0.5f;
            BorderColor = Color.Transparent;
            Text = new UIText(npc.Name);
            Text.HAlign = 0.5f;
            OnLeftClick += OnButtonClick;
        }

        public UIText Text { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Append(Text);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            if (IsMouseHovering)
            {
                BackgroundColor = Color.Lerp(BackgroundColor, Color.Yellow * 0.5f, 0.1f);
            }
            else
            {
                BackgroundColor = Color.Lerp(BackgroundColor, Color.Blue * 0.5f, 0.1f);
            }
        }

        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            _action(_npc);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }

    internal class NPCViewerUIState : UIState
    {
        private readonly Action<ModNPC> _onSelect;
        public NPCViewer ui;
        public NPCViewerUIState(Action<ModNPC> onSelect) : base()
        {
            _onSelect = onSelect;
        }

        public override void OnInitialize()
        {
            ui = new NPCViewer(_onSelect);
            Append(ui);
        }
    }

    internal class NPCViewer : UIPanel
    {
        private readonly Action<ModNPC> _onSelect;
        private StructureBackground _background;
        private UIList _uiList;
        private UIPanel _panel;
        private UIGrid _grid;
        private UIScrollbar _scrollbar;
        private static ModNPC[] _allNpcs;
        public static ModNPC[] AllNPCs
        {
            get
            {
                if (_allNpcs == null)
                    _allNpcs = Stellamod.Instance.GetContent<ModNPC>().ToArray();
                return _allNpcs;
            }
        }

        public NPCViewer(Action<ModNPC> onSelect)
        {
            _onSelect = onSelect;
            _background = new StructureBackground();
            _panel = new UIPanel();
            _grid = new UIGrid();
            _scrollbar = new FancyScrollbar();
            _uiList = new UIList();
        }

        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => Main.screenWidth / 2 - (int)Width.Pixels / 2;
        internal int RelativeTop => Main.screenHeight / 2 - (int)Height.Pixels / 2;

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 350;
            Height.Pixels = 210;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            Append(_background);

            _panel.Width.Pixels = Width.Pixels;
            _panel.Height.Pixels = Height.Pixels;
            _panel.BackgroundColor = Color.Transparent;
            _panel.BorderColor = Color.Transparent;
            Append(_panel);

            _grid.Width.Set(0, 1f);
            _grid.Height.Set(0, 1f);
            _grid.HAlign = 0.5f;
            _grid.ListPadding = 2f;
            _panel.Append(_grid);

            _scrollbar.Width.Set(20, 0);
            _scrollbar.Height.Set(340, 0);
            _scrollbar.Left.Set(0, 0.93f);
            _scrollbar.Top.Set(0, 0f);

            float maxViewSize = 48 * 8f;
            _scrollbar.SetView(0, maxViewSize);
            Append(_scrollbar);

            _uiList.Width.Pixels = Width.Pixels;
            _uiList.Height.Pixels = Height.Pixels;
            _uiList.Add(_panel);
            _uiList.SetScrollbar(_scrollbar);
            Append(_uiList);

            _grid.Clear();
            foreach (var npc in AllNPCs)
            {
                NPCViewerSlot slot = new NPCViewerSlot(npc, _onSelect);
                _grid.Add(slot);
            }
        }

        public ModNPC SelectedModNpc { get; set; }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            _panel.Height.Pixels = _grid.GetTotalHeight() + 32;

            _panel.BackgroundColor = Color.Transparent;
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
}

﻿using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Stellamod.Core.Helpers;
using Stellamod.Core.UI;

namespace Stellamod.Core.StructureSelector
{
    internal class StructureSelectorUI : UIPanel
    {
        private StructureBackground _background;
        private UIList _uiList;
        private UIPanel _panel;
        private UIGrid _grid;
        private UIScrollbar _scrollbar;

        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => Main.screenWidth / 2 - (int)Width.Pixels / 2;
        internal int RelativeTop => Main.screenHeight / 2 - (int)Height.Pixels / 2;

        public StructureSelectorUI() : base()
        {
            _background = new StructureBackground();
            _panel = new UIPanel();
            _grid = new UIGrid();
            _scrollbar = new FancyScrollbar();
            _uiList = new UIList();
        }

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
            string[] structurePaths = Structurizer.GetPaths();
            foreach (var path in structurePaths)
            {
                StructureSelectionButton btn = new StructureSelectionButton(path);
                _grid.Add(btn);
            }
        }

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

        static Mod Mod = ModContent.GetInstance<Stellamod>();
        public void Refresh()
        {
            _grid.Clear();
            string[] filePaths = Directory.GetFiles(Main.SavePath + $"/ModSources/{Mod.Name}", "*.str",
                                         SearchOption.AllDirectories);
            Uri path1 = new Uri(Main.SavePath + $"/ModSources/{Mod.Name}");
            foreach (var filePath in filePaths)
            {
                if (!filePath.Contains("Structures"))
                    continue;

                Uri path2 = new Uri(filePath);
                Uri diff = path1.MakeRelativeUri(path2);
                string finalString = diff.ToString();
                finalString = finalString.Replace(Mod.Name + "/", "");
                StructureSelectionButton btn = new StructureSelectionButton(finalString);
                _grid.Add(btn);
            }
        }
    }
}

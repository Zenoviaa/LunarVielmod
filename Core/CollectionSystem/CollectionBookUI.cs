﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.Core.CollectionSystem
{
    internal class CollectionBookUI : UIPanel
    {
        public Book book;

        internal const int width = 432;
        internal const int height = 800;

        public QuestTab questTab;
        public LoreTab loreTab;
        public CollectionTab collectionTab;
        public FragmentTab fragmentTab;
        internal int RelativeLeft => Main.screenWidth / 2 - width / 2;
        internal int RelativeTop => Main.screenHeight / 2 - height / 2 + 128;

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = width;
            Height.Pixels = height;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            book = new Book();
            Append(book);

            questTab = new QuestTab();
            questTab.Left.Pixels = 0;
            questTab.Top.Pixels = 0;
            Append(questTab);

            loreTab = new LoreTab();
            int o = 64;
            loreTab.Left.Pixels = questTab.Left.Pixels + o;
            loreTab.Top.Pixels = questTab.Top.Pixels;
            Append(loreTab);

            collectionTab = new CollectionTab();
            collectionTab.Left.Pixels = loreTab.Left.Pixels + o;
            collectionTab.Top.Pixels = loreTab.Top.Pixels;
            Append(collectionTab);

            fragmentTab = new FragmentTab();
            fragmentTab.Left.Pixels = collectionTab.Left.Pixels + o;
            fragmentTab.Top.Pixels = collectionTab.Top.Pixels;
            Append(fragmentTab);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;

            questTab.Left.Pixels = 242;
            questTab.Top.Pixels = 420;

            int o = 42;
            loreTab.Left.Pixels = questTab.Left.Pixels + o;
            loreTab.Top.Pixels = questTab.Top.Pixels;

            collectionTab.Left.Pixels = loreTab.Left.Pixels + o;
            collectionTab.Top.Pixels = loreTab.Top.Pixels;

            fragmentTab.Left.Pixels = collectionTab.Left.Pixels + o;
            fragmentTab.Top.Pixels = collectionTab.Top.Pixels;
        }
    }
}

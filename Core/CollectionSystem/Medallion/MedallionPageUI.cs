using Microsoft.Xna.Framework;
using Stellamod.Content.Items.Fragments;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace Stellamod.Core.CollectionSystem.Medallion
{
    internal class MedallionPageUI : UIPanel
    {
        internal const int width = 432;
        internal const int height = 155;

        internal int RelativeLeft => Main.screenWidth / 2 - width / 2 - 64;
        internal int RelativeTop => Main.screenHeight / 2 - height / 2 - 196;
        public MedallionPageUI() : base()
        {
            Fragments = new MedallionButton[5];
            Fragments[0] = new MedallionButton(ModContent.GetModItem(ModContent.ItemType<MakMedal>()) as MakMedal);
            Fragments[1] = new MedallionButton(ModContent.GetModItem(ModContent.ItemType<DraekusMedal>()) as DraekusMedal);
            Fragments[2] = new MedallionButton(ModContent.GetModItem(ModContent.ItemType<ApilithyMedal>()) as ApilithyMedal);
            Fragments[3] = new MedallionButton(ModContent.GetModItem(ModContent.ItemType<GothiviaMedal>()) as GothiviaMedal);
            Fragments[4] = new MedallionButton(ModContent.GetModItem(ModContent.ItemType<KariMedal>()) as KariMedal);
            BackgroundTop = new();
            BackgroundMiddle = new();

        }

        public MedallionButton[] Fragments { get; private set; }
        public BackgroundTop BackgroundTop { get; private set; }
        public BackgroundMiddle BackgroundMiddle { get; private set; }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 48 * 6f;
            Height.Pixels = 48 * 9;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            Append(BackgroundMiddle);
            Append(BackgroundTop);
            for (int i = 0; i < Fragments.Length; i++)
            {
                Append(Fragments[i]);
            }

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;

            float centerLeftX = Width.Pixels / 2;
            float centerTopX = Height.Pixels / 2;

            BackgroundTop.Top.Pixels = 8;
            BackgroundTop.Left.Pixels = -12;

            BackgroundMiddle.Top.Pixels = 96;
            BackgroundMiddle.Left.Pixels = -12;

            Fragments[0].Left.Pixels = centerLeftX - 180;
            Fragments[0].Top.Pixels = centerTopX - 150;


            Fragments[1].Left.Pixels = centerLeftX - 15;
            Fragments[1].Top.Pixels = centerTopX - 150;

            Fragments[2].Left.Pixels = centerLeftX - 5;
            Fragments[2].Top.Pixels = centerTopX - 60;

            Fragments[3].Left.Pixels = centerLeftX - 100;
            Fragments[3].Top.Pixels = centerTopX - 25;

            Fragments[4].Left.Pixels = centerLeftX - 90;
            Fragments[4].Top.Pixels = centerTopX - 120;
        }
    }
}

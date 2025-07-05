using Microsoft.Xna.Framework;
using Stellamod.Content.TriggerTiles;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.Map.UI
{
    internal class MapUI : UIPanel
    {
        private bool _initFog;
        internal int RelativeLeft => Main.screenWidth / 2 - (int)Width.Pixels / 2;
        internal int RelativeTop => Main.screenHeight / 2 - (int)Height.Pixels / 2;
        public Background Background { get; set; }
        public SpringHills SpringHills { get; set; }
        public WarriorsDoor WarriorsDoor { get; set; }
        public WitchTown WitchTown { get; set; }
        public Compass Compass { get; set; }
        public Border Border { get; set; }

        public Border2 Border2 { get; set; }
        public MapButton SpringHillsInnerButton { get; set; }
        public MapButton SpringHillsOuterButton { get; set; }
        public MapButton WarriorsDoorButton { get; set; }
        public MapButton WitchTownButton { get; set; }
        public MapMarker MapMarker { get; set; }
        public AreaPreview AreaPreview { get; set; }
        public bool ShowAreaPreview => HoverTimer > 0;
        public float HoverTimer { get; set; }
        public MapUI()
        {
            Background = new Background();
            SpringHills = new SpringHills();
            WarriorsDoor = new WarriorsDoor();
            WitchTown = new WitchTown();
            Compass = new Compass();
            Border = new Border();
            Border2 = new Border2();
            MapMarker = new MapMarker();
            AreaPreview = new AreaPreview(this);

            ModWall marker = ModContent.GetModWall(ModContent.WallType<SpringHillsInnerMarker>());
            SpringHillsInnerButton = new MapButton("SpringHillsInner", this, marker);
            //Set inner button hitbox
            SpringHillsInnerButton.Left.Pixels = 580;
            SpringHillsInnerButton.Top.Pixels = 300;
            SpringHillsInnerButton.Width.Pixels = 24;
            SpringHillsInnerButton.Height.Pixels = 80;

            marker = ModContent.GetModWall(ModContent.WallType<SpringHillsOuterMarker>());
            SpringHillsOuterButton = new MapButton("SpringHillsOuter", this, marker);

            //Set outer button hitbox
            SpringHillsOuterButton.Left.Pixels = 656;
            SpringHillsOuterButton.Top.Pixels = 300;
            SpringHillsOuterButton.Width.Pixels = 32;
            SpringHillsOuterButton.Height.Pixels = 96;

            marker = ModContent.GetModWall(ModContent.WallType<WarriorsDoorMarker>());
            WarriorsDoorButton = new MapButton("WarriorsDoor", this, marker);

            //Set warriors door button hitbox
            WarriorsDoorButton.Left.Pixels = 605;
            WarriorsDoorButton.Top.Pixels = 320;
            WarriorsDoorButton.Width.Pixels = 48;
            WarriorsDoorButton.Height.Pixels = 48;

            marker = ModContent.GetModWall(ModContent.WallType<WitchTownMarker>());
            WitchTownButton = new MapButton("WitchTown", this, marker);
            //Set witch town hitbox
            WitchTownButton.Left.Pixels = 605;
            WitchTownButton.Top.Pixels = 293;
            WitchTownButton.Width.Pixels = 48;
            WitchTownButton.Height.Pixels = 24;

            //Setup area visibilities

        }

        private void FogUp(int left, int top, int width, int height, Func<bool> checkFunc)
        {

        }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 1280;
            Height.Pixels = 720;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            Append(Background);
            Append(SpringHills);
            Append(WarriorsDoor);
            Append(WitchTown);
            Append(Compass);
            Append(Border);
            Append(Border2);

            Append(SpringHillsInnerButton);
            Append(SpringHillsOuterButton);
            Append(WarriorsDoorButton);
            Append(WitchTownButton);
            Append(MapMarker);
            Append(AreaPreview);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            HoverTimer--;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;








            AreaPreview.Left.Pixels = 16;
            AreaPreview.Top.Pixels = 32;

            Border2.Left.Pixels = -50;
            Border2.Top.Pixels = -30;
        }
    }

    internal class MapUIState : UIState
    {
        public MapUI ui;
        public MapUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            ui = new MapUI();
            Append(ui);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.TriggerTiles;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.Core.TileEntityEditorSystem
{
    internal class BossSpawnTileUIState : BaseModTileEditorUIState
    {
        public BossSpawnTileUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new BossSpawnTileEditor();
            Append(ui);
        }
    }

    internal class BossSpawnTileEditor : BaseModTileEditor
    {
        private UIGrid _grid;
        internal int RelativeLeft => Main.screenWidth - Main.screenWidth / 4 - (int)Width.Pixels / 2;
        internal int RelativeTop => Main.screenHeight - Main.screenHeight / 2 - (int)Height.Pixels * 2;
        public BossSpawnTileEditor() : base()
        {
            SpawnPoint = new DraggablePointField();
            TopLeft = new DraggablePointField();
            BottomRight = new DraggablePointField();
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            _grid = new UIGrid();
            _grid.Width.Set(0, 1f);
            _grid.Height.Set(0, 1f);
            _grid.HAlign = 0f;
            _grid.ListPadding = 2f;
            Append(_grid);

            SelectNPCButton = new NPCSelectButton("Boss", (npc) => { BossToSpawn = npc; });
            _grid.Add(SelectNPCButton);

            SpawnOffsetButton = new("Spawn Offset", SpawnPoint);
            _grid.Add(SpawnOffsetButton);

            TopLeftButton = new("Top Left", TopLeft);
            _grid.Add(TopLeftButton);

            BottomRightButton = new("Bottom Right", BottomRight);
            _grid.Add(BottomRightButton);

            SaveButton = new SaveTileEntityButton();
            _grid.Add(SaveButton);

            Width.Pixels = 256;
            Height.Pixels = (_grid.Count + 1) * 64;
        }

        //Edit the Spawn Point
        //Edit the Top Left
        //Edit the Bottom Right


        //Fields to Edit
        public DraggablePointField SpawnPoint;
        public DraggablePointField TopLeft;
        public DraggablePointField BottomRight;
        public ModNPC BossToSpawn;

        public DraggablePointButton SpawnOffsetButton;
        public DraggablePointButton TopLeftButton;
        public DraggablePointButton BottomRightButton;
        public NPCSelectButton SelectNPCButton;
        public SaveTileEntityButton SaveButton;
        public override void Load(ModTileEntity modTileEntity)
        {
            base.Load(modTileEntity);
            BossSpawnTileEntity bossSpawnTileEntity = (BossSpawnTileEntity)modTileEntity;
            if (bossSpawnTileEntity == null)
                return;

            Point point = new Point(modTileEntity.Position.X, modTileEntity.Position.Y);
            if (!string.IsNullOrEmpty(bossSpawnTileEntity.BossToSpawn))
                BossToSpawn = ModContent.Find<ModNPC>(bossSpawnTileEntity.BossToSpawn);
            SpawnPoint.Point = bossSpawnTileEntity.SpawnOffset + point;
            TopLeft.Point = bossSpawnTileEntity.TopLeftOffset + point;
            BottomRight.Point = bossSpawnTileEntity.BottomRightOffset + point;
        }

        public override void Apply(ModTileEntity modTileEntity)
        {
            base.Apply(modTileEntity);
            BossSpawnTileEntity bossSpawnTileEntity = (BossSpawnTileEntity)modTileEntity;
            if (bossSpawnTileEntity == null)
                return;

            Point point = new Point(modTileEntity.Position.X, modTileEntity.Position.Y);
            bossSpawnTileEntity.BossToSpawn = Stellamod.Instance.Name + "/" + BossToSpawn.Name;
            bossSpawnTileEntity.SpawnOffset = SpawnPoint.Point - point;
            bossSpawnTileEntity.TopLeftOffset = TopLeft.Point - point;
            bossSpawnTileEntity.BottomRightOffset = BottomRight.Point - point;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            spriteBatch.RestartDefaults();

            Vector2 spawnPoint = SpawnPoint.Point.ToWorldCoordinates() - Main.screenPosition;
            Vector2 topLeft = TopLeft.Point.ToWorldCoordinates() - Main.screenPosition;
            Vector2 bottomRight = BottomRight.Point.ToWorldCoordinates() - Main.screenPosition;
            int width = (int)bottomRight.X - (int)topLeft.X;
            int height = (int)bottomRight.Y - (int)topLeft.Y;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)topLeft.X, (int)topLeft.Y, width, height), Color.Red * 0.5f);

            if (BossToSpawn != null)
            {
                Texture2D texture = ModContent.Request<Texture2D>(BossToSpawn.Texture).Value;
                int frameCount = Main.npcFrameCount[BossToSpawn.Type];
                int frameHeight = texture.Height / frameCount;
                Rectangle rectangle = new Rectangle(0, 0, texture.Width, frameHeight);
                spriteBatch.Draw(texture,
                    spawnPoint, rectangle, Color.White * 0.5f, 0, rectangle.Size() / 2, Vector2.One, SpriteEffects.None, 0);
            }


            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
        }
    }
}

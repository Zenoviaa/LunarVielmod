using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.TriggerTiles;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.Core.TileEntityEditorSystem
{
    internal class SetNPCSpawnTileUIState : BaseModTileEditorUIState
    {
        public SetNPCSpawnTileUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new SetNPCSpawnTileEditor();
            Append(ui);
        }
    }

    internal class SetNPCSpawnTileEditor : BaseModTileEditor
    {
        private UIGrid _grid;
        internal int RelativeLeft => Main.screenWidth - Main.screenWidth / 4 - (int)Width.Pixels / 2;
        internal int RelativeTop => Main.screenHeight - Main.screenHeight / 2 - (int)Height.Pixels * 2;
        public SetNPCSpawnTileEditor() : base()
        {
            SpawnPoint = new DraggablePointField();
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

            SelectNPCButton = new NPCSelectButton("NPC", (npc) => { BossToSpawn = npc; });
            _grid.Add(SelectNPCButton);

            SpawnOffsetButton = new("Spawn Offset", SpawnPoint);
            _grid.Add(SpawnOffsetButton);

            SaveButton = new SaveTileEntityButton();
            _grid.Add(SaveButton);

            Width.Pixels = 256;
            Height.Pixels = (_grid.Count + 1) * 64;
        }

        //Edit the Spawn Point
        //Edit the Top Left
        //Edit the Bottom Right


        //Fields to Edit
        public ModNPC BossToSpawn;
        public DraggablePointField SpawnPoint;
        public DraggablePointButton SpawnOffsetButton;
        public NPCSelectButton SelectNPCButton;
        public SaveTileEntityButton SaveButton;
        public override void Load(ModTileEntity modTileEntity)
        {
            base.Load(modTileEntity);
            SetNPCSpawnTileEntity bossSpawnTileEntity = (SetNPCSpawnTileEntity)modTileEntity;
            if (bossSpawnTileEntity == null)
                return;

            Point point = new Point(modTileEntity.Position.X, modTileEntity.Position.Y);
            if (!string.IsNullOrEmpty(bossSpawnTileEntity.BossToSpawn))
                BossToSpawn = ModContent.Find<ModNPC>(bossSpawnTileEntity.BossToSpawn);
            SpawnPoint.Point = bossSpawnTileEntity.SpawnOffset + point;
        }

        public override void Apply(ModTileEntity modTileEntity)
        {
            base.Apply(modTileEntity);
            SetNPCSpawnTileEntity bossSpawnTileEntity = (SetNPCSpawnTileEntity)modTileEntity;
            if (bossSpawnTileEntity == null)
                return;

            Point point = new Point(modTileEntity.Position.X, modTileEntity.Position.Y);
            bossSpawnTileEntity.BossToSpawn = Stellamod.Instance.Name + "/" + BossToSpawn.Name;
            bossSpawnTileEntity.SpawnOffset = SpawnPoint.Point - point;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            spriteBatch.RestartDefaults();

            Vector2 spawnPoint = SpawnPoint.Point.ToWorldCoordinates() - Main.screenPosition;
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

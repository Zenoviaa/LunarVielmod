using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.Core.TriggersSystem.Triggers
{
    public class BossSpawnTrigger :
        Trigger,
        ISaveData,
        INetData
    {
        private ModNPC _modNPC;
        public string bossToSpawn;
        public Point spawnOffset;
        public Point topLeftOffset;
        public Point bottomRightOffset;

        private ModNPC GetModNPC()
        {
            if (_modNPC == null || _modNPC.Name != bossToSpawn)
            {
                _modNPC = ModContent.Find<ModNPC>(bossToSpawn);
            }
            return _modNPC;
        }

        public override void Edit()
        {
            TriggerSelector editorPopupUISystem = ModContent.GetInstance<TriggerSelector>();
            BossSpawnTileUIState uiState = new BossSpawnTileUIState(this);
            editorPopupUISystem.OpenUI(uiState);
        }

        public override void DrawHandles(SpriteBatch spriteBatch)
        {
            Point spawnPoint = new Point(position.X, position.Y);
            spawnPoint.X += spawnOffset.X;
            spawnPoint.Y += spawnOffset.Y;
            Vector2 spawnPos = spawnPoint.ToWorldCoordinates() - Main.screenPosition;
            Vector2 startDrawPos = new Point(position.X, position.Y).ToWorldCoordinates() - Main.screenPosition;
            spriteBatch.DrawLine(startDrawPos, spawnPos, Color.Red);
        }
        public override void Invoke()
        {
            Point spawnPoint = new Point(position.X, position.Y);
            spawnPoint.X += spawnOffset.X;
            spawnPoint.Y += spawnOffset.Y;
            NPC.NewNPC(new EntitySource_TileBreak(spawnPoint.X, spawnPoint.Y), spawnPoint.X * 16, spawnPoint.Y * 16, GetModNPC().Type);
        }

        public override bool ShouldInvoke()
        {
            //Triggers should only run on the host
            //We don't have to check that I don't think
            if (string.IsNullOrEmpty(bossToSpawn))
                return false;
            if (GetModNPC() == null)
                return false;
            if (NPC.AnyNPCs(_modNPC.Type))
                return false;

            ModNPC modNPC = GetModNPC();
            if(modNPC is ScarletBoss scarletBoss && !scarletBoss.CanFight())
            {
                return false;
            }
            bool allPlayersFar = true;

            Point spawnPoint = new Point(position.X, position.Y);
            spawnPoint.X += spawnOffset.X;
            spawnPoint.Y += spawnOffset.Y;
            Vector2 spawnPos = spawnPoint.ToWorldCoordinates();


            Point topLeft = new Point(position.X + topLeftOffset.X, position.Y + topLeftOffset.Y);
            Point bottomRight = new Point(position.X + bottomRightOffset.X, position.Y + bottomRightOffset.Y);
            Vector2 worldPos = topLeft.ToWorldCoordinates();
            int width = bottomRight.X - topLeft.X;
            int height = bottomRight.Y - topLeft.Y;
            Rectangle rectangle = new Rectangle((int)worldPos.X, (int)worldPos.Y, width * 16, height * 16);
            foreach (var player in Main.ActivePlayers)
            {
                if (rectangle.Contains((int)player.position.X, (int)player.position.Y))
                {

                    allPlayersFar = false;
                    break;
                }
            }

            if (allPlayersFar)
                return false;
            return true;
        }

        public void NetSend(BinaryWriter writer)
        {
            writer.Write(bossToSpawn);
            writer.Write(spawnOffset.X);
            writer.Write(spawnOffset.Y);
            writer.Write(topLeftOffset.X);
            writer.Write(topLeftOffset.Y);
            writer.Write(bottomRightOffset.X);
            writer.Write(bottomRightOffset.Y);
        }

        public void NetReceive(BinaryReader reader)
        {
            bossToSpawn = reader.ReadString();
            spawnOffset.X = reader.ReadInt32();
            spawnOffset.Y = reader.ReadInt32();
            topLeftOffset.X = reader.ReadInt32();
            topLeftOffset.Y = reader.ReadInt32();
            bottomRightOffset.X = reader.ReadInt32();
            bottomRightOffset.Y = reader.ReadInt32();
        }

        public void SaveData(TagCompound tag)
        {
            tag["boss"] = bossToSpawn;
            tag["spawnOffset"] = spawnOffset;
            tag["left"] = topLeftOffset;
            tag["right"] = bottomRightOffset;
        }

        public void LoadData(TagCompound tag)
        {
            bossToSpawn = tag.Get<string>("boss");
            spawnOffset = tag.Get<Point>("spawnOffset");
            topLeftOffset = tag.Get<Point>("left");
            bottomRightOffset = tag.Get<Point>("right");
        }
    }
    public class BossSpawnTileUIState : EditorUIState
    {
        private readonly BossSpawnTrigger _bossSpawnTrigger;
        private BossSpawnTileEditor _ui;
        public BossSpawnTileUIState(BossSpawnTrigger bossSpawnTrigger) : base()
        {
            _bossSpawnTrigger = bossSpawnTrigger;
        }



        public override void OnInitialize()
        {
            _ui = new BossSpawnTileEditor(_bossSpawnTrigger);
            Append(_ui);
        }
        public override void Close()
        {

        }
        public override void Open()
        {

        }
    }

    public class BossSpawnTileEditor : UIPanel
    {
        private UIGrid _grid;
        private readonly BossSpawnTrigger _bossSpawnTrigger;
        public int RelativeLeft => Main.screenWidth - Main.screenWidth / 4 - (int)Width.Pixels / 2;
        public int RelativeTop => Main.screenHeight - Main.screenHeight / 2 - (int)Height.Pixels * 2;
        public BossSpawnTileEditor(BossSpawnTrigger bossSpawnTrigger) : base()
        {
            _bossSpawnTrigger = bossSpawnTrigger;
            SpawnPoint = new DraggablePointField();
            TopLeft = new DraggablePointField();
            BottomRight = new DraggablePointField();

            Point point = new Point(_bossSpawnTrigger.position.X, _bossSpawnTrigger.position.Y);
            if (!string.IsNullOrEmpty(_bossSpawnTrigger.bossToSpawn))
                BossToSpawn = ModContent.Find<ModNPC>(_bossSpawnTrigger.bossToSpawn);
            SpawnPoint.Point = _bossSpawnTrigger.spawnOffset + point;
            TopLeft.Point = _bossSpawnTrigger.topLeftOffset + point;
            BottomRight.Point = _bossSpawnTrigger.bottomRightOffset + point;


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

            SaveButton = new SaveTriggerButton();
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
        public SaveTriggerButton SaveButton;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (BossToSpawn == null)
                return;
            Point point = new Point(_bossSpawnTrigger.position.X, _bossSpawnTrigger.position.Y);
            _bossSpawnTrigger.bossToSpawn = Stellamod.Instance.Name + "/" + BossToSpawn.Name;
            _bossSpawnTrigger.spawnOffset = SpawnPoint.Point - point;
            _bossSpawnTrigger.topLeftOffset = TopLeft.Point - point;
            _bossSpawnTrigger.bottomRightOffset = BottomRight.Point - point;
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

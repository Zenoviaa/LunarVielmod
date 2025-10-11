using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.Core.TriggersSystem.Triggers
{
    public class NPCSpawnTrigger :
        Trigger,
        ISaveData,
        INetData
    {
        private ModNPC _modNPC;
        public string bossToSpawn;

        public Point spawnOffset;
        public Point topLeftOffset;
        public Point bottomRightOffset;
        public override void Invoke()
        {
            Point spawnPoint = new Point(position.X, position.Y);
            spawnPoint.X += spawnOffset.X;
            spawnPoint.Y += spawnOffset.Y;
            Vector2 spawnPos = spawnPoint.ToWorldCoordinates();
            NPC.NewNPC(new EntitySource_TileBreak(position.X, position.Y), spawnPoint.X * 16, spawnPoint.Y * 16, GetModNPC().Type);
        }

        private ModNPC GetModNPC()
        {
            if (_modNPC == null || _modNPC.Name != bossToSpawn)
            {
                _modNPC = ModContent.Find<ModNPC>(bossToSpawn);
            }
            return _modNPC;
        }

        public override bool ShouldInvoke()
        {
            //Triggers should only run on the host
            //We don't have to check that I don't think
            if (NPC.AnyDanger())
                return false;
            if (string.IsNullOrEmpty(bossToSpawn))
                return false;
            if (GetModNPC() == null)
                return false;
            if (NPC.AnyNPCs(_modNPC.Type))
                return false;

            bool allPlayersFar = true;

            Point spawnPoint = new Point(position.X, position.Y);
            spawnPoint.X += spawnOffset.X;
            spawnPoint.Y += spawnOffset.Y;
            Vector2 spawnPos = spawnPoint.ToWorldCoordinates();

            foreach (var player in Main.ActivePlayers)
            {
                float distanceToPlayer = Vector2.Distance(player.position, spawnPos);
                if (distanceToPlayer <= 2100)
                {
                    allPlayersFar = false;
                    break;
                }
            }
            if (allPlayersFar)
                return false;
            return true;
        }

        public override void Edit()
        {
            TriggerSelector editorPopupUISystem = ModContent.GetInstance<TriggerSelector>();
            NPCTriggerUIState uiState = new NPCTriggerUIState(this);
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
        public void SaveData(TagCompound tag)
        {
            tag["boss"] = bossToSpawn;
            tag["spawnOffset"] = spawnOffset;
        }

        public void LoadData(TagCompound tag)
        {
            bossToSpawn = tag.Get<string>("boss");
            spawnOffset = tag.Get<Point>("spawnOffset");
        }

        public void NetSend(BinaryWriter writer)
        {
            writer.Write(bossToSpawn);
            writer.Write(spawnOffset.X);
            writer.Write(spawnOffset.Y);
        }

        public void NetReceive(BinaryReader reader)
        {
            bossToSpawn = reader.ReadString();
            spawnOffset.X = reader.ReadInt32();
            spawnOffset.Y = reader.ReadInt32();
        }
    }
    public class NPCTriggerUIState : EditorUIState
    {
        private readonly NPCSpawnTrigger _npcTrigger;
        private NPCTriggerEditor _ui;
        public NPCTriggerUIState(NPCSpawnTrigger npcTrigger) : base()
        {
            _npcTrigger = npcTrigger;
        }

        public override void OnInitialize()
        {
            _ui = new NPCTriggerEditor(_npcTrigger);
            Append(_ui);
        }
        public override void Open()
        {
            //throw new System.NotImplementedException();
        }
        public override void Close()
        {
            //  throw new System.NotImplementedException();
        }
    }

    public class NPCTriggerEditor : UIPanel
    {
        private UIGrid _grid;
        private NPCSpawnTrigger _npcTrigger;
        public int RelativeLeft => Main.screenWidth - Main.screenWidth / 4 - (int)Width.Pixels / 2;
        public int RelativeTop => Main.screenHeight - Main.screenHeight / 2 - (int)Height.Pixels * 2;
        public NPCTriggerEditor(NPCSpawnTrigger npcTrigger) : base()
        {
            _npcTrigger = npcTrigger;
            SpawnPoint = new DraggablePointField();

            Point point = new Point(_npcTrigger.position.X, _npcTrigger.position.Y);
            if (!string.IsNullOrEmpty(_npcTrigger.bossToSpawn))
                BossToSpawn = ModContent.Find<ModNPC>(_npcTrigger.bossToSpawn);
            SpawnPoint.Point = _npcTrigger.spawnOffset + point;
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

            SaveButton = new SaveTriggerButton();
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
        public SaveTriggerButton SaveButton;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (BossToSpawn == null)
                return;
            Point point = new Point(_npcTrigger.position.X, _npcTrigger.position.Y);
            _npcTrigger.bossToSpawn = Stellamod.Instance.Name + "/" + BossToSpawn.Name;
            _npcTrigger.spawnOffset = SpawnPoint.Point - point;
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

using ReLogic.Content;
using Stellamod.Common.DungeonGeneration;
using Stellamod.Core.StructureSelector;
using Stellamod.Core.ZTileSystem;
using Stellamod.WorldG.StructureManager;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Helpers
{
    public abstract class BaseSelectionProjectile : ModProjectile
    {
        private bool _pressed;
        public float YOffset;
        private Rectangle Rectangle
        {
            get
            {
                Rectangle rectangle = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);
                return rectangle;
            }
        }
        private bool IsMouseHovering
        {
            get
            {
                if (Rectangle.Contains(Main.MouseWorld.ToPoint()))
                {
                    return true;
                }
                return false;
            }
        }
        private Projectile Parent
        {
            get
            {
                return Main.projectile[(int)Projectile.ai[0]];
            }
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 54;
            Projectile.height = 22;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = int.MaxValue;
        }

        public override void AI()
        {
            base.AI();
            if (!Parent.active)
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.position = Parent.TopRight;
                Projectile.position.X += 8;
                Projectile.position.Y += YOffset;
            }

            if (Main.mouseLeftRelease && _pressed)
            {
                Press();
                _pressed = false;
            }
            if (IsMouseHovering && Main.mouseLeft && !_pressed)
            {
                _pressed = true;

            }
            if (IsMouseHovering)
            {
                Main.LocalPlayer.itemTime = 12;
                Main.LocalPlayer.itemAnimation = 12;
                Main.LocalPlayer.heldProj = Projectile.whoAmI;
            }

        }

        protected virtual void Press()
        {

        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 scale = Vector2.One;
            Color drawColor = Color.White;
            if (IsMouseHovering)
            {
                scale *= 1.5f;
                drawColor = Color.LightGoldenrodYellow;
            }
            spriteBatch.Restart(samplerState: SamplerState.PointWrap);


            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.Draw(TextureAssets.Projectile[Type].Value, drawPos, null, drawColor, Projectile.rotation, TextureAssets.Projectile[Type].Value.Size() / 2, scale, SpriteEffects.None, 0);
            return false;
        }
    }
    public class Magic : BaseSelectionProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            YOffset = 24;
        }
        protected override void Press()
        {
            base.Press();
            StructureSelection structureSelection = ModContent.GetInstance<StructureSelection>();
            structureSelection.OpenMagicSelectionUI();
        }
    }
    public class Save : BaseSelectionProjectile
    {
        protected override void Press()
        {
            base.Press();
            StructureSelection structureSelection = ModContent.GetInstance<StructureSelection>();
            structureSelection.OpenSaveSelectionUI();
        }
    }

    public abstract class AbstractStructureWand : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
    public class EraseWand : AbstractStructureWand
    {
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                StructureSelection selection = ModContent.GetInstance<StructureSelection>();
                selection.Erase();
            }

            //selection.SpawnSelection = true;
            SoundEngine.PlaySound(SoundID.Item47);
            return true;
        }
    }

    public class CopyWand : AbstractStructureWand
    {
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                StructureSelection selection = ModContent.GetInstance<StructureSelection>();
                if (player.altFunctionUse == 2)
                {
                  //  selection.Erase();
                }
                else
                {
                    selection.Copy();
                }
          
          
            }

            //selection.SpawnSelection = true;
            SoundEngine.PlaySound(SoundID.Item47);
            return true;
        }
    }

    public class PasteWand : AbstractStructureWand
    {
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                StructureSelection selection = ModContent.GetInstance<StructureSelection>();
                if (player.altFunctionUse == 2)
                {
                    Structurizer.FlipStructure = !Structurizer.FlipStructure;
                    Main.NewText($"FLIP: {Structurizer.FlipStructure}");
                //    selection.Erase();
                }
                else
                {
                    selection.Paste();
                }
          
            
            }

            //selection.SpawnSelection = true;
            SoundEngine.PlaySound(SoundID.Item47);
            return true;
        }
    }


    [Autoload(Side = ModSide.Client)]
    public class StructureSelection : ModSystem
    {
        public class StructurePoint
        {
            private Asset<Texture2D> _structurePointAsset;
            private static bool _capturedMouse;
            private bool _isDragging;

            private Rectangle Rectangle
            {
                get
                {
                    Rectangle rectangle = new Rectangle((int)position.X - 8, (int)position.Y - 8, 16, 16);
                    return rectangle;
                }
            }
            private bool IsMouseHovering
            {
                get
                {
                    if (Rectangle.Contains(Main.MouseWorld.ToPoint()))
                    {
                        return true;
                    }
                    return false;
                }
            }
            private bool IsDragging
            {
                get
                {
                    return IsMouseHovering && Main.mouseLeft;
                }
            }

            public Vector2 position;
            public bool isTopRight;
            public void Update()
            {
                if (IsMouseHovering && Main.mouseLeft && !_capturedMouse)
                {
                    _isDragging = true;
                    _capturedMouse = true;
                }
                if (_isDragging && !Main.mouseLeft)
                {
                    _isDragging = false;
                    _capturedMouse = false;
                }
                StructureSelection structureSelection = ModContent.GetInstance<StructureSelection>();
                if (_isDragging)
                {
                    int x = (int)Main.MouseWorld.X / 16;
                    int y = (int)Main.MouseWorld.Y / 16;
                    Vector2 roundedPoint = new Vector2(x, y) * 16;
                    position = roundedPoint;
                }
                if (isTopRight)
                {
                    structureSelection.TopRight = position.ToTileCoordinates();
                }
                else
                {
                    structureSelection.BottomLeft = position.ToTileCoordinates();
                }

                if (IsMouseHovering)
                {
                    Main.LocalPlayer.itemTime = 12;
                    Main.LocalPlayer.itemAnimation = 12;
                }
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                _structurePointAsset ??= ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/StructurePoint");
                Vector2 scale = Vector2.One;
                Color drawColor = Color.White;
                if (IsMouseHovering)
                {
                    scale *= 1.5f;
                    drawColor = Color.LightGoldenrodYellow;
                }

                StructureSelection structureSelection = ModContent.GetInstance<StructureSelection>();
                Texture2D line = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/StructureLine").Value;

                Vector2 drawPos = position - Main.screenPosition;
                int x = (int)drawPos.X;
                int y = (int)drawPos.Y;

                //This should scroll the texture



                Color chainColor = Color.White;
                if (isTopRight)
                {
                    //Draw Down/Left
                    Vector2 drawOrigin = new Vector2(0, line.Height / 2);
                    Rectangle destinationRectangle = new Rectangle(x, y, (int)structureSelection.XDistance, line.Height);
                    Rectangle sourceRectangle = new Rectangle(0, 0, (int)structureSelection.XDistance, line.Height);
                    sourceRectangle.X += (int)(Main.GlobalTimeWrappedHourly * 32);
                    spriteBatch.Draw(line, destinationRectangle, sourceRectangle, chainColor, 0 - MathHelper.ToRadians(180), drawOrigin, SpriteEffects.None, 0);


                    destinationRectangle.Width = (int)structureSelection.YDistance;
                    sourceRectangle.Width = (int)structureSelection.YDistance;

                    spriteBatch.Draw(line, destinationRectangle, sourceRectangle, chainColor, 0 + MathHelper.ToRadians(90), drawOrigin, SpriteEffects.None, 0);
                }
                else
                {
                    //Draw Up/Right
                    Vector2 drawOrigin = new Vector2(0, line.Height / 2);
                    Rectangle destinationRectangle = new Rectangle(x, y, (int)structureSelection.XDistance, line.Height);
                    Rectangle sourceRectangle = new Rectangle(0, 0, (int)structureSelection.XDistance, line.Height);
                    sourceRectangle.X += (int)(Main.GlobalTimeWrappedHourly * 32);
                    spriteBatch.Draw(line, destinationRectangle, sourceRectangle, chainColor, 0, drawOrigin, SpriteEffects.None, 0);


                    destinationRectangle.Width = (int)structureSelection.YDistance;
                    sourceRectangle.Width = (int)structureSelection.YDistance;

                    spriteBatch.Draw(line, destinationRectangle, sourceRectangle, chainColor, 0 - MathHelper.ToRadians(90), drawOrigin, SpriteEffects.None, 0);
                }

                spriteBatch.Draw(_structurePointAsset.Value, position - Main.screenPosition, null, drawColor, 0, _structurePointAsset.Value.Size() / 2, scale, SpriteEffects.None, 0);
            }
        }

        public class CopySelection
        {
            public byte[] structureBytes;
            public TagCompound triggerCompoundRoot;
            public TagCompound tileEntityCompoundRoot;
            public TagCompound zTileCompoundRoot;
            public int width;
            public int height;
        }

        public Point BottomLeft;
        public Point TopRight;
        public StructurePoint bottomLeftPoint;
        public StructurePoint topRightPoint;
        public CopySelection copySelection;
        public Vector2 TopRightWorld => TopRight.ToWorldCoordinates();
        public Vector2 BottomLeftWorld => BottomLeft.ToWorldCoordinates();
        public float XDistance => TopRightWorld.X - BottomLeftWorld.X;
        public float YDistance => BottomLeftWorld.Y - TopRightWorld.Y;
        public override void Load()
        {
            base.Load();
            copySelection = new CopySelection();
            bottomLeftPoint = new StructurePoint();
            topRightPoint = new StructurePoint();
        }

        public bool ShowStructureSelection()
        {
            if (Main.LocalPlayer.HeldItem.type == ModContent.ItemType<WandSaver>())
                return true;
            if (Main.LocalPlayer.HeldItem.ModItem is not null && Main.LocalPlayer.HeldItem.ModItem is AbstractStructureWand)
                return true;
            return false;
        }
        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            if (!ShowStructureSelection())
                return;

            bottomLeftPoint.Update();
            topRightPoint.Update();
        }

        public void MakeNewSelection()
        {
            Point tilePoint = Main.MouseWorld.ToTileCoordinates();
            int x = (int)Main.MouseWorld.X / 16;
            int y = (int)Main.MouseWorld.Y / 16;
            Vector2 roundedPoint = new Vector2(x, y) * 16;
            Vector2 roundedPoint2 = new Vector2(x + 15, y - 15) * 16;
            BottomLeft = tilePoint;
            TopRight = tilePoint + new Point(1, -1);

            bottomLeftPoint = new StructurePoint();

            topRightPoint = new StructurePoint();
            topRightPoint.isTopRight = true;
            bottomLeftPoint.position = BottomLeftWorld;
            topRightPoint.position = TopRightWorld;
        }

        public override void PostDrawTiles()
        {
            base.PostDrawTiles();
            if (!ShowStructureSelection())
                return;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            bottomLeftPoint.Draw(spriteBatch);
            topRightPoint.Draw(spriteBatch);
            if(Main.LocalPlayer.HeldItem.type == ModContent.ItemType<PasteWand>())
            {
                int width, height;
                width = copySelection.width;
                height = copySelection.height;

                Point tilePoint = Main.MouseWorld.ToTileCoordinates();
                int x = (int)Main.MouseWorld.X / 16;
                int y = (int)Main.MouseWorld.Y / 16;

                Vector2 topLeft = tilePoint.ToWorldCoordinates();
                topLeft.Y -= height * 16;
                Rectangle pasteRectangle = new Rectangle((int)topLeft.X-(int)Main.screenPosition.X, (int)topLeft.Y-(int)Main.screenPosition.Y, width*16, height*16);
                pasteRectangle.Y += 8;
                pasteRectangle.X -= 8;
                spriteBatch.Draw(TextureAssets.BlackTile.Value, pasteRectangle, null, Color.Red * ExtraMath.Osc(0.5f, 1f, speed: 6));
                //Primitives2D.DrawRectangle(spriteBatch, pasteRectangle, Color.Red * ExtraMath.Osc(0.5f, 1f, speed: 6));
            }
            spriteBatch.End();
        }

        public void OpenSaveSelectionUI()
        {
            ModContent.GetInstance<StructureSelectorUISystem>().OpenSaveUI();
        }

        public void OpenMagicSelectionUI()
        {
            ModContent.GetInstance<StructureSelectorUISystem>().OpenMagicWandUI();
        }

        public void Copy()
        {
            Point bottomLeft, topRight;
            bottomLeft = BottomLeft;
            topRight = TopRight;
            copySelection = new CopySelection();
            copySelection.width = topRight.X - bottomLeft.X;
            copySelection.height = bottomLeft.Y - topRight.Y;
            copySelection.structureBytes = Structurizer.Serialize(bottomLeft, topRight);
            copySelection.triggerCompoundRoot = TriggerStructurizer.Serialize(bottomLeft, topRight);
            copySelection.tileEntityCompoundRoot = TileEntityStructurizer.Serialize(bottomLeft, topRight);
            copySelection.zTileCompoundRoot = ZTileStructurizer.Serialize(bottomLeft, topRight);
            Main.NewText("$Copied Structure!");
        }

        public void Paste()
        {
            if (copySelection == null)
                return;

            Point tilePoint = Main.MouseWorld.ToTileCoordinates();
            int x = (int)Main.MouseWorld.X / 16;
            int y = (int)Main.MouseWorld.Y / 16;
            // = tilePoint;
            Structurizer.DeSerialize(copySelection.structureBytes, tilePoint);
            TriggerStructurizer.DeSerialize(copySelection.triggerCompoundRoot, tilePoint);
            TileEntityStructurizer.DeSerialize(copySelection.tileEntityCompoundRoot, tilePoint);
            ZTileStructurizer.DeSerialize(copySelection.zTileCompoundRoot, tilePoint);
            Main.NewText("Pasted Structure");

            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            int width = copySelection.width;
            int height = copySelection.height;
            Point topLeft = tilePoint;
            topLeft.Y -= height;
            NetMessage.SendTileSquare(-1, topLeft.X, topLeft.Y, width, height);
            ModContent.GetInstance<ZTileMap>().SendZTileSyncPacket();
        }

        public void Erase()
        {
            ZTileMap ztileMap = ModContent.GetInstance<ZTileMap>();
            for (int x = BottomLeft.X; x < TopRight.X; x++)
            {
                for(int y = TopRight.Y; y < BottomLeft.Y; y++)
                {
                    Main.tile[x, y].ClearEverything();
                    ztileMap.KillAnyTile(new Point(x, y));
                }
            }

            int width = TopRight.X - BottomLeft.X;
            int height = BottomLeft.Y - TopRight.Y;
            Point topLeft = BottomLeft;
            topLeft.Y = TopRight.Y;

            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            NetMessage.SendTileSquare(-1, topLeft.X, topLeft.Y, width, height);
            ztileMap.SendZTileSyncPacket();
        }

        public void SaveSelection(string fileName)
        {
            if (DungeonGenerationHelper.DoorInRectangle(BottomLeft, TopRight))
            {
                string structurePath = $"Dungeon/{fileName}";
                string savePath = Main.SavePath + $"/ModSources/{Mod.Name}/Structures/{structurePath}{DungeonGenerationHelper.FileExtension}";
                using var doorStream = File.Open(savePath, FileMode.Create);
                DungeonGenerationHelper.SaveDoors(doorStream, BottomLeft, TopRight);


                Structurizer.SaveStruct(structurePath, BottomLeft, TopRight);
                TriggerStructurizer.SaveStruct(structurePath, BottomLeft, TopRight);
                TileEntityStructurizer.SaveStruct(structurePath, BottomLeft, TopRight);
                ZTileStructurizer.SaveStruct(structurePath, BottomLeft, TopRight);
            }
            else
            {
                Structurizer.SaveStruct(fileName, BottomLeft, TopRight);
                TriggerStructurizer.SaveStruct(fileName, BottomLeft, TopRight);
                TileEntityStructurizer.SaveStruct(fileName, BottomLeft, TopRight);
                ZTileStructurizer.SaveStruct(fileName, BottomLeft, TopRight);
            }

            SoundEngine.PlaySound(SoundID.AchievementComplete);
        }

        public void MagicWandReplace(Item targetItem, Item replaceItem)
        {
            //So you can undo this
            SnapshotSystem snapshotSystem = ModContent.GetInstance<SnapshotSystem>();
            snapshotSystem.Save(BottomLeft, TopRight);
            if (targetItem.createTile != -1 && replaceItem.createWall != -1)
            {
                MagicWandTileToWall(targetItem.createTile, replaceItem.createWall);
            }
            else if (targetItem.createTile != -1 && replaceItem.createTile != -1)
            {
                MagicWandTileToTile(targetItem.createTile, replaceItem.createTile);
            }
            else if (targetItem.createWall != -1 && replaceItem.createTile != -1)
            {
                MagicWandWallToTile(targetItem.createWall, replaceItem.createTile);
            }
            else if (targetItem.createWall != -1 && replaceItem.createWall != -1)
            {
                MagicWandWallToWall(targetItem.createWall, replaceItem.createWall);
            }

            SoundEngine.PlaySound(SoundID.AchievementComplete);
        }

        public void MagicWandTileToWall(int targetTileType, int newWallType)
        {
            for (int x = BottomLeft.X; x <= TopRight.X; x++)
            {
                for (int y = TopRight.Y; y <= BottomLeft.Y; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.TileType == targetTileType)
                    {
                        WorldGen.KillTile(x, y, noItem: true);
                        if (tile.WallType != 0)
                            tile.WallType = 0;

                        WorldGen.PlaceWall(x, y, newWallType);
                    }
                }
            }
        }

        public void MagicWandWallToTile(int targetWallType, int newTileType)
        {
            for (int x = BottomLeft.X; x <= TopRight.X; x++)
            {
                for (int y = TopRight.Y; y <= BottomLeft.Y; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.WallType == targetWallType)
                    {
                        tile.WallType = 0;
                        WorldGen.PlaceTile(x, y, newTileType);
                        // ModTile modTile = ModContent.GetModTile(newTileType);
                        // modTile.PlaceInWorld(x, y, new Item());
                        //WorldGen.PlaceTile(x, y, newTileType);
                    }
                }
            }
        }
        public void MagicWandTileToTile(int targetTileType, int newTileType)
        {
            for (int x = BottomLeft.X; x <= TopRight.X; x++)
            {
                for (int y = TopRight.Y; y <= BottomLeft.Y; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.TileType == targetTileType)
                    {
                        tile.TileType = (ushort)newTileType;
                    }
                }
            }
        }

        public void MagicWandWallToWall(int targetWallType, int newWallType)
        {
            for (int x = BottomLeft.X; x <= TopRight.X; x++)
            {
                for (int y = TopRight.Y; y <= BottomLeft.Y; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.WallType == targetWallType)
                    {
                        tile.WallType = (ushort)newWallType;
                    }
                }
            }
        }
    }
}

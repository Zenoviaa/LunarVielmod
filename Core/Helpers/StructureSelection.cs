using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Helpers;
using Stellamod.Core.StructureSelector;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Helpers
{
    internal abstract class BaseSelectionProjectile : ModProjectile
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
    internal class Magic : BaseSelectionProjectile
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
    internal class Save : BaseSelectionProjectile
    {
        protected override void Press()
        {
            base.Press();
            StructureSelection structureSelection = ModContent.GetInstance<StructureSelection>();
            structureSelection.OpenSaveSelectionUI();
        }
    }

    internal class StructurePoint : ModProjectile
    {
        private static bool _capturedMouse;
        private bool _isDragging;
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
        private bool IsDragging
        {
            get
            {
                return IsMouseHovering && Main.mouseLeft;
            }
        }

        private bool IsTopRight
        {
            get
            {
                return Projectile.ai[0] == 1;
            }
        }
        private ref float Timer => ref Projectile.ai[1];

        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 51200;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = int.MaxValue;
        }

        public override void AI()
        {
            base.AI();

            Player owner = Main.player[Projectile.owner];
            Timer++;
            if (Timer == 1)
            {
                _capturedMouse = false;
                _isDragging = false;
                if (IsTopRight)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Vector2.Zero,
                        ModContent.ProjectileType<Save>(), 1, 1, Projectile.owner, ai0: Projectile.whoAmI);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Vector2.Zero,
                        ModContent.ProjectileType<Magic>(), 1, 1, Projectile.owner, ai0: Projectile.whoAmI);
                }
            }

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
                Projectile.position = roundedPoint;


            }
            if (IsTopRight)
            {
                structureSelection.TopRight = Projectile.position.ToTileCoordinates();
            }
            else
            {
                structureSelection.BottomLeft = Projectile.position.ToTileCoordinates();
            }
            if (IsMouseHovering)
            {
                Main.LocalPlayer.itemTime = 12;
                Main.LocalPlayer.itemAnimation = 12;
                Main.LocalPlayer.heldProj = Projectile.whoAmI;
            }
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

            StructureSelection structureSelection = ModContent.GetInstance<StructureSelection>();
            Texture2D line = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/StructureLine").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            int x = (int)drawPos.X;
            int y = (int)drawPos.Y;

            //This should scroll the texture



            Color chainColor = Color.White;
            if (IsTopRight)
            {
                //Draw Down/Left
                Vector2 drawOrigin = new Vector2(0, line.Height / 2);
                Rectangle destinationRectangle = new Rectangle(x, y, (int)structureSelection.XDistance, line.Height);
                Rectangle sourceRectangle = new Rectangle(0, 0, (int)structureSelection.XDistance, line.Height);
                sourceRectangle.X += (int)(Main.GlobalTimeWrappedHourly * 32);
                spriteBatch.Draw(line, destinationRectangle, sourceRectangle, chainColor, Projectile.rotation - MathHelper.ToRadians(180), drawOrigin, SpriteEffects.None, 0);


                destinationRectangle.Width = (int)structureSelection.YDistance;
                sourceRectangle.Width = (int)structureSelection.YDistance;

                spriteBatch.Draw(line, destinationRectangle, sourceRectangle, chainColor, Projectile.rotation - MathHelper.ToRadians(90), drawOrigin, SpriteEffects.None, 0);
            }
            else
            {
                //Draw Up/Right
                Vector2 drawOrigin = new Vector2(0, line.Height / 2);
                Rectangle destinationRectangle = new Rectangle(x, y, (int)structureSelection.XDistance, line.Height);
                Rectangle sourceRectangle = new Rectangle(0, 0, (int)structureSelection.XDistance, line.Height);
                sourceRectangle.X += (int)(Main.GlobalTimeWrappedHourly * 32);
                spriteBatch.Draw(line, destinationRectangle, sourceRectangle, chainColor, Projectile.rotation, drawOrigin, SpriteEffects.None, 0);


                destinationRectangle.Width = (int)structureSelection.YDistance;
                sourceRectangle.Width = (int)structureSelection.YDistance;

                spriteBatch.Draw(line, destinationRectangle, sourceRectangle, chainColor, Projectile.rotation + MathHelper.ToRadians(90), drawOrigin, SpriteEffects.None, 0);
            }
            spriteBatch.RestartDefaults();
            spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, TextureAssets.Projectile[Type].Value.Size() / 2, scale, SpriteEffects.None, 0);
            return false;
        }
    }

    internal class StructureSelection : ModSystem
    {
        public bool SpawnSelection;
        public Point BottomLeft;
        public Point TopRight;
        public Vector2 TopRightWorld => TopRight.ToWorldCoordinates();
        public Vector2 BottomLeftWorld => BottomLeft.ToWorldCoordinates();
        public float XDistance => TopRightWorld.X - BottomLeftWorld.X;
        public float YDistance => TopRightWorld.Y - BottomLeftWorld.Y;
        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            Player player = Main.LocalPlayer;
            bool showSelection = player.HeldItem.type == ModContent.ItemType<Modelizer>();

            if (SpawnSelection)
            {
                foreach (var proj in Main.ActiveProjectiles)
                {
                    if (proj.type == ModContent.ProjectileType<StructurePoint>())
                        proj.Kill();
                }

                Point tilePoint = Main.MouseWorld.ToTileCoordinates();
                int x = (int)Main.MouseWorld.X / 16;
                int y = (int)Main.MouseWorld.Y / 16;
                Vector2 roundedPoint = new Vector2(x, y) * 16;
                Vector2 roundedPoint2 = new Vector2(x + 15, y - 15) * 16;


                Projectile.NewProjectile(player.GetSource_FromThis(), roundedPoint, Vector2.Zero, ModContent.ProjectileType<StructurePoint>(), 0, 0,
                    player.whoAmI);
                Projectile.NewProjectile(player.GetSource_FromThis(), roundedPoint2, Vector2.Zero, ModContent.ProjectileType<StructurePoint>(), 0, 0,
                    player.whoAmI, ai0: 1);
                SpawnSelection = false;
            }
        }

        public void OpenSaveSelectionUI()
        {
            ModContent.GetInstance<StructureSelectorUISystem>().OpenSaveUI();
        }
        public void OpenMagicSelectionUI()
        {
            ModContent.GetInstance<StructureSelectorUISystem>().OpenMagicWandUI();
        }

        public void SaveSelection(string fileName)
        {
            Structurizer.SaveStruct(fileName, BottomLeft, TopRight);
            TileEntityStructurizer.SaveStruct(fileName, BottomLeft, TopRight);
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

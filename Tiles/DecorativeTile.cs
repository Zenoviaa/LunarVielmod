using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Tiles
{
    public class DecorativeGlobalTile : GlobalTile
    {
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            base.KillTile(i, j, type, ref fail, ref effectOnly, ref noItem);
            if (effectOnly)
                return;
            if (fail)
                return;


            Tile tile = Main.tile[i, j];
            if (tile.WallType != 0)
            {
                DecorativeWall decorativeWall = ModContent.GetModWall(tile.WallType) as DecorativeWall;
                if (decorativeWall == null)
                    return;

                //TODO Add dust or sound or something later maybe idk
                WorldGen.KillWall(i, j);
            }
        }
    }

    public abstract class DecorativeWallItem : ModItem
    {

        private BaseSpecialWall _specialWall;
        private BaseSpecialWall SpecialWall
        {
            get
            {
                _specialWall ??= ModContent.GetModWall(Item.createWall) as BaseSpecialWall;
                return _specialWall;
            }
        }
        public override string Texture => "Stellamod/Tiles/ExampleDecorativeWallItem";
        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (SpecialWall != null)
            {
                if (Main.HoverItem.type == Type)
                    SpecialDecorativeWall.drawBig = true;
                SpecialWall.DrawItem(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
                return false;
            }
            return false;
        }
        public void DrawPreview(int i, int j)
        {
            SpecialWall?.DrawPreview(i, j);
        }

    }


    public abstract class BaseSpecialTile : ModTile
    {
        public virtual void DrawPreview(int i, int j)
        {

        }
    }

    public class WallPreviewSystem : ModSystem
    {
        public override void PostDrawTiles()
        {
            base.PostDrawTiles();

            SpriteBatch spriteBatch = Main.spriteBatch;
            Player player = Main.LocalPlayer;
            if (player.HeldItem.ModItem is DecorativeWallItem decorativeWallItem)
            {
                Vector2 mouseWorld = Main.MouseWorld;
                int i = (int)(mouseWorld.X / 16f);
                int j = (int)(mouseWorld.Y / 16f);

                Texture2D wallTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
                Vector2 drawOrigin = wallTexture.Size() / 2f;

                Rectangle frame = new Rectangle(0, 0, 16, 16);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                decorativeWallItem.DrawPreview(i, j);


                Vector2 tilePos = new Vector2(i, j) * 16;

                spriteBatch.Draw(TextureAssets.Tile[0].Value, tilePos - Main.screenPosition, frame, Color.Green * 0.5f, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                Vector2 cameraCenterWorld = Main.Camera.Center;
                Vector2 cameraTopLeft = cameraCenterWorld - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
                Vector2 cameraBottomRight = cameraCenterWorld + new Vector2(Main.screenWidth, Main.screenHeight) / 2;
                cameraTopLeft -= new Vector2(128);
                cameraBottomRight += new Vector2(128);

                Point topLeftTile = cameraTopLeft.ToTileCoordinates();
                Point bottomRightTile = cameraBottomRight.ToTileCoordinates();

                for (int x = topLeftTile.X; x < bottomRightTile.X; x++)
                {
                    for (int y = topLeftTile.Y; y < bottomRightTile.Y; y++)
                    {
                        if (!WorldGen.InWorld(x, y))
                            continue;

                        Tile tile = Main.tile[x, y];
                        if (!tile.HasTile)
                            continue;
                        if (tile.WallType == 0)
                            continue;
                        var specialWall = ModContent.GetModWall(tile.WallType) as BaseSpecialWall;
                        if (specialWall == null)
                            continue;

                        Vector2 drawPos = new Vector2(x, y) * 16;
                        Color drawColor = Color.Red;

                        spriteBatch.Draw(TextureAssets.Tile[0].Value, drawPos - Main.screenPosition, frame, drawColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    }
                }

                spriteBatch.End();
            }


        }
    }
    public abstract class BaseSpecialWall : ModWall
    {
        public Color BackgroundColor => Color.Lerp(Color.White, Color.Black, 0.5f);
        public virtual void DrawPreview(int i, int j)
        {

        }
        public virtual void DrawItem(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
        }
    }
    public abstract class SpecialDecorativeWall : BaseSpecialWall
    {
        public enum DrawOrigin
        {
            BottomUp,
            TopDown,
            Center
        }
        private float _hoverLerp;
        private bool _shouldClick;
        public Color StructureColor { get; set; }
        public override string Texture => (GetType().FullName + "_S").Replace(".", "/");
        public string StructureTexture { get; set; }
        public DrawOrigin Origin { get; set; } = DrawOrigin.BottomUp;
        public int FrameCount { get; set; } = 1;
        public int HorizontalFrameCount { get; set; } = 1;
        public int VerticalFrameCount { get; set; } = 1;
        public float FrameSpeed { get; set; } = 1f;
        public bool DesyncAnimations { get; set; } = false;
        public float DrawScale { get; set; } = 1f;
        public float WindSwayOffset { get; set; } = 0f;
        public float WindSwayMagnitude { get; set; } = 0f;
        public float WindSwaySpeed { get; set; } = 0f;
        public bool BlackIsTransparency { get; set; } = false;
        public bool IgnoreLightning { get; set; } = false;

        public float Alpha { get; set; } = 1f;
        public Color ClickColor { get; set; }
        public Action HoverFunc { get; set; }
        public Action ClickFunc { get; set; }
        public float Rotation { get; private set; }
        public bool gintzeHopping;
        public float gintzeHoppingTimer;
        public bool AdditiveDraw { get; set; }
        public static bool drawBig;
        public override void SetStaticDefaults()
        {
            StructureColor = Color.White;
            StructureTexture = GetType().FullName + "_S";
            StructureTexture = StructureTexture.Replace(".", "/");
            Main.wallHouse[Type] = false;

            AddMapEntry(new Color(200, 200, 200));
        }

        public override bool Drop(int i, int j, ref int type)
        {
            return false;
        }

        public override void DrawItem(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int textureWidth = texture.Width;
            int textureHeight = texture.Height;
            Rectangle drawFrame = texture.GetFrame(0, FrameCount);

            if (HorizontalFrameCount > 1)
            {
                drawFrame = texture.GetFrame(0, HorizontalFrameCount, VerticalFrameCount);
            }
            Vector2 drawOrigin = drawFrame.Size() / 2f;
            Vector2 frameSize = drawFrame.Size();
            Vector2 targetSize = frame.Size();
            Vector2 mult = targetSize / frameSize;
            if (drawBig)
                mult = Vector2.One; ;
            drawBig = false;
            spriteBatch.Draw(texture, position, drawFrame, drawColor, 0, drawOrigin, scale * mult, SpriteEffects.None, 0);
        }
        public override bool CanExplode(int i, int j) => false;

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            return false;
        }

        private float GetLeafSway(float offset, float magnitude, float speed)
        {
            return (float)Math.Sin(Main.GameUpdateCount * speed + offset) * magnitude;
        }

        public virtual void Update(int i, int j) { }
        public void DrawDecor(int i, int j, SpriteBatch spriteBatch)
        {
            int lightI = i;
            int lightJ = j;
            while (WorldGen.SolidTile(lightI, lightJ))
            {
                switch (Origin)
                {
                    case DrawOrigin.Center:
                    case DrawOrigin.BottomUp:
                        lightJ--;
                        break;
                    case DrawOrigin.TopDown:
                        lightJ++;
                        break;
                }
            }
            Color color2 = Lighting.GetColor(lightI, lightJ);
            
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int textureWidth = texture.Width;
            int textureHeight = texture.Height;



            Rectangle drawFrame = texture.GetFrame(0, FrameCount);
            if (FrameCount > 1)
            {
                //Let's use main global time wrappy
                float timer = Main.GlobalTimeWrappedHourly;
                timer *= FrameSpeed;
                int time = (int)timer;
                if (DesyncAnimations)
                {
                    time += i * 10;
                    time += j * 10;
                }

                int frame = time % FrameCount;
                drawFrame = texture.GetFrame(frame, FrameCount);
                if (HorizontalFrameCount > 1)
                {
                    drawFrame = texture.GetFrame(frame, HorizontalFrameCount, VerticalFrameCount);
                }
            }

            Vector2 drawPos = new Vector2(i, j) * 16;

            if (gintzeHopping)
            {
                drawPos.Y += ExtraMath.Osc(0f, -8f, speed: 3, offset: i) * gintzeHoppingTimer / 30f;
            }

            Vector2 drawOrigin = new Vector2(drawFrame.Width / 2, drawFrame.Height);
            switch (Origin)
            {
                case DrawOrigin.BottomUp:
                    drawOrigin = new Vector2(drawFrame.Width / 2, drawFrame.Height);
                    break;
                case DrawOrigin.TopDown:
                    drawOrigin = new Vector2(drawFrame.Width / 2, 0);
                    break;
                case DrawOrigin.Center:
                    drawOrigin = new Vector2(drawFrame.Width / 2, drawFrame.Height / 2);
                    break;
            }
            Color drawColor = StructureColor;
            if (!IgnoreLightning)
            {
                drawColor = drawColor.MultiplyRGB(color2);
            }
            if (BlackIsTransparency)
            {
                drawColor.A = 0;
            }

            if (_drawAlpha)
            {
                drawColor *= 0.5f;
                _drawAlpha = false;
            }

            float offset = WindSwayOffset;
            offset += i * 10;
            offset += j * 10;
            float leafSway = GetLeafSway(offset, WindSwayMagnitude, WindSwaySpeed);
            bool isMouseHovering = false;
            if (ClickFunc != null)
            {
                Player player = Main.LocalPlayer;
                float distanceToPlayer = Vector2.Distance(player.position, drawPos);

                Rectangle rectangle = new Rectangle((int)(drawPos.X - drawOrigin.X), (int)(drawPos.Y - drawOrigin.Y), texture.Width, texture.Height);

                //Check for collision last so it's optimized
                isMouseHovering = rectangle.Contains(Main.MouseWorld.ToPoint())
                    && distanceToPlayer < 160 && Collision.CanHitLine(player.position, 1, 1, rectangle.Center.ToVector2(), 1, 1);
                if (isMouseHovering)
                {
                    SpritebatchDrawer clickDrawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>("Stellamod/Assets/Textures/UI/Mouse"), drawPos - new Vector2(-8, 88));
                    int frame = 0;
                    if (ExtraMath.Osc(0f, 1f, speed: 6) > 0.5f)
                        frame = 2;
                    clickDrawer.VerticalFrame(frame, 3);
                    clickDrawer.CenterOrigin();
                    clickDrawer.color = Color.White * ExtraMath.Osc(0.5f, 1f, speed: 6);
                    spriteBatch.Draw(clickDrawer);

                    Main.LocalPlayer.mouseInterface = true;
                    if (Main.mouseLeft)
                    {

                        _shouldClick = true;
                    }
                    if (Main.mouseLeftRelease && _shouldClick)
                    {
                        ClickFunc();
                        _shouldClick = false;
                    }
                }
                else
                {

                }
                drawColor = drawColor.MultiplyRGB(Color.Lerp(Color.White, Color.Goldenrod, _hoverLerp * 5));
            }
            drawColor *= Alpha;
            Rotation = leafSway;
            if (ClickFunc != null && MathF.Sin(Main.GlobalTimeWrappedHourly * 8) < 0f)
            {
                float o = 2;
                spriteBatch.Restart(sortMode: SpriteSortMode.Immediate, blendState: BlendState.Additive);

                for (int c = 0; c < 8; c++)
                {
                    spriteBatch.Draw(texture,
                        drawPos - Main.screenPosition + Vector2.UnitX * o,
                        drawFrame, Color.White, leafSway, drawOrigin, DrawScale + _hoverLerp, GetSpriteEffects(i, j), 0);
                    spriteBatch.Draw(texture,
                        drawPos - Main.screenPosition - Vector2.UnitX * o,
                        drawFrame, Color.White, leafSway, drawOrigin, DrawScale + _hoverLerp, GetSpriteEffects(i, j), 0);
                    spriteBatch.Draw(texture,
                        drawPos - Main.screenPosition + Vector2.UnitY * o,
                        drawFrame, Color.White, leafSway, drawOrigin, DrawScale + _hoverLerp, GetSpriteEffects(i, j), 0);
                    spriteBatch.Draw(texture,
                        drawPos - Main.screenPosition - Vector2.UnitY * o,
                        drawFrame, Color.White, leafSway, drawOrigin, DrawScale + _hoverLerp, GetSpriteEffects(i, j), 0);
                }

                spriteBatch.RestartDefaults();
            }

            if (isMouseHovering)
            {
                float o = 2;
                spriteBatch.Restart(sortMode: SpriteSortMode.Immediate, blendState: BlendState.Additive);


                for (int c = 0; c < 8; c++)
                {
                    spriteBatch.Draw(texture,
                        drawPos - Main.screenPosition + Vector2.UnitX * o,
                        drawFrame, Color.White, leafSway, drawOrigin, DrawScale + _hoverLerp, GetSpriteEffects(i, j), 0);
                    spriteBatch.Draw(texture,
                        drawPos - Main.screenPosition - Vector2.UnitX * o,
                        drawFrame, Color.White, leafSway, drawOrigin, DrawScale + _hoverLerp, GetSpriteEffects(i, j), 0);
                    spriteBatch.Draw(texture,
                        drawPos - Main.screenPosition + Vector2.UnitY * o,
                        drawFrame, Color.White, leafSway, drawOrigin, DrawScale + _hoverLerp, GetSpriteEffects(i, j), 0);
                    spriteBatch.Draw(texture,
                        drawPos - Main.screenPosition - Vector2.UnitY * o,
                        drawFrame, Color.White, leafSway, drawOrigin, DrawScale + _hoverLerp, GetSpriteEffects(i, j), 0);
                }

                spriteBatch.RestartDefaults();
                if (HoverFunc != null)
                {
                    HoverFunc();
                }


            }
            if (AdditiveDraw)
            {
                spriteBatch.Restart(blendState: BlendState.Additive);

                for (int w = 0; w < 3; w++)
                {
                    spriteBatch.Draw(texture, drawPos - Main.screenPosition, drawFrame, drawColor * 0.75f, leafSway, drawOrigin, DrawScale + _hoverLerp, GetSpriteEffects(i, j), 0);
                }

                spriteBatch.RestartDefaults();
            }
            else
            {
                spriteBatch.Draw(texture,
                drawPos - Main.screenPosition,
                drawFrame, drawColor, leafSway, drawOrigin, DrawScale + _hoverLerp, GetSpriteEffects(i, j), 0);

            }

            /*
            ToolsUISystem uiSystem = ModContent.GetInstance<ToolsUISystem>();
            if (uiSystem.ShowHitboxes)
            {
                TileHelper.DrawInvisTileNoAdj(i, j, spriteBatch);
            }*/
        }
        private bool _drawAlpha;
        public override void DrawPreview(int i, int j)
        {
            base.DrawPreview(i, j);
            _drawAlpha = true;
            DrawDecor(i, j, Main.spriteBatch);
        }
        public virtual SpriteEffects GetSpriteEffects(int i, int j)
        {
            return SpriteEffects.None;
        }

    }

    public abstract class BehindDecorativeWall : SpecialDecorativeWall
    {


    }
    public abstract class DecorativeWall : SpecialDecorativeWall
    {


    }
}

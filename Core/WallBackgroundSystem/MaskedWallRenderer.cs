using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.WallBackgroundSystem
{
    public class MaskingWallBlock : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("This is a modded wall.");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 400;
        }
        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 7;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createWall = ModContent.WallType<MaskingWall>();
        }
    }

    public class MaskingWall : ModWall
    {
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //   MaskedWallRenderer.QueueDraw(new Point(i, j));
            Vector2 worldPos = new Point(i, j).ToWorldCoordinates(0, 0);
            worldPos += new Vector2(Main.offScreenRange);

            MaskedWallRenderer wallRenderer = ModContent.GetInstance<MaskedWallRenderer>();
            Texture2D texture = wallRenderer.GetBackgroundTexture();
            Rectangle sourceRect = wallRenderer.GetSourceRectangle(i, j);
            spriteBatch.Draw(texture, worldPos - Main.screenPosition, sourceRect, Color.White);
            return false;
        }
    }

    [Autoload(Side = ModSide.Client)]
    public class MaskedWallRenderer : ModSystem,
        IRenderer
    {
        private int _renderTimer;
        private bool _rendered;
        private Queue<Point> _drawQueue;
        private ManagedRenderTarget _wallMaskRenderTarget;
        private ManagedRenderTarget _backgroundTarget;
        private ManagedRenderTarget _combinedTarget;
        public int Priority => 0;

        public override void OnModLoad()
        {
            base.OnModLoad();
            _drawQueue = new Queue<Point>();
            _wallMaskRenderTarget = ManagedRenderTarget.New();
            _backgroundTarget = ManagedRenderTarget.New();
            _combinedTarget = ManagedRenderTarget.New();
            On_Main.DoDraw_WallsTilesNPCs += DrawWalls;
           // Main
        }
        private void DrawWalls(On_Main.orig_DoDraw_WallsTilesNPCs orig, Main self)
        {
            _renderTimer--;
            if (_renderTimer > 0)
                DrawMaskedBG();
            orig(self);
        }

        public Texture2D GetBackgroundTexture()
        {
            Texture2D backgroundTexture = ModContent.Request<Texture2D>("Stellamod/Assets/Biomes/AlcadziaBiomeBackground2").Value;
            return backgroundTexture;
        }
        public Rectangle GetSourceRectangle(int tileX, int tileY)
        {
            Texture2D backgroundTexture = GetBackgroundTexture();
            int loopedX = (tileX * 16) % backgroundTexture.Width;
            int loopedY = (tileY * 16) % backgroundTexture.Height;
            Rectangle sourceRect = new Rectangle(loopedX, loopedY, 16, 16);
            return sourceRect;
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.DoDraw_WallsTilesNPCs -= DrawWalls;
        }


        private void DrawMaskedBG()
        {
           
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(_backgroundTarget, Vector2.Zero, null, Color.White);
        }
        public void Render()
        {
            return;
            //No need to render if there's no draws
            Point topLeft = (Main.Camera.Center - new Vector2(Main.screenWidth * 0.5f, Main.screenHeight * 0.5f)).ToTileCoordinates();

            int tileWidth = Main.screenWidth / 16;
            int tileHeight = Main.screenHeight / 16;
            Point bottoMRight = new Point(topLeft.X + tileWidth, topLeft.Y + tileHeight);
            for(int x = topLeft.X; x < bottoMRight.X; x++)
            {
                for(int y = topLeft.Y; y < bottoMRight.Y; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (tile.WallType == ModContent.WallType<MaskingWall>())
                        _drawQueue.Enqueue(new Point(x, y));
                }
            }
     
            if (_drawQueue.Count <= 0)
                return;
            RenderMask();
            _renderTimer = 64;
        }

        private void RenderMask()
        {
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_wallMaskRenderTarget);
            graphicsDevice.Clear(Color.Transparent);
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null,
                Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = AssetManager.GlowMask.WhiteSquare.Value;
            Vector2 drawOrigin = new Vector2(8);
            Vector2 drawScale = Vector2.One;
            while (_drawQueue.Count > 0)
            {
                Point tilePoint = _drawQueue.Dequeue();
                Vector2 worldPosition = tilePoint.ToWorldCoordinates();
               // Dust.NewDust(worldPosition, 1, 1, DustID.GemAmethyst);
                Vector2 drawPosition = worldPosition - Main.screenPosition;
                spriteBatch.Draw(texture, drawPosition, null, Color.White, 0, drawOrigin, drawScale, SpriteEffects.None, 0);
            }
            spriteBatch.End();

            graphicsDevice.SetRenderTarget(_backgroundTarget);
            graphicsDevice.Clear(Color.Transparent);
            BackgroundParallaxShader backgroundShader = BackgroundParallaxShader.Instance;
            float parallaxX = Main.screenPosition.X * 0.25f * 1f;


            backgroundShader.Parallax = new Vector2(parallaxX * 0.01f, 0.1f);
            Texture2D backgroundTexture = ModContent.Request<Texture2D>("Stellamod/Assets/Biomes/AlcadziaBiomeBackground2").Value;
            float yOffset = Main.screenHeight - backgroundTexture.Height;
            Vector2 drawPos = new Vector2(0, yOffset);

            Rectangle sourceRect = new Rectangle(0, 0, _backgroundTarget.Width, backgroundTexture.Height);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer,
             backgroundShader.Effect);
            spriteBatch.Draw(backgroundTexture, drawPos, sourceRect, Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, CustomBlendStates.Multiply);
            spriteBatch.Draw(_wallMaskRenderTarget, Vector2.Zero, null, Color.White);
            spriteBatch.End();

        }
        public static void QueueDraw(Point tilePoint)
        {
            MaskedWallRenderer renderer = ModContent.GetInstance<MaskedWallRenderer>();
            renderer._drawQueue.Enqueue(tilePoint);
        }


    }
}

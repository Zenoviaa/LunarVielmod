using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.WallBackgroundSystem
{
    public class MaskedWallDrawLayer
    {
        public Asset<Texture2D> textureAsset;
        public Vector2 parallax;
        public bool additive;
    }

    public abstract class MaskedWallBackground : ModType
    {
        public float DrawScale { get; set; }
        public float Alpha { get; set; }
        public MaskedWallDrawLayer[] DrawLayers { get; private set; }
        public Vector2 StartParallaxPosition { get; set; }

        public Color Color { get; set; }
        public bool dontDrawCenter;
        protected override void Register()
        {
            ModTypeLookup<MaskedWallBackground>.Register(this);
        }

        public sealed override void SetupContent()
        {
            base.SetupContent();
            SetStaticDefaults();
        }

        public override void Unload()
        {
            base.Unload();
            DrawLayers = null;
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DrawLayers = new MaskedWallDrawLayer[10];
            for (int i = 0; i < DrawLayers.Length; i++)
                DrawLayers[i] = new MaskedWallDrawLayer();
            Color = Color.White;
        }

        public virtual bool UseCustomDrawing()
        {
            return false;
        }

        /// <summary>
        /// If UseCustomDrawing returns true, this will run, keep in mind sprite batch needs to be begun and ended in here
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }


        public virtual bool IsActive(Player player)
        {
            return false;
        }

        /// <summary>
        /// Called before the background gets drawn, load textures and set parallax values in here, gonna call this every frame for hot reload purposes, but afterwards make it only call once
        /// If editing the draw layers array, note that there is a buffer of 10 layers, if you go above that you'll get an index out of range exception
        /// 0 is the front, 9 is the back
        /// </summary>
        public virtual void SetupDrawLayers()
        {

        }
    }

    [Autoload(Side = ModSide.Client)]
    public class MaskedWallRenderer : ModSystem,
        IRenderer
    {
        private int _renderTimer;
        private Queue<Point> _drawQueue;
        private ManagedRenderTarget _wallMaskRenderTarget;
        private ManagedRenderTarget _backgroundTarget;

        private MaskedWallBackground[] _maskedWallBackgrounds;
        private MaskedWallBackground _activeMaskedWallBackground;
        public int Priority => 0;

        public override void OnModLoad()
        {
            base.OnModLoad();
            _drawQueue = new Queue<Point>();
            _wallMaskRenderTarget = ManagedRenderTarget.New();
            _backgroundTarget = ManagedRenderTarget.New();

            _maskedWallBackgrounds = ModContent.GetContent<MaskedWallBackground>().ToArray();
            On_Main.DoDraw_WallsTilesNPCs += DrawWalls;
        }
        private void QueueDraws()
        {
            int width = Main.screenWidth;
            int height = Main.screenHeight;

            int padding = 32;


            Point topLeftTile = Main.screenPosition.ToTileCoordinates();
            topLeftTile += new Point(-padding / 2, -padding / 2);
            int tileWidth = width / 16;
            int tileHeight = height / 16;

            tileWidth += padding;
            tileHeight += padding;
            Point bottomRight = topLeftTile + new Point(tileWidth, tileHeight);

            //Clamp world bounds
            topLeftTile.X = (int)MathHelper.Clamp(topLeftTile.X, 0, Main.maxTilesX - 1);
            topLeftTile.Y = (int)MathHelper.Clamp(topLeftTile.Y, 0, Main.maxTilesY - 1);
            bottomRight.X = (int)MathHelper.Clamp(bottomRight.X, 0, Main.maxTilesX - 1);
            bottomRight.Y = (int)MathHelper.Clamp(bottomRight.Y, 0, Main.maxTilesY - 1);
            int wallType = ModContent.WallType<MaskingWall>();
            for (int x = topLeftTile.X; x < bottomRight.X; x++)
            {
                for (int y = topLeftTile.Y; y < bottomRight.Y; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.WallType != wallType)
                        continue;

                    QueueDraw(new Point(x, y));
                }
            }

        }
        private void DrawWalls(On_Main.orig_DoDraw_WallsTilesNPCs orig, Main self)
        {

            _renderTimer--;
            if (_renderTimer > 0 && _activeMaskedWallBackground != null)
            {
                QueueDraws();
                DrawMaskedBG();
            }

            orig(self);
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

        private void SelectActiveMaskedWallBackground()
        {

            //What we're gonna do is select the first background that has an alpha
            _activeMaskedWallBackground = null;
            for (int i = 0; i < _maskedWallBackgrounds.Length; i++)
            {
                MaskedWallBackground maskedWallBackground = _maskedWallBackgrounds[i];
                maskedWallBackground.SetupDrawLayers();
                Player player = Main.LocalPlayer;
                bool isActive = maskedWallBackground.IsActive(player);
                if (maskedWallBackground.Alpha <= 0)
                {
                    maskedWallBackground.StartParallaxPosition = Main.Camera.Center;
                }
                if (maskedWallBackground.Alpha > 0 && !isActive)
                {
                    _activeMaskedWallBackground = maskedWallBackground;
                    maskedWallBackground.Alpha -= 0.1f;
                    maskedWallBackground.Alpha = MathHelper.Clamp(maskedWallBackground.Alpha, 0f, 1f);
                    break;
                }
                else if (maskedWallBackground.Alpha <= 1 && isActive)
                {
                    _activeMaskedWallBackground = maskedWallBackground;
                    maskedWallBackground.Alpha += 0.1f;
                    maskedWallBackground.Alpha = MathHelper.Clamp(maskedWallBackground.Alpha, 0f, 1f);
                    break;
                }
            }
        }
        public void Render()
        {
            SelectActiveMaskedWallBackground();
            if (_activeMaskedWallBackground == null)
                return;

            RenderMask();
            _renderTimer = 64;
        }

        private void RenderMask()
        {

            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            if (_drawQueue.Count > 0)
            {

                graphicsDevice.SetRenderTarget(_wallMaskRenderTarget);
                graphicsDevice.Clear(Color.Transparent);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, null,
                    Main.GameViewMatrix.TransformationMatrix);

                Texture2D texture = AssetManager.GlowMask.WhiteSquare.Value;
                Vector2 drawOrigin = new Vector2(8);
                Vector2 drawScale = Vector2.One;
                while (_drawQueue.Count > 0)
                {
                    Point tilePoint = _drawQueue.Dequeue();
                    Vector2 worldPosition = tilePoint.ToWorldCoordinates();
                    Vector2 drawPosition = worldPosition - Main.screenPosition;
                    spriteBatch.Draw(texture, drawPosition, null, Color.White, 0, drawOrigin, drawScale, SpriteEffects.None, 0);
                }

                spriteBatch.End();
            }


            graphicsDevice.SetRenderTarget(_backgroundTarget);
            graphicsDevice.Clear(Color.Transparent);

            if (_activeMaskedWallBackground.UseCustomDrawing())
            {
                _activeMaskedWallBackground.Draw(spriteBatch);
            }
            else
            {
                for (int i = 0; i < _activeMaskedWallBackground.DrawLayers.Length; i++)
                {
                    MaskedWallDrawLayer drawLayer = _activeMaskedWallBackground.DrawLayers[i];
                    if (drawLayer == null)
                        break;
                    if (drawLayer.textureAsset == null)
                        break;
                    BackgroundParallaxShader backgroundShader = BackgroundParallaxShader.Instance;
                    Vector2 cameraMovement = Main.Camera.Center - _activeMaskedWallBackground.StartParallaxPosition;
                    backgroundShader.Parallax = drawLayer.parallax * 0.001f * (cameraMovement);
                    spriteBatch.Begin(default,
                        default,
                        SamplerState.PointWrap,
                        default,
                        default,
                        effect: backgroundShader.Effect);
                    Vector2 drawPosition = Vector2.Zero;
                    Rectangle drawRectangle = new Rectangle(0, 0, Main.screenWidth * 2, Main.screenHeight * 2);
                    Color drawColor = _activeMaskedWallBackground.Color * _activeMaskedWallBackground.Alpha;
                    if (drawLayer.additive)
                        drawColor.A = 0;
                    Vector2 drawOrigin = drawLayer.textureAsset.Value.Size() * 0.5f;
                    if (_activeMaskedWallBackground.dontDrawCenter)
                        drawOrigin = Vector2.Zero;
                    spriteBatch.Draw(drawLayer.textureAsset.Value, drawPosition, drawRectangle, drawColor, 0, drawOrigin, _activeMaskedWallBackground.DrawScale, SpriteEffects.None, 0);
                    spriteBatch.End();
                }


            }

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

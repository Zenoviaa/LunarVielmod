using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Biomes;
using Stellamod.Core.MoonWaters;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private void DrawWalls(On_Main.orig_DoDraw_WallsTilesNPCs orig, Main self)
        {
            _renderTimer--;
            if (_renderTimer > 0 && _activeMaskedWallBackground != null)
                DrawMaskedBG();
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
                if (maskedWallBackground.Alpha > 0 && !isActive)
                {
                    _activeMaskedWallBackground = maskedWallBackground;
                    maskedWallBackground.Alpha -= 0.1f;
                    maskedWallBackground.Alpha = MathHelper.Clamp(maskedWallBackground.Alpha, 0f, 1f);
                    break;
                } else if (maskedWallBackground.Alpha <= 1 && isActive)
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
  
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null,
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
            for (int i = 0; i < _activeMaskedWallBackground.DrawLayers.Length; i++)
            {
                MaskedWallDrawLayer drawLayer = _activeMaskedWallBackground.DrawLayers[i];
                if (drawLayer == null)
                    break; 
                if (drawLayer.textureAsset == null)
                    break;
                BackgroundParallaxShader backgroundShader = BackgroundParallaxShader.Instance;
                backgroundShader.Parallax = drawLayer.parallax * 0.001f * Main.Camera.Center;
                spriteBatch.Begin(default,
                    default,
                    SamplerState.PointClamp,
                    default,
                    default,
                    effect: backgroundShader.Effect);
                Vector2 drawPosition = Vector2.Zero;
                Rectangle drawRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
                Color drawColor = Color.White * _activeMaskedWallBackground.Alpha;
                if (drawLayer.additive)
                    drawColor.A = 0;
                spriteBatch.Draw(drawLayer.textureAsset.Value, drawPosition, drawRectangle, drawColor, 0, drawLayer.textureAsset.Value.Size() * 0.5f, _activeMaskedWallBackground.DrawScale, SpriteEffects.None, 0);
                spriteBatch.End();
            }

            /*
            spriteBatch.Begin(SpriteSortMode.Deferred, CustomBlendStates.Multiply);
            spriteBatch.Draw(_wallMaskRenderTarget, Vector2.Zero, null, Color.White);
            spriteBatch.End();*/

        }
        public static void QueueDraw(Point tilePoint)
        {
            MaskedWallRenderer renderer = ModContent.GetInstance<MaskedWallRenderer>();
            renderer._drawQueue.Enqueue(tilePoint);
        }


    }
}

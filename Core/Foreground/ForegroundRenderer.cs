using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.BossBannerSystem;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Core.Foreground
{

    /// <summary>
    /// Draws in front of everything, basically our background system in the foreground
    /// </summary>
    public abstract class ForegroundLayer : ModTexturedType
    {
        public float fade;
        public bool tilingInBothAxes;
        public sealed override void SetupContent()
        {
            base.SetupContent();
            SetStaticDefaults();
        }

        protected override void Register()
        {
            ModTypeLookup<ForegroundLayer>.Register(this);
        }

        /// <summary>
        /// Returns whether the foreground should be drawn
        /// </summary>
        /// <returns></returns>
        public virtual bool IsActive()
        {
            return true;
        }

        /// <summary>
        /// Set the order in the foreground that still 
        /// </summary>
        /// <param name="zLayer"></param>
        public virtual void SetLayering(ref float zLayer, ref Vector2 parallax)
        {

        }
    }

    [Autoload(Side = ModSide.Client)]
    public class ForegroundRenderer : ModSystem,
        IPostProcessingPass
    {
        private bool _drawForeground;
        private ForegroundLayer[] _layers;

        public int PostProcessPriority => 20;

        public override void OnModLoad()
        {
            base.OnModLoad();
            _layers = ModContent.GetContent<ForegroundLayer>().ToArray();
            PostProcessingRenderer.AddPass(this);
        }

        public override void PostUpdateDusts()
        {
            base.PostUpdateDusts();
            UpdateActiveForegrounds();
        }

        private void UpdateActiveForegrounds()
        {
            _drawForeground = false;
            for(int i = 0; i < _layers.Length; i++)
            {
                ForegroundLayer layer = _layers[i];
                bool isActive = layer.IsActive();
                if (isActive)
                {
                    layer.fade += 0.01f;
                }
                else
                {
                    layer.fade -= 0.01f;
                }
                layer.fade = MathHelper.Clamp(layer.fade, 0f, 1f);
                if(layer.fade > 0)
                {
                    _drawForeground = true;
                }
            }
        }

        /// <summary>
        /// Draws all active foreground elements together
        /// </summary>
        private void DrawActiveForegrounds()
        {
            if (!_drawForeground)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
  
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < _layers.Length; i++)
            {
                ForegroundLayer layer = _layers[i];
                if (layer.fade > 0 )
                {
                    if (layer.tilingInBothAxes)
                    {
                        DrawForegroundXY(spriteBatch, layer);
                    }
                    else
                    {
                        DrawForeground(spriteBatch, layer);
                    }
                }
            }
            spriteBatch.End();
        
        }

        private void DrawForegroundXY(SpriteBatch spriteBatch, ForegroundLayer layer)
        {
            float scale = 4;
            Texture2D foregroundTexture = ModContent.Request<Texture2D>(layer.Texture).Value;
            Vector2 drawOrigin = Vector2.Zero;
            Color drawColor = Color.White * layer.fade;

            float drawWidth = foregroundTexture.Width * scale;
            float drawHeight = foregroundTexture.Height * scale;


            Vector2 parallax = Vector2.Zero;
            float zLayer = 0f;
            layer.SetLayering(ref zLayer, ref parallax);

            float cameraX = (Main.Camera.Center.X);
            float cameraY = (Main.Camera.Center.Y);
            float xParallax = (cameraX * parallax.X);
            float yParallax = (cameraY * parallax.Y);


            Vector2 drawPosition = Vector2.Zero;
            drawPosition.Y -= yParallax;
            drawPosition.X -= xParallax;


            Vector2 cameraCenterWorld = Main.Camera.Center;
            Vector2 cameraTopLeft = cameraCenterWorld - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            Vector2 cameraBottomRight = cameraCenterWorld + new Vector2(Main.screenWidth, Main.screenHeight) / 2;

            int repeats = 50;

            //Offset the draw position so we have a bit of breathing room
            drawPosition -= new Vector2(drawWidth, drawHeight) * repeats / 2;
            Rectangle scissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
            for (int x = 0; x < repeats; x++)
            {
                for(int y = 0; y < repeats; y++)
                {
                    Vector2 foregroundPosition = drawPosition;
                    foregroundPosition.X += x * drawWidth;
                    foregroundPosition.Y += y * drawHeight;
                    Rectangle drawRectangle = new Rectangle((int)foregroundPosition.X, (int)foregroundPosition.Y, (int)drawWidth, (int)drawHeight);
                    if(scissorRectangle.Contains(drawRectangle) || scissorRectangle.Intersects(drawRectangle))
                    {
                        spriteBatch.Draw(foregroundTexture, foregroundPosition, null, drawColor * 0.62f, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                    }
               
                }
            }
        }

        /// <summary>
        /// Draws a foreground element
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="layer"></param>
        private void DrawForeground(SpriteBatch spriteBatch, ForegroundLayer layer)
        {
            float scale = 4;
            Texture2D foregroundTexture = ModContent.Request<Texture2D>(layer.Texture).Value;
            Vector2 drawOrigin = Vector2.Zero;
            Color drawColor = Color.White * layer.fade;

            float drawWidth = foregroundTexture.Width;
            float drawHeight = foregroundTexture.Height * scale;
        
            int worldSurfaceY = (int)((Main.worldSurface - 50) * 16);
            int cameraY = (int)Main.Camera.Center.Y;
            int diff = cameraY - worldSurfaceY;
            diff = -diff;
       

            Vector2 parallax = Vector2.Zero;
            float zLayer = 0f;
            layer.SetLayering(ref zLayer, ref parallax);

            float yParallax = (diff * parallax.Y);

            float cameraX = (Main.Camera.Center.X);
            float xParallax = (cameraX * parallax.X);

            float y = (Main.screenHeight - drawHeight);
            Vector2 drawPosition = Vector2.Zero;
            drawPosition.Y += y;
            drawPosition.Y += yParallax;
            drawPosition.X -= xParallax;

            Vector2 cameraCenterWorld = Main.Camera.Center;
            Vector2 cameraTopLeft = cameraCenterWorld - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            Vector2 cameraBottomRight = cameraCenterWorld + new Vector2(Main.screenWidth, Main.screenHeight) / 2;

            float width = drawWidth * scale;
            float height = drawHeight;
    
            //Set the scissor rectangle so sprites outside don't get drawn
            Rectangle scissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
            for (int i = 0; i < 10; i++)
            {
                Vector2 leftPosition = drawPosition;
                leftPosition.X += i * width;
                spriteBatch.Draw(foregroundTexture, leftPosition, null, drawColor * 0.62f, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
        }

        public void RenderToScreen()
        {
            DrawActiveForegrounds();
        }
    }
}

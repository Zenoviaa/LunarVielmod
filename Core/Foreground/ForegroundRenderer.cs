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
    public class ForegroundRenderer : ModSystem
    {
        private bool _drawForeground;
        private ForegroundLayer[] _layers;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _layers = ModContent.GetContent<ForegroundLayer>().ToArray();
        }
        public override void Load()
        {
            base.Load();
            On_OverlayManager.Draw += DrawActiveForegrounds;
        }

        public override void Unload()
        {
            base.Unload();
            On_OverlayManager.Draw -= DrawActiveForegrounds;
        }

        private void DrawActiveForegrounds(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
        {
            orig(self, spriteBatch, layer, beginSpriteBatch);
            //Gotta make sure we're in front of the water layer
            if (layer == RenderLayers.ForegroundWater)
            {
                DrawActiveForegrounds();
            }
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
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer,
                null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < _layers.Length; i++)
            {
                ForegroundLayer layer = _layers[i];
                if (layer.fade > 0)
                {
                    DrawForeground(spriteBatch, layer);
                }
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        }

        /// <summary>
        /// Draws a foreground element
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="layer"></param>
        private void DrawForeground(SpriteBatch spriteBatch, ForegroundLayer layer)
        {
            Texture2D foregroundTexture = ModContent.Request<Texture2D>(layer.Texture).Value;
            Vector2 drawOrigin = Vector2.Zero;
            Color drawColor = Color.White * layer.fade;


            int y = (Main.screenHeight - foregroundTexture.Height);
            int worldSurfaceY = (int)(Main.worldSurface * 16);
            int cameraY = (int)Main.Camera.Center.Y;
            int diff = cameraY - worldSurfaceY;
            
            if (diff < 0)
                diff = 0;


            Vector2 parallax = Vector2.Zero;
            float zLayer = 0f;
            layer.SetLayering(ref zLayer, ref parallax);
            Rectangle locationRectangle = new Rectangle(0, y + diff, Main.screenWidth, foregroundTexture.Height);

            int xParallax = (int)(Main.screenPosition.X * parallax.X);
          
            Rectangle sourceRectangle = new Rectangle(xParallax, 0, foregroundTexture.Width, foregroundTexture.Height);
            spriteBatch.Draw(foregroundTexture, locationRectangle, sourceRectangle, drawColor);
        }
    }
}

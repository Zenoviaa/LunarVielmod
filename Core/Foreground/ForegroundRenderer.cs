using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Effects;
using Stellamod.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Terraria;
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
        public bool showWhenNotGrounded;
        public Vector2 totalParallax;
        public Vector2 drawOffset;
        public IShader shader;
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

        public virtual float GetFloorY() { return 0f; }
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
            for (int i = 0; i < _layers.Length; i++)
            {
                ForegroundLayer layer = _layers[i];
                bool isActive = layer.IsActive();
                if(layer.fade <= 0 && !layer.showWhenNotGrounded)
                {
                    bool isPlayerTouchingGround = Main.LocalPlayer.velocity.Y == 0;
                    if (!isPlayerTouchingGround)
                        isActive = false;
                }
     
                if (isActive)
                {
                    layer.fade += 0.01f;
                }
                else
                {
                    if(layer.fade > 0)
                    {
                        layer.fade -= 0.01f;
                        if(layer.fade <= 0)
                        {
                            layer.totalParallax = Vector2.Zero;
                        }
                    }
      

                }
                layer.fade = MathHelper.Clamp(layer.fade, 0f, 1f);
                if (layer.fade > 0)
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
           // Main.NewText("e");
            for (int i = 0; i < _layers.Length; i++)
            {
                ForegroundLayer layer = _layers[i];
                if(layer.shader != null)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                        layer.shader.Effect, Main.GameViewMatrix.TransformationMatrix);
                    EffectParameter parallaxParameter = layer.shader.Effect.Parameters["uImageOffset"];
                    if(parallaxParameter != null)
                    {
                        Vector2 parallax = Vector2.Zero;
                        float zLayer = 0f;
                        layer.SetLayering(ref zLayer, ref parallax);
                        float cameraX = (Main.Camera.Center.X);
                        float cameraY = (Main.Camera.Center.Y);
                        float xParallax = (cameraX * parallax.X);
                        float yParallax = (cameraY * parallax.Y);
                        parallaxParameter.SetValue(new Vector2(xParallax, yParallax) * -0.0005f);
                    }
                }
                if (layer.fade > 0)
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
            //Main.NewText(layer.fade);
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


            Rectangle drawRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
            spriteBatch.Draw(foregroundTexture, drawRectangle, null, Color.White * 1f * layer.fade, 0, Vector2.Zero, SpriteEffects.None, 0);

            /*
            Vector2 cameraCenterWorld = Main.Camera.Center;
            Vector2 cameraTopLeft = cameraCenterWorld - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            Vector2 cameraBottomRight = cameraCenterWorld + new Vector2(Main.screenWidth, Main.screenHeight) / 2;

            int repeats = 50;

            //Offset the draw position so we have a bit of breathing room
            drawPosition -= new Vector2(drawWidth, drawHeight) * repeats / 2;
            Rectangle scissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
            for (int x = 0; x < repeats; x++)
            {
                for (int y = 0; y < repeats; y++)
                {
                    Vector2 foregroundPosition = drawPosition;
                    foregroundPosition.X += x * drawWidth;
                    foregroundPosition.Y += y * drawHeight;
                    Rectangle drawRectangle = new Rectangle((int)foregroundPosition.X, (int)foregroundPosition.Y, (int)drawWidth, (int)drawHeight);
                    if (scissorRectangle.Contains(drawRectangle) || scissorRectangle.Intersects(drawRectangle))
                    {
                        spriteBatch.Draw(foregroundTexture, foregroundPosition, null, Color.White * 0.6f * layer.fade, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                    }

                }
            }*/
        }

        /// <summary>
        /// Draws a foreground element
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="layer"></param>
        private void DrawForeground(SpriteBatch spriteBatch, ForegroundLayer layer)
        {
            float scale = 3;
            Texture2D foregroundTexture = ModContent.Request<Texture2D>(layer.Texture).Value;
            Vector2 drawOrigin = Vector2.Zero;
            Color drawColor = Color.White * layer.fade;

            float drawWidth = foregroundTexture.Width;
            float drawHeight = foregroundTexture.Height * scale;


            Vector2 parallax = Vector2.Zero;
            float zLayer = 0f;
            layer.SetLayering(ref zLayer, ref parallax);
            float y = layer.GetFloorY() - Main.screenPosition.Y;
        //    Console.WriteLine(new Vector2(0, layer.GetFloorY()).ToTileCoordinates());
            //y = (Main.screenHeight - drawHeight);
            Vector2 oldScreenPosition = Main.screenLastPosition;
            Vector2 screenPosition = Main.screenPosition;
            Vector2 cameraMovement = screenPosition - oldScreenPosition;
            Vector2 parallaxAdd = cameraMovement * parallax;
            layer.totalParallax += parallaxAdd;

            Vector2 drawPosition = Vector2.Zero;
            drawPosition.Y += y;
        //    drawPosition.Y -= layer.totalParallax.Y ;
            drawPosition.X -= layer.totalParallax.X;
            drawPosition.X -= 5000;
            drawPosition += layer.drawOffset;



            Vector2 cameraCenterWorld = Main.Camera.Center;
            Vector2 cameraTopLeft = cameraCenterWorld - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            Vector2 cameraBottomRight = cameraCenterWorld + new Vector2(Main.screenWidth, Main.screenHeight) / 2;

            float width = drawWidth * scale;
            float height = drawHeight;

            //Set the scissor rectangle so sprites outside don't get drawn
            Rectangle scissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

            for (int i = 0; i < 20; i++)
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

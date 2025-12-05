
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.PaletteShadingSystem
{
    public enum PalettePriority : byte
    {
        Low,
        Medium,
        High,
        Highest
    }

    public enum PaletteType : byte
    {
        VanillaShader,
        LunarShader
    }

    /// <summary>
    /// Handles applying a palette shader effect to the screen
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    public class PaletteShaderRenderer : ModSystem,
        IPostProcessingPass
    {
        public int PostProcessPriority => 10;
        private PaletteEffect[] _paletteEffects;
        private RenderTarget2D _paletteRenderRT;
        private Vector2 _previousScreenSize;
        public override void Load()
        {
            ResizeRenderTarget(true);
        }

        public override void OnModLoad()
        {
            base.OnModLoad();
            _paletteEffects = ModContent.GetContent<PaletteEffect>().ToArray();
            PostProcessingRenderer.AddPass(this);
        }

        private void UpdatePaletteEffects()
        {
            for (int i = 0; i < _paletteEffects.Length; i++)
            {
                PaletteEffect paletteEffect = _paletteEffects[i];
                if (paletteEffect.IsActive(Main.LocalPlayer))
                {
                    paletteEffect.fade += 0.02f;
                }
                else
                {
                    paletteEffect.fade -= 0.02f;
                }
                paletteEffect.fade = MathHelper.Clamp(paletteEffect.fade, 0f, 1f);

            }
        }

        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            ResizeRenderTarget(false);
            UpdatePaletteEffects();
        }

        private void ResizeRenderTarget(bool load)
        {
            if (!Main.gameMenu && !Main.dedServ || load && !Main.dedServ)
            {
                Vector2 currentScreenSize = new(Main.screenWidth, Main.screenHeight);
                if (currentScreenSize != _previousScreenSize)
                {
                    Main.QueueMainThreadAction(() =>
                    {
                        if (_paletteRenderRT != null && !_paletteRenderRT.IsDisposed)
                            _paletteRenderRT.Dispose();


                        _paletteRenderRT = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight, false,
                            SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

                    });
                }

                _previousScreenSize = currentScreenSize;
            }
        }

        public void RenderToScreen()
        {
            Effect paletteEffect = null;
            PalettePriority priority = PalettePriority.Low;
            float fade = 0f;
            for (int i = 0; i < _paletteEffects.Length; i++)
            {
                PaletteEffect pEffect = _paletteEffects[i];
                if (pEffect.fade > 0 && pEffect.fade >= fade && pEffect.Priority >= priority)
                {
                    fade = pEffect.fade;
                    priority = pEffect.Priority;
                    paletteEffect = pEffect.GetShader();

                }
            }

            if (paletteEffect == null)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_paletteRenderRT);
            graphicsDevice.Clear(Color.Transparent);


            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
            spriteBatch.End();

            graphicsDevice.SetRenderTarget(Main.screenTarget);
            graphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, Main.Rasterizer, paletteEffect);
            spriteBatch.Draw(_paletteRenderRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
            spriteBatch.End();
        }
    }
}

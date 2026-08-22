using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.Rendering;
using Stellamod.Core.Utilities;
using System.Linq;
using Terraria;
using Terraria.GameContent;
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
        private RenderTargetProvider _paletteRenderRT = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
        public override void Unload()
        {
            base.Unload();
            _paletteEffects = null;
            _paletteRenderRT = null;
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
            UpdatePaletteEffects();
        }

        public void RenderToScreen()
        {

            if (Main.gameMenu)
                return;
         
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

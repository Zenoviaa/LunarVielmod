using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
using Stellamod.Core.Utilities;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL
{
    [Autoload(Side = ModSide.Client)]
    public class IceRenderer : ModSystem,
        IRenderer
    {

        private ManagedRenderTarget _icicleMaskRT;
        private ManagedRenderTarget _iceRT;
        private ManagedRenderTarget _icicleRT;
        private Queue<PixelTarget.SpritebatchDrawAction> _drawActionQueue;
        private bool _ices;
        private bool _reRenderIce;

        public int Priority => 0;

        public override void OnModLoad()
        {
            base.OnModLoad();
            _drawActionQueue = new Queue<PixelTarget.SpritebatchDrawAction>(100);
            _iceRT = ManagedRenderTarget.New(ManagedRenderTarget.GetScreenTargetSize);
            _icicleMaskRT = ManagedRenderTarget.New(ManagedRenderTarget.GetScreenTargetSize);
            _icicleRT = ManagedRenderTarget.New(ManagedRenderTarget.GetScreenTargetSize);
            _reRenderIce = true;
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
        }

        public void Render()
        {
            RenderIcicleMask();

            if (_ices)
            {
                RenderIceTexture();
                RenderIcicles();
                PixelationManager.QueueSpritebatchDrawAction(DrawMaskToPixelTarget, DrawLayer.OverNPCsWithOutline);
            }
        }

        private void RenderIceTexture()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_iceRT);
            graphicsDevice.Clear(Color.Transparent);

            IceShader iceShader = IceShader.Instance;
            iceShader.NoiseTexture = TrailRegistry.Clouds3;
            iceShader.Tiling = Vector2.One * 132;


            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, iceShader.Effect);
            spriteBatch.Draw(_icicleMaskRT, Vector2.Zero, Color.White);

            spriteBatch.End();

            graphicsDevice.SetRenderTarget(null);
        }

        private void RenderIcicleMask()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_icicleMaskRT);
            graphicsDevice.Clear(Color.Transparent);
            _ices = _drawActionQueue.Count > 0;
            if (_ices)
            {

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                while (_drawActionQueue.Count > 0)
                {
                    var drawAction = _drawActionQueue.Dequeue();
                    drawAction(spriteBatch, Main.screenPosition);
                }
                spriteBatch.End();
            }
            graphicsDevice.SetRenderTarget(null);
        }
        private void RenderIcicles()
        {
            MaskCombineShader combineShader = MaskCombineShader.Instance;
            combineShader.MixTexture = _iceRT;

            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_icicleRT);
            graphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, combineShader.Effect);
            spriteBatch.Draw(_icicleMaskRT, Vector2.Zero, Color.White);
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
        }

        private void DrawMaskToPixelTarget(SpriteBatch spriteBatch, Vector2 screenPos)
        {

            spriteBatch.Draw(_icicleRT, Vector2.Zero, Color.White);
        }


        public static void QueueDrawAction(PixelTarget.SpritebatchDrawAction drawAction)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            IceRenderer renderer = ModContent.GetInstance<IceRenderer>();
            renderer._drawActionQueue.Enqueue(drawAction);
        }
    }
}

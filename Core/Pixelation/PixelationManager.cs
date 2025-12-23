using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Pixelation
{
    public static class RenderTargetExtensions
    {
        public static void Release(this RenderTarget2D rt)
        {
            if (rt != null && !rt.IsDisposed)
                rt.Dispose();
        }
    }

    /// <summary>
    /// Handles pixelation effects
    /// </summary>
    public class PixelTarget
    {
        public delegate void PrimitivesDrawAction(GraphicsDevice graphicsDevice);
        public delegate void SpritebatchDrawAction(SpriteBatch spriteBatch, Vector2 screenPos);

        private ManagedRenderTarget _downScaleRenderTarget;
        private ManagedRenderTarget _originalRenderTarget;
        private Queue<SpritebatchDrawAction> _spritebatchActionsQueue;
        private Queue<PrimitivesDrawAction> _primitivesActionsQueue;
        private float _downSamples;
        public PixelTarget(int downSamples = 2)
        {
            _downSamples = downSamples;
            _downScaleRenderTarget = ManagedRenderTarget.New(ManagedRenderTarget.GetScreenTargetSize, downSamples);
            _originalRenderTarget = ManagedRenderTarget.New(ManagedRenderTarget.GetScreenTargetSize);
            _spritebatchActionsQueue = new Queue<SpritebatchDrawAction>(100);
            _primitivesActionsQueue = new Queue<PrimitivesDrawAction>(100);
        }

        public void QueueSpritebatchDrawAction(SpritebatchDrawAction action)
        {
            _spritebatchActionsQueue.Enqueue(action);
        }

        public void QueuePrimitiveDrawAction(PrimitivesDrawAction action)
        {
            _primitivesActionsQueue.Enqueue(action);
        }

        public void Render()
        {
            RenderToOriginalRenderTarget();
            RenderToDownscaledRenderTarget();
        }

        private void RenderToOriginalRenderTarget()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_originalRenderTarget);
            graphicsDevice.Clear(Color.Transparent);

            //Primitives cannot draw within the spritebatch cause they modify the graphics state
            //Which would cause inconsistent results if they drew within the spritebatch
            //To get around this we just have them draw before
            while (_primitivesActionsQueue.Count > 0)
            {
                PrimitivesDrawAction drawAction = _primitivesActionsQueue.Dequeue();
                drawAction(graphicsDevice);
            }

            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);
            while(_spritebatchActionsQueue.Count > 0)
            {
                SpritebatchDrawAction drawAction = _spritebatchActionsQueue.Dequeue();
                drawAction(spriteBatch, Main.screenPosition);
            }
            spriteBatch.End();

        }

        private void RenderToDownscaledRenderTarget()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_downScaleRenderTarget);
            graphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);
            float downScale = 1f / _downSamples;
            spriteBatch.Draw(_originalRenderTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, downScale, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        public void DrawToScreen()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);
            spriteBatch.Draw(_downScaleRenderTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, _downSamples, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }

    /// <summary>
    /// Manages create a pixelation effect for our weapons
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    public class PixelationManager : ModSystem
    {
        private PixelTarget _overNPCsPixelTarget;
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_Main.CheckMonoliths += RenderToPixelationRT;
            On_Main.DoDraw_DrawNPCsOverTiles += DrawOverNPCs;
            _overNPCsPixelTarget = new PixelTarget(downSamples: 2);
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.CheckMonoliths -= RenderToPixelationRT;
            On_Main.DoDraw_DrawNPCsOverTiles -= DrawOverNPCs;
        }

        private void RenderToPixelationRT(On_Main.orig_CheckMonoliths orig)
        {
            orig();
            if (Main.gameMenu)
                return;
            _overNPCsPixelTarget.Render();
        }

       
        private void DrawOverNPCs(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            orig(self);
            if (Main.gameMenu)
                return;

            _overNPCsPixelTarget.DrawToScreen();
        }

        public static void QueueSpritebatchDrawAction(PixelTarget.SpritebatchDrawAction drawAction, DrawLayer drawLayer = DrawLayer.OverNPCs)
        {
            //TODO: have multiple draw layers, for nowe don't need it
            if (Main.netMode == NetmodeID.Server)
                return;
            PixelTarget target = ModContent.GetInstance<PixelationManager>()._overNPCsPixelTarget;
            target.QueueSpritebatchDrawAction(drawAction);
        }

        public static void QueuePrimitivesDrawAction(PixelTarget.PrimitivesDrawAction drawAction, DrawLayer drawLayer = DrawLayer.OverNPCs)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            PixelTarget target = ModContent.GetInstance<PixelationManager>()._overNPCsPixelTarget;
            target.QueuePrimitiveDrawAction(drawAction);
        }
    }

    public enum DrawLayer
    {
        OverNPCs = 0
    }
}

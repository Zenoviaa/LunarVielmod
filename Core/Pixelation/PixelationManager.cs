using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Areas.Illuria.WeaponsIL;
using Stellamod.Core.Shaders;
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
        private BlendState _blendState;
        public PixelTarget(int downSamples = 2, BlendState blendState = null)
        {
            _downSamples = downSamples;
            _downScaleRenderTarget = ManagedRenderTarget.New(ManagedRenderTarget.GetScreenTargetSize, downSamples);
            _originalRenderTarget = ManagedRenderTarget.New(ManagedRenderTarget.GetScreenTargetSize);
            _spritebatchActionsQueue = new Queue<SpritebatchDrawAction>(100);
            _primitivesActionsQueue = new Queue<PrimitivesDrawAction>(100);
            _blendState = blendState == null ? BlendState.AlphaBlend : blendState;
        }

        public Color? outlineColor;
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
            while (_spritebatchActionsQueue.Count > 0)
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
            if (outlineColor.HasValue)
            {
                Vector2 v = Vector2.UnitX * 2;
                Vector2 h = Vector2.UnitY * 2;
                Color oColor = outlineColor.Value;

                SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;
                spriteBatch.Begin(SpriteSortMode.Deferred, _blendState, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, whiteShader.Effect);

                spriteBatch.Draw(_downScaleRenderTarget, Vector2.Zero + v, null, oColor, 0, Vector2.Zero, _downSamples, SpriteEffects.None, 0);
                spriteBatch.Draw(_downScaleRenderTarget, Vector2.Zero - v, null, oColor, 0, Vector2.Zero, _downSamples, SpriteEffects.None, 0);
                spriteBatch.Draw(_downScaleRenderTarget, Vector2.Zero + h, null, oColor, 0, Vector2.Zero, _downSamples, SpriteEffects.None, 0);
                spriteBatch.Draw(_downScaleRenderTarget, Vector2.Zero - h, null, oColor, 0, Vector2.Zero, _downSamples, SpriteEffects.None, 0);

                spriteBatch.End();
            }
            spriteBatch.Begin(SpriteSortMode.Deferred, _blendState, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);

            spriteBatch.Draw(_downScaleRenderTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, _downSamples, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }

    public interface IRenderer
    {
        int Priority { get; }
        void Render();
    }

    public class RendererComparer : IComparer<IRenderer>
    {
        public int Compare(IRenderer x, IRenderer y)
        {
            return x.Priority.CompareTo(y.Priority);
        }
    }
    [Autoload(Side = ModSide.Client)]
    public class RTManager : ModSystem
    {
        private IRenderer[] _renderers;
        private RendererComparer _comparer;
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_Main.CheckMonoliths += ManageCustomRenderTargets;
            List<IRenderer> renderers = new List<IRenderer>();
            foreach(var modSystem in ModContent.GetContent<ModSystem>())
            {
                if(modSystem is IRenderer renderer)
                {
                    renderers.Add(renderer);
                }
            }
            _renderers = renderers.ToArray();
            _comparer = new RendererComparer();
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.CheckMonoliths -= ManageCustomRenderTargets;
        }
        private void ManageCustomRenderTargets(On_Main.orig_CheckMonoliths orig)
        {
            if (!Main.gameMenu)
            {
                //I'm pretty sure order matters here, so we're going to just manually setup an order
                Array.Sort(_renderers, _comparer);
                for(int i = 0; i < _renderers.Length; i++)
                {
                    IRenderer renderer = _renderers[i];

                    renderer.Render();
                }
            }
            orig();
        }
    }

    /// <summary>
    /// Manages create a pixelation effect for our weapons
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    public class PixelationManager : ModSystem,
        IRenderer
    {
        private PixelTarget _overNPCsPixelTarget;
        private PixelTarget _overNPCsPixelTargetAdditive;
        private PixelTarget _overNPCsPixelTargetWithOutline;
        private PixelTarget _behindNPCsPixelTargetWithOutline;

        //This one needs to go last
        public int Priority => 10;

        public override void OnModLoad()
        {
            base.OnModLoad();
            On_Main.DoDraw_DrawNPCsOverTiles += DrawOverNPCs;
            _overNPCsPixelTarget = new PixelTarget(downSamples: 2, BlendState.AlphaBlend);
            _overNPCsPixelTargetWithOutline = new PixelTarget(downSamples: 2, BlendState.AlphaBlend);
            _overNPCsPixelTargetWithOutline.outlineColor = Color.Black;
            _behindNPCsPixelTargetWithOutline = new PixelTarget(downSamples: 2, BlendState.AlphaBlend);
            _behindNPCsPixelTargetWithOutline.outlineColor = Color.Black;
            _overNPCsPixelTargetAdditive = new PixelTarget(downSamples: 2, BlendState.Additive);
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.DoDraw_DrawNPCsOverTiles -= DrawOverNPCs;
        }


        private void DrawOverNPCs(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            if (!Main.gameMenu)
            {
                _behindNPCsPixelTargetWithOutline.DrawToScreen();
            }
                orig(self);
            if (!Main.gameMenu)
            {
                _overNPCsPixelTarget.DrawToScreen();
                _overNPCsPixelTargetWithOutline.DrawToScreen();
                _overNPCsPixelTargetAdditive.DrawToScreen();
            }
         


        }

        private PixelTarget GetPixelTarget(DrawLayer drawLayer)
        {
            switch (drawLayer)
            {
                default:
                    return _overNPCsPixelTarget;
                case DrawLayer.OverNPCsWithOutline:
                    return _overNPCsPixelTargetWithOutline;
                case DrawLayer.OverNPCsAdditive:
                    return _overNPCsPixelTargetAdditive;
                case DrawLayer.BehindNPCsWithOutline:
                    return _behindNPCsPixelTargetWithOutline;
            }
        }
        public static void QueueSpritebatchDrawAction(PixelTarget.SpritebatchDrawAction drawAction, DrawLayer drawLayer = DrawLayer.OverNPCs)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            PixelTarget target = ModContent.GetInstance<PixelationManager>().GetPixelTarget(drawLayer);
            target.QueueSpritebatchDrawAction(drawAction);
        }

        public static void QueuePrimitivesDrawAction(PixelTarget.PrimitivesDrawAction drawAction, DrawLayer drawLayer = DrawLayer.OverNPCs)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            PixelTarget target = ModContent.GetInstance<PixelationManager>().GetPixelTarget(drawLayer);
            target.QueuePrimitiveDrawAction(drawAction);
        }

        public void Render()
        {
            _overNPCsPixelTarget.Render();
            _overNPCsPixelTargetWithOutline.Render();
            _overNPCsPixelTargetAdditive.Render();
            _behindNPCsPixelTargetWithOutline.Render();
        }
    }

    public enum DrawLayer
    {
       
        OverNPCs = 0,
        OverNPCsWithOutline = 1,
        OverNPCsAdditive = 2,
             BehindNPCsWithOutline = 3,
    }
}

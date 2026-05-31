using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Illuria.WeaponsIL;
using Stellamod.Core.Utilities;
using Stellamod.Core.ZTileSystem;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
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

    public class PixelateShader : CrystalShader<PixelateShader>
    {
        private EffectParameter _widthParam;
        private EffectParameter _heightParam;
        public float Width
        {
            set
            {
                _widthParam ??= Effect.Parameters["width"];
                _widthParam.SetValue(value);
            }
        }

        public float Height
        {
            set
            {
                _heightParam ??= Effect.Parameters["height"];   
                _heightParam.SetValue(value);
            }
        }
    }

    /// <summary>
    /// Handles pixelation effects
    /// </summary>
    public class PixelTarget
    {
        public delegate void PrimitivesDrawAction(GraphicsDevice graphicsDevice);
        public delegate void SpritebatchDrawAction(SpriteBatch spriteBatch, Vector2 screenPos);

        private int _renderCount = 0;
        private ManagedRenderTarget _downscaleRenderTarget;
        private ManagedRenderTarget _originalRenderTarget;
        private Queue<SpritebatchDrawAction> _spritebatchActionsQueue;
        private Queue<PrimitivesDrawAction> _primitivesActionsQueue;
        private float _downSamples;
        private BlendState _blendState;
        public PixelTarget(ManagedRenderTarget downScaleRenderTarget, int downSamples = 2, BlendState blendState = null, bool mipMap = false)
        {
            _downSamples = downSamples;
            _downscaleRenderTarget = downScaleRenderTarget;
            _originalRenderTarget = ManagedRenderTarget.New(mipMap: mipMap);
            _spritebatchActionsQueue = new Queue<SpritebatchDrawAction>(100);
            _primitivesActionsQueue = new Queue<PrimitivesDrawAction>(100);
            _blendState = blendState == null ? BlendState.AlphaBlend : blendState;
        }

        public Color? outlineColor;
        public bool HasRenders => (_spritebatchActionsQueue.Count + _primitivesActionsQueue.Count + _renderCount) > 0;
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
            _renderCount = 0;
            if (_primitivesActionsQueue.Count <= 0 && _spritebatchActionsQueue.Count <= 0)
            {
                return;
            }
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_originalRenderTarget);
            graphicsDevice.Clear(Color.Transparent);

            //Primitives cannot draw within the spritebatch cause they modify the graphics state
            //Which would cause inconsistent results if they drew within the spritebatch
            //To get around this we just have them draw before
            while (_primitivesActionsQueue.Count > 0)
            {
                graphicsDevice.RasterizerState = RasterizerState.CullNone;
                PrimitivesDrawAction drawAction = _primitivesActionsQueue.Dequeue();
                drawAction(graphicsDevice);
                _renderCount++;
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            while (_spritebatchActionsQueue.Count > 0)
            {
                SpritebatchDrawAction drawAction = _spritebatchActionsQueue.Dequeue();
                drawAction(spriteBatch, Main.screenPosition);
                _renderCount++;
            }
            spriteBatch.End();

            //Draw to the downscaled render target
            graphicsDevice.SetRenderTarget(_downscaleRenderTarget);
            graphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);
            float downScale = 1f / _downSamples;
            spriteBatch.Draw(_originalRenderTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, downScale, SpriteEffects.None, 0);
            spriteBatch.End();

            //Draw back to the original render target
            graphicsDevice.SetRenderTarget(_originalRenderTarget);
            graphicsDevice.Clear(Color.Transparent);

            if (outlineColor.HasValue)
            {
                Vector2 v = Vector2.UnitX * 2;
                Vector2 h = Vector2.UnitY * 2;
                Color oColor = outlineColor.Value;

                SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;
                spriteBatch.Begin(SpriteSortMode.Deferred, _blendState, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, whiteShader.Effect);

                spriteBatch.Draw(_downscaleRenderTarget, Vector2.Zero + v, null, oColor, 0, Vector2.Zero, _downSamples, SpriteEffects.None, 0);
                spriteBatch.Draw(_downscaleRenderTarget, Vector2.Zero - v, null, oColor, 0, Vector2.Zero, _downSamples, SpriteEffects.None, 0);
                spriteBatch.Draw(_downscaleRenderTarget, Vector2.Zero + h, null, oColor, 0, Vector2.Zero, _downSamples, SpriteEffects.None, 0);
                spriteBatch.Draw(_downscaleRenderTarget, Vector2.Zero - h, null, oColor, 0, Vector2.Zero, _downSamples, SpriteEffects.None, 0);

                spriteBatch.End();
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, _blendState, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);
            spriteBatch.Draw(_downscaleRenderTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, _downSamples, SpriteEffects.None, 0);
            spriteBatch.End();
        }


        public void DrawToScreen()
        {
            if (_renderCount <= 0)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, _blendState, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);
            spriteBatch.Draw(_originalRenderTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
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
        public override void Unload()
        {
            base.Unload();
            _renderers = null;
            _comparer = null;
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

    public interface IDrawToRenderTarget
    {
        void DrawToRenderTargets();
    }


    [Autoload(Side = ModSide.Client)]
    public class PrepareRenderTargetDrawsSystem : ModSystem
    {
        public static event Action OnRenderTargetDrawsReady;
        public override void Load()
        {
            base.Load();
            On_Main.CheckMonoliths += CheckRenderTargetDraws;
        }

        private void CheckRenderTargetDraws(On_Main.orig_CheckMonoliths orig)
        {
            if (!Main.gameMenu)
            {
                foreach (var proj in Main.ActiveProjectiles)
                {
                    if (proj.ModProjectile is IDrawToRenderTarget drawToRenderTarget)
                        drawToRenderTarget.DrawToRenderTargets();
                }

                foreach (var npc in Main.ActiveNPCs)
                {
                    if (npc.ModNPC is IDrawToRenderTarget drawToRenderTarget)
                        drawToRenderTarget.DrawToRenderTargets();
                }
                OnRenderTargetDrawsReady?.Invoke();
            }

            orig();
        }
    }
    /// <summary>
    /// Manages create a pixelation effect for our weapons
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    public class PixelationManager : ModSystem
    {
        private ManagedRenderTarget _downscaledTarget;

        private PixelTarget _overNPCsPixelTarget;
        private PixelTarget _overNPCsPixelTargetAdditive;
        private PixelTarget _overNPCsPixelTargetWithOutline;
        private PixelTarget _behindNPCsPixelTargetWithOutline;
        private PixelTarget _frontGrassPixelTarget;
        private PixelTarget _backGrassPixelTarget;
        private PixelTarget _overPlayersPixelTarget;
        private PixelTarget _behindTilesPixelTarget;
        private PixelTarget _behindTilesOutlinePixelTarget;
        private PixelTarget _behindNPCsPixelTarget;
        //This one needs to go last
        public int Priority => 10;
        public static event Action OnBehindGrass;
        public static event Action OnInFrontGrass;
        public static event Action OnPreRender;
        public override void Load()
        {
            base.Load();
            PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady += Render;
            On_FilterManager.EndCapture += RenderToPixelRTs;
            On_Main.DoDraw_Tiles_NonSolid += RenderBehindTiles2;
            On_Main.DoDraw_DrawNPCsBehindTiles += RenderBehindTiles;
            On_Main.DoDraw_DrawNPCsOverTiles += DrawOverNPCs;
            On_Main.DrawPlayers_AfterProjectiles += RenderOverPlayers;
            //On_Main.DrawCachedProjs += RenderLater;
            ZTileMap.OnRenderForeground += RenderLater;
        }

        private void RenderLater()
        {
            if (!Main.gameMenu)
            {
                _overPlayersPixelTarget.DrawToScreen();
            }

            //   throw new NotImplementedException();
        }

        private void RenderLater(On_Main.orig_DrawCachedProjs orig, Main self, List<int> projCache, bool startSpriteBatch)
        {
        
            orig(self, projCache, startSpriteBatch);
        }

        private void RenderToPixelRTs(On_FilterManager.orig_EndCapture orig,
            FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, 
            Color clearColor)
        {
            /*
            if (!Main.gameMenu)
            {
          
                Render();
            }*/
             orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);

        }

        public override void OnModLoad()
        {
            base.OnModLoad();
            _downscaledTarget = ManagedRenderTarget.New(ManagedRenderTarget.GetHalfScreenTargetSize);
            _overNPCsPixelTarget = new PixelTarget(_downscaledTarget, downSamples: 2, BlendState.AlphaBlend);
            _overNPCsPixelTargetWithOutline = new PixelTarget(_downscaledTarget, downSamples: 2, BlendState.AlphaBlend);
            _overNPCsPixelTargetWithOutline.outlineColor = Color.Black;

            _behindNPCsPixelTargetWithOutline = new PixelTarget(_downscaledTarget, downSamples: 2, BlendState.AlphaBlend);
            _behindNPCsPixelTargetWithOutline.outlineColor = Color.Black;

            _overNPCsPixelTargetAdditive = new PixelTarget(_downscaledTarget, downSamples: 2, BlendState.Additive);

            _frontGrassPixelTarget = new PixelTarget(_downscaledTarget, downSamples: 2, BlendState.AlphaBlend, true);
            _frontGrassPixelTarget.outlineColor = Color.Lerp(Color.Goldenrod, Color.Black, 0.7f);

            _backGrassPixelTarget = new PixelTarget(_downscaledTarget, downSamples: 2, BlendState.AlphaBlend, true);
            _backGrassPixelTarget.outlineColor = Color.Lerp(Color.Goldenrod, Color.Black, 0.7f);

            _overPlayersPixelTarget = new PixelTarget(_downscaledTarget, downSamples: 2, BlendState.AlphaBlend);


            _behindTilesPixelTarget = new PixelTarget(_downscaledTarget, downSamples: 2, BlendState.AlphaBlend);
            _behindTilesOutlinePixelTarget = new PixelTarget(_downscaledTarget, downSamples: 2, BlendState.AlphaBlend);
        }
        public override void Unload()
        {
            base.Unload();
            ZTileMap.OnRenderForeground -= RenderLater;
            _behindNPCsPixelTarget = null;
            _overNPCsPixelTarget = null;
            _overNPCsPixelTargetWithOutline = null;
            _behindNPCsPixelTargetWithOutline = null;
            _overNPCsPixelTargetAdditive = null;
            _frontGrassPixelTarget = null;
            _backGrassPixelTarget = null;
            _overPlayersPixelTarget = null;
            _behindTilesPixelTarget = null;
            _behindTilesOutlinePixelTarget = null;
        }

        private void RenderBehindTiles2(On_Main.orig_DoDraw_Tiles_NonSolid orig, Main self)
        {
            if (_behindTilesPixelTarget.HasRenders)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                spriteBatch.End();
                _behindTilesPixelTarget.DrawToScreen();
                _behindTilesOutlinePixelTarget.DrawToScreen();
                OnBehindGrass?.Invoke();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }

            orig(self);
        }

        private void RenderBehindTiles(On_Main.orig_DoDraw_DrawNPCsBehindTiles orig, Main self)
        {

            _backGrassPixelTarget.DrawToScreen();
            orig(self);
        }

        private void RenderOverPlayers(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            orig(self);
            if (!Main.gameMenu)
            {
                _frontGrassPixelTarget.DrawToScreen();
                OnInFrontGrass?.Invoke();
        
            }
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
                case DrawLayer.FrontGrassTarget:
                    return _frontGrassPixelTarget;
                case DrawLayer.BackGrassTarget:
                    return _backGrassPixelTarget;
                case DrawLayer.OverPlayers:
                    return _overPlayersPixelTarget;
                case DrawLayer.BehindTiles:
                    return _behindTilesPixelTarget;
                case DrawLayer.BehindTilesOutline:
                    return _behindTilesOutlinePixelTarget;
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
            Color skyColor = Main.ColorOfTheSkies;
            Color grassColor = Color.DarkGreen.MultiplyRGB(skyColor);
            _frontGrassPixelTarget.outlineColor = Color.Lerp(grassColor, Color.Black, 0.7f);
            _backGrassPixelTarget.outlineColor = Color.Lerp(grassColor, Color.Black, 0.7f);
  
            _overNPCsPixelTarget.Render();
            _overNPCsPixelTargetWithOutline.Render();
            _overNPCsPixelTargetAdditive.Render();
            _behindNPCsPixelTargetWithOutline.Render();
            _frontGrassPixelTarget.Render();
            _backGrassPixelTarget.Render();
            _overPlayersPixelTarget.Render();
            _behindTilesPixelTarget.Render();

            _behindTilesOutlinePixelTarget.outlineColor = Color.Black;
            _behindTilesOutlinePixelTarget.Render(); 
        }
    }

    public enum DrawLayer
    {
       
        OverNPCs = 0,
        OverNPCsWithOutline = 1,
        OverNPCsAdditive = 2,
             BehindNPCsWithOutline = 3,
             FrontGrassTarget=4,
             BackGrassTarget=5,
             OverPlayers=6,
             BehindTiles,
             BehindTilesOutline
    }
}

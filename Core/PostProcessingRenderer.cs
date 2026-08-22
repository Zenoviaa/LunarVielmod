using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.LunarLightingSystem;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Core
{
    public interface IPostProcessingPass
    {
        int PostProcessPriority { get; }
        void RenderToScreen();
    }

    [Autoload(Side = ModSide.Client)]
    public class PostProcessingRenderer : ModSystem
    {
        private ProcessingComparer _processingComparer;
        private static List<IPostProcessingPass> _passes;
        public override void Load()
        {
            base.Load();
      
            _processingComparer = new ProcessingComparer();
            _passes = new List<IPostProcessingPass>();
            On_OverlayManager.Draw += DrawPostProcessingPasses;
        }

        public override void Unload()
        {
            base.Unload();
            _passes?.Clear();
            _passes = null;
            On_OverlayManager.Draw -= DrawPostProcessingPasses;
        }
        public static void AddPass(IPostProcessingPass pass)
        {
            _passes.Add(pass);
        }
        private void DrawPostProcessingPasses(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
        {
            if (layer == RenderLayers.All && beginSpriteBatch && !Main.gameMenu && LightingHelper.CanRenderPostProcessingEffects)
            {
                RenderPostProcessing();
            }
            orig(self, spriteBatch, layer, beginSpriteBatch);
        }

        private void RenderPostProcessing()
        {
            _passes.Sort(_processingComparer);
            for (int i = 0; i < _passes.Count; i++)
            {
                IPostProcessingPass pass = _passes[i];
                pass.RenderToScreen();
            }
        }
    }

    public class ProcessingComparer : IComparer<IPostProcessingPass>
    {
        public int Compare(IPostProcessingPass x, IPostProcessingPass y)
        {
            return x.PostProcessPriority.CompareTo(y.PostProcessPriority);
        }
    }
}

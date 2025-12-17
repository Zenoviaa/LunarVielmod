using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem
{
    public sealed class SpriteBatchOverride : ModSystem
    {
        private static float _drawCount;

        public override void OnModLoad()
        {
            base.OnModLoad();
            Patch();
            On_Main.DoDraw += PDraw;
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.DoDraw-= PDraw;
        }

        private void PDraw(On_Main.orig_DoDraw orig, Main self, GameTime gameTime)
        {
            _drawCount = 0;
            orig(self, gameTime);
            //Console.WriteLine(_drawCount);
        }

        public override void PreUpdateTime()
        {
            base.PreUpdateTime();
           
        }

        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
         
        }
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            base.PostDrawInterface(spriteBatch);
    
        }
        // TODO: Should this rather be in class constructor?
        internal static void Patch()
        {
            /*
             * 		public void Begin(
			            SpriteSortMode sortMode,
			            BlendState blendState,
			            SamplerState samplerState,
			            DepthStencilState depthStencilState,
			            RasterizerState rasterizerState,
			            Effect effect,
			            Matrix transformMatrix
             */
            Type[] parameters = new Type[]
            {
                typeof(SpriteSortMode),
                typeof(BlendState),
                typeof(SamplerState),
                typeof(DepthStencilState),
                typeof(RasterizerState),
                typeof(Effect),
                typeof(Matrix)
            };
            var beginMethodInfo = typeof(SpriteBatch).GetRuntimeMethod(nameof(SpriteBatch.Begin), parameters);

            Debug.Assert(beginMethodInfo != null);
   
            MonoModHooks.Add(beginMethodInfo, (Action<SpriteBatch, SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Matrix> orig,
                SpriteBatch self, SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix matrix) => {
                    _drawCount++;
                
                    orig(self, sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, matrix);
            });
        }
    }
}

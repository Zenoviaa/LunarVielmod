using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Skies;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace Stellamod.Helpers.Separate
{
    public static class StellasEffectsRegistry
    {
        #region Texture Shaders
        public static Ref<Effect> FluidSimulatorShader
        {
            get;
            internal set;
        }

       // public static MiscShaderData UnderwaterRayShader => GameShaders.Misc["Stellamod:UnderwaterRays"];
             #endregion

        #region Screen Shaders
        public static Filter BloomShader => Filters.Scene["Stellamod:Bloom"];
    
        #endregion

        #region Methods
        public static void LoadEffects()
        {
            var assets = Stellamod.Instance.Assets;

          
            LoadScreenShaders(assets);

         
        }



        public static void LoadScreenShaders(AssetRepository assets)
        {
         

            // Flower of the ocean sky.
            Filters.Scene["Stellamod:GovheilSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.1f, 0.2f, 0.5f).UseOpacity(0.53f), EffectPriority.High);
            SkyManager.Instance["Stellamod:GovheilSky"] = new GovheilSky();

            // Fireball shader.
     

          

           
        }
        #endregion
    }
}
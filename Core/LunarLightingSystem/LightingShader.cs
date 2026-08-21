using Stellamod.Common.Shaders;

namespace Stellamod.Core.LunarLightingSystem
{
    public class LightingShader : CrystalShader<LightingShader>
    {
        private EffectParameter _transformMatrixParam;
        private EffectParameter _shadowMapTexture;
        private EffectParameter _levelsParam;

        public float Levels
        {
            set
            {
                _levelsParam = Effect.Parameters["levels"];
                _levelsParam.SetValue(value);
            }
        }
        
        public float ShadowAlpha
        {
            set
            {
                Effect.Parameters["shadowAlpha"].SetValue(value);
            }
        }

        public Matrix TransformMatrix
        {
            set
            {
                _transformMatrixParam = Effect.Parameters["transformMatrix"];
                _transformMatrixParam.SetValue(value);
            }
        }
        public Texture2D ShadowMap
        {
            set
            {
                _shadowMapTexture = Effect.Parameters["shadowMap"];
                _shadowMapTexture.SetValue(value);
            }
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            TransformMatrix = TrailDrawer.WorldViewPoint2;
            Levels = 400;
        }
    }
}

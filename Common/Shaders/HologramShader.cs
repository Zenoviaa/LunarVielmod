namespace Stellamod.Common.Shaders
{
    public class HologramShader : BaseShader
    {
        private EffectParameter _noiseTextureSizeParam;
        private EffectParameter _noiseTextureParam;
        private EffectParameter _parallaxParam;
        private EffectParameter _timeParam;
        public Texture2D NoiseTexture
        {
            set
            {
                _noiseTextureParam ??= Effect.Parameters["noiseTexture"];
                _noiseTextureParam.SetValue(value);
            }
        }
        public Vector2 NoiseTextureSize
        {
            set
            {
                _noiseTextureSizeParam ??= Effect.Parameters["noiseTextureSize"];
                _noiseTextureSizeParam.SetValue(value);
            }
        }

        public Vector2 Parallax
        {
            set
            {
                _parallaxParam ??= Effect.Parameters["uImageOffset"];
                _parallaxParam.SetValue(value);
            }
        }

        public Vector2 Time
        {
            set
            {
                _timeParam ??= Effect.Parameters["time"];
                _timeParam.SetValue(value);
            }
        }
        private static HologramShader _instance;
        public static HologramShader Instance
        {
            get
            {
                _instance ??= new HologramShader();
                _instance.SetDefaults();
                return _instance;
            }
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Parallax = Vector2.Zero;
            Time = Vector2.Zero;
        }
      
    }
}

namespace Stellamod.Common.Shaders
{
    public class ScrollingShader : CrystalShader<ScrollingShader>
    {
        private EffectParameter _offsetParam;
        private EffectParameter _tilingParam;
        public Vector2 Offset
        {
            set
            {
                _offsetParam = Effect.Parameters["offset"];
                _offsetParam.SetValue(value);
            }
        }
        public Vector2 Tiling
        {
            set
            {
                _tilingParam = Effect.Parameters["tiling"];
                _tilingParam.SetValue(value);
            }
        }
    }
}

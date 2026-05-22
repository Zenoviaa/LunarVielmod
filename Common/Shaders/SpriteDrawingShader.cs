using ReLogic.Content;

namespace Stellamod.Common.Shaders
{
    public class SpriteDrawingShader : CrystalShader<SpriteDrawingShader>
    {
        private EffectParameter _textureParam;
        public Asset<Texture2D> SpriteTexture
        {
            set
            {
                _textureParam ??= Effect.Parameters["spriteTexture"];
                _textureParam.SetValue(value.Value);
            }
        }
    }
}

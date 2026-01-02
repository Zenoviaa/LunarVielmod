using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace Stellamod.Common.Shaders
{
    public class MagicCircleShader : CrystalShader<MagicCircleShader>
    {
        private EffectParameter _tilingOffsetParam;
        private EffectParameter _textureParam;
        public Vector4 TilingOffset
        {
            set
            {
                _tilingOffsetParam ??= Effect.Parameters["tilingOffset"];
                _tilingOffsetParam.SetValue(value);
            }
        }

        public Asset<Texture2D> RingTexture
        {
            set
            {
                _textureParam ??= Effect.Parameters["spriteTexture"];
                _textureParam.SetValue(value.Value);
            }
        }
    }
}

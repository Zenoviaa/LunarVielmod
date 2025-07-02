using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;

namespace Stellamod.Core.Effects
{
    internal abstract class Shader
    {
        public Shader()
        {

        }

        public string EffectPath => GetType().Name;
        public MiscShaderData Data => GameShaders.Misc[$"{Stellamod.Instance.Name}:{EffectPath}"];
        public Effect Effect => Data.Shader;
        public BlendState BlendState { get; set; } = BlendState.AlphaBlend;
        public SamplerState SamplerState { get; set; } = SamplerState.LinearWrap;

        public virtual void ApplyToEffect()
        {

        }
    }
}

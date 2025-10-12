using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;

namespace Stellamod.Core.Effects
{
    public interface IShader
    {
        void ModifyGraphicsDevice(GraphicsDevice device);
        void ApplyPasses();
        void ApplyToEffect();
        void SetLightColor(Color lightColor);
    }
    public abstract class Shader : IShader
    {
        public Shader()
        {
            LightColor = Color.White;
        }

        public string EffectPath => GetType().Name;
        public MiscShaderData Data => GameShaders.Misc[$"{Stellamod.Instance.Name}:{EffectPath}"];
        public Effect Effect => Data.Shader;
        public BlendState BlendState { get; set; } = BlendState.Additive;
        public SamplerState SamplerState { get; set; } = SamplerState.LinearWrap;

        public Color LightColor { get; set; }
        public void SetLightColor(Color lightColor)
        {
            LightColor = lightColor;
        }
        public void ModifyGraphicsDevice(GraphicsDevice device)
        {
            device.BlendState = BlendState;
            device.SamplerStates[0] = SamplerState;
        }

        public void ApplyPasses()
        {
            foreach (var pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }
        }

        public virtual void ApplyToEffect()
        {

        }


    }
}

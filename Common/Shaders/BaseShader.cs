using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Effects;
using Terraria.Graphics.Shaders;

namespace Stellamod.Common.Shaders
{
    public abstract class CrystalShader<T> : BaseShader where T : BaseShader, new()
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }
    }

    public abstract class BaseShader : IShader
    {
        public virtual string EffectPath
        {
            get
            {
                return GetType().Name.Replace("Shader", "");
            }
        }


        public int Type { get; set; }
        public MiscShaderData Data => GameShaders.Misc[$"LunarVeil:{EffectPath}"];
        public Effect Effect => Data.Shader;
        public BlendState BlendState { get; set; } = BlendState.Additive;
        public SamplerState SamplerState { get; set; } = SamplerState.LinearWrap;
        public bool FillShape { get; set; }
        public virtual void Apply() { OnApply(); }
        protected virtual void OnApply()
        {

        }

        public virtual void SetDefaults()
        {

        }

        public void ModifyGraphicsDevice(GraphicsDevice device)
        {
            device.BlendState = BlendState;
            device.SamplerStates[0] = SamplerState;
        }

        public void ApplyPasses()
        {
            var transformMatrix = Effect.Parameters["transformMatrix"];
            if (transformMatrix != null)
            {
                transformMatrix.SetValue(TrailDrawer.WorldViewPoint2);
            }
            foreach (var pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }
        }

        public void ApplyPassesFromEffect()
        {
            foreach (var pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }
        }

        public void ApplyToEffect()
        {
        
        }

        public void SetLightColor(Color lightColor)
        {

        }
    }
}

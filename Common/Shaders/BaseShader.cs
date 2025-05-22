using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Common.Shaders
{
    public abstract class BaseShader 
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
    }
}

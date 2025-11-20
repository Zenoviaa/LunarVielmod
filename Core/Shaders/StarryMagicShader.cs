using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Shaders
{
    public class StarryMagicShader : BaseShader
    {

        private EffectParameter _matrixParam;
        private EffectParameter _timeParam;
        private EffectParameter _glowColorParam;
        private EffectParameter _glowColor2Param;
        private EffectParameter _tilingParam;
        private EffectParameter _starryTextureParam;
        private static StarryMagicShader _instance;
        public static StarryMagicShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public Matrix TransformMatrix
        {
            set
            {
                _matrixParam ??= Effect.Parameters["transformMatrix"];
                _matrixParam.SetValue(value);
            }
        }

        public Asset<Texture2D> StarryTexture
        {
            set
            {
                _starryTextureParam ??= Effect.Parameters["starryTexture"];
                _starryTextureParam.SetValue(value.Value);
            }
        }
        public Color GlowColor
        {
            set
            {
                _glowColorParam ??= Effect.Parameters["glowColor2"];
                _glowColorParam.SetValue(value.ToVector3());
            }
        }
        public Color GlowColor2
        {
            set
            {
                _glowColor2Param ??= Effect.Parameters["glowColor"];
                _glowColor2Param.SetValue(value.ToVector3());
            }
        }
        public Vector2 Tiling
        {
            set
            {
                _tilingParam ??= Effect.Parameters["tiling"];
                _tilingParam.SetValue(value);
            }
        }



        public float Time
        {
            set
            {
                _timeParam ??= Effect.Parameters["time"];
                _timeParam.SetValue(value);
            }
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            StarryTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/StarryMagic");
            TransformMatrix = TrailDrawer.WorldViewPoint2;
            GlowColor = Color.Violet;
            BlendState = BlendState.Additive;
            Time = Main.GlobalTimeWrappedHourly * 12;
            Tiling = Vector2.One;
        }
    }
}

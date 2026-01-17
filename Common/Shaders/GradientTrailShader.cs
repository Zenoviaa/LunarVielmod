using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Trails;
using Terraria;

namespace Stellamod.Common.Shaders
{
    public class GradientTrailShader : BaseShader
    {
        private static GradientTrailShader _instance;
        public static GradientTrailShader Instance
        {
            get
            {
                _instance ??= new GradientTrailShader();
                _instance.SetDefaults();
                return _instance;
            }
        }
        private EffectParameter _tilingParam;
        private EffectParameter _matrixParam;
        private EffectParameter _gradientTextureParam;
        private EffectParameter _laserTextureParam;
        private EffectParameter _timeParam;
        public Matrix TransformMatrix
        {
            set
            {
                _matrixParam ??= Effect.Parameters["transformMatrix"];
                _matrixParam.SetValue(value);
            }
        }

        public Texture2D GradientTexture
        {
            set
            {
                _gradientTextureParam ??= Effect.Parameters["gradientTexture"];
                _gradientTextureParam.SetValue(value);
            }
        }

        public Asset<Texture2D> LaserTexture
        {
            set
            {
                _laserTextureParam ??= Effect.Parameters["laserTexture"];
                _laserTextureParam.SetValue(value.Value);
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


        public Vector2 Tiling
        {
            set
            {
                _tilingParam ??= Effect.Parameters["tiling"];
                _tilingParam.SetValue(value);
            }
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            TransformMatrix = TrailDrawer.WorldViewPoint2;
            //InnerColor = Color.Yellow;
            //  OuterColor = Color.Red;

            LaserTexture = TrailRegistry.BeamTrail;
            Time = Main.GlobalTimeWrappedHourly * 12;
            Tiling = Vector2.One;
        }
    }
}

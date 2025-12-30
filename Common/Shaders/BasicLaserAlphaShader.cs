using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Trails;
using Terraria;

namespace Stellamod.Common.Shaders
{
    public class BasicLaserAlphaShader : BaseShader
    {
        private EffectParameter _tilingParam;
        private EffectParameter _matrixParam;
        private EffectParameter _laserTextureParam;
        private EffectParameter _timeParam;
        private EffectParameter _innerColorParam;
        private EffectParameter _outerColorParam;

        private static BasicLaserAlphaShader _instance;
        public static BasicLaserAlphaShader Instance
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

        public Asset<Texture2D> LaserTexture
        {
            set
            {
                _laserTextureParam ??= Effect.Parameters["laserTexture"];
                _laserTextureParam.SetValue(value.Value);
            }
        }


        public Color InnerColor
        {
            set
            {
                _innerColorParam ??= Effect.Parameters["innerColor"];
                _innerColorParam.SetValue(value.ToVector3());
            }
        }

        public Color OuterColor
        {
            set
            {
                _outerColorParam ??= Effect.Parameters["outerColor"];
                _outerColorParam.SetValue(value.ToVector3());
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
            Time = Main.GlobalTimeWrappedHourly * 24;
            Tiling = Vector2.One;
        }
    }
}

using ReLogic.Content;
using Stellamod.Assets;
using Terraria;

namespace Stellamod.Common.Shaders
{
    public class HairShader : CrystalShader<HairShader>
    {
        private EffectParameter _tilingParam;
        private EffectParameter _matrixParam;
        private EffectParameter _laserTextureParam;
        private EffectParameter _timeParam;
        private EffectParameter _waveFrequencyParam;
        private EffectParameter _waveAmplitudeParam;
        private EffectParameter _xOffsetParam;

        public float WaveFrequency
        {
            set
            {
                _waveFrequencyParam ??= Effect.Parameters["waveFrequency"];
                _waveFrequencyParam.SetValue(value);
            }
        }
        public float WaveAmplitude
        {
            set
            {
                _waveAmplitudeParam ??= Effect.Parameters["waveAmplitude"];
                _waveAmplitudeParam.SetValue(value);
            }
        }
        public float XOffset
        {
            set
            {
                _xOffsetParam ??= Effect.Parameters["xOffset"];
                _xOffsetParam.SetValue(value);
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
            WaveFrequency = 1f;
            WaveAmplitude = 0.2f;
            XOffset = 1f;

            LaserTexture = TrailRegistry.BeamTrail;
            Time = Main.GlobalTimeWrappedHourly * 24;
            Tiling = Vector2.One;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Common.Shaders
{
    internal class ThunderCloudShader : BaseShader
    {
        private static ThunderCloudShader _instance;
        public static ThunderCloudShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public Asset<Texture2D> CloudTexture { get; set; }
        public Asset<Texture2D> NoiseTexture { get; set; }
        public Vector2 SourceSize { get; set; }
        public Color CloudColor { get; set; }
        public float Speed { get; set; }
        public float Pixelation { get; set; }
        public float Distortion { get; set; }
        public override void SetDefaults()
        {
            base.SetDefaults();

            CloudTexture = TrailRegistry.Clouds3;
            NoiseTexture = TrailRegistry.CrystalNoise;
            SourceSize = CloudTexture.Value.Size();

            CloudColor = Color.Lerp(Color.DarkGray,  Color.DarkGoldenrod, 0.5f);
            Speed = 5;
            Pixelation = 0.01f;
            Distortion = 0.0015f;
        }

        public override void Apply()
        {
            Effect.Parameters["noiseTexture"].SetValue(NoiseTexture.Value);
            Effect.Parameters["distortion"].SetValue(Distortion);
            Effect.Parameters["pixelation"].SetValue(Pixelation);
            Effect.Parameters["sourceSize"].SetValue(SourceSize);
            Effect.Parameters["cloudTexSize"].SetValue(CloudTexture.Value.Size());
            Effect.Parameters["cloudTexture"].SetValue(CloudTexture.Value);
            Effect.Parameters["cloudColor"].SetValue(CloudColor.ToVector3());
            Effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * Speed);
        }
    }
}

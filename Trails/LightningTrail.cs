using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Stellamod.Trails
{
    internal class CommonLightning
    {
        public CommonLightning()
        {
            Trails = new LightningTrail[4];
            for(int t = 0; t < Trails.Length; t++)
            {
                Trails[t] = new LightningTrail();
            }

            ColorFunction = DefaultColorFunction;
            WidthTrailFunction = DefaultWidthFunction;
            WidthMultiplier = 1f;
        }

        public float WidthMultiplier { get; set; }
        public float Width { get; set; }
        public PrimDrawer.ColorTrailFunction ColorFunction { get; set; }
        public PrimDrawer.WidthTrailFunction WidthTrailFunction { get; set; }
        public LightningTrail[] Trails { get; init; }
        public bool SyncOffsets { get; set; }
        public float DefaultWidthFunction(float completionRatio)
        {
            float progress = completionRatio / 0.3f;
            float rounded = Easing.SpikeOutCirc(progress);
            float spikeProgress = Easing.SpikeOutExpo(completionRatio);
            float fireball = MathHelper.Lerp(rounded, spikeProgress, Easing.OutExpo(1.0f - completionRatio));
            float midWidth = 6 * Width * WidthMultiplier;
            return MathHelper.Lerp(0, midWidth, fireball);
        }

        public void SetBoltDefaults()
        {
            for (int i = 0; i < Trails.Length; i++)
            {
                float progress = (float)i / (float)Trails.Length;
                var trail = Trails[i];
                trail.LightningRandomOffsetRange = MathHelper.Lerp(8, 2, progress);
                trail.LightningRandomExpand = MathHelper.Lerp(16, 4, progress);
                trail.PrimaryColor = Color.Lerp(Color.White, Color.Yellow, progress);
                trail.NoiseColor = Color.Lerp(Color.White, Color.Yellow, progress);
            }
        }

        public Color DefaultColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.Yellow, p);
            return trailColor;
        }

        public void RandomPositions(Vector2[] oldPos)
        {
            if (SyncOffsets)
            {
                Trails[0].RandomPositions(oldPos);
                for(int t = 1; t < Trails.Length; t++)
                {
                    Trails[t].ClonePositions(Trails[0]);
                }
            }
            else
            {
                for (int t = 0; t < Trails.Length; t++)
                {
                    Trails[t].RandomPositions(oldPos);
                }
            }
  
        }

        public void Draw(SpriteBatch spriteBatch, 
            Vector2[] oldPos, 
            float[] oldRot, Vector2? offset = null)
        {
            var prevBelndState = Main.graphics.GraphicsDevice.BlendState;
            Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;

            Width = 1;
            for (int t = 0; t < Trails.Length; t++)
            {
                Trails[t].Draw(spriteBatch, oldPos, oldRot, ColorFunction, WidthTrailFunction, offset);
                Width -= 0.1f;
            }
            Main.graphics.GraphicsDevice.BlendState = prevBelndState;
        }
        public void DrawAlpha(SpriteBatch spriteBatch,
            Vector2[] oldPos,
            float[] oldRot, Vector2? offset = null)
        {
            Width = 1;

            var prevBelndState = Main.graphics.GraphicsDevice.BlendState;
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            for (int t = 0; t < Trails.Length; t++)
            {
                Trails[t].Draw(spriteBatch, oldPos, oldRot, ColorFunction, WidthTrailFunction, offset);
                Width -= 0.1f;
            }
            Main.graphics.GraphicsDevice.BlendState = prevBelndState;
        }
    }

    internal class LightningTrail
    {
        private Vector2[] _offsets;
        public LightningTrail()
        {
            PrimaryTexture = TrailRegistry.LightningTrail2;
            NoiseTexture = TrailRegistry.LightningTrail3;
            PrimaryColor = Color.Yellow;
            NoiseColor = Color.DarkGoldenrod;
            Speed = 5;
            Distortion = 0.2f;
            Power = 1.5f;
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float LightningRandomOffsetRange = 5;
        public float LightningRandomExpand = 24;
        public Color PrimaryColor;
        public Color NoiseColor;
        public Asset<Texture2D> PrimaryTexture;
        public Asset<Texture2D> NoiseTexture;
        public float Speed;
        public float Distortion;
        public float Power; 
        public void ClonePositions(LightningTrail other)
        {
            _offsets = other._offsets;
        }

        public void ClonePositions(Vector2[] other)
        {
            _offsets = other;
        }

        public void RandomPositions(Vector2[] oldPos)
        {
            Vector2[] offsets = new Vector2[oldPos.Length];
            offsets[0] = Vector2.Zero;
            for (int i = 1; i < offsets.Length - 1; i++)
            {

                Vector2 prevPosition = oldPos[i - 1];
                Vector2 nextPosition = oldPos[i + 1];
                if (prevPosition == Vector2.Zero || nextPosition == Vector2.Zero)
                {
                    offsets[i] = Vector2.Zero;
                    continue;
                }

                Vector2 normalDir = (prevPosition - nextPosition).SafeNormalize(Vector2.One).RotatedBy(MathHelper.PiOver2);
                float length = Main.rand.NextFromList(-1, 1) * Main.rand.NextFloat(LightningRandomOffsetRange, LightningRandomOffsetRange);
                offsets[i] = normalDir * length + Main.rand.NextVector2Circular(LightningRandomExpand, LightningRandomExpand);
            }
            offsets[offsets.Length - 1] = Vector2.Zero;
            _offsets = offsets;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2[] oldPos, float[] oldRot,
            PrimDrawer.ColorTrailFunction colorFunc,
            PrimDrawer.WidthTrailFunction widthFunc, Vector2? offset = null)
        {
            if (_offsets == null)
                return;
            Vector2[] lightningTrailPos = new Vector2[oldPos.Length];
            for (int i = 0; i < lightningTrailPos.Length && i < _offsets.Length; i++)
            {
                lightningTrailPos[i] = oldPos[i] + _offsets[i];
            }

            MiscShaderData shaderData = GameShaders.Misc["LunarVeil:LightningBolt"];
            shaderData.Shader.Parameters["primaryColor"].SetValue(PrimaryColor.ToVector3());
            shaderData.Shader.Parameters["noiseColor"].SetValue(NoiseColor.ToVector3());
            shaderData.Shader.Parameters["primaryTexture"].SetValue(PrimaryTexture.Value);
            shaderData.Shader.Parameters["noiseTexture"].SetValue(NoiseTexture.Value);
            shaderData.Shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * Speed);
            shaderData.Shader.Parameters["distortion"].SetValue(Distortion);
            shaderData.Shader.Parameters["power"].SetValue(Power);

            TrailDrawer ??= new PrimDrawer(widthFunc, colorFunc, shaderData);
            TrailDrawer.WidthFunc = widthFunc;
            TrailDrawer.ColorFunc = colorFunc;

            Vector2 trailOffset = -Main.screenPosition;

            TrailDrawer.DrawPrims(lightningTrailPos, trailOffset, 155);
        }
    }
}

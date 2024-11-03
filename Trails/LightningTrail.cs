using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Stellamod.Trails
{
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

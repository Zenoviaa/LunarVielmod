using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL
{
    public class Icicle
    {
        private readonly int _seed;
        private float _yScale;
        private FastNoiseLite _fastNoise;
        private Asset<Texture2D> _icicleCircleTextureAsset;
        public Icicle(int steps, int seed = -1)
        {
            _icicleCircleTextureAsset = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/IceCrystal");
            this.steps = steps;
            if (seed == -1)
                this._seed = Main.rand.Next(0, int.MaxValue);
            this.maxAngleOffset = MathHelper.ToRadians(30);
            this.stepSizeLoss = 0.025f;
            this.stepDistance = 8;
            _yScale = Main.rand.NextFloat(0.5f, 1f);
            _fastNoise = new FastNoiseLite(_seed);
            initialSize = Main.rand.NextFloat(0.6f, 1f);
        }


        public Vector2 initialPosition;
        public Vector2 position;
        public Vector2 initialVelocity;

        public float steps;
        public float time;
        public float maxAngleOffset;
        public float stepSizeLoss;
        public float stepDistance;
        public float initialSize;
        public void Update()
        {
            //otherwise we'll just use the initial position directly.
            position = initialPosition;
        }

        private float SampleNoise(float step)
        {

            //Rember, the noise sample is between -1 and 1
            //So we can just use a range
            float noiseSample = _fastNoise.GetNoise(0, step);
            return noiseSample;
        }


        public void DrawIcicleSegment(SpriteBatch spriteBatch, Vector2 drawPosition, float scale, float rotation)
        {
            Vector2 drawOrigin = _icicleCircleTextureAsset.Size() / 2f;
            Texture2D textureToDraw = _icicleCircleTextureAsset.Value;
            Color drawColor = Color.White;
            drawColor *= 1f - scale;
            Vector2 drawScale = Vector2.One * scale;
            drawScale.Y *= _yScale;
            spriteBatch.Draw(textureToDraw, drawPosition, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }


        public void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            //Calculate how many steps to take, with a minimum of 1 step
            float numOfSteps = steps * time + 1;
            for (float n = 0; n < numOfSteps; n++)
            {
                Vector2 startingPosition = position;
                Vector2 velocity = initialVelocity;
                float noise = SampleNoise(n);
                float noiseAngleOffset = noise * maxAngleOffset;



                //Calculate how big the icicle circle needs to be
                float sizeLoss = n * stepSizeLoss;
                float scale = initialSize - sizeLoss;
                float ratio = n / numOfSteps;
                float fullMultiplier = MathHelper.Lerp(1f, 0f, ratio);
                scale *= fullMultiplier;
                if (scale < 0f)
                    scale = 0f;


                //Calculate the new position based on the noise values
                Vector2 newVelocity = velocity.RotatedBy(noiseAngleOffset);
                Vector2 positionAtStep = startingPosition + newVelocity * stepDistance * n;
                float rotation = newVelocity.ToRotation();
                DrawIcicleSegment(spriteBatch, positionAtStep - screenPos, scale, rotation);
            }
        }
    }
}

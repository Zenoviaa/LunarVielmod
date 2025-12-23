using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL
{
    public class Frosting : ModBuff
    {
        public override string Texture => TextureRegistry.EmptyTexture;
    }

    public class FreezeRayNPC : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);
            int frostingType = ModContent.BuffType<Frosting>();
            if (npc.HasBuff(frostingType))
            {
                //Create Ice Statue here

            }
        }
    }

    [Autoload(Side = ModSide.Client)]
    public class IceRenderer : ModSystem
    {
        private ManagedRenderTarget _icicleMaskRT;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _icicleMaskRT = ManagedRenderTarget.New(ManagedRenderTarget.GetScreenTargetSize);
        }
    }

    public class Icicle
    {
        private readonly int _seed;
        private FastNoiseLite _fastNoise;
        private Asset<Texture2D> _icicleCircleTextureAsset;
        public Icicle(int steps, int seed = -1)
        {
            _icicleCircleTextureAsset = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Circle64");
            this.steps = steps;
            if (seed == -1)
                this._seed = Main.rand.Next(0, int.MaxValue);
            this.maxAngleOffset = MathHelper.ToRadians(30);
            this.stepSizeLoss = 0.05f;
            this.stepDistance = 4;
        }

        public Entity parent;

        public Vector2 initialPosition;
        public Vector2 position;
        public Vector2 initialVelocity;

        public float steps;
        public float time;
        public float maxAngleOffset;
        public float stepSizeLoss;
        public float stepDistance;
        public void Update()
        {
            //If we have a parent then the position is based off of that
            if (parent != null)
            {
                position = parent.Center + initialPosition;
            }
            else
            {
                //otherwise we'll just use the initial position directly.
                position = initialPosition;
            }
        }

        private float SampleNoise(float step)
        {
            _fastNoise ??= new FastNoiseLite(_seed);
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
            spriteBatch.Draw(textureToDraw, drawPosition, null, drawColor, rotation, drawOrigin, scale, SpriteEffects.None, 0);
        }


        public void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            //Calculate how many steps to take, with a minimum of 1 step
            float numOfSteps = steps * time + 1;
            for(float n = 0; n < numOfSteps; n++)
            {
                Vector2 startingPosition = position;
                Vector2 velocity = initialVelocity;
                float noise = SampleNoise(n);
                float noiseAngleOffset = noise * maxAngleOffset;

                //Calculate the new position based on the noise values
                Vector2 newVelocity = velocity.RotatedBy(noiseAngleOffset);
                Vector2 positionAtStep = startingPosition + newVelocity * stepDistance;
                
                //Calculate how big the icicle circle needs to be
                float sizeLoss = n * stepSizeLoss;
                float scale = 1f - sizeLoss;
                if (scale < 0f)
                    scale = 0f;

                float rotation = newVelocity.ToRotation();
                DrawIcicleSegment(spriteBatch, positionAtStep - screenPos, scale, rotation);
            }
        }
    }

    public class FreezeRay
    {
    }
}

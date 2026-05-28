using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class ThrustParticle : Particle<ThrustParticle>
    {
        private Vector2 _stretchScale;
        private float _completionRatio;

        public int FrameWidth = 250;
        public int FrameHeight = 250;
        public int MaxFrameCount = 1;
        public Color innerColor;
        public Color bloomColor;
        public float time;
        public Vector2 scale2;
       
        public override void OnSpawn()
        {
            scale2 = Vector2.One;
            _stretchScale = new Vector2(1.2f, 0.4f);
            time = 15;
            innerColor = Color.White;
            bloomColor = Color.Red;
            Frame = new Rectangle(0, FrameHeight * Main.rand.Next(MaxFrameCount), FrameWidth, FrameHeight);
            customShader = DustBloomShader.Instance;
            Scale = Main.rand.NextFloat(0.5f, 1f);
        }

        public override void Update()
        {
            Velocity *= 0.95f;
            Rotation = Velocity.ToRotation();
            fadeIn++;
            if (fadeIn > time)
                active = false;

            _completionRatio = fadeIn / time;
            _stretchScale.X = MathHelper.Lerp(0.2f, 1.2f, EasingFunction.OutExpo(_completionRatio));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerPos = DrawPosition;
            DustBloomShader shader = DustBloomShader.Instance;
            shader.InnerColor = innerColor;
            shader.BloomColor = bloomColor;
            shader.Apply();


            Color multiplyColor = Color.Lerp(Color.White, Color.Black, _completionRatio);
            var textureAsset = GetTexture();
            spriteBatch.Draw(textureAsset.Value, centerPos, Frame, multiplyColor, Rotation, Frame.Size() / 2f, Scale * _stretchScale * 0.4f * scale2, SpriteEffects.None, 0);
            
            multiplyColor = Color.Lerp(Color.Lerp(color, bloomColor, _completionRatio), Color.Black, _completionRatio);
            spriteBatch.Draw(textureAsset.Value, centerPos, Frame, multiplyColor, Rotation, Frame.Size() / 2f, Scale * _stretchScale * 1.2f * scale2, SpriteEffects.None, 0);
            spriteBatch.Draw(textureAsset.Value, centerPos, Frame, multiplyColor, Rotation, Frame.Size() / 2f, Scale * _stretchScale * scale2, SpriteEffects.None, 0);
        }
    }
}

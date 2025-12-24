
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER
{
    public class STARBOMBERGUN
    {
        private float _primeTimer;
        private float _recoilTimer;
        private Vector2 _lastPosition;
        public STARBOMBERGUN(string texturePath)
        {
            TextureAsset = ModContent.Request<Texture2D>(texturePath);
            drawScale = Vector2.One;
            recoilDistance = 24;
            drawColor = Color.White;
        }
        public Asset<Texture2D> TextureAsset;
        public Vector2 drawScale;
        public float recoilDistance;
        public Color drawColor;
        public float aimingReticle;
        public Color aimingReticleColor;
        public float muzzleOffset;
        public Vector2 GetMuzzlePosition(Vector2 anchorPosition, Vector2 direction)
        {
            Vector2 muzzlePosition = anchorPosition + direction * muzzleOffset;
            muzzlePosition -= direction * recoilDistance;
            return muzzlePosition;
        }

        public Vector2 GetRecoilOffset(Vector2 direction)
        {
            float progress = _recoilTimer / 8f;
            return -direction * progress * recoilDistance;
        }
        public void Recoil()
        {
            _recoilTimer = 8;
            FXUtil.ShakeCamera(_lastPosition, 256, 8);
        }

        public void Prime()
        {
            _primeTimer = 45f;
            SoundStyle primeSound = new SoundStyle("Stellamod/Assets/Sounds/MiniPistol2");
            primeSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(primeSound, _lastPosition); 
            var part = LegacyParticle.NewParticle<GlowDonutParticle>(_lastPosition, Vector2.Zero, Color.White);
            part.Scale *= 4;
            part.shrink = true;
            part.noStretch = true;
        }
        public void Update()
        {
            if (_primeTimer > 0)
                _primeTimer--;
            if (_recoilTimer > 0)
            {
                _recoilTimer--;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 direction, Color lightColor)
        {
            _lastPosition = position;
            Vector2 drawPosition = position - Main.screenPosition;
            drawPosition += GetRecoilOffset(direction);

            float primeProgress = _primeTimer / 45f;
            drawPosition += Main.rand.NextVector2Circular(4, 4) * primeProgress;

            float recoilAmount = _recoilTimer / 8f;
            Color finalColor = drawColor.MultiplyRGB(lightColor);
            Vector2 drawOrigin = new Vector2(0, TextureAsset.Height() / 2f);
            Vector2 finalScale = drawScale;
            finalScale += Vector2.One * recoilAmount * 0.1f;
            float rotation = direction.ToRotation();
            float angle = MathHelper.WrapAngle(rotation);
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (direction.X < 0)
            {
                spriteEffects = SpriteEffects.FlipVertically;
                drawOrigin.Y = TextureAsset.Height() - drawOrigin.Y;
            }
            spriteBatch.Draw(TextureAsset.Value, drawPosition, null, finalColor, rotation, drawOrigin, finalScale, spriteEffects, 0);


            Color glowColor = Color.White;
            glowColor.A = 0;
            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(TextureAsset.Value, drawPosition, null, glowColor * recoilAmount, rotation, drawOrigin, finalScale, spriteEffects, 0);

            }
            glowColor = Color.Red;
            glowColor.A = 0;
            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(TextureAsset.Value, drawPosition, null, glowColor * recoilAmount, rotation, drawOrigin, finalScale, spriteEffects, 0);
            }
            if(_primeTimer > 0)
            {
                glowColor = Color.Red;
                glowColor *= primeProgress;
                glowColor.A = 0;
                for (int i = 0; i < 3; i++)
                {
                    spriteBatch.Draw(TextureAsset.Value, drawPosition, null, glowColor, rotation, drawOrigin, finalScale, spriteEffects, 0);

                }
            }


            Color aimingLineColor = aimingReticleColor;
            aimingLineColor *= aimingReticle;
            aimingReticleColor.A = 0;
            Texture2D aimingLine = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
            Vector2 lineOrigin = new Vector2(aimingLine.Size().X / 2f, 0f);
            Vector2 lineScale = new Vector2(0.01f, 1f);
            spriteBatch.Draw(aimingLine, drawPosition, null, aimingLineColor, rotation - MathHelper.PiOver2, lineOrigin, lineScale, SpriteEffects.None, 0);
        }
        

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 position, Vector2 direction, Color lightColor)
        {
            Draw(spriteBatch, position - Vector2.UnitY * 2, direction, lightColor);
            Draw(spriteBatch, position + Vector2.UnitY * 2, direction, lightColor);
            Draw(spriteBatch, position - Vector2.UnitX * 2, direction, lightColor);
            Draw(spriteBatch, position + Vector2.UnitX * 2, direction, lightColor);
        }
    }
}

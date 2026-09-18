
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using static Stellamod.Tiles.SpecialDecorativeWall;

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
            recoilDistance = 36;
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
        public void Prime2(Vector2 muzzlePoint)
        {
            _primeTimer = 45f;
            SoundStyle primeSound = new SoundStyle("Stellamod/Assets/Sounds/MiniPistol2");
            primeSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(primeSound, muzzlePoint);
            var part = LegacyParticle.NewParticle<GlowDonutParticle>(muzzlePoint, Vector2.Zero, Color.White);
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
            Vector2 drawPosition = position;
            drawPosition += GetRecoilOffset(direction);

            float primeProgress = _primeTimer / 45f;
            drawPosition += Main.rand.NextVector2Circular(4, 4) * primeProgress;

            SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, 
                drawPosition + direction * 56);
            glowDrawer.color = Color.Blue * 0.3f;
            glowDrawer.color.A = 0;
            glowDrawer.scale *= 0.6f;
            spriteBatch.Draw(glowDrawer);

            float recoilAmount = _recoilTimer / 8f;
            Color finalColor = drawColor.MultiplyRGB(lightColor);

            Vector2 finalScale = drawScale;
            finalScale += Vector2.One * recoilAmount * 0.1f;
            float rotation = direction.ToRotation();
            float angle = MathHelper.WrapAngle(rotation);
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (direction.X < 0)
            {
                spriteEffects = SpriteEffects.FlipVertically;
  
            }

            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(TextureAsset, drawPosition);
            drawer.color = finalColor;
            drawer.rotation = rotation;
            drawer.scale = finalScale;
            drawer.spriteEffects = spriteEffects;
            drawer.VerticalFrame(0, 2);
            drawer.LeftCenterOrigin();
            if(direction.X < 0)
            {
                drawer.drawOrigin.Y = drawer.sourceRect.Value.Height - drawer.drawOrigin.Y;
            }
            spriteBatch.Draw(drawer);

            drawer.VerticalFrame(1, 2);
            drawer.color = Color.White * ExtraMath.Osc(0f, 0.3f, speed: 6);
            drawer.color.A = 0;
            spriteBatch.Draw(drawer);


            Color glowColor = Color.White;
            glowColor.A = 0;
            for (int i = 0; i < 3; i++)
            {
                drawer.color = glowColor * recoilAmount;
                spriteBatch.Draw(drawer);

            }
            glowColor = Color.Red;
            glowColor.A = 0;
            for (int i = 0; i < 3; i++)
            {
                drawer.color = glowColor * recoilAmount;
                spriteBatch.Draw(drawer);
            }
            if(_primeTimer > 0)
            {
                glowColor = Color.Red;
                glowColor *= primeProgress;
                glowColor.A = 0;
                drawer.color = glowColor;
         
                for (int i = 0; i < 3; i++)
                {
                    spriteBatch.Draw(drawer);
                }
            }


            Color aimingLineColor = aimingReticleColor;
            aimingLineColor *= aimingReticle;
            aimingReticleColor.A = 0;
            Texture2D aimingLine = AssetReferences.Assets.NoiseTextures.BloomLine.Asset.Value;
            Vector2 lineOrigin = new Vector2(aimingLine.Size().X / 2f, 0f);
            Vector2 lineScale = new Vector2(0.01f, 1f);
            spriteBatch.Draw(aimingLine, drawPosition, null, aimingLineColor, rotation - MathHelper.PiOver2, lineOrigin, lineScale, SpriteEffects.None, 0);
        }
        

     
        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 position, Vector2 direction, Color lightColor)
        {
            Vector2 drawPosition = position;
            drawPosition += GetRecoilOffset(direction);

            float primeProgress = _primeTimer / 45f;
            drawPosition += Main.rand.NextVector2Circular(4, 4) * primeProgress;

            float recoilAmount = _recoilTimer / 8f;
            Color finalColor = drawColor.MultiplyRGB(lightColor);

            Vector2 finalScale = drawScale;
            finalScale += Vector2.One * recoilAmount * 0.1f;
            float rotation = direction.ToRotation();
            float angle = MathHelper.WrapAngle(rotation);
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (direction.X < 0)
            {
                spriteEffects = SpriteEffects.FlipVertically;

            }

            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(TextureAsset, drawPosition);
            drawer.color = finalColor;
            drawer.rotation = rotation;
            drawer.scale = finalScale;
            drawer.spriteEffects = spriteEffects;
            drawer.VerticalFrame(0, 2);
            drawer.LeftCenterOrigin();
            if (direction.X < 0)
            {
                drawer.drawOrigin.Y = drawer.sourceRect.Value.Height - drawer.drawOrigin.Y;
            }
            spriteBatch.Draw(drawer);
        }
    }
}

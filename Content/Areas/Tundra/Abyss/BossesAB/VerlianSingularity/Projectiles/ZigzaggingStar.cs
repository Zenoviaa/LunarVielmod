using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.VerlianSingularity.Projectiles
{
    public class ZigzaggingStar : VSProjectile
    {
        private float _scale;
        private Vector2 _stretchScale;
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float ZigZagOffsetRadians => ref Projectile.ai[2];
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 48;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            base.AI();

            NPC parent = GetParentNPC();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle starSingle = AssetRegistry.Sounds.Stars.Starsingle3;
                starSingle.PitchVariance = 0.2f;
                SoundEngine.PlaySound(starSingle, GetParentNPC().position);
                Vector2 velocityToParent = (parent.Center - Projectile.Center);
                velocityToParent = velocityToParent.SafeNormalize(Vector2.Zero);
                Projectile.velocity = velocityToParent;
            }
            if (Timer >= 180)
            {
                if (this.OwnedByLocalClient())
                {
                    float range = MathHelper.ToRadians(45);
                    ZigZagOffsetRadians = ZigZagOffsetRadians <= 0 ? range : -range;
                    Projectile.netUpdate = true;
                }
                SoundStyle starSingle2 = AssetRegistry.Sounds.Stars.Starsingle2;
                starSingle2.PitchVariance = 0.2f;
                SoundEngine.PlaySound(starSingle2, Projectile.position);
                FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightCyan, Color.DarkBlue, baseSize: 0.06f);
                Timer = 0;
            }

         
    
            Vector2 velToParent = (parent.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
            velToParent = velToParent.RotatedBy(ZigZagOffsetRadians);
            velToParent *= Projectile.velocity.Length();
            Projectile.velocity = velToParent;
            if (Timer < 45)
            {
                _scale *= 0.9f;
                Projectile.velocity *= 1.04f;
                if(Timer % 9 == 0)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), newColor: Color.White, Scale: 0.5f);
                }
            }
            else
            {
                Projectile.velocity *= 0.98f;
            }
            if(Timer >= 135)
            {
                _scale *= 1.05f;
            }

            float interpolant = Projectile.velocity.Length() / 15f;
            float ease = EasingFunction.InOutSine(interpolant);
            _stretchScale = Vector2.Lerp(Vector2.One, new Vector2(1.1f, 0.9f), ease);
            Projectile.rotation = Projectile.velocity.ToRotation();


            float distanceToParent = Vector2.Distance(parent.Center, Projectile.Center);
            if (distanceToParent <= 64)
            {
                Projectile.velocity *= 1.04f;
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile.Center, parent.Center, Projectile.velocity, 16);
            }
            if (distanceToParent <= 32)
            {
                _scale = MathHelper.SmoothStep(0f, 1f, distanceToParent / 32f);
            }
            else
            {
                _scale = MathHelper.Lerp(_scale, 1f, 0.1f);
            }
            if (distanceToParent <= 16)
            {
                Projectile.Kill();
            }
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightCyan, Color.DarkBlue, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            float w = 10;
            float ew = w / 10;
            float width = w;

            float p = completionRatio / 0.5f;
            float ep = EasingFunction.OutCirc(p);
            float circleWidth = MathHelper.Lerp(0, w, ep);
            float trailWidth = MathHelper.Lerp(width, 0, EasingFunction.OutCirc(completionRatio));
            return MathHelper.Lerp(circleWidth, trailWidth, EasingFunction.OutExpo(completionRatio)) * _scale;
        }

        private void DrawTrail()
        {
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;
            shader.Repeats = 1f;
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, ColorFunction, WidthFunction, shader);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();
            string texturePath = "Stellamod/Assets/NoiseTextures/Extra_63";
            Texture2D starTexture = ModContent.Request<Texture2D>(texturePath).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White;
            drawColor.A = 0;
            Vector2 drawOrigin = starTexture.Size() / 2f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(starTexture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, _scale * 0.2f, SpriteEffects.None, 0);
            spriteBatch.Draw(starTexture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, _scale * 0.05f, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Cyan, Color.DarkBlue);
        }
    }
}

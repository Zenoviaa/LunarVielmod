using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER.Projectiles
{
    public class SteamLaser : ModProjectile
    {
        private Vector2[] _laserPoints;
        private Vector2[] LaserPoints
        {
            get
            {
                _laserPoints ??= new Vector2[64];
                for(int  i = 0; i < _laserPoints.Length; i++)
                {
                    float f = i;
                    float numPoints = _laserPoints.Length;
                    float completionRatio = f / numPoints;
                    Vector2 point = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity, completionRatio);
                    _laserPoints[i] = point;
                }
                return _laserPoints;
            }
        }

        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.hostile = true;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            float maxBeamLength = 2400;
            Projectile.velocity =
                Projectile.velocity.SafeNormalize(Vector2.Zero) * ProjectileHelper.PerformBeamHitscan(Projectile.Center, Projectile.velocity, maxBeamLength);
      
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return ProjectileHelper.OldPosColliding(LaserPoints, projHitbox, targetHitbox);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        

        private Color ColorFunction(float completionRatio)
        {
            return Color.White;
        }
        private float WidthFunction(float completionRatio)
        {
            return 64;
        }
        private void DrawLaser()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            BlackFireShader shader = BlackFireShader.Instance;
            shader.PrimaryTexture = TrailRegistry.WhispyTrail;
            shader.PrimaryTexture2 = TrailRegistry.StarTrail;
            shader.InnerColor = Color.Aqua;
            shader.OuterColor = Color.LightBlue;
            shader.Distortion = MathHelper.Lerp(0.6f, 0.2f, EasingFunction.InOutSine(Timer / 30f)) * MathHelper.Lerp(1, 0, EasingFunction.InOutExpo(Timer / 90f));
            shader.Time = Timer * 0.03f;
            TrailDrawer.Draw(spriteBatch, LaserPoints, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);

        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawLaser();
            return false;
        }
    }
}

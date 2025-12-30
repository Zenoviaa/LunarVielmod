using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER.Projectiles
{
    public class WingSnipe : ModProjectile
    {
        private float _flameTimer;
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 80;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 1600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 32;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }

        public override void AI()
        {

            base.AI();
            _flameTimer += 0.3f;
            Timer++;
            if(Timer == 1)
            {
                SoundStyle shootSound = new SoundStyle("Stellamod/Assets/Sounds/STARSHOOT");
                shootSound.PitchVariance = 0.1f;
                SoundEngine.PlaySound(shootSound, Projectile.Center);

                //Shoot effects
                var part = FXUtil.GlowDonutParticle(Projectile.Center, -Projectile.velocity, Color.Gray, Color.Pink, Color.Purple);
                part.Scale *= 0.2f;

                var shotPart = FXUtil.GlowStretch(Projectile.Center, Projectile.velocity);
                shotPart.Scale *= Main.rand.NextFloat(0.2f, 0.75f);
            }

            if (Timer % 10 == 0)
            {
                var d = Dust.NewDustPerfect(Projectile.Center,
                    ModContent.DustType<GlowSparkleDust>(), newColor: Color.Gray, Scale: Main.rand.NextFloat(0f, 0.5f));
                d.velocity *= 0;
            }


            Projectile.velocity *= 1.00001f;
            Projectile.scale = MathHelper.Lerp(0.2f, 0.3f, EasingFunction.InOutSine(Timer / 60f));
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Purple, Color.Transparent, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(24, 0, completionRatio);
        }

        private void CustomDraw()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            BlackFireShader shader = BlackFireShader.Instance;
            shader.PrimaryTexture = TrailRegistry.WhispyTrail;
            shader.PrimaryTexture2 = TrailRegistry.StarTrail;
            shader.InnerColor = Color.Aqua;
            shader.OuterColor = Color.LightBlue;
            shader.Distortion = MathHelper.Lerp(0.6f, 0.2f, EasingFunction.InOutSine(Timer / 30f)) * MathHelper.Lerp(1, 0, EasingFunction.InOutExpo(Timer / 90f));
            shader.Time = _flameTimer;
            TrailDrawer.Draw(spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);


            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Texture2D starTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Vector2 drawOrigin = starTexture.Size() / 2f;
            Color cometColor = Color.Gray;
            cometColor.A = 0;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                Vector2 oldDrawPos = oldPos - Main.screenPosition;
                float f = i;
                float interpolant = f / (float)Projectile.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.Gray, Color.Blue, interpolant) * 0.05f;
                fadeColor *= (1.0f - interpolant);
                fadeColor.A = 0;
                oldDrawPos += Projectile.Size / 2f;
                spriteBatch.Draw(starTexture, oldDrawPos, null, fadeColor, Projectile.oldRot[i], drawOrigin, Projectile.scale * 2.2f, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(starTexture, drawPosition, null, cometColor * 0.4f, Projectile.rotation, drawOrigin, Projectile.scale * 2, SpriteEffects.None, 0);
            spriteBatch.Draw(starTexture, drawPosition, null, cometColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CustomDraw();
            return false;
        }
    }
}

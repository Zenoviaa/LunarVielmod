using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    internal class PegasusMinionFrostBombProj : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        public PrimDrawer TrailDrawer { get; private set; } = null;
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float LifeTime => 360;
        private float MaxScale => 0.22f;
        private float Scale;
        private Vector2 OldVelocity;
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)LifeTime;
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                OldVelocity = Projectile.velocity;
            }
            Player owner = Main.player[Projectile.owner];
            if (owner != null)
            {
                Vector2 velocityToTarget = Projectile.Center.DirectionTo(owner.Center) * OldVelocity.Length();
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, velocityToTarget, 0.02f);
            }

            if (Timer == 1)
            {
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 / 2) * Main.rand.NextFloat(0.5f, 1f), 0, Color.LightSkyBlue, 1f).noGravity = true;
                }
            }
            if (Timer > LifeTime - 60)
            {
                Projectile.hostile = false;
                Scale = MathHelper.Lerp(Scale, 0f, 0.1f);
            }
            else
            {
                Scale = MathHelper.Lerp(Scale, 1f, 0.1f);
            }
 
            Projectile.rotation += 0.03f + Projectile.velocity.Length() * 0.05f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.Frostburn2, 120);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.RoyalBlue, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.FadedStreak);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);

            var textureAsset = ModContent.Request<Texture2D>("Stellamod/Particles/AuroranSlashParticle");

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = textureAsset.Size();
            Vector2 drawOrigin = drawSize / 2;
            Color drawColor = new Color(255, 255, 255, 0);
            //Calculate the scale with easing
            float drawScale = Projectile.scale * MaxScale * Scale;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);



            // Retrieve reference to shader
            var shader = ShaderRegistry.MiscFireWhitePixelShader;
            shader.UseOpacity(1f * Scale);

            //How intense the colors are
            //Should be between 0-1
            shader.UseIntensity(1f);

            //How fast the extra texture animates
            float speed = 0.2f;
            shader.UseSaturation(speed);

            //Color
            shader.UseColor(Color.White);

            //Texture itself
            shader.UseImage1(textureAsset);

            // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
            shader.Apply(null);

            float drawRotation = MathHelper.TwoPi;
            float num = 8;
            for (int i = 0; i < num; i++)
            {
                float nextDrawScale = drawScale;
                float nextDrawRotation = Projectile.rotation + drawRotation * (i / num);
                spriteBatch.Draw(textureAsset.Value, drawPosition, null, drawColor, nextDrawRotation, drawOrigin, nextDrawScale, SpriteEffects.None, 0f);
            }


            spriteBatch.End();
            spriteBatch.Begin();

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for(int i = 0; i < 32; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(7, 7);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), velocity, newColor: Color.White);
            }
        }
    }
}
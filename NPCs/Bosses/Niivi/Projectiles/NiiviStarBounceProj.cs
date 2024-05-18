
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Projectiles;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    internal class NiiviStarBounceProj : ModProjectile
    {
        private Vector2 OldVelocity;
        private PrimitiveTrail TrailDrawer;
        public override string Texture => TextureRegistry.EmptyTexture;

        private ref float Timer => ref Projectile.ai[0];
        private ref float VelTimer => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 14;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 5;
            Projectile.light = 0.34f;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Timer++;
            VelTimer++;
            if(VelTimer == 1)
            {
                OldVelocity = Projectile.velocity;
                SoundStyle soundStyle = SoundRegistry.Niivi_StarSummon2;
                soundStyle.PitchVariance = .1f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }

            if (Timer % 7 == 0)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(2, 2);
                Color[] colors = new Color[] { Color.LightCyan, Color.Cyan, Color.Blue, Color.White };
                Color color = colors[Main.rand.Next(0, colors.Length)];
                float scale = Main.rand.NextFloat(0.5f, 0.8f);
                ParticleManager.NewParticle<StarParticle2>(Projectile.Center, velocity, color, scale);
            }


            Projectile.velocity *= 0.99f;
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver4 / 90f);
            float maxDetectDistance = 9000;
            Player closestPlayer = PlayerHelper.FindClosestPlayer(Projectile.position, maxDetectDistance);
            Vector2 directionToPlayer = Projectile.Center.DirectionTo(closestPlayer.Center);
            if(Timer == 66)
            {
                Projectile.velocity = Vector2.Zero;
            }

            if(Timer >= 70)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2"), Projectile.position);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, 0, 0,
                     ModContent.ProjectileType<AlcadizBombExplosion>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, 0f, 0f);

                float speed = OldVelocity.Length();
                Vector2 newVelocity = directionToPlayer * speed;
                Projectile.velocity = newVelocity;
                Timer = 0;
            }
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.RoyalBlue, completionRatio) * (1f - completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            return Projectile.width * Projectile.scale * (1f - completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);
            TrailDrawer.SpecialShader = TrailRegistry.FireWhiteVertexShader;
            TrailDrawer.SpecialShader.UseColor(Color.Lerp(Color.White, Color.LightBlue, 0.3f));
            TrailDrawer.SpecialShader.SetShaderTexture(TrailRegistry.CausticTrail);
            TrailDrawer.Draw(Projectile.oldPos, -Main.screenPosition, Projectile.oldPos.Length);

            var textureAsset = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = textureAsset.Size();
            Vector2 drawOrigin = drawSize / 2;
            float scale = 1f;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

  

            // Retrieve reference to shader
            var shader = ShaderRegistry.MiscFireWhitePixelShader;

            //You have to set the opacity/alpha here, alpha in the spritebatch won't do anything
            //Should be between 0-1
            float opacity = 0.75f;
            shader.UseOpacity(opacity);

            //How intense the colors are
            //Should be between 0-1
            shader.UseIntensity(0.15f);

            //How fast the extra texture animates
            float speed = 1.0f;
            shader.UseSaturation(speed);

            //Color
            shader.UseColor(new Color(Color.RoyalBlue.R, Color.RoyalBlue.G, Color.RoyalBlue.B, 0));

            //Texture itself
            shader.UseImage1(textureAsset);

            // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
            shader.Apply(null);

            for (int i = 0; i < 4; i++)
            {
                float drawScale = scale * (i / 4f);
                float drawRotation = Projectile.rotation * (i / 4f);
                spriteBatch.Draw(textureAsset.Value, drawPosition, null, Color.White, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin();
            return true;
        }


        public override void OnKill(int timeLeft)
        {
            SoundStyle soundStyle = SoundRegistry.Niivi_StarringDeath;
            soundStyle.PitchVariance = 0.1f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
            for (int i = 0; i < 8; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                float scale = Main.rand.NextFloat(0.3f, 0.5f);
                ParticleManager.NewParticle<StarParticle2>(Projectile.Center, velocity, Color.White, scale);
            }
        }
    }
}

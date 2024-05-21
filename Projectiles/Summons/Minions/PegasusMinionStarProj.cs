using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Projectiles;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
namespace Stellamod.Projectiles.Summons.Minions
{
    internal class PegasusMinionStarProj : ModProjectile
    {
        public override string Texture => TextureRegistry.ZuiEffect;
        public PrimDrawer TrailDrawer { get; private set; } = null;

        private Vector2 OldVelocity;
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private bool HasBounced
        {
            get => Projectile.ai[1] == 1;
            set
            {
                if (value == true)
                {
                    Projectile.ai[1] = 1;
                }
                else
                {
                    Projectile.ai[1] = 0;
                }
            }
        }

        private ref float VelTimer => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
        }

        public override void AI()
        {


            Timer++;
            if(Timer == 1)
            {
                OldVelocity = Projectile.velocity;
            }
            float maxDetectDistance = 512;
            NPC closestNpc = NPCHelper.FindClosestNPC(Projectile.position, maxDetectDistance);
            if (closestNpc != null)
            {
                Vector2 velocityToTarget = Projectile.Center.DirectionTo(closestNpc.Center) * OldVelocity.Length();
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, velocityToTarget, 0.2f);
            }

            VelTimer++;
            if (Timer == 1)
            {
                SoundStyle soundStyle = SoundRegistry.Niivi_StarSummon;
                soundStyle.PitchVariance = 0.15f;
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

            Projectile.rotation += 0.05f;
            Lighting.AddLight(Projectile.position, Color.White.ToVector3() * 0.78f);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Main.DiscoColor * 0.3f, Color.Transparent, completionRatio);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.White.R,
                Color.White.G,
                Color.White.B, 0) * (1f - Projectile.alpha / 50f);
        }



        public override bool PreDraw(ref Color lightColor)
        {
            //Draw the texture
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;

            //Draw the trail
            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            }

            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.WhispyTrail);

            Vector2 frameSize = new Vector2(16, 16);
            //Could also set this manually like
            //frameSize = new Vector2(58, 34);
            TrailDrawer.DrawPrims(Projectile.oldPos, frameSize * 0.5f - Main.screenPosition, 155);

            float scale = 1f;
            Color drawColor = (Color)GetAlpha(lightColor);
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < 2; i++)
            {
                float rotOffset = MathHelper.TwoPi * (i / 4f);
                rotOffset += Timer * 0.003f;
                float drawScale = scale * (i / 4f);
                spriteBatch.Draw(texture, drawPosition, null, drawColor, Projectile.rotation + rotOffset,
                    drawOrigin, drawScale, SpriteEffects.None, 0f);
            }
            return false;
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
                ParticleManager.NewParticle<StarParticle2>(Projectile.Center, velocity, Main.DiscoColor, scale);
            }
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.Summons.Glyph;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons
{
    public class SeekerProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 18;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 2;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {

            Timer++;
            if (Timer < 60)
            {
                Projectile.velocity.Y *= 0.9f;
            }
            if (Timer == 71)
            {
                Projectile.velocity.Y = 2f;
            }
            if (Timer % 12 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.White, 1f).noGravity = true;
            }

            float maxDetectDistance = 1024;
            NPC nearest = ProjectileHelper.FindNearestEnemy(Projectile.position, maxDetectDistance);
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Timer > 71)
            {
                if (nearest != null)
                {
                    Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, degreesToRotate: 6f);
                }
                if (Projectile.velocity.Length() < 15)
                    Projectile.velocity *= 1.02f;
            }

        }
        private float WidthFunction(float completionRatio)
        {
            return MathHelper.Lerp(24f, 0f, completionRatio);
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Main.DiscoColor, Color.Transparent, completionRatio);
        }

        private Color ColorFunction2(float completionRatio)
        {
            return Color.Lerp(Main.DiscoColor, Color.Transparent, completionRatio);
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        private void DrawTrail()
        {
            Main.spriteBatch.RestartDefaults();
            Vector2 drawOffset = -Main.screenPosition + Projectile.Size / 2f;
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            TrailDrawer.ColorFunc = ColorFunction;
            TrailDrawer.Shader = GameShaders.Misc["VampKnives:SuperSimpleTrail"];
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.BeamTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, drawOffset, 155);
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int style = Main.rand.Next(0, 7);
            float num = Main.rand.Next(3, 8);
            Projectile.velocity *= 0.25f;
            if (Projectile.penetrate == 2)
                return;
            for (float n = 0; n < num; n++)
            {
                Vector2 velocity = Vector2.UnitY.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1f, 6f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, velocity,
                    ModContent.ProjectileType<AuroranGlyph>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner, ai1: style);
            }
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: new Color(Main.rand.Next(0, 255), Main.rand.Next(0, 255), Main.rand.Next(0, 255)),
                    outerGlowColor: Color.Black,
                    baseSize: Main.rand.NextFloat(0.06f, 0.12f));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }


        }


        private void DrawEnergyBall(ref Color lightColor)
        {
            //Draw Code for the orb
            Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.EmptyGlowParticle).Value;
            Vector2 centerPos = Projectile.Center - Main.screenPosition;
            GlowCircleShader shader = GlowCircleShader.Instance;

            //How quickly it lerps between the colors
            shader.Speed = 10f;

            //This effects the distribution of colors
            shader.BasePower = 2.5f;

            //Radius of the circle
            shader.Size = 0.06f;


            //Colors
            Color startInner = Color.White;
            Color startGlow = new Color(VectorHelper.Osc(0f, 1f, speed: 2), VectorHelper.Osc(0f, 1f, speed: 4), VectorHelper.Osc(0f, 1f, speed: 7));
            Color startOuterGlow = Color.Black;

            if (Projectile.penetrate == 1)
            {
                startOuterGlow = startGlow;
                startGlow = Color.White;

                shader.Size *= 1.5f;
            }

            shader.InnerColor = startInner;
            shader.GlowColor = startGlow;
            shader.OuterGlowColor = startOuterGlow;

            //Idk i just included this to see how it would look
            //Don't go above 0.5;
            shader.Pixelation = 0.005f;

            //This affects the outer fade
            shader.OuterPower = 13.5f;
            shader.Apply();


            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            for (int i = 0; i < 2; i++)
            {
                spriteBatch.Draw(texture, centerPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();
            DrawEnergyBall(ref lightColor);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.White, 1f).noGravity = true;
            }
        }
    }
}



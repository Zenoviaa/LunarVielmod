using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class VoidHand : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public PrimDrawer Trail { get; set; }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 1;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.timeLeft = 700;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/VoidHand"), Projectile.position);
                Projectile.rotation = Projectile.velocity.ToRotation();

                for (int j = 0; j < 10; j++)
                {
                    Vector2 vector2 = Vector2.UnitX * -Projectile.width / 2f;
                    vector2 += -Utils.RotatedBy(Vector2.UnitY, (j * 3.141591734f / 6f), default(Vector2)) * new Vector2(8f, 16f);
                    vector2 = Utils.RotatedBy(vector2, (Projectile.rotation - 1.57079637f), default(Vector2));
                    int num8 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Shadowflame, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                    Main.dust[num8].scale = 1.3f;
                    Main.dust[num8].noGravity = true;
                    Main.dust[num8].position = Projectile.Center + vector2;
                    Main.dust[num8].velocity = Projectile.velocity * 0.1f;
                    Main.dust[num8].noLight = true;
                    Main.dust[num8].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num8].position) * 1.25f;
                }
            }

            if (Timer % 8 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame);
            }

            if (Timer >= 20)
            {
                Projectile.tileCollide = true;
            }
            if (Projectile.alpha >= 0)
            {
                Projectile.alpha -= 12;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, Color.MediumPurple.ToVector3() * 1.75f * Main.essScale);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 24; i++)
            {
                float progress = (float)i / 24f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 velocity = rot.ToRotationVector2() * 4;
                Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame, velocity);
            }
            SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, Projectile.position);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<Skullboom>(), Projectile.damage / 4, Projectile.knockBack, Projectile.owner);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.62f;
            return MathHelper.SmoothStep(36, baseWidth, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(new Color(190, 31, 200), Color.Transparent, completionRatio);
        }
        private void DrawGlow(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawRotation = Projectile.rotation;
            float drawScale = 1f;
            spriteBatch.Restart(blendState: BlendState.Additive);
            for (float f = 0f; f < 1f; f += 0.12f)
            {
                float rot = f * MathHelper.ToRadians(360);
                rot += Main.GlobalTimeWrappedHourly * 8;
                Vector2 offset = rot.ToRotationVector2() * VectorHelper.Osc(6f, 9f);
                offset -= Projectile.velocity * f * 1;
                Vector2 glowDrawPos = drawPos + offset;
                spriteBatch.Draw(texture, glowDrawPos, frame, drawColor * 0.52f, drawRotation, drawOrigin, drawScale, SpriteEffects.None, layerDepth: 0);
            }

            spriteBatch.RestartDefaults();
        }

        protected virtual void DrawTrail(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.RestartDefaults();
            Trail ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.BeamTrail);
            Trail.DrawPrims(Projectile.oldPos, -Main.screenPosition + Projectile.Size / 2, totalTrailPoints: 155);
        }

        protected virtual void DrawSprite(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawRotation = Projectile.rotation;
            float drawScale = 1f;
            spriteBatch.Draw(texture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, layerDepth: 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawGlow(ref lightColor);
            DrawTrail(ref lightColor);
            DrawSprite(ref lightColor);

            return false;
        }

    }
}



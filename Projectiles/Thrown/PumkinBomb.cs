using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.JackTheScholar.Projectiles;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown
{
    internal class PumkinBomb : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private float _scale;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 60;
       
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            _scale = Main.rand.NextFloat(0.75f, 1.0f);
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 12 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, vel, Scale: 1);
                d.noGravity = true;
            }
            if (Timer % 6 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.Torch, vel, Scale: 1);
                d.noGravity = true;
            }
            Projectile.velocity.Y += 0.1f;
            Projectile.rotation -= Projectile.velocity.Length() * 0.025f;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            //Explode
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(Projectile.position, DustID.GoldCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 1f).noGravity = true;
            }
            for (int i = 0; i < 12; i++)
            {
                Dust.NewDustPerfect(Projectile.position, DustID.Torch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 2f).noGravity = true;
            }

            float shootSpeedMult = 1.2f;
            Vector2 firePoint = Projectile.Center;
            Vector2 fireVelocity = Projectile.velocity * shootSpeedMult;
            int laughingBlastType = ModContent.ProjectileType<PumkinBolt>();
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, fireVelocity, laughingBlastType, Projectile.damage, Projectile.knockBack, Projectile.owner);
            fireVelocity = Projectile.velocity * shootSpeedMult;
            fireVelocity = fireVelocity.RotatedBy(MathHelper.ToRadians(15));
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, fireVelocity, laughingBlastType, Projectile.damage, Projectile.knockBack, Projectile.owner);
            fireVelocity = Projectile.velocity * shootSpeedMult;
            fireVelocity = fireVelocity.RotatedBy(MathHelper.ToRadians(-15));
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, fireVelocity, laughingBlastType, Projectile.damage, Projectile.knockBack, Projectile.owner);

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss, Projectile.position);
        }


        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.2f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightGoldenrodYellow * 0.1361f, Color.Transparent, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.StarTrail);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawRotation = Projectile.rotation;
            float drawScale = _scale;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.Draw(texture, drawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < 4; i++)
            {
                float rot = (float)i / 4f;
                Vector2 vel = rot.ToRotationVector2() * VectorHelper.Osc(0f, 4f, speed: 16);
                Vector2 flameDrawPos = drawPos + vel + Main.rand.NextVector2Circular(2, 2);
                flameDrawPos -= Vector2.UnitY * 4;
                spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 flameDrawPos = drawPos + Main.rand.NextVector2Circular(2, 2);
                spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(texture, drawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            Texture2D dimLightTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            float drawScale = 1f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            int chance = (int)MathHelper.Lerp(16, 1, Timer / 60f);
            for (int i = 0; i < 3; i++)
            {
                Color glowColor = Main.rand.NextBool(chance) ? Color.LightGoldenrodYellow : new Color(85, 45, 15);
                glowColor *= 0.5f;
                glowColor.A = 0;
                Vector2 drawPos = Projectile.Center - Main.screenPosition;
                drawPos += Main.rand.NextVector2Circular(4, 4);
                spriteBatch.Draw(dimLightTexture, drawPos, null, glowColor,
                    Projectile.rotation, dimLightTexture.Size() / 2f, drawScale * VectorHelper.Osc(0.75f, 1f, speed: 32, offset: Projectile.whoAmI), SpriteEffects.None, 0f);
            }
        }
    }
}

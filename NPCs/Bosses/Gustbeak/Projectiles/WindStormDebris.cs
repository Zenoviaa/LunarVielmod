using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Gustbeak.Projectiles
{
    internal class WindStormDebris : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float FallDownTime => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1 && Main.myPlayer == Projectile.owner)
            {
                FallDownTime = Main.rand.NextFloat(80, 120);
                Projectile.netUpdate = true;
            }
            Projectile.velocity.Y += MathF.Sin(Timer * 0.2f) * 0.1f;
            Projectile.rotation += 0.02f;
            Projectile.rotation -= Projectile.velocity.Length() * 0.025f;
            if (Timer > FallDownTime)
            {
                Projectile.tileCollide = true;
                Projectile.velocity.Y += 1f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (float f = 0; f < 12; f++)
            {
                float rot = (f / 12f) * MathHelper.TwoPi;
                Vector2 velOffset = rot.ToRotationVector2() * 4;
                Dust.NewDustPerfect(Projectile.Center, DustID.GemDiamond, velOffset, Scale: 1f);
            }
        }

        public PrimDrawer Trail { get; set; }
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.62f;
            return MathHelper.SmoothStep(364, baseWidth, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.White, Easing.SpikeOutCirc(completionRatio));
        }

        protected virtual void DrawWindTrail(ref Color lightColor)
        {
            Trail ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.SimpleTrail);
            Trail.DrawPrims(Projectile.oldPos, -Main.screenPosition + Projectile.Size / 2, totalTrailPoints: 155);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.RestartDefaults();
            DrawWindTrail(ref lightColor);

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2;

            float drawScale = 1f;
            float drawRotation = Projectile.rotation;
            Color colorToDrawIn = Color.White.MultiplyRGB(lightColor);
            SpriteEffects spriteEffects = SpriteEffects.None;

            spriteBatch.Restart(blendState: BlendState.Additive);


            for (float f = 0f; f < 1f; f += 0.1f)
            {
                Vector2 o = (f * MathHelper.TwoPi).ToRotationVector2() * VectorHelper.Osc(2f, 3f, speed: 3f);
                spriteBatch.Draw(texture, drawPos + o, null, colorToDrawIn, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
            }
            spriteBatch.RestartDefaults();
            spriteBatch.Draw(texture, drawPos, null, colorToDrawIn, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
            return false;
        }
    }
}

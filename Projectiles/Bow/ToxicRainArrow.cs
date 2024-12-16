using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Bow
{
    internal class ToxicRainArrow : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            base.AI();
            Timer++; ProjectileID.Sets.TrailCacheLength[Type] = 4;
            if (Timer % 16 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.GreenTorch, Projectile.velocity * 0.1f, Scale: Main.rand.NextFloat(0.5f, 1f));
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.penetrate < 3)
            {
                Projectile.velocity.Y += 1f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.penetrate--;
            Projectile.velocity *= 0.6f;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            return false;
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Green, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            SpriteBatch spriteBatch = Main.spriteBatch;
       //     spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Green.MultiplyRGB(lightColor), Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.LightningTrail2);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(2))
            {
                target.AddBuff(BuffID.Poisoned, 180);
            }
            if (Main.rand.NextBool(2))
            {
                target.AddBuff(ModContent.BuffType<AcidFlame>(), 180);
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (float f = 0; f < 4; f++)
            {
                Dust.NewDustPerfect(Projectile.Center + Projectile.oldVelocity, ModContent.DustType<GlyphDust>(), -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.NextFloat(0.25f, 1f), Scale: Main.rand.NextFloat(0.25f, 0.75f), newColor: Color.Green);
            }
        }
    }
}

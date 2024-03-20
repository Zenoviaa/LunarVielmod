using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    internal class XX4160Shot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 5;
        }

        private ref float AI_Timer => ref Projectile.ai[0];
        public override void AI()
        {
            AI_Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }



        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slow, 300);
            if (Main.rand.NextBool(3))
            {

                for (int i = 0; i <= 10; i++)
                {
                    Dust.NewDust(base.Projectile.Center, 22, 22, ModContent.DustType<GlowLineDust>(), base.Projectile.velocity.X * 0.2f, base.Projectile.velocity.Y * 0.2f);
                    Dust.NewDust(base.Projectile.Center, 22, 22, ModContent.DustType<GlowDust>(), 0f, 0f, 0, Color.Red, 0.3f);
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 10; i++)
            {
                Dust.NewDust(base.Projectile.Center, 22, 22, ModContent.DustType<GlowLineDust>(), base.Projectile.velocity.X * 0.2f, base.Projectile.velocity.Y * 0.2f);
                Dust.NewDust(base.Projectile.Center, 22, 22, ModContent.DustType<GlowDust>(), 0f, 0f, 0, Color.Red, 0.3f);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.6f;
            return MathHelper.SmoothStep(baseWidth, baseWidth * 0.8f, completionRatio);
        }
        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DarkRed, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Player player = Main.player[Projectile.owner];
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);


            Texture2D Texture2 = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(Texture2, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(Texture2.Width / 2, Texture2.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);


            return false;
        }
    }
}

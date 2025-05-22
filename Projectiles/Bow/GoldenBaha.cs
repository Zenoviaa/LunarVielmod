
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Gores;
using Stellamod.Trails;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Dusts;

namespace Stellamod.Projectiles.Bow
{
    internal class GoldenBaha : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Archarilite Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int gore1 = GoreHelper.TypeFallingLeafWhite;
            int gore2 = GoreHelper.TypeFallingLeafRed;
            for (int i = 0; i < 2; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                Gore.NewGore(Projectile.GetSource_FromThis(), target.position, velocity, gore1);

                velocity = velocity.RotatedByRandom(MathHelper.TwoPi);
                Gore.NewGore(Projectile.GetSource_FromThis(), target.position, velocity, gore2);
            }

            for (int i = 0; i < 4; i++)
            {
                Dust.NewDust(target.position, Projectile.width, Projectile.height, 
                    ModContent.DustType<GunFlash>(), Scale: 0.8f);
                Dust.NewDustPerfect(target.Center, 
                    ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 2)).RotatedByRandom(19.0), 0, Color.DarkGoldenrod, 1f).noGravity = true;
            }

            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.OnFire3, 180);
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.knockBack = 12.9f;
            Projectile.friendly = true;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Timer++;
            if (Timer % 5 == 0)
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, 0f, 0f, 25, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }
            Projectile.velocity *= 1.02f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
        }
      
        public override void OnKill(int timeLeft)
        {
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 512f, 4);
            for (int i = 0; i < 16; i++)
            {
                float progress = (float)i / 16f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 vel = rot.ToRotationVector2();
                Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, vel, 0, default, 1f).noGravity = false;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.7f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightGoldenrodYellow, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
      
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);

            SpriteBatch spriteBatch = Main.spriteBatch;
            int height = Main.player[Projectile.owner].height / 1; // 5 is frame count
            int y = height * Projectile.frame;
            var drawOrigin = texture.Size() / 2;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + Projectile.Size / 2 + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                color *= 0.8f;
                spriteBatch.Draw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, drawOrigin, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.75f * Main.essScale);
        }
    }
}

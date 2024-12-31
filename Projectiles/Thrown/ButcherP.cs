using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown
{
    public class ButcherP : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Beachthrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 3;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 600;
            Projectile.scale = 0.65f;
            AIType = ProjectileID.PossessedHatchet;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Red, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SpikyTrail1);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            return false;
        }

        public override bool PreAI()
        {
            if (Main.rand.NextBool(6))
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Firework_Red, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                int dust1 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Firework_Red, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust1].velocity *= 0f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust1].noGravity = true;
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 20; i++)
            {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Red, 0f, -2f, 0, default(Color), .8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num1].position != Projectile.Center)
                {
                    Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
                }

                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Red, 0f, -2f, 0, default(Color), .8f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num].position != Projectile.Center)
                {
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
                }

                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            }
            for (int i = 0; i < 180; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Red, 0f, -2f, 0, default(Color), 1.5f);
                Main.dust[num].noGravity = true;
                Main.dust[num].scale = 1.9f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                {
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 10f;
                }
            }
            for (int i = 0; i < 80; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Red, 0f, -2f, 0, default(Color), 2.5f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                {
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 10f;
                }
            }
        }
    }
}

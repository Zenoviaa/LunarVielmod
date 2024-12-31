using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Trails;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DreadMire
{

    internal class DreadMireDash : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sun Death");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.timeLeft = 450;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.velocity *= 1.01f;
            Projectile.spriteDirection = Projectile.direction;
            Timer++;
            if (Timer == 2)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
            }


            if(Timer % 6 == 0 && Main.myPlayer == Projectile.owner)
            {
                Vector2 velocity;
                if (Main.rand.NextBool(2))
                {
                    velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2);
                }
                else
                {
                    velocity = Projectile.velocity.RotatedBy(-MathHelper.PiOver2);
                }
                velocity *= 0.3f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ModContent.ProjectileType<DreadSineSkull>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, ModContent.DustType<RuneDust>());
            dust.velocity *= -1f;
            dust.scale *= .8f;
            dust.noGravity = true;
            Vector2 vector2_1 = new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-80, 81));
            vector2_1.Normalize();
            Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 100) * 0.04f);
            dust.velocity = vector2_2;
            vector2_2.Normalize();
            Vector2 vector2_3 = vector2_2 * 34f;
            dust.position = Projectile.Center - vector2_3;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.PaleVioletRed.ToVector3() * 1.75f * Main.essScale);
            Lighting.AddLight(Projectile.Center, Color.DarkRed.ToVector3() * 1.75f * Main.essScale);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Red, 0f, -2f, 0, default, .8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num1].position != Projectile.Center)
                    Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Red, 0f, -2f, 0, default, .8f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num].position != Projectile.Center)
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
            }
            SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.position);
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
            return Color.Lerp(Color.DarkRed, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 scale = new(Projectile.scale, 1f);
            Color drawColor = Projectile.GetAlpha(lightColor);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.TerraTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<RuneDust>(), 0f, 0f, 150, Color.Red, 2f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }

            return false;
        }
    }

}


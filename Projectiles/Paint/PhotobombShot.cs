

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Trails;
using Stellamod.Utilis;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Paint
{
    public class PhotobombShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cactius2");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Bullet);
            AIType = ProjectileID.Bullet;
            Projectile.penetrate = 1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Confused, 180);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
                Projectile.Kill();
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;

                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }

            SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.Center);
            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 8)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob3>(), speed * 2, 0, default(Color), 4f).noGravity = false;
            }

            for (int i = 0; i < 7; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<PaintBlob1>());
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob5>(), (Vector2.One * Main.rand.Next(1, 8)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }
            return false;
        }

        public override bool PreAI()
        {

            Player player = Main.player[Projectile.owner];
            if (Main.rand.NextBool(8))
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob3>(), speed * 2, 0, default(Color), 4f).noGravity = false;

            }

            if (Main.rand.NextBool(8))
            {

                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob5>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;

            }

            if (Main.rand.NextBool(8))
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob4>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;

            }


            if (Main.rand.NextBool(9))
            {
                float speedXa = Main.rand.NextFloat(-60f, 60f);
                float speedYa = Main.rand.Next(-60, 60);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb1>(), (Projectile.damage * 2) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob3>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (Main.rand.NextBool(10))
            {
                float speedXa = Main.rand.NextFloat(-60f, 60f);
                float speedYa = Main.rand.Next(-60, 60);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb2>(), Projectile.damage + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
            }

            if (Main.rand.NextBool(20))
            {
                float speedXa = Main.rand.NextFloat(-60f, 60f);
                float speedYa = Main.rand.Next(-60, 60);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb3>(), (Projectile.damage * 3) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }


            if (Main.rand.NextBool(20))
            {
                float speedXa = Main.rand.NextFloat(-60f, 60f);
                float speedYa = Main.rand.Next(-60, 60);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb5>(), (Projectile.damage * 3) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (Main.rand.NextBool(8))
            {
                float speedXa = Main.rand.NextFloat(-60f, 60f);
                float speedYa = Main.rand.Next(-60, 60);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb4>(), (Projectile.damage * 3) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (Main.rand.NextBool(12))
            {
                float speedXa = Main.rand.NextFloat(-60f, 60f);
                float speedYa = Main.rand.Next(-60, 60);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb6>(), (Projectile.damage * 3) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (player.GetModPlayer<MyPlayer>().PPPaintI)
            {
                if (Main.rand.NextBool(10))
                {
                    float speedXa = Main.rand.NextFloat(-60f, 60f);
                    float speedYa = Main.rand.Next(-60, 60);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb7>(), (Projectile.damage * 4) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                    Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob5>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                }
            }

            if (player.GetModPlayer<MyPlayer>().PPPaintII)
            {
                if (Main.rand.NextBool(20))
                {
                    float speedXa = Main.rand.NextFloat(-35f, 35f);
                    float speedYa = Main.rand.Next(-35, 35);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb8>(), (Projectile.damage * 3) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                    Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob1>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                }
            }

        
            return true;
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }
        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Turquoise, Color.Transparent, completionRatio) * 0.7f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<PaintBlob2>(), 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.75f * Main.essScale);
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<PaintBlob3>(), 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.Center);
            for (int i = 0; i < 15; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);

                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob1>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }


            if (Main.rand.NextBool(2))
            {
                float speedXa = Main.rand.NextFloat(-80f, 80f);
                float speedYa = Main.rand.Next(-80, 80);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb1>(), (Projectile.damage * 2) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob3>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (Main.rand.NextBool(1))
            {
                float speedXa = Main.rand.NextFloat(-80f, 80f);
                float speedYa = Main.rand.Next(-80, 80);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb2>(), Projectile.damage + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
            }

            if (Main.rand.NextBool(4))
            {
                float speedXa = Main.rand.NextFloat(-80f, 80f);
                float speedYa = Main.rand.Next(-80, 80);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb3>(), (Projectile.damage * 3) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (player.GetModPlayer<MyPlayer>().PPPaintI)
            {
                if (Main.rand.NextBool(4))
                {
                    float speedXa = Main.rand.NextFloat(-80f, 80f);
                    float speedYa = Main.rand.Next(-80, 80);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb7>(), (Projectile.damage * 4) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                    Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob5>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                }
            }

            if (player.GetModPlayer<MyPlayer>().PPPaintII)
            {
                if (Main.rand.NextBool(7))
                {
                    float speedXa = Main.rand.NextFloat(-35f, 35f);
                    float speedYa = Main.rand.Next(-35, 35);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb8>(), (Projectile.damage * 3) + player.GetModPlayer<MyPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                    Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob1>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                }
            }
        }
    }
}
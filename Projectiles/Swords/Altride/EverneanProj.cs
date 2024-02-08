using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords.Altride
{
    internal class EverneanProj : ModProjectile
    {
        bool Moved;
        Vector2 StartVelocity;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gladiator Spear");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            base.Projectile.penetrate = -1;
            base.Projectile.width = 30;
            base.Projectile.height = 30;
            base.Projectile.timeLeft = 700;
            base.Projectile.alpha = 255;
            base.Projectile.friendly = true;
            base.Projectile.hostile = false;
            base.Projectile.ignoreWater = true;
            base.Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Projectile.ai[1]++;
            if (!Moved && Projectile.ai[1] >= 0)
            {
                Moved = true;
            }
            if (Projectile.ai[1] == 1)
            {

                StartVelocity = Projectile.velocity;
            }
            if (Projectile.ai[1] == 2)
            {


                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                Projectile.velocity = -Projectile.velocity;
            }
            if (Projectile.ai[1] == 5 || Projectile.ai[1] == 10 || Projectile.ai[1] == 15 || Projectile.ai[1] == 20)
            {
                var EntitySource = Projectile.GetSource_FromThis();
                if (Main.rand.NextBool(8))
                {
                    int randy = Main.rand.Next(-50, 50);
                    int randx = Main.rand.Next(-50, 50);
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X + randx, Projectile.Center.Y + randy, StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Altride4>(), Projectile.damage * 4, 1, Projectile.owner, 0, 0);
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X + randx, Projectile.Center.Y + randy, StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Radial2>(), Projectile.damage * 0, 1, Projectile.owner, 0, 0);
                }

                if (Main.rand.NextBool(3))
                {
                    int randy = Main.rand.Next(-50, 50);
                    int randx = Main.rand.Next(-50, 50);
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X + randx, Projectile.Center.Y + randy, StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Altride4>(), Projectile.damage * 2, 1, Projectile.owner, 0, 0);
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X + randx, Projectile.Center.Y + randy, StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Radial2>(), Projectile.damage * 0, 1, Projectile.owner, 0, 0);

                }
                else
                {
                    int Sound = Main.rand.Next(1, 4);
                    if (Sound == 1)
                    {

                    }
                   
                    else
                    {

                    }
                    int randy = Main.rand.Next(-75, 75);
                    int randx = Main.rand.Next(-75, 75);
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X + randx, Projectile.Center.Y + randy, StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Altride5>(), Projectile.damage, 1, Projectile.owner, 0, 0);
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X + randx, Projectile.Center.Y + randy, StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Radial>(), Projectile.damage * 0, 1, Projectile.owner, 0, 0);


                }
            }
            if (Projectile.ai[1] >= 0 && Projectile.ai[1] <= 20)
            {
                Projectile.velocity *= .86f;

            }
            if (Projectile.ai[1] == 20)
            {
                Projectile.velocity = -Projectile.velocity;
            }
            if (Projectile.ai[1] >= 21 && Projectile.ai[1] <= 60)
            {
                Projectile.velocity /= .90f;

            }
            if (Projectile.ai[1] == 60)
            {
                Projectile.tileCollide = true;
            }




        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Gold, 0f, -2f, 0, default(Color), .8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num1].position != Projectile.Center)
                    Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Gold, 0f, -2f, 0, default(Color), .8f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num].position != Projectile.Center)
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Gold, 0f, 0f, 150, Color.Gold, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Gold) * (float)(((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) / 2);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return true;
        }
        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.PaleGoldenrod.ToVector3() * 1.75f * Main.essScale);

        }
    }
}
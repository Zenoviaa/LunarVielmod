using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;


namespace Stellamod.NPCs.Bosses.Jack
{
    public class JackFire : ModProjectile
    {
        public float VEL = 1;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Red Skull");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
        }

        public override bool PreAI()
        {
            Projectile.alpha -= 40;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            Projectile.spriteDirection = Projectile.direction;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4)
                    Projectile.frame = 0;

            }
            return true;
        }
        float alphaCounter = 1.5f;
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 50; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FlameBurst, 0f, -2f, 0, default(Color), 1.5f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                {
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
                }
            }
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.position);
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.27f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.27f * (5 + 0.6f), SpriteEffects.None, 0f);
            Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3() * 1.75f * Main.essScale);

        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            Projectile.localAI[0] += 1f;

            Player player;
            if ((player = VectorHelper.GetNearestPlayerDirect(base.Projectile.position, Alive: true)) != null)
            {
                if (Projectile.position.X <= player.position.X)
                {
                    Projectile.velocity.X += 0.1f;
                }
                else
                {
                    Projectile.velocity.X -= 0.1f;
                }
            }

            Projectile.velocity.Y += 0.01f;

            VEL *= 1.01f;

            if (Main.rand.NextBool(29))
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FlameBurst, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.5f;
            }

            if (Main.rand.NextBool(29))
            {
                int dust3 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FlameBurst, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust3].noGravity = true;
                Main.dust[dust3].scale = 1.5f;
            }

        }

    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Projectiles.Gun
{
    public class DeathShotBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("DeathShotBomb");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = 0;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 30;
            Projectile.timeLeft = 300;
            Projectile.height = 1;
            Projectile.width = 1;
            Projectile.extraUpdates = 1;
        }

        private Vector2 ProjectilePos;
        private Vector2 alphaPos;
        private float alphaCounter = 0.1f;
        private float alphaCounter3 = 0.1f;
        private float alphaCounter2 = 0.2f;
        public override void AI()
        {
            var EntitySource = Projectile.GetSource_FromThis();
            ProjectilePos.Y = Projectile.Center.Y;
            ProjectilePos.X = Projectile.Center.X;
            Projectile.ai[0]++;
            if (Projectile.ai[0] == 2)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.position, 2048f, 54f);
                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DeathShotBomb"), Projectile.position);
                }
                else
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DeathShotBomb2"), Projectile.position);
                }
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<SmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, default(Color), 1f).noGravity = true;
                }

                for (int i = 0; i < 13; i++)
                {
                    Projectile.NewProjectile(EntitySource, ProjectilePos.X += Main.rand.Next(-10, 10), ProjectilePos.Y += Main.rand.Next(-10, 10), Main.rand.Next(-2, 2), Main.rand.Next(-2, 2), 
                        ModContent.ProjectileType<DeathShotBombFX>(), Projectile.damage / 2, 1, Projectile.owner, 0, 0);
                }
            }

            if (Projectile.ai[0] <= 100)
            {
                if (alphaCounter <= 1.5)
                {
                    alphaCounter3 += 0.08f;
                    alphaCounter += 0.08f;
                }
            }
            else
            {
                if (alphaCounter >= 0)
                {
                    alphaCounter -= 0.08f;
                }
                else
                {
                    Projectile.active = false;
                }
            }

            alphaCounter2 *= 1.01f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            alphaPos.X = Projectile.Center.X;
            alphaPos.Y = Projectile.Center.Y - alphaCounter2;
            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/Skull").Value;
            Main.spriteBatch.Draw(texture2D4, alphaPos - Main.screenPosition, null, new Color((int)(65f * alphaCounter), (int)(15f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(24, 24), 0.4f * (alphaCounter3 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, alphaPos - Main.screenPosition, null, new Color((int)(65f * alphaCounter), (int)(15f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(24, 24), 0.4f * (alphaCounter3 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, alphaPos - Main.screenPosition, null, new Color((int)(65f * alphaCounter), (int)(15f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(24, 24), 0.4f * (alphaCounter3 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, alphaPos - Main.screenPosition, null, new Color((int)(65f * alphaCounter), (int)(15f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(24, 24), 0.4f * (alphaCounter3 + 0.6f), SpriteEffects.None, 0f);
            return true;
        }
    }
}
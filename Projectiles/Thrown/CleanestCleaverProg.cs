
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.Swords;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown
{
    public class CleanestCleaverProg : ModProjectile
    {
        public bool Sound;
        public int Bloody;
        public Vector2 ProjectileVelocityBeforeHit;
        public bool Hit;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("virulent Missile");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 11;
            Projectile.width = 21;
            Projectile.height = 25;
            Projectile.friendly = true;
            Projectile.timeLeft = 540;

        }

        public override void AI()
        {
            if (Hit)
            {
                Projectile.ai[0]++;
                if (Projectile.ai[0] <= 7)
                {
                    if (!Sound)
                    {
                        if (Bloody == 3)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CleanestCleaver3"), Projectile.position);
                        }
                        else
                        {
                            int Sound = Main.rand.Next(1, 3);
                            if (Sound == 1)
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CleanestCleaver1"), Projectile.position);
                            }
                            else
                            {
                                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CleanestCleaver2"), Projectile.position);
                            }
                        }
                        Sound = true;
                    }


                    Projectile.friendly = false;
                    Projectile.velocity = Vector2.Zero;
                }
                else
                {

                    Projectile.friendly = true;
                    Hit = false;
                    Projectile.velocity = ProjectileVelocityBeforeHit;
                }
            }
            else
            {
                Sound = false;
                if (Bloody == 3)
                {
                    Projectile.velocity.Y += 0.3f;
                    Projectile.velocity.X *= 0.96f;
                    Projectile.ai[0] = 0;
   
                }
                else
                {
                    Projectile.ai[0] = 0;
                }

                if(Projectile.velocity.X >= 0)
                {
                    Projectile.rotation += 0.35f;
                }
                else
                {
                    Projectile.rotation -= 0.35f;
                }
            }
        }
        public override void PostDraw(Color lightColor)
        {
            string B3Texture = Texture + "_Blood3";
            string B2Texture = Texture + "_Blood2";
            string B1Texture = Texture + "_Blood1";
            Texture2D Blood1Texture = ModContent.Request<Texture2D>(B1Texture).Value;
            Texture2D Blood2Texture = ModContent.Request<Texture2D>(B2Texture).Value;
            Texture2D Blood3Texture = ModContent.Request<Texture2D>(B3Texture).Value;

            Vector2 textureSize = new Vector2(21, 25);
            Vector2 drawOrigin = textureSize;

            //Lerping
            if(Bloody == 1)
            {
                Color drawColor = Color.Lerp(Color.Transparent, Color.White, 1);
                Vector2 drawPosition = Projectile.position - Main.screenPosition + drawOrigin;
                Main.spriteBatch.Draw(Blood1Texture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            if (Bloody == 2)
            {
                Color drawColor = Color.Lerp(Color.Transparent, Color.White, 1);
                Vector2 drawPosition = Projectile.position - Main.screenPosition + drawOrigin;
                Main.spriteBatch.Draw(Blood2Texture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            if (Bloody == 3)
            {
                Color drawColor = Color.Lerp(Color.Transparent, Color.White, 1);
                Vector2 drawPosition = Projectile.position - Main.screenPosition + drawOrigin;
                Main.spriteBatch.Draw(Blood3Texture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }

        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Bloody == 3)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CleanestCleaver4"), Projectile.position);
                Projectile.Kill();
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 63f);
                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 180, Color.DarkGray, Main.rand.NextFloat(1, 2.1f)).noGravity = true;
                }
                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<BloodDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 1f).noGravity = true;
                }
            }
            else
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 13f);
                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CleanestCleaver6"), Projectile.position);
                }
                else
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CleanestCleaver5"), Projectile.position);
                }
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 180, Color.Gray, Main.rand.NextFloat(0.5f, 1.5f)).noGravity = true;
                }
                Projectile.Kill();
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Hit && Bloody <= 2)
            {
                Bloody += 1;
                if(Bloody == 3)
                {

                    Projectile.velocity.Y = -10;
                    Projectile.velocity.X = -Projectile.velocity.X / 2;
                }
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 16f);
                for (int i = 0; i < 15; i++)
                {
                    SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
                    Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<BloodDust>());
                    Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<BloodDust>());
                }
                Hit = true;
                ProjectileVelocityBeforeHit = Projectile.velocity;
            }


        }
        public override bool PreDraw(ref Color lightColor)
        {    
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.White, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }
    }
}
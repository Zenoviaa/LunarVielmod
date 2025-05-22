
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Core.Utils;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;


namespace Stellamod.NPCs.Bosses.Veiizal
{
    public class VeiizalBullet : ModProjectile
    {
   
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Red Skull");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
        }


        float alphaCounter = 1.5f;
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 50; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Red, 0f, -2f, 0, default, 1.5f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                {
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
                }
            }
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
            Vector2 scale = new(Projectile.scale, 1f);
            Color drawColor = Projectile.GetAlpha(lightColor);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.WhispyTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);


            return false;
        }
        bool initialized = false;
        Vector2 initialSpeed = Vector2.Zero;
        public float Timer = 0;
        float SpeedADD = 0;
        float distance = 8;
        float t = 0;
        public float TimerAgain = 0;
        public override void AI()
        {

            Timer++;
            TimerAgain += 15;

            if (Timer == 1)
            {

                SpeedADD  = Main.rand.NextFloat(0.5f, 0.9f);;

            }


            Projectile.velocity *= 0.991f;
            alphaCounter += 0.04f;
            int rightValue = (int)Projectile.ai[1] - 1;
            if (rightValue < (double)Main.projectile.Length && rightValue != -1)
            {
                Projectile other = Main.projectile[rightValue];
                Vector2 direction9 = other.Center - Projectile.Center;
                int distance = (int)Math.Sqrt((direction9.X * direction9.X) + (direction9.Y * direction9.Y));
                direction9.Normalize();
            }
            if (!initialized)
            {
                initialSpeed = Projectile.velocity;
                initialized = true;
            }
            if (initialSpeed.Length() < 20)
                initialSpeed *= 1.51f;
            Projectile.spriteDirection = 1;
            if (Projectile.ai[0] > 0)
            {
                Projectile.spriteDirection = 0;
            }

            distance += 0.4f;


            Vector2 offset = initialSpeed.RotatedBy(Math.PI / 2);
            offset.Normalize();
            offset *= (float)(Math.Cos(TimerAgain * (Math.PI / 180)) * (distance / 3));
            Projectile.velocity = initialSpeed + offset * SpeedADD;


            t += 0.01f;

            Projectile.ai[0]++;
        }

    }
}
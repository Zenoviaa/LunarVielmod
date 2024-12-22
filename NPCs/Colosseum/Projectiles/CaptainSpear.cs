using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Colosseum.Projectiles
{
    internal class CaptainSpear : ModProjectile
    {
        private Vector2 InitialVelocity;
        private ref float Timer => ref Projectile.ai[0];
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(InitialVelocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            InitialVelocity = reader.ReadVector2();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hostile = false;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.timeLeft = 180;
            Projectile.light = 0.1f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            //We want this thing to go backward and then back out basically, it should look pretty cool
            Timer++;
            if (Timer == 1)
            {
                InitialVelocity = Projectile.velocity;
                Projectile.velocity = -Projectile.velocity;
                SoundStyle soundStyle;
                switch (Main.rand.Next(2))
                {
                    default:
                    case 0:
                        soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GladiatorMirage1");
                        break;
                    case 1:
                        soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GladiatorMirage2");
                        break;
                }
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }

            if (Timer % 16 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond, Scale: 0.5f);
            }


            if (Timer < 30)
            {
                Projectile.velocity *= 0.96f;
                Projectile.rotation = InitialVelocity.ToRotation();
            }
            if (Timer == 30)
            {
                Projectile.velocity = InitialVelocity;
            }
            if (Timer > 30)
            {
                Projectile.hostile = true;
                Projectile.velocity.Y += 0.05f;
                Projectile.velocity *= 1.01f;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            if (Timer > 40)
            {

                Projectile.tileCollide = true;
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float rotation = Projectile.rotation;
            float drawScale = 1f;

            spriteBatch.Restart(blendState: BlendState.Additive);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float trailProgress = (float)i / (float)Projectile.oldPos.Length;
                Vector2 oldPos = Projectile.oldPos[i];
                Vector2 glowDrawPos = oldPos + Projectile.Size / 2;
                Color startColor = Color.White;
                Color endColor = Color.Transparent;
                Color glowDrawColor = Color.Lerp(startColor, endColor, trailProgress);
                spriteBatch.Draw(texture, glowDrawPos, null, glowDrawColor, Projectile.oldRot[i], drawOrigin, drawScale, SpriteEffects.None, 0f);
            }
            spriteBatch.RestartDefaults();
            float b = (Timer / 30f);
            b = MathHelper.Clamp(b, 0f, 1f);
            spriteBatch.Draw(texture, drawPos, null, drawColor * b, rotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 4; i++)
            {
                Vector2 vel = -Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(15)).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 5f);
                Dust.NewDustPerfect(Projectile.Center, DustID.GemDiamond, vel);
            }
        }
    }
}

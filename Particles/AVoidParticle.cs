
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Particles
{
    public class AVoidParticle : Particle
    {
        private int frameCount;
        private int frameTick;

        public override void SetDefaults()
        {
            width = 1;
            height = 1;
            Scale = 3f;
            timeLeft = 40;

        }

        public override void AI()
        {


            rotation += Utils.Clamp(velocity.X * 0f, -ai[0], ai[0]);
            velocity *= 0.99f;


            if (Scale <= 0f)
                active = false;


            color = Color.Lerp(new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0f), Color.Multiply(new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0f), 0.5f), (360f - timeLeft) / 360f);

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D tex = Request<Texture2D>("Stellamod/Particles/AVoidParticle2").Value;
            float alpha = timeLeft <= 20 ? 1f - 1f / 20f * (20 - timeLeft) : 1f;

            if (alpha < 0f)
                alpha = 0f;

            Color color = Color.Multiply(new(1f, 1f, 1f, 0), alpha);
            spriteBatch.Draw(tex, position - Main.screenPosition, new Rectangle(0, 0, tex.Width, tex.Height), color, MathHelper.ToRadians(ai[0]).AngleLerp(MathHelper.ToRadians((ai[0] * 180f)), (120f - timeLeft) / 120f), new Vector2(tex.Width / 2f, tex.Height / 2f), scale, SpriteEffects.None, 0f);
            return false;
        }
        public void Spawn()
        {
            ai[1] = Main.rand.NextFloat(2f, 8f) / 1f;
            ai[2] = Main.rand.Next(0, 2);
            ai[3] = Main.rand.NextFloat(0f, 20f);
            timeLeft = (int)ai[4] > 0 ? (int)ai[4] : timeLeft;
        }
    }

}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Particles
{
    public class AuroranSlashParticle : Particle
    {
        private int frameCount;
        private int frameTick;

        public override void SetDefaults()
        {
            width = 1;
            height = 1;
            Scale = 1f;
            timeLeft = 18;

        }

        public override void AI()
        {
            Player cuh = Main.LocalPlayer;


            position = Main.projectile[(int)ai[1]].Center;




            color = Color.Lerp(new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0f), Color.Multiply(new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0f), 0.5f), (360f - timeLeft) / 360f);

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Player player = Main.LocalPlayer;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (player.GetModPlayer<CorrectSwing>().SwingChange == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            else
            {
                spriteEffects = SpriteEffects.None;
            }


            Texture2D tex3 = Request<Texture2D>("Stellamod/Particles/AuroranSlash").Value;
            Texture2D tex4 = Request<Texture2D>("Stellamod/Particles/AuroranSlashOpp").Value;

            float alpha = timeLeft <= 20 ? 1f - 1f / 20f * (20 - timeLeft) : 1f;
            if (alpha < 0f) alpha = 0f;
            Color color = Color.Multiply(new(0.5f, 1.2f, 1.5f, 0), alpha);
            // spriteBatch.Draw(tex2, position - Main.screenPosition, new Rectangle(0, 0, tex2.Width, tex2.Height), color, ai[1].InRadians().AngleLerp((ai[1] * 180f).InRadians(), (120f - timeLeft) / 120f), new Vector2(tex2.Width / 2f, tex2.Height / 2f), 0.05f * Scale, SpriteEffects.None, 0f);
            if (Main.projectile[(int)ai[1]].direction == -1)
            {
                spriteBatch.Draw(tex4, Center - Main.screenPosition, tex4.AnimationFrame(ref frameCount, ref frameTick, 1, 18, true), color, velocity.ToRotation() + 180, new Vector2(111f, 111f) * 0.5f, 3f * Scale, spriteEffects, 0f);


            }
            else
            {
                spriteBatch.Draw(tex3, Center - Main.screenPosition, tex3.AnimationFrame(ref frameCount, ref frameTick, 1, 18, true), color, velocity.ToRotation() + 180, new Vector2(111f, 111f) * 0.5f, 3f * Scale, spriteEffects, 0f);

            }

            // spriteBatch.Draw(tex, position - Main.screenPosition, tex.AnimationFrame(ref frameCount, ref frameTick, 4, 7, true), color, 0f, new Vector2(width / 2, height / 2), 0.5f, SpriteEffects.None, 0f);
            return false;
        }
    }
 
}
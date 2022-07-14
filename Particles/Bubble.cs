using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Particles
{
    public class Bubble : Particle
    {
        private const bool V = true;
        private int frameCount;
        private int frameTick;
        private bool ProjDed;

        public override void SetDefaults()
        {
            width = 1;
            height = 1;
            scale = 1f;
            timeLeft = 300;
        }

        public override void AI()
        {
            Player player = Main.LocalPlayer;
            rotation += 0.4f;

            position = Main.projectile[(int)ai[0]].Center;
            if (!Main.projectile[(int)ai[0]].active)
            {
                if (!ProjDed)
                {
                    timeLeft = 44;
                }
                ProjDed = true;






                
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {

            Texture2D tex3 = Request<Texture2D>("Stellamod/Particles/Bubble").Value;
            spriteBatch.Draw(tex3, Bottom - Main.screenPosition, tex3.AnimationFrame(ref frameCount, ref frameTick, 22, 2, true), color, velocity.ToRotation() + 180, new Vector2(270f, 249f) * 0.5f, 1.35f * scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
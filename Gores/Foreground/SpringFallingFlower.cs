using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System;
using Terraria;

namespace Stellamod.Gores.Foreground
{
    public class SpringFallingFlower : ParallaxHelper
    {
        private int timer = 0;
        private int offscreenTimer = 0;

        public static int SpawnChance(Player p)
        {
            return 5;
        }

        public SpringFallingFlower(Vector2 pos) : base(pos, Vector2.Zero, 1f, "SpringFallingFlower")
        {
            parallax = Main.rand.Next(25, 150) * 0.01f;
            scale = parallax + 0.5f;
            source = new Rectangle(0, Main.rand.Next(9) * 16, 16, 16);
        }

        public override void Update()
        {
            base.Update();
            float xVel = (float)Math.Sin(timer++ * 0.036) * 0.48f * scale;
            velocity.X = xVel + (position.Y < Main.worldSurface * 16 ? Main.windSpeedCurrent * 8 : 0);
            velocity.Y = (-Math.Abs(xVel) + scale) * 0.4f;
            rotation = velocity.X * -0.5f;

            if (!new Rectangle((int)Main.screenPosition.X - 60, (int)Main.screenPosition.Y - 60, Main.screenWidth + 120, Main.screenHeight + 120).Contains(drawPosition.ToPoint()))
                offscreenTimer++;
            else
                offscreenTimer = 0;

            if (offscreenTimer > 900)
                killMe = true;
        }

        public override void Draw()
        {
            drawPosition = position + ParallaxPosition();
            Color lightColour = Lighting.GetColor((int)(drawPosition.X / 16f), (int)(drawPosition.Y / 16f));
            Color frontColour = position.Y / 16f < Main.worldSurface ? Main.ColorOfTheSkies : new Color(85, 85, 85);
            drawColor = Color.Lerp(lightColour, frontColour, (parallax - 0.25f) / 1.25f);
            base.Draw();
        }
    }
}

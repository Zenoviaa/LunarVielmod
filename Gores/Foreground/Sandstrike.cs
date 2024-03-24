using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System;
using Terraria;

namespace Stellamod.Gores.Foreground
{



    public class Sandstrike : ParallaxHelper
    {
        private int timer = 0;
        private int offscreenTimer = 0;

        public static int SpawnChance(Player p)
        {

            if (Main.raining && p.Center.Y / 16f < Main.worldSurface)
                return 5;
            if (p.Center.Y / 16f > Main.worldSurface)
                return 5;
            if (p.Center.Y / 16f < Main.worldSurface)
                return 5;
            return -1;
        }

        public Sandstrike(Vector2 pos) : base(pos, Vector2.Zero, 1f, "Sandstrike")
        {
            parallax = Main.rand.Next(25, 150) * 0.01f;
            scale = parallax + 0.5f;

            source = new Rectangle(0, 0, 20, 22);
            int r = Main.rand.Next(30);
            if (r < 4)
                source.Y = 23;
            else if (r > 25)
                source.Y = 46;
            else
                source.Y = Main.rand.NextBool() ? 0 : 69;
        }

        public override void Update()
        {
            base.Update();
            float xVel = (float)Math.Sin(timer++ * 0.036) * -0.48f * scale;
            velocity.X = xVel + (position.Y < Main.worldSurface * 16 ? Main.windSpeedCurrent * 8 : 0);
            velocity.Y = (Math.Abs(xVel) + scale) * 0.4f;
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
            Color frontColour = (position.Y / 16f < Main.worldSurface) ? Main.ColorOfTheSkies : new Color(85, 85, 85);
            drawColor = Color.Lerp(lightColour, frontColour, (parallax - (0.25f)) / 1.25f);

            base.Draw();
        }
    }
}
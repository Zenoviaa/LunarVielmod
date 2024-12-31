using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Gores
{
    public abstract class Splash : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.numFrames = 3;
            gore.frame = (byte)Main.rand.Next(3);
            gore.alpha = Main.rand.Next(0, 125);
            gore.timeLeft = 805;
            UpdateType = 910;
        }

        public override bool Update(Gore gore)
        {
            gore.frameCounter = 0;
            gore.velocity = Vector2.Zero;
            gore.timeLeft--;
            if(gore.timeLeft<= 0)
            {
                gore.active = false;
            }
            return false;
        }
    }

    public class SplashRed : Splash
    {

    }


    public class SplashBlack : Splash
    {

    }


    public class SplashGreen : Splash
    {

    }


    public class SplashOrange : Splash
    {

    }

    public class SplashBlue : Splash
    {

    }

    public class SplashYellow : Splash
    {

    }
}
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Particles
{
    public static class ParticleUtils
    {



        public static int ParticleType<T>() where T : LegacyParticle => ModContent.GetInstance<T>()?.Type ?? 0;

        public static bool OnScreen(Vector2 pos) => pos.X > -16 && pos.X < Main.screenWidth + 16 && pos.Y > -16 && pos.Y < Main.screenHeight + 16;

    }
}

using Terraria.ModLoader;

namespace Stellamod
{
    public class Stellamod : Mod
    {
        public Stellamod()
        {
            Instance = this;

        }

        public static Stellamod Instance;
        public override void Load()
        {
            Instance = this;
        }
    }
}


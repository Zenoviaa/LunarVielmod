using Terraria.ModLoader;

namespace Stellamod
{


    public class Stellamod : Mod
    {
        public static Mod Instance { get; private set; }
        public override void Load()
        {
            base.Load();
            Instance = this;
            ShaderLoader.LoadShaders(this);
        }
    }
}


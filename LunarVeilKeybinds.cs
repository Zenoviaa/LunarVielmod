using Terraria.ModLoader;

namespace Stellamod
{
    internal class LunarVeilKeybinds : ModSystem
    {
        public static ModKeybind DashKeybind { get; private set; }
        public override void Load()
        {
            // Register keybinds            
            DashKeybind = KeybindLoader.RegisterKeybind(Mod, "Dash", "F");
        }
    }
}

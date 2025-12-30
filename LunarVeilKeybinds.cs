using Terraria.ModLoader;

namespace Stellamod
{
    public class LunarVeilKeybinds : ModSystem
    {
        public static ModKeybind FlaskKeybind { get; private set; }
        public static ModKeybind DashKeybind { get; private set; }
        public static ModKeybind QuestKeybind { get; private set; }
        public static ModKeybind BellKeybind { get; private set; }

        public static ModKeybind DecorRotateLeft { get; private set; }
        public static ModKeybind DecorRotateRight { get; private set; }
        public static ModKeybind DecorPrevFrame { get; private set; }
        public static ModKeybind DecorNextFrame { get; private set; }
        public static ModKeybind DecorUpscale { get; private set; }
        public static ModKeybind DecorDownscale { get; private set; }
        public static ModKeybind DecorUpZ { get; private set; }
        public static ModKeybind DecorDownZ { get; private set; }
        public override void Load()
        {
            // Register keybinds            
            DashKeybind = KeybindLoader.RegisterKeybind(Mod, "Dash", "F");
            QuestKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Questbook", "Q");
            FlaskKeybind = KeybindLoader.RegisterKeybind(Mod, "Use Xixian Flask", "G");
            BellKeybind = KeybindLoader.RegisterKeybind(Mod, "Use Summoning Bell", "R");


            DecorRotateLeft = KeybindLoader.RegisterKeybind(Mod, "Rotate Decoration Left", "T");
            DecorRotateRight = KeybindLoader.RegisterKeybind(Mod, "Rotate Decoration Right", "Y");

            DecorPrevFrame = KeybindLoader.RegisterKeybind(Mod, "Previous Decoration Frame", "O");
            DecorNextFrame = KeybindLoader.RegisterKeybind(Mod, "Next Decoration Frame", "P");

            DecorUpscale = KeybindLoader.RegisterKeybind(Mod, "Upscale Decoration", "K");
            DecorDownscale = KeybindLoader.RegisterKeybind(Mod, "Downscale Decoration", "L");

            DecorUpZ = KeybindLoader.RegisterKeybind(Mod, "Increase Decoration Z", ",");
            DecorDownZ = KeybindLoader.RegisterKeybind(Mod, "Decrease Decoration Z", ".");
        }
    }
}

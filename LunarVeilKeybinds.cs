
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod
{
    public static class ModKeybindExtensions
    {
        public static string AssignedKeybindString(this ModKeybind keybind)
        {

            List<string> keys = keybind.GetAssignedKeys();
            if(keys.Count == 0)
            {
                return string.Empty;
            }
            if(keys.Count == 1)
            {
                return keys[0];
            }
            string assignedKeys = string.Empty;
            for(int i = 0; i < keys.Count; i++)
            {
                assignedKeys += keys[i];
                if(i + 1 < keys.Count)
                {
                    assignedKeys += " ";
                }
            }
            return assignedKeys;
        }
    }
    public class LunarVeilKeybinds : ModSystem
    {
        public static ModKeybind AbilityKeybind { get; private set; }
        public static ModKeybind FlaskKeybind { get; private set; }
        public static ModKeybind DashKeybind { get; private set; }
        public static ModKeybind QuestKeybind { get; private set; }
        public static ModKeybind BellKeybind { get; private set; }
        public static ModKeybind ToolKeybind { get; private set; }

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
            AbilityKeybind = KeybindLoader.RegisterKeybind(Mod, "Armor Ability", "J");
            DashKeybind = KeybindLoader.RegisterKeybind(Mod, "Dash", "F");
            QuestKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Questbook", "Q");
            FlaskKeybind = KeybindLoader.RegisterKeybind(Mod, "Use Xixian Flask", "G");
            BellKeybind = KeybindLoader.RegisterKeybind(Mod, "Use Summoning Bell", "R");
            ToolKeybind = KeybindLoader.RegisterKeybind(Mod, "Use Combat Tool", "T");


            DecorRotateLeft = KeybindLoader.RegisterKeybind(Mod, "Rotate Decoration Left", "T");
            DecorRotateRight = KeybindLoader.RegisterKeybind(Mod, "Rotate Decoration Right", "Y");

            DecorPrevFrame = KeybindLoader.RegisterKeybind(Mod, "Previous Decoration Frame", "O");
            DecorNextFrame = KeybindLoader.RegisterKeybind(Mod, "Next Decoration Frame", "P");

            DecorUpscale = KeybindLoader.RegisterKeybind(Mod, "Upscale Decoration", "K");
            DecorDownscale = KeybindLoader.RegisterKeybind(Mod, "Downscale Decoration", "L");

            DecorUpZ = KeybindLoader.RegisterKeybind(Mod, "Increase Decoration Z", ",");
            DecorDownZ = KeybindLoader.RegisterKeybind(Mod, "Decrease Decoration Z", ".");
        }
        public override void Unload()
        {
            base.Unload();
            AbilityKeybind = null;
            DashKeybind = null;
            QuestKeybind = null;
            FlaskKeybind = null;
            BellKeybind = null;
            ToolKeybind = null;
            DecorRotateLeft = null;
            DecorRotateRight = null;
            DecorPrevFrame = null;
            DecorNextFrame = null;
            DecorUpscale = null;
            DecorDownscale = null;
            DecorUpZ = null;
            DecorDownZ = null;
        }
    }
}

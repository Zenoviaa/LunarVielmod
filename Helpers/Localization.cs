using System;
using Terraria.Localization;
using Terraria.ModLoader;
namespace Stellamod.Helpers
{
    public static class LangText
    {
        public static string Chat(ModNPC npc, string Path)
        {
            return Language.GetTextValue($"Mods.Stellamod.NPCs.{npc.Name}." + Path);
        }
        public static string Chat(ModNPC npc, string Path, object arg0)
        {
            return Language.GetTextValue($"Mods.Stellamod.NPCs.{npc.Name}." + Path, arg0);
        }
        public static LocalizedText CreateBestiary(ModNPC npc, string Text, string key = null)
        {
            return Language.GetOrRegister($"Mods.Stellamod.NPCs.{npc.Name}.Bestiary" + key, () => Text);
        }
        /// <summary>
        /// OrginText doesn't influence anything.
        /// You should edit Mods.Stellamod.NPCs.hjson instead of OrginText.
        /// </summary>
        /// <param name="OrginText"></param>
        /// <returns></returns>
        public static string Bestiary(ModNPC npc, string OrginText, string key = null)
        {
            //return (string)Language.GetOrRegister($"Mods.Stellamod.NPCs.{npc.Name}.Bestiary" + key, () => OrginText);
            return Language.GetTextValue($"Mods.Stellamod.NPCs.{npc.Name}.Bestiary" + key, OrginText);
        }

        public static string Tooltip(ModItem item, string key)
        {
            return Language.GetTextValue($"Mods.Stellamod.NPCs.{item.Name}." + key);
        }
    }
}
﻿using Stellamod.Core.ArmorReforge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Core.Helpers
{
    public static class LangText
    {
        public static string Map(string localizationKey, string Path)
        {
            return Language.GetTextValue($"Mods.Stellamod.Map.{localizationKey}." + Path);
        }
        /*
        public static string Quest(Quest quest, string Path)
        {
            return Language.GetTextValue($"Mods.Stellamod.Quests.{quest.Name}." + Path);
        }*/
        public static string Medallion(string Path)
        {
            return Language.GetTextValue($"Mods.Stellamod.Items.Medallion." + Path);
        }

        public static string TownDialogue(string Path)
        {
            return Language.GetTextValue($"Mods.Stellamod.TownDialogue." + Path);
        }

        public static string Chat(ModNPC npc, string Path)
        {
            return Language.GetTextValue($"Mods.Stellamod.NPCs.{npc.Name}." + Path);
        }
        public static string Chat(ModNPC npc, string Path, object arg0)
        {
            return Language.GetTextValue($"Mods.Stellamod.NPCs.{npc.Name}." + Path, arg0);
        }
        public static string Item(ModItem item, string Path)
        {
            return Language.GetTextValue($"Mods.Stellamod.Items.{item.Name}." + Path);
        }
        public static string Item(ModItem item, string Path, object arg0)
        {
            return Language.GetTextValue($"Mods.Stellamod.Items.{item.Name}." + Path, arg0);
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
        public static string ArmorShopClass(ModItem item, string key = null, object arg0 = null)
        {
            return Language.GetTextValue($"Mods.Stellamod.ArmorShop.{item.Name}", arg0);
        }
        
        public static string ArmorReforge(ArmorReforgeType type, string Path, object arg0 = null)
        {
            return Language.GetTextValue($"Mods.Stellamod.ArmorReforge.{type.ToString()}." + Path, arg0);
        }
        
        public static string Common(string Path, object arg0 = null)
        {
            return Language.GetTextValue("Mods.Stellamod.Items.Common." + Path, arg0);
        }
        public static string Special(ModItem item, string key = null, object arg0 = null)
        {
            return Language.GetTextValue($"Mods.Stellamod.Items.{item.Name}.Special" + key, arg0);
        }
        public static string SetBonus(ModItem item)
        {
            return Language.GetTextValue($"Mods.Stellamod.Items.SetBonus.{item.Name}");
        }

        public static string Misc(string key)
        {
            return Language.GetTextValue("Mods.Stellamod.Misc." + key);
        }
    }
}

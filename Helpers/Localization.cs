using Stellamod.Common.ArmorReforge;
using Stellamod.Common.BossBannerSystem;
using Stellamod.Common.QuestSystem;
using Stellamod.Core.DialogueSystem;
using Stellamod.Core.ZTileSystem;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;


namespace Stellamod.Helpers
{
    public static class LangText
    {
        public static string Dialogue(BaseDialogue dialogue, string Path)
        {
            return Language.GetTextValue($"Mods.Stellamod.TownDialogue.{dialogue.Name}." + Path);
        }


        public static int TipCount
        {
            get
            {
                //This is an extremely stupid way to calculate this,
                //But I don't want to try doing a better way right now.
                int maxPossibleTipCount = 1000;
                for(int i = 0; i < maxPossibleTipCount; i++)
                {
                    string loadingScreenTip = Tip(i);
                    if (loadingScreenTip.Contains("Mods"))
                    {
                        return i;
                    }
                }
                return maxPossibleTipCount;
            }
        }
        public static string LoadingScreen(string category, string key)
        {
            return Language.GetTextValue($"Mods.Stellamod.LoadingScreen.{category}.{key}");
        }
        public static string Tip(int helpNumber)
        {
            return Language.GetTextValue($"Mods.Stellamod.LoadingScreen.Tips.T{helpNumber}");
        }
        public static string Quest(Quest quest, string Path)
        {
            return Language.GetTextValue($"Mods.Stellamod.Quests.{quest.Name}." + Path);
        }
        public static string BossBanners(BossBanner item, string Path)
        {
            return Language.GetTextValue($"Mods.Stellamod.BossBanners.{item.Name}." + Path);
        }
        public static string BossBanners(BossBannerType type, string Path)
        {
            return Language.GetTextValue($"Mods.Stellamod.BossBanners.{type.ToString()}." + Path);
        }
        public static string BossBanners(string Path)
        {
            return Language.GetTextValue($"Mods.Stellamod.BossBanners." + Path);
        }
        public static string BossPages(BossPage item, string Path)
        {
            return Language.GetTextValue($"Mods.Stellamod.BossPages.{item.Name}." + Path);
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
        public static string AccessoryReforge(AccessoryReforgeType type, string Path, object arg0 = null)
        {
            return Language.GetTextValue($"Mods.Stellamod.ArmorReforge.{type.ToString()}." + Path, arg0);
        }

        public static string Common(string Path, params object[] args)
        {
            return Language.GetTextValue("Mods.Stellamod.Items.Common." + Path, args);
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
        public static string ZTile(ZTile item, string Path)
        {
            return Language.GetTextValue($"Mods.Stellamod.ZTiles.{item.Name}." + Path);
        }
        public static string Armor(Item item, string key)
        {
            if(item.ModItem != null)
            {
                return Armor(item.ModItem, key);
            }
            return Language.GetTextValue($"Mods.Stellamod.Armor.{item.GetTypeFileName()}." + key);
        }
        public static string Armor(ModItem item, string key)
        {
            string value = Language.GetTextValue($"Mods.Stellamod.Armor.{item.Name}." + key);

            List<string> assignedKeys = LunarVeilKeybinds.AbilityKeybind.GetAssignedKeys();
            if (assignedKeys.Count > 0)
            {
                value = value.Replace("[ABILITY]", assignedKeys[0]);
            }
            else
            {
                value = value.Replace("[ABILITY]", LangText.Common("Unbound"));
            }

            return value;
        }

        public static string Armor(string key, params object[] args)
        {
            return Language.GetTextValue($"Mods.Stellamod.Armor." + key, args);
        }
    }
}
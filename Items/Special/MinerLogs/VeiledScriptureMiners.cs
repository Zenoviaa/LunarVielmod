using Stellamod.Helpers;
using Stellamod.UI.Scripture;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Items.Special.MinerLogs
{
    internal abstract class VeiledScriptureMiner : ModItem
    {
        public override string Texture => "Stellamod/Items/Special/MinerLogs/VeiledScriptureMiner1";
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ModContent.RarityType<GoldenSpecialRarity>();
        }

        public override void UpdateInventory(Player player)
        {
            base.UpdateInventory(player);
            ScripturePlayer scripturePlayer = player.GetModPlayer<ScripturePlayer>();
            scripturePlayer.hasScripture = true;
        }

        public override bool? UseItem(Player player)
        {
            ScriptureSystem scriptureSystem = ModContent.GetInstance<ScriptureSystem>();
            scriptureSystem.IsVisible = !scriptureSystem.IsVisible;
            scriptureSystem.Panel.Popup.Texture = "Stellamod/UI/Scripture/MinerScripture";

            string localizedText = this.GetLocalization("Content").Value;
            scriptureSystem.Panel.Text.SetText(localizedText);
            return true;
        }
    }

    //These are the actual items, text is automatically grabbed from the localization
    internal class VeiledScriptureMiner1 : VeiledScriptureMiner { }

    internal class VeiledScriptureMiner2 : VeiledScriptureMiner { }

    internal class VeiledScriptureMiner3 : VeiledScriptureMiner { }

    internal class VeiledScriptureMiner4 : VeiledScriptureMiner { }

    internal class VeiledScriptureMiner5 : VeiledScriptureMiner { }

    internal class VeiledScriptureMiner6 : VeiledScriptureMiner { }

    internal class VeiledScriptureMiner7 : VeiledScriptureMiner { }

    internal class VeiledScriptureMiner8 : VeiledScriptureMiner { }

    internal class VeiledScriptureMiner9 : VeiledScriptureMiner { }

    internal class VeiledScriptureMiner10 : VeiledScriptureMiner { }
}

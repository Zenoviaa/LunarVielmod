using Stellamod.Helpers;
using Stellamod.UI.Scripture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Special.MinerLogs
{
    public abstract class VeiledScriptureMiner : ModItem
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
    public class VeiledScriptureMiner1 : VeiledScriptureMiner { }

    public class VeiledScriptureMiner2 : VeiledScriptureMiner { }

    public class VeiledScriptureMiner3 : VeiledScriptureMiner { }

    public class VeiledScriptureMiner4 : VeiledScriptureMiner { }

    public class VeiledScriptureMiner5 : VeiledScriptureMiner { }

    public class VeiledScriptureMiner6 : VeiledScriptureMiner { }

    public class VeiledScriptureMiner7 : VeiledScriptureMiner { }

    public class VeiledScriptureMiner8 : VeiledScriptureMiner { }

    public class VeiledScriptureMiner9 : VeiledScriptureMiner { }

    public class VeiledScriptureMiner10 : VeiledScriptureMiner { }
}

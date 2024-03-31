using Stellamod.Helpers;
using Stellamod.UI.Scripture;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Items.Special
{
    internal class ExampleScriptureItem : ModItem
    {
        public static LocalizedText ContentText { get; private set; }

        public override void SetStaticDefaults()
        {
            // Step 2: Assign RestoreLifeText to the result of GetLocalization
            ContentText = this.GetLocalization("Content");
        }


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
            if(player.HeldItem.type == Type)
            {
                ScripturePlayer scripturePlayer = player.GetModPlayer<ScripturePlayer>();
                scripturePlayer.hasScripture = true;
            }
        }

        public override bool? UseItem(Player player)
        {
            ScriptureSystem scriptureSystem = ModContent.GetInstance<ScriptureSystem>();
            scriptureSystem.IsVisible = !scriptureSystem.IsVisible;
            scriptureSystem.Panel.Popup.Texture = "Stellamod/UI/Scripture/ExampleScripture";

            string localizedText = ContentText.Value;
            scriptureSystem.Panel.Text.SetText(localizedText);
            return true;
        }
    }
}

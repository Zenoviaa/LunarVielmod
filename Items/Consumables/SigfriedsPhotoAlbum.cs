

using Stellamod.Helpers;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee.Greatswords;
using Stellamod.Items.Weapons.Melee.Greatswords.INY;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Items.Weapons.Summon.Orbs;
using Stellamod.Items.Weapons.Thrown.Jugglers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Consumables
{
    public class SigfriedsPhotoAlbum : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Electronic Death Remote (EDR)");
            // Tooltip.SetDefault("'that big red button probably will do something you’ll regret... \n Your conscience advises you to press it and see what happens!'");
        }

        public override void SetDefaults()
        {
            Item.maxStack = 1;
            Item.width = 18;
            Item.height = 28;
            Item.rare = ModContent.RarityType<GoldenSpecialRarity>();
            Item.value = Item.sellPrice(0, 0, 1, 0);
     
        }


       
    }
}
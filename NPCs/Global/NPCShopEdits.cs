
using Stellamod.Items.Armors.Vanity.Astolfo;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Special.MinerLogs;
using Stellamod.Items.Weapons.PowdersItem;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Ranged.Crossbows;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Global
{
    class NPCShopEdits : GlobalNPC
	{


		public override void ModifyShop(NPCShop shop)
		{
			if (shop.NpcType == NPCID.PartyGirl)
			{
				// Adding an item to a vanilla NPC is easy:
				// This item sells for the normal price.
				shop.Add(ModContent.ItemType<AstolfoMask>());
				shop.Add(ModContent.ItemType<AstolfoSkirt>());
				shop.Add(ModContent.ItemType<AstolfoBody>());

				// We can use shopCustomPrice and shopSpecialCurrency to support custom prices and currency. Usually a shop sells an item for item.value.
				// Editing item.value in SetupShop is an incorrect approach.

				// This shop entry sells for 2 Defenders Medals.

			};

			if(shop.NpcType == NPCID.Cyborg)
			{
                shop.Add(ModContent.ItemType<GlocketRouncher>());
            }
            if (shop.NpcType == NPCID.Clothier)
            {
                shop.Add(ModContent.ItemType<VeiledScriptureMiner10>());
            }

            if (shop.NpcType == NPCID.Demolitionist)
			{
				shop.Add(ModContent.ItemType<RocketLauncher>());
			}

            if (shop.NpcType == NPCID.SkeletonMerchant)
            {
                shop.Add(ModContent.ItemType<VeiledScriptureMiner6>());
            }

            if (shop.NpcType == NPCID.Merchant)
			{
				// Adding an item to a vanilla NPC is easy:
				// This item sells for the normal price.
				shop.Add(ModContent.ItemType<MerchantCrossbow>());


				// We can use shopCustomPrice and shopSpecialCurrency to support custom prices and currency. Usually a shop sells an item for item.value.
				// Editing item.value in SetupShop is an incorrect approach.

				// This shop entry sells for 2 Defenders Medals.

			};

			if (shop.NpcType == NPCID.ArmsDealer)
			{
				// Adding an item to a vanilla NPC is easy:
				// This item sells for the normal price.
				shop.Add(ModContent.ItemType<FlamePowder>());
                shop.Add(ModContent.ItemType<BasicGunParts>());
				shop.Add(ModContent.ItemType<ShottyPitol>());
                if (Main.hardMode)
                {
					shop.Add(ModContent.ItemType<RustlockPistol>());
					shop.Add(ModContent.ItemType<Rustvolver>());
				}
				// We can use shopCustomPrice and shopSpecialCurrency to support custom prices and currency. Usually a shop sells an item for item.value.
				// Editing item.value in SetupShop is an incorrect approach.

				// This shop entry sells for 2 Defenders Medals.

			};


			if (shop.NpcType == NPCID.Wizard)
			{
				// Adding an item to a vanilla NPC is easy:
				// This item sells for the normal price.
			
				if (Main.hardMode)
				{
					shop.Add(ItemID.Book);
					shop.Add(ItemID.MagicMissile);
					shop.Add(ItemID.Muramasa);
				}
				// We can use shopCustomPrice and shopSpecialCurrency to support custom prices and currency. Usually a shop sells an item for item.value.
				// Editing item.value in SetupShop is an incorrect approach.

				// This shop entry sells for 2 Defenders Medals.

			};

		}
		
	}
}
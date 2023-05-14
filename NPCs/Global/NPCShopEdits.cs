
using Stellamod.Items.Armors.Vanity.Astolfo;
using Stellamod.Items.Weapons.PowdersItem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Global
{
	class NPCShopEdits : GlobalNPC
	{
		public override void SetupShop(int type, Chest shop, ref int nextSlot)
		{
			// This example does not use the AppliesToEntity hook, as such, we can handle multiple npcs here by using if statements.
			if (type == NPCID.ArmsDealer)
			{
				if (Main.expertMode)
				{
					// Adding an item to a vanilla NPC is easy:
					// This item sells for the normal price.
					shop.item[nextSlot].SetDefaults(ModContent.ItemType<FlamePowder>());
					nextSlot++; // Don't forget this line, it is essential.
				}
				

				// We can use shopCustomPrice and shopSpecialCurrency to support custom prices and currency. Usually a shop sells an item for item.value.
				// Editing item.value in SetupShop is an incorrect approach.

				// This shop entry sells for 2 Defenders Medals.
				

	
			}

			if (type == NPCID.PartyGirl)
			{
				
					// Adding an item to a vanilla NPC is easy:
					// This item sells for the normal price.
					shop.item[nextSlot].SetDefaults(ModContent.ItemType<AstolfoMask>());
					nextSlot++; // Don't forget this line, it is essential.
					
					shop.item[nextSlot].SetDefaults(ModContent.ItemType<AstolfoSkirt>());
					nextSlot++; // Don't forget this line, it is essential.

					shop.item[nextSlot].SetDefaults(ModContent.ItemType<AstolfoBody>());
					nextSlot++; // Don't forget this line, it is essential.


				// We can use shopCustomPrice and shopSpecialCurrency to support custom prices and currency. Usually a shop sells an item for item.value.
				// Editing item.value in SetupShop is an incorrect approach.

				// This shop entry sells for 2 Defenders Medals.



			}
		}
	}
}
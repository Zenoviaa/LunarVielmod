using Microsoft.Xna.Framework;

using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class SapContainer : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(0, 10);
			Item.rare = ItemRarityID.Blue;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetDamage(DamageClass.Magic) += 0.07f; // Increase ALL player damage by 100%
			player.GetModPlayer<MyPlayer>().ArcaneM = true;
			player.GetModPlayer<MyPlayer>().ArcaneMCooldown++;
		}
	}
}
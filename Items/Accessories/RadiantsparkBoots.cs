using Microsoft.Xna.Framework;
using Stellamod.Items.Accessories.Players;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    [AutoloadEquip(EquipType.Waist)] // Load the spritesheet you create as a shield for the player when it is equipped.
    public class RadiantsparkBoots : BaseDashItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.buyPrice(platinum: 3);
            Item.rare = ModContent.RarityType<Helpers.GoldenSpecialRarity>();
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            dashPlayer.DashVelocity += 12;
            player.lifeRegen += 1;
            player.maxRunSpeed *= 1.4f;
            player.statLifeMax2 += 30;
            player.moveSpeed += 0.8f;
            player.fairyBoots = true;
            player.lavaImmune = true;
            player.GetModPlayer<MyPlayer>().GIBomb = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GIBomb>()] == 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<GIBomb>(), 70, 4, player.whoAmI);
            }
        }
    }
}
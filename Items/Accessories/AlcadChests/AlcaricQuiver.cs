using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Arrows;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.AlcadChests
{
    internal class AlcaricQuiverPlayer : ModPlayer
    {
        public bool hasQuiver;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasQuiver = false;
        }

        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly && hasQuiver)
            {
                SoundEngine.PlaySound(SoundID.Item176, Player.position);
                type = ModContent.ProjectileType<VoidArrow>();
                damage += 3;
                velocity *= 2f;
            }
        }
    }

    internal class AlcaricQuiver : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 46;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            player.GetModPlayer<AlcaricQuiverPlayer>().hasQuiver = true;
        }
    }
}

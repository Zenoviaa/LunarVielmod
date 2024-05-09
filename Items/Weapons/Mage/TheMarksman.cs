using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class TheMarksman : ModItem
    {
        public override void SetDefaults()
        {
            Item.staff[Item.type] = true;
            Item.damage = 21;
            Item.mana = 50;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.buyPrice(gold: 15);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_LightingZap");
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MarksmanLightningProj>();
            Item.shootSpeed = 15f;
            Item.crit = 4;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
            Vector2 spawnOffset = new Vector2(0, -768);
            position = Main.MouseWorld + spawnOffset;
            velocity = Vector2.UnitY;
        }
    }
}

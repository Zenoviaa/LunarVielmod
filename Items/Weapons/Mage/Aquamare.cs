using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class Aquamare : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Ranged;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 70;
            Item.mana = 0;
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fungal Flace");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public int Star;
        public override void SetDefaults()
        {
            Item.damage = 98;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Green;
            Item.shootSpeed = 15;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<AquamareProj>();
            Item.shootSpeed = 10f;
            Item.mana = 10;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.consumeAmmoOnLastShotOnly = true;
        }
       
      
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Star += 1;
            if (Star >= 1)
            {
                Star = 0;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Astalaiya3"), player.position);
                type = ModContent.ProjectileType<AquamareProj>();
            }
            if (Star == 2)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/iceshake"), player.position);
                type = ModContent.ProjectileType<AquamareProj>();
            }
            if (Star == 3)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MoonBlow"), player.position);
                type = ModContent.ProjectileType<AquamareProj>();
            }
        }



    }
}

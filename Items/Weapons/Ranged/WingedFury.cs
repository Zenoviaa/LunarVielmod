using Microsoft.Xna.Framework;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Bow;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class WingedFury : ClassSwapItem
    {

        public override DamageClass AlternateClass => DamageClass.Magic;
        public int combo;
        public override void SetClassSwappedDefaults()
        {
            Item.damage = 7;
            Item.mana = 5;
        }

        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Blue;

            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 31;
            Item.scale = 1f;
            Item.useTime = 31;
            Item.consumeAmmoOnLastShotOnly = true;
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0f);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
            combo++;
            if(combo == 3)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/HeatFeather"), player.position);
                type = ModContent.ProjectileType<HeatArrow>();
                velocity *= 1.1f;
                damage += 18;
                knockback *= 1.2f;
                combo = 0;
            }
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBow>(), material: ModContent.ItemType<GintzlMetal>());
        }
    }
}

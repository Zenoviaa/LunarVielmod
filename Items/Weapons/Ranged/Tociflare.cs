using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Ranged
{
    public class Tociflare : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            Item.mana = 4;
            Item.damage = 6;
        }

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 4;
            Item.useAnimation = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item34;
            Item.shoot = ProjectileType<TociBolt4>();
            Item.useAmmo = AmmoID.Gel;
            Item.shootSpeed = 8;
            Item.autoReuse = true;
            Item.noMelee = true;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankGun>(), material: ModContent.ItemType<VirulentPlating>());
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }
    }
}
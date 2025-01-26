using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class EvasiveBalls : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Throwing;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 5;
            Item.mana = 0;
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pearlescent Ice Ball");
            // Tooltip.SetDefault("Shoots fast homing sparks of light!");
        }
        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.mana = 10;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Magic;
            Item.value = 200;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.DD2_DarkMageAttack;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.MagicMissile;
            Item.shootSpeed = 7f;
            Item.autoReuse = true;
            Item.crit = 12;
            Item.noUseGraphic = true;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankOrb>(), material: ModContent.ItemType<WinterbornShard>());
        }
    }
}










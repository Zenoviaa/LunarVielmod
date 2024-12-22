using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Summon
{
    internal class CanOfLeaves : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 3;
        }

        public override void SetDefaults()
        {
            Item.damage = 7;
            Item.mana = 1;
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Guitar;
            Item.staff[Item.type] = true;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.DamageType = DamageClass.Summon;
            Item.value = 200;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.DD2_DrakinShot;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Logger>();
            Item.shootSpeed = 10f;
            Item.autoReuse = true;
            Item.crit = 15;
        }
    }
}








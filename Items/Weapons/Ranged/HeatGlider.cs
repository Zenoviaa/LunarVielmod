using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Bow;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class HeatGlider : ClassSwapItem
    {

        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 10;
            Item.mana = 4;
        }
        private int _comboIndex;
        public override void SetDefaults()
        {
            Item.damage = 19;
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
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 24;
            Item.useTime = 24;
            Item.consumeAmmoOnLastShotOnly = true;
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2f, 0f);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
            _comboIndex++;
            if (_comboIndex >= 3)
            {
                type = ModContent.ProjectileType<HuntrianArrow>();
                _comboIndex = 0;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/HeatFeather"), player.position);
            }
        }
    }
}

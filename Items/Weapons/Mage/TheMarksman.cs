using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class TheMarksman : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Summon;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 21;
            Item.mana = 150;
        }
        public override void SetDefaults()
        {
            Item.staff[Item.type] = true;
            Item.damage = 100;
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

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Passing in a seed so the rng is the same for everyone
            float seed = Main.rand.Next(1, int.MaxValue);
            position = Main.MouseWorld + new Vector2(0, -768);
            velocity = Vector2.UnitY * velocity.Length();
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}

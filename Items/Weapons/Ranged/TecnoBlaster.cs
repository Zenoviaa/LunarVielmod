using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    public class TecnoBlaster : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 7;
            Item.mana = 7;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.LastPrism);
            Item.damage = 13;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 11;
            Item.useAnimation = 11;
            Item.mana = 0;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 20, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TechnoBeam>();
            Item.shootSpeed = 15f;

            SoundStyle useSound = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew9");
            useSound.PitchVariance = 0.15f;
            Item.UseSound = useSound;
            Item.noMelee = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] == 0;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankGun>(), material: ModContent.ItemType<AuroreanStarI>());
        }
    }
}
using Stellamod.Content.CommonMaterials;

using Stellamod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH
{
    public class Stump : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 600;
        }
    }

    public class HikersBackpackPlayer : ModPlayer
    {
        public bool hasBackpack;
        public float cooldown;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasBackpack = false;
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            cooldown--;
            if (hasBackpack && cooldown <= 0)
            {
                if (Main.myPlayer == Player.whoAmI)
                {
                    int damage = 5;
                    int kb = 1;
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -1.1f,
                        ModContent.ProjectileType<Stump>(), damage, kb, Player.whoAmI);
                }

                cooldown = 60;
            }
        }
    }

    public class HikersBackpack : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.sellPrice(silver: 12);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<HikersBackpackPlayer>().hasBackpack = true;
            player.moveSpeed += 0.08f;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(),
                material: ModContent.ItemType<Mushroom>());
        }
    }
}
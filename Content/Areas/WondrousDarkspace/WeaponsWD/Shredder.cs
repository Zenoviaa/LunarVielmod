using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD
{
    public class Shredder : ClassSwapItem
    {

        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 27;
            Item.mana = 7;
        }

        public override void SetDefaults()
        {
            Item.damage = 55;
            Item.crit = 4;
            Item.knockBack = 3f;
            Item.width = 62;
            Item.height = 54;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ModContent.ProjectileType<ShredderProj>();
            Item.shootSpeed = 25;
            Item.autoReuse = true;
            Item.noMelee = true;
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StormDragon_CloudBolt"), player.position);

            //Funny Screenshake
            FXUtil.ShakeCamera(player.position, 4, 8);
            int numProjectiles = Main.rand.Next(2, 5);

            for (int p = 0; p < numProjectiles; p++)
            {
                float direction = Main.rand.NextBool(2) ? -1 : 1;
                float speedMultiplier = Main.rand.NextFloat(0.5f, 1f);
                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectileDirect(source, position, newVelocity * speedMultiplier, type, damage, knockback, player.whoAmI, direction);
            }
            return false;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankGun>(), material: ModContent.ItemType<HypnotizedSoul>());
        }
    }
}

using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Bow;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Ranged
{
    public class Dunderbustion : ModItem
    {
        public int WinterboundArrow;
        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.LightRed;

            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.UseSound = SoundID.Item36;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 35f;
            Item.useAmmo = AmmoID.Bullet;
            Item.useAnimation = 36;
            Item.useTime = 6; // one third of useAnimation
            Item.reuseDelay = 100;
            Item.consumeAmmoOnLastShotOnly = true;
            Item.noMelee = true;
        }
   

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            float rot = velocity.ToRotation();
            float spread = 0.4f;
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol3"));

            Vector2 offset = new Vector2(2, -0f * player.direction).RotatedBy(rot);
            for (int k = 0; k < 15; k++)
            {
                Vector2 direction = offset.RotatedByRandom(spread);

                Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(8), 125, new Color(150, 80, 40), Main.rand.NextFloat(0.2f, 0.5f));
            }


            int numProjectiles = Main.rand.Next(1, 6);
            for (int p = 0; p < numProjectiles; p++)
            {
               

                Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(150, 80, 40), 1);
                Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));
          


                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(25));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }


            return false;
        }


    }
}

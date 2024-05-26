
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Magic;
using Stellamod.Projectiles.Slashers;
using Stellamod.Projectiles.Slashers.Ixy;
using Stellamod.Projectiles.Slashers.Swingaling;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    public class Swingaling : ClassSwapItem
    {
        //Alternate class you want it to change to
        public override DamageClass AlternateClass => DamageClass.Melee;

        //Defaults for the other class
        public override void SetClassSwappedDefaults()
        {
            //Do if(IsSwapped) if you want to check for the alternate class
            //Stats to have when in the other class
            Item.mana = 0;
            Item.damage = 12;
        }
        public int AttackCounter = 1;
        public int combowombo = 0;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Auroran"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("Sends shockwaves through the air" +
                "\nHitting enemies with sword will increase speed!" +
                "\nDivergency Inspired!"); */
        }

        public override void SetDefaults()
        {
            Item.damage = 8;
            Item.DamageType = DamageClass.Magic;
            Item.width = 0;
            Item.height = 0;
            Item.useTime = 110;
            Item.useAnimation = 110;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = 10000;
            Item.noMelee = true;
            Item.mana = 5;
            Item.UseSound = SoundID.DD2_LightningAuraZap;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SwingalingProj1>();
            Item.shootSpeed = 4f;
            Item.noUseGraphic = true;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.Blue;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.GetModPlayer<MyPlayer>().SwordCombo >= 0)
            {
                type = ModContent.ProjectileType<SwingalingProj1>();

            }
            if (player.GetModPlayer<MyPlayer>().SwordCombo >= 4)
            {
                type = ModContent.ProjectileType<SwingalingProj1>();
                SoundEngine.PlaySound(SoundID.Item34, player.position);
            }


            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StormDragon_LightingZap"), player.position);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            int dir = AttackCounter;
            if (player.direction == 1)
            {
                player.GetModPlayer<CorrectSwing>().SwingChange = AttackCounter;
            }
            else
            {
                player.GetModPlayer<CorrectSwing>().SwingChange = AttackCounter * -1;

            }
            AttackCounter = -AttackCounter;
            Projectile.NewProjectile(source, position, velocity, type, damage * 4, knockback, player.whoAmI, 1, dir);



            float recoilStrength = 1;
            Vector2 targetVelocity = -velocity.SafeNormalize(Vector2.Zero) * recoilStrength;
            player.velocity = VectorHelper.VelocityUpTo(player.velocity, targetVelocity);

            //Funny Screenshake
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 32f);
            int numProjectiles = Main.rand.Next(5, 13);
            for (int p = 0; p < numProjectiles; p++)
            {
                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<SwingingBolt>(), damage * 2, knockback, player.whoAmI, 1, dir);
            }

            //Dust Burst Towards Mouse
            int count = 48;
            for (int k = 0; k < count; k++)
            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Dust.NewDust(position, 0, 0, DustID.CopperCoin, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
            }

            //Dust Burst in Circle at Muzzle
            float degreesPer = 360 / (float)count;
            for (int k = 0; k < count; k++)
            {
                float degrees = k * degreesPer;
                Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                Vector2 vel = direction * 8;
                Dust.NewDust(position, 0, 0, DustID.Torch, vel.X * 0.5f, vel.Y * 0.5f);
            }




            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.Anvils);

            recipe.AddIngredient(ItemID.Book, 5);
            recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<UnknownCircuitry>(), 25);
            recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 3);
            recipe.Register();
        }
    }
}
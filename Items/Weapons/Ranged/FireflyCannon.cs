using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    //Use class swap item
    public class FireflyCannon : ClassSwapItem
    {
        //Alternate class you want it to change to
        public override DamageClass AlternateClass => DamageClass.Magic;

        //Defaults for the other class
        public override void SetClassSwappedDefaults()
        {
            //Do if(IsSwapped) if you want to check for the alternate class
            //Stats to have when in the other class
            Item.mana = 20;
            Item.damage = 34;
        }

        public override void SetDefaults()
        {
            Item.damage = 54;
            Item.width = 94;
            Item.height = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 12;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 15;
            Item.autoReuse = false;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ModContent.ProjectileType<FireflyBomb>();
            Item.shootSpeed = 30;
            Item.UseSound = SoundID.Pixie;
            Item.useAnimation = 70;
            Item.useTime = 70;
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-16f, 0f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Funny Recoil
            float recoilStrength = 14;
            Vector2 targetVelocity = -velocity.SafeNormalize(Vector2.Zero) * recoilStrength;
            player.velocity = VectorHelper.VelocityUpTo(player.velocity, targetVelocity);

            //Funny Screenshake
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 32f);
            int numProjectiles = Main.rand.Next(8, 12);
            for (int p = 0; p < numProjectiles; p++)
            {
                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
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
                Dust.NewDust(position, 0, 0, DustID.CopperCoin, vel.X * 0.5f, vel.Y * 0.5f);
            }

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Starexplosion"), player.position);
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 12);
            recipe.AddIngredient(ModContent.ItemType<BasicGunParts>(), 1);
            recipe.AddIngredient(ModContent.ItemType<PearlescentScrap>(), 12);
            recipe.AddIngredient(ModContent.ItemType<MetallicOmniSource>(), 4);
            recipe.AddIngredient(ItemID.SoulofLight, 5);
            recipe.AddIngredient(ItemID.Firefly, 3);
            recipe.Register();
        }
    }
}

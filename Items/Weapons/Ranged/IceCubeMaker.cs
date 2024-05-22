using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class IceCubeMaker : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Magic;
        public override void SetClassSwappedDefaults()
        {
            Item.mana = 12;
            Item.damage = 32;
        }

        public override void SetDefaults()
        {
            Item.width = 72;
            Item.height = 40;
            Item.rare = ItemRarityID.Green;

            //Damage
            Item.damage = 24;
            Item.DamageType = DamageClass.Ranged;

            Item.shoot = ModContent.ProjectileType<IceCubeMakerProj>();
            Item.shootSpeed = 25;

            //Use
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.NPCHit11;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-16, -2);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float rot = velocity.ToRotation();
            float spread = 0.4f;

            SoundStyle soundStyle = SoundRegistry.ExplosionCrystalShard;
            soundStyle.PitchVariance = 0.33f;
            SoundEngine.PlaySound(soundStyle, position);
            Vector2 offset = new Vector2(2, -0f * player.direction).RotatedBy(rot);
            float distance = 32;
            int numProjectiles = 3;
            for (int p = 0; p < numProjectiles; p++)
            {
                Dust.NewDustPerfect(position + offset * distance, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.LightCyan, 1);
                Dust.NewDustPerfect(player.Center + offset * distance, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, Color.LightCyan * 0.5f, Main.rand.NextFloat(0.5f, 1));



                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(45));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.Anvils);
            recipe.AddIngredient(ModContent.ItemType<BasicGunParts>(), 1);
            recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 3);
            recipe.AddIngredient(ModContent.ItemType<SpacialDistortionFragments>(), 15);
            recipe.AddIngredient(ModContent.ItemType<UnknownCircuitry>(), 15);
            recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 5);
            recipe.Register();
        }
    }
}

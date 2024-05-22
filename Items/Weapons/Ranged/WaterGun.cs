using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Particles;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    public class WaterGun : ClassSwapItem
    {
        private int _index;
        private int _comboCounter;
        public override DamageClass AlternateClass => DamageClass.Magic;
        public override void SetClassSwappedDefaults()
        {
            Item.mana = 4;
        }

        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.damage = 132;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 4;
            Item.useAnimation = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Lime;
          //  Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<WaterGunNodeProj>();
            Item.shootSpeed = 6;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6, -2);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool hasConnector = false;
            int connectorType = ModContent.ProjectileType<WaterGunConnectorProj>();
            foreach (var proj in Main.ActiveProjectiles)
            {
                if(proj.type == connectorType)
                {
                    hasConnector = true;
                    break;
                }
            }
            _comboCounter++;
            if (_comboCounter % 9 == 0)
            {
                SoundStyle soundStyle = SoundRegistry.BubbleIn;
                soundStyle.PitchVariance = 0.2f;
                SoundEngine.PlaySound(soundStyle, position);
            }

            float rot = velocity.ToRotation();
            float distance = 16;
            Vector2 offset = new Vector2(3.2f, -0.1f * player.direction).RotatedBy(rot);
            Dust.NewDustPerfect(position + (offset * distance) + new Vector2(0, 6), ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.LightSkyBlue, 1);
            for (int i = 0; i < 1; i++)
            {
                Vector2 bubbleVelocity = velocity.RotatedByRandom(MathHelper.PiOver4);
                float scale = Main.rand.NextFloat(0.6f, 1f);
                ParticleManager.NewParticle(position + (offset * distance) + new Vector2(0, -6), bubbleVelocity, ParticleManager.NewInstance<BubbleParticle>(),
                    Color.White, scale);
            }

            if (!hasConnector)
            {
                Projectile.NewProjectile(source, position, Vector2.Zero, connectorType, damage, knockback, player.whoAmI);
            }

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.WaterGun);
            recipe.AddIngredient(ModContent.ItemType<IllurineScale>(), 8);
            recipe.AddIngredient(ItemID.Ectoplasm, 4);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
